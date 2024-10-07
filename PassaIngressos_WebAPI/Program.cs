using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PassaIngressos_WebAPI.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Passa Ingressos", Version = "v1" });
});

// Database configuration
builder.Services.AddDbContext<DbPassaIngressos>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("db_PassaIngressos")));

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));
}

// Enable HTTPS redirection
// app.UseHttpsRedirection();
// app.UseRouting();

app.UseAuthorization();

// Enable CORS
app.UseCors("AllowAll");

app.MapControllers();

app.Run();