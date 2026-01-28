var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddFido2(options =>
{
    options.ServerDomain = "localhost";
    options.ServerName = "Fido2 Test Server";
    options.Origins= new HashSet<string>()
    {
        "https://localhost:5001"
    };
    options.ChallengeSize = 32;
});


var app = builder.Build();

app.MapControllers();

app.Run();
