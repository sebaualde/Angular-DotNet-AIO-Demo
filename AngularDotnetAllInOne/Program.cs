using AngularDotnetAllInOne.ProgramConfigs;

try
{
	var builder = WebApplication.CreateBuilder(args);


	builder.Services.AddSecurityConfigs(builder.Configuration, builder.Environment);
	builder.Services.AddServicesConfigs(builder.Configuration, builder.Environment);

	WebApplication app = builder.Build();

	app.UseMiddlewaresConfigs();
	app.Run();

}
catch (Exception ex)
{

	Console.WriteLine($"Error: {ex.Message}");
}