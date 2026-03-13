using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace AngularDotnetAllInOne.ProgramConfigs;

public static class SecurityConfigs
{
    public static IServiceCollection AddSecurityConfigs(
       this IServiceCollection services,
       IConfiguration configuration,
       IWebHostEnvironment environment)
    {
        // services.AddCloudflareConfigs(); // Configuración de Cloudflare (si es necesario)

        // 1. Persistir claves de Data Protection para que las cookies sobrevivan reinicios del servidor
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(configuration["DataProtection"]
                ?? throw new InvalidOperationException("DataProtection path not configured.")))
            .SetApplicationName("AngularDotnetAllInOne")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(365)); // Tiempo de vida de las claves de encriptación (no de las cookies)


        // 2. Identity (debe ir ANTES de ConfigureApplicationCookie) en este ejemplo no lo usamos pero lo dejo comentado para referencia futura
        //services.AddIdentity<User, IdentityRole>(cfg =>
        //{
        //    cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
        //    cfg.SignIn.RequireConfirmedEmail = true;
        //    cfg.User.RequireUniqueEmail = true;
        //    cfg.Password.RequireDigit = false;
        //    cfg.Password.RequiredUniqueChars = 0;
        //    cfg.Password.RequireLowercase = false;
        //    cfg.Password.RequireNonAlphanumeric = false;
        //    cfg.Password.RequireUppercase = false;
        //    cfg.Password.RequiredLength = 6;

        //    cfg.Lockout.MaxFailedAccessAttempts = 5;
        //    cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //})
        //.AddEntityFrameworkStores<DataContext>()
        //.AddDefaultTokenProviders();

        // 3. Intervalo de revalidación del SecurityStamp (detecta cambios de contraseña/rol y cierra sesiones antiguas)
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromHours(1);
        });

        // 4. Token de reset de contraseña y confirmación de email (1 día es suficiente)
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromDays(1);
        });

        // 5. Configurar cookie de autenticación (DESPUÉS de AddIdentity para que no se sobreescriba)
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "AngularDotnetAllInOne";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;

            if (environment.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
            else
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Domain = ".tu_web_direccion.com"; //no olvidar el punto al inicio para que funcione en subdominios
            }

            // ExpireTimeSpan controla la expiración del ticket de autenticación DENTRO de la cookie.
            // Para login normal (sin RememberMe): 1 día.
            // Para login persistente (con RememberMe): se extiende en OnSigningIn.
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.SlidingExpiration = true; // Renueva la expiración con cada request autenticada

            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                },
                OnSigningIn = context =>
                {
                    if (context.Properties.IsPersistent)
                    {
                        // RememberMe: extender la expiración del ticket a 6 meses
                        context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6);
                    }

                    return Task.CompletedTask;
                },
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync
            };
        });

        // Configuración de CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                string[] origins = environment.IsDevelopment()
                    ? ["http://localhost:4200", "https://localhost:4200"]
                    : [configuration["FrontendUrl"] ?? ""];

                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // 6. Configurar políticas de autorización (ejemplo de política para admin, se puede expandir con más roles o claims) en este ejemplo no lo usamos pero lo dejo comentado para referencia futura
        //services.AddAuthorizationBuilder()
        //   .AddPolicy(PolicyKeys.IsAdmin, policy => policy.RequireClaim(ClaimsKeys.Role, UserRole.Admin.ToString()));

        services.AddAuthorization();

        return services;
    }
}