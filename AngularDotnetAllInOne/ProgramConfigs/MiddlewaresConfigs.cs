namespace AngularDotnetAllInOne.ProgramConfigs;


public static class MiddlewaresConfigs
{
    public static WebApplication UseMiddlewaresConfigs(this WebApplication app)
    {
        //app.UseCloudflareForwardedHeaders(); //No implementado en este ejemplo.
        
        if (app.Environment.IsDevelopment()) 
        { 
            app.UseDeveloperExceptionPage(); 
            app.MapOpenApi(); 
        }

        app.UseResponseCompression(); //Reduce el tamaño de las respuestas HTTP enviadas al cliente, mejorando el rendimiento.

        app.UseHttpsRedirection();

        /*Estos middlewares permiten que ASP.NET Core sirva:
            - archivos estáticos generales(wwwroot)
            -archivos compilados del frontend Angular */
        app.UseStaticFiles(); 
        app.UseSpaStaticFiles(); 

        app.UseCors(); 
        app.UseAuthentication(); 
        app.UseAuthorization();

        //app.UseRateLimiter(); //No implementado en este ejemplo.
        //app.UseErrorHandlingMiddleware();  //No implementado en este ejemplo.

        app.MapControllers();

        /*
         Esta configuración permite que:
            - en desarrollo, Angular corra de forma independiente con "ng serve" en http://localhost:4200,
              donde proxy.conf.json redirige /api al backend.
            - en producción, ASP.NET Core sirva directamente los archivos estáticos generados por Angular
         */
        if (!app.Environment.IsDevelopment())
        {
            app.UseSpa(spa => 
            { 
                spa.Options.SourcePath = "ClientApp"; 
            });
        }

        return app;
    }
}
