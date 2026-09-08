using Backend.Interfaces;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient<IPokemonService, PokemonService>();
builder.Services.AddSingleton<Backend.Helpers.PokemonBattleEngine>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(policy => policy
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()
          .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthorization();

app.MapControllers();

app.Run();
