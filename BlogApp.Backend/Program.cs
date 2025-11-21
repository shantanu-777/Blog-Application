using BlogApp.Backend.Services;
using BlogApp.Backend.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/blogapp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR for real-time features
builder.Services.AddSignalR();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5000", "http://localhost:5001", "http://localhost:7001", "http://localhost:64241", "https://blprbnewsuqkwdmaafcs.supabase.co")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
    
    // More permissive policy for development
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add HTTP client for AI services and Supabase REST access
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("SupabaseRest");

// Add custom services
builder.Services.AddSingleton<SupabaseRestClient>();
builder.Services.AddSingleton<ISupabaseBlogRepository, SupabaseBlogRepository>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IAIContentService, AIContentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISiteSettingsService, SiteSettingsService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<IContactService, ContactService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for development with HTTP
app.UseCors("AllowAll"); // Using permissive policy for development
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// Ports are configured in launchSettings.json

app.Run();
