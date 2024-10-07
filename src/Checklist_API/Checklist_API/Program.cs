using Check_List_API.Data;
using Checklist_API.Extensions;
using Checklist_API.Features.JWT.Features;
using Checklist_API.Features.Users.Repository;
using Checklist_API.Features.Users.Repository.Interfaces;
using Checklist_API.Features.Users.Service;
using Checklist_API.Features.Users.Service.Interfaces;
using Checklist_API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.RegisterMappers();

var configuration = builder.Configuration; // kan fjernes etter vi har samlet alle services i servicecollectio etterhvert
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddDbContext<CheckListDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0)))
    );

builder.Services.AddScoped<GlobalExceptionMiddleware>(); // samle senere: public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
builder.Services.AddSingleton<ExceptionHandler>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<TokenGenerator>();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }