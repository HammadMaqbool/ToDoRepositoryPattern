using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ToDoRepositoryPattern.Data;
using FluentValidation;
using ToDoRepositoryPattern.DTOs.AutoMappings;
using ToDoRepositoryPattern.Repositories;
using ToDoRepositoryPattern.Repositories.Interfaces;
using Serilog;
using ToDoRepositoryPattern.Middlewares;
using ToDoRepositoryPattern.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWTSettings:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:SecretKey"]!))
        };
    });

// Add services to the container.
builder.Services.AddControllers( options=> { 
    options.Filters.Add<RandomFilter>();
});
builder.Services.AddOpenApi();

//Fluent Validation Registration
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//AutoMapper Registration
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ToDoMaps>();
    cfg.AddProfile<UserMaps>();
    //Keep adding the Maps for all the classes here. . .. 
});


//Configuring the Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

//To replace the inbuilt logger with Serilog
builder.Host.UseSerilog();

//IMemoryCache
builder.Services.AddMemoryCache();

//Repositories Registration
builder.Services.AddScoped<IToDoRepository, ToDoRepository>();
builder.Services.AddScoped<AuthRepository>();

var ConnectionString = builder.Configuration.GetConnectionString("conString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionString));

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", ()=> Results.Redirect("/scalar/v1"));
}

//Serilog Request Logging Middleware
app.UseSerilogRequestLogging();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
