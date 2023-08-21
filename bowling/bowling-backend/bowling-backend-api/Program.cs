using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true);

builder.Services.AddControllers();

// Add any origin for CORS. DON'T use this in any real world szenario, unless you really know 
// what you're doing.
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader()));

var auth0Config = builder.Configuration.GetSection("auth0");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options => {
        options.Audience = auth0Config.GetValue<string>("Audience");
        options.Authority = auth0Config.GetValue<string>("Authority");
    });

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
