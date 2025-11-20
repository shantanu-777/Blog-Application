using BlogApp.Shared.Models;

namespace BlogApp.Backend.Services;

public interface ISiteSettingsService
{
    Task<SiteSettings> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task<SiteSettings> UpdateSettingsAsync(SiteSettings settings, CancellationToken cancellationToken = default);
}

public class SiteSettingsService : ISiteSettingsService
{
    private readonly SupabaseRestClient _client;
    private readonly ILogger<SiteSettingsService> _logger;

    public SiteSettingsService(SupabaseRestClient client, ILogger<SiteSettingsService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<SiteSettings> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _client.GetAsync<SupabaseSiteSettingsDto>("site_settings?select=*&limit=1", cancellationToken);
        var dto = settings.FirstOrDefault();
        if (dto != null)
            return dto.ToModel();

        var created = await _client.PostAsync<SupabaseSiteSettingsDto>("site_settings", new SiteSettings().ToDto(), cancellationToken);
        _logger.LogInformation("Initialized default site settings record.");
        return (created ?? new SiteSettings().ToDto()).ToModel();
    }

    public async Task<SiteSettings> UpdateSettingsAsync(SiteSettings settings, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(settings.Id))
        {
            var existing = await GetSettingsAsync(cancellationToken);
            settings.Id = existing.Id;
        }

        settings.UpdatedAt = DateTime.UtcNow;
        var dto = settings.ToDto();

        var updated = await _client.PatchAsync<SupabaseSiteSettingsDto>($"site_settings?id=eq.{Uri.EscapeDataString(dto.Id)}", dto, cancellationToken);
        _logger.LogInformation("Updated site settings at {UpdatedAt}", settings.UpdatedAt);
        return updated?.ToModel() ?? settings;
    }
}

