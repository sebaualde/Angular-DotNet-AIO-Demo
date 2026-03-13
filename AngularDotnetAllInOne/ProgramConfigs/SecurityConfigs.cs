using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace AngularDotnetAllInOne.ProgramConfigs;

public static class SecurityConfigs
{
    public static IServiceCollection AddSecurityConfigs(
       this IServiceCollection services,
       IConfiguration configuration,
       IWebHostEnvironment environment)
    {
        /* Identity (preparado para autenticación futura) crear el dbcontext etc. */
        //services.AddIdentity<IdentityUser, IdentityRole>(cfg =>
        //{
        //    cfg.SignIn.RequireConfirmedEmail = true;
        //    cfg.User.RequireUniqueEmail = true;

        //    cfg.Password.RequireDigit = false;
        //    cfg.Password.RequireLowercase = false;
        //    cfg.Password.RequireUppercase = false;
        //    cfg.Password.RequireNonAlphanumeric = false;
        //    cfg.Password.RequiredLength = 6;

        //    cfg.Lockout.MaxFailedAccessAttempts = 5;
        //    cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //})
        //.AddDefaultTokenProviders();

        //// Expiración de tokens (reset password / confirm email)
        //services.Configure<DataProtectionTokenProviderOptions>(options =>
        //{
        //    options.TokenLifespan = TimeSpan.FromDays(1);
        //});

        // Configuración de cookies
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
            }

            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.SlidingExpiration = true;

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
                    bool isPersistent = context.Properties.IsPersistent;
                    if (isPersistent)
                    {
                        context.Properties.ExpiresUtc = DateTimeOffset.Now.AddMonths(6); // Duración de la cookie para usuarios persistentes
                    }

                    return Task.CompletedTask;
                },
                OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync // fuerza revalidación del SecurityStamp
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

        services.AddAuthorization();

        return services;
    }
}