using TechForClimate.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços de Controllers e CORS
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registra os Serviços do Backend
builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddSingleton<OccurrenceService>();

var app = builder.Build();

app.UseCors("AllowAll");

// Habilita arquivos estáticos (index.html, JS, CSS)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

app.Run();