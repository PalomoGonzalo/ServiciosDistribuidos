using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Productos.Repositorios;
using Serilog;
using Serilog.Events;
using Productos.Servicios;

var builder = WebApplication.CreateBuilder(args);




Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                 .Enrich.WithProcessName()
                 .Enrich.WithProcessId()
                 .Enrich.WithThreadId()
                 .Enrich.WithThreadName()
                 .MinimumLevel.Information()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                 .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                   .WriteTo.File(builder.Configuration.GetSection("LoggingConf").GetValue<string>("outputPath").Trim() + "log_productos.txt",
                                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                                    rollOnFileSizeLimit: true,
                                    retainedFileCountLimit: 5,
                                    rollingInterval: RollingInterval.Day)
                   .CreateLogger();



Log.Write(LogEventLevel.Information, "Se inicio el servicio rest Productos Backend");



builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Control de stock", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                   Id = "Bearer",
                   Type = ReferenceType.SecurityScheme
                },
                Scheme = "Oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }

     });

});

builder.Services.AddCors(x => x.AddPolicy("EnableCors", builder =>
{
    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyOrigin()
            //.WithOrigins("https://codestack.com")
            .AllowAnyMethod()
            //.WithMethods("PATCH", "DELETE", "GET", "HEADER")
            .AllowAnyHeader();
    //.WithHeaders("X-Token", "content-type")
}));

var pass= builder.Configuration["Authentication:SecretKey"];


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(pass))
    };
});


builder.Services.AddScoped<IProductos,Producto>();
builder.Services.AddScoped<ILogService,LoggService>();



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

//app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseCors("EnableCors");
app.UseAuthorization();

app.MapControllers();

app.Run();
