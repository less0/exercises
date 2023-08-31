using editable_table_backend.Persistence;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(corsOptions =>{ 
    CorsPolicyBuilder policyBuilder = new();
    policyBuilder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
    corsOptions.AddDefaultPolicy(policyBuilder.Build());
    });

var app = builder.Build();

app.CreateDummyData();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
