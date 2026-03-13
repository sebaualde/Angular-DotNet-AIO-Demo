using System.Text.Json.Serialization;

namespace AngularDotnetAllInOne.ProgramConfigs;

public static class ServicesConfigs
{
    public static IServiceCollection AddServicesConfigs(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IWebHostEnvironment environment)
    {
        services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; }); 
            
        // Configuración para servir la SPA (Angular)       
        services.AddSpaStaticFiles(config => { 
            config.RootPath = environment.IsDevelopment() 
            ? "ClientApp/dist/ClientApp/browser" 
            : "wwwroot/spa"; 
        }); 
        
        // Compresión de respuestas HTTP
        services.AddResponseCompression(options => { options.EnableForHttps = true; }); 
        
        // Documentación de la API
        services.AddOpenApi(options => 
        { 
            options.AddDocumentTransformer((document, context, cancellationToken) => { 
                document.Info.Title = "AngularDotnetAllInOne API"; 
                document.Info.Version = "1.0.0"; 
                return Task.CompletedTask; }); 
        }); 
        
        // Cache en memoria
        services.AddMemoryCache(); 
        
        // Acceso al contexto HTTP
        services.AddHttpContextAccessor(); 
        
        // Explorer de endpoints
        services.AddEndpointsApiExplorer(); 
        
        return services;
    }
}