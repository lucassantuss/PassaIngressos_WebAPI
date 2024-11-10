using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PassaIngressos_WebAPI.Database;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT configuration
var keyJWT = Environment.GetEnvironmentVariable("Jwt_ChaveSecreta_PassaIngressos")
    ?? builder.Configuration["Jwt_ChaveSecreta_PassaIngressos"];

var key = Encoding.ASCII.GetBytes(keyJWT);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Add services to the container.
builder.Services.AddControllers();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Passa Ingressos DEV", Version = "v1" });
});

// Database configuration
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__db_PassaIngressos")
    ?? builder.Configuration.GetConnectionString("db_PassaIngressos");

builder.Services.AddDbContext<DbPassaIngressos>(options =>
            options.UseSqlServer(connectionString));

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            // Permite o domínio do frontend
            policy.WithOrigins("http://localhost:3000",
                               "https://passa-ingressos.vercel.app")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowSpecificOrigin");

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();