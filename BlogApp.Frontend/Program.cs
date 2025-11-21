using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Blazored.LocalStorage;
using BlogApp.Frontend.Services;
using BlogApp.Frontend; // ensure App component is available

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add MudBlazor
builder.Services.AddMudServices();

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Configure HttpClient for API calls (change base address if needed)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:64241/") });

// Add Authentication services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Add custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IAIContentService, AIContentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<IContactService, ContactService>();

await builder.Build().RunAsync();
