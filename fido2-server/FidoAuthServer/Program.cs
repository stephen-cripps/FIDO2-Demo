using FidoAuthServer.Data;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddFido2(options =>
{
    options.ServerDomain = "localhost";
    options.ServerName = "Fido2 Test Server";
    options.Origins = new HashSet<string>()
    {
        "http://localhost:3000"
    };
    options.ChallengeSize = 32;
});

services.AddCors(opts =>
{
    opts.AddDefaultPolicy(pol =>
    {
        pol
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

services.AddTransient<IUserRepository, UserRepository>();

services.AddControllers();

var app = builder.Build();

app.UseCors();
app.MapControllers();

DbInitialiser.Initialise();
app.Run();
