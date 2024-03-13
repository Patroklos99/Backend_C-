using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Project.auth.session;
using Project.messages.repository;
using Project.temperatures.model;

var builder = WebApplication.CreateBuilder(args);

// Register FirebaseInitializer with the DI container
InitializeFirebase();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            // policy.AllowAnyOrigin()
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:8080")
                .AllowAnyMethod()
                .AllowAnyHeader();
            // .AllowCredentials();
        }
    );
});

// FirebaseApp.Create();
ConfigurerServices(builder.Services);
var app = builder.Build();
ConfigurerPipeline();

void ConfigurerServices(IServiceCollection builderServices)
{
    builderServices.AddControllers();
    builderServices.AddHttpContextAccessor();
    ConfigurerServicesSwagger(builder.Services);
    builderServices.AddScoped<SessionDataAccessor>();
    builderServices.AddScoped<MessageRepository>();
    builderServices.AddSingleton<SessionManager>();
    builderServices.AddSignalR();
}

void ConfigurerPipeline()
{
    // app.UseHttpsRedirection(); // Middleware for HTTPS redirection
    app.UseRouting(); // Routing
    app.UseCors(); // Apply CORS
    // app.UseMiddleware<AuthFilter>(); // AuthFilter

    // Development-specific configurations
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization(); // Authorization
    app.MapControllers(); // Map controllers
    // app.MapHub<WebSocketManagerHandler>("/notifications");
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
    string projectd = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
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