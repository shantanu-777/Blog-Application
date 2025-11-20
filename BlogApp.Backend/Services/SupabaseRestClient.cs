using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlogApp.Backend.Services;

public class SupabaseRestClient
{
    private readonly HttpClient _httpClient;
    private readonly string _restBaseUrl;
    private readonly JsonSerializerOptions _serializerOptions;

    public SupabaseRestClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        var supabaseUrl = configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase Url is not configured.");
        var anonKey = configuration["Supabase:AnonKey"] ?? throw new InvalidOperationException("Supabase Anon Key is not configured.");

        _restBaseUrl = $"{supabaseUrl.TrimEnd('/')}/rest/v1";

        _httpClient = httpClientFactory.CreateClient("SupabaseRest");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", anonKey);
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("apikey", anonKey);
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Profile", "public");

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<List<T>> GetAsync<T>(string relativePath, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(BuildUrl(relativePath), cancellationToken);
        await EnsureSuccess(response);

        return await ReadListAsync<T>(response, cancellationToken);
    }

    public async Task<(List<T> Items, int? Count)> GetWithCountAsync<T>(string relativePath, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(relativePath));
        request.Headers.TryAddWithoutValidation("Prefer", "count=exact");
        var response = await _httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccess(response);

        var items = await ReadListAsync<T>(response, cancellationToken);
        var count = ParseTotalCount(response.Headers);
        return (items, count);
    }

    public async Task<T?> PostAsync<T>(string table, object payload, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(table));
        request.Headers.TryAddWithoutValidation("Prefer", "return=representation");
        request.Content = JsonContent.Create(payload, options: _serializerOptions);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccess(response);

        return (await ReadListAsync<T>(response, cancellationToken)).FirstOrDefault();
    }

    public async Task<T?> PatchAsync<T>(string tableWithFilters, object payload, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, BuildUrl(tableWithFilters));
        request.Headers.TryAddWithoutValidation("Prefer", "return=representation");
        request.Content = JsonContent.Create(payload, options: _serializerOptions);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccess(response);

        return (await ReadListAsync<T>(response, cancellationToken)).FirstOrDefault();
    }

    public async Task DeleteAsync(string tableWithFilters, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(BuildUrl(tableWithFilters), cancellationToken);
        await EnsureSuccess(response);
    }

    private string BuildUrl(string relativePath)
    {
        if (relativePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return relativePath;

        if (relativePath.StartsWith("/"))
            relativePath = relativePath.TrimStart('/');

        return $"{_restBaseUrl}/{relativePath}";
    }

    private async Task<List<T>> ReadListAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentLength == 0)
            return new List<T>();

        var result = await response.Content.ReadFromJsonAsync<List<T>>(_serializerOptions, cancellationToken);
        return result ?? new List<T>();
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var details = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Supabase REST request failed ({response.StatusCode}): {details}");
    }

    private static int? ParseTotalCount(HttpResponseHeaders headers)
    {
        if (!headers.TryGetValues("Content-Range", out var ranges))
            return null;

        var range = ranges.FirstOrDefault();
        if (string.IsNullOrEmpty(range))
            return null;

        var slashIndex = range.LastIndexOf('/');
        if (slashIndex < 0 || slashIndex == range.Length - 1)
            return null;

        var totalSpan = range[(slashIndex + 1)..];
        return int.TryParse(totalSpan, out var total) ? total : null;
    }
}

