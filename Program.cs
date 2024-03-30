using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Project.auth.filter;
using Project.auth.session;
using Project.messages.repository;
using Project.WebSocket;

var builder = WebApplication.CreateBuilder(args);

// Register FirebaseInitializer with the DI container
InitializeFirebase();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            // .AllowCredentials();
        }
    );
});

ConfigurerServices(builder.Services);
var app = builder.Build();
ConfigurerPipeline();
app.Run();

void ConfigurerServices(IServiceCollection builderServices)
{
    builderServices.AddControllers();
    builderServices.AddHttpContextAccessor();
    ConfigurerServicesSwagger(builder.Services);
    builderServices.AddScoped<SessionDataAccessor>();
    builderServices.AddScoped<MessageRepository>();
    builderServices.AddSingleton<SessionManager>();
    builder.Services.AddScoped<SimplePasswordHasher>();
    builder.Services.AddScoped<UserAccountRepository>();
    builderServices.AddSignalR();
}

void ConfigurerPipeline()
{
    app.UseHttpsRedirection(); // Middleware for HTTPS redirection
    app.UseCors(); // Apply CORS
    app.UseRouting(); // Routing
    app.UseMiddleware<AuthFilter>(); // AuthFilter
    app.UseAuthorization(); // Authorization
    app.UseAuthentication();

    // Development-specific configurations
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.MapControllers(); // Map controllers
    app.MapHub<WebSocketManagerHandler>("/notifications");
}


void ConfigurerServicesSwagger(IServiceCollection services)
{
// Add services to the container. More Swagger/OpenAPI details at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

// Define the InitializeFirebase method
void InitializeFirebase()
{
    var projectId = "chat-project-d5a0e";
    var projectd = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
    var serviceAccountKeyPath = "/home/kaped/RiderProjects/Project/firebase-key.json";
    if (File.Exists(serviceAccountKeyPath))
    {
        var appOptions = new AppOptions
        {
            Credential = GoogleCredential.FromFile(serviceAccountKeyPath),
            ProjectId = projectId
        };
        FirebaseApp.Create(appOptions);
    }
    else
    {
        Console.WriteLine(
            "Firebase service account key file not found. Attempting to initialize Firebase with default credentials.");
        try
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize Firebase with application default credentials: {ex.Message}");
        }
    }
}