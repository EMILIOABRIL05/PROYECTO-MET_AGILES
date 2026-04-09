using Api.Ventas.Data;
using Api.Ventas.Services;
using Api.Ventas.Services.EventBus;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddOpenApi();

// Entity Framework Core — SQL Server
builder.Services.AddDbContext<VentasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias — Servicios
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
builder.Services.AddHttpClient();

// CORS — Permitir React Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactApp");
app.MapControllers();

// Auto-crear la base de datos (Code First) en desarrollo
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VentasDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
