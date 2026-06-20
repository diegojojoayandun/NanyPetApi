using System.Text;
using BusinessLogicLayer.Services.BlobService;
using BusinessLogicLayer.Services.GeoService;
using BusinessLogicLayer.Services.GenericService;
using BusinessLogicLayer.Services.NotificationService;
using BusinessLogicLayer.Services.OwnerService;
using BusinessLogicLayer.Services.PaymentService;
using BusinessLogicLayer.Services.UserService;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories.Generic;
using DataAccessLayer.Repositories.Users;
using DotEnv.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NanyPetAPI;
using NanyPetAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

new EnvLoader().Load();
Configuration.AddEnvironmentVariables();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresar Bearer [space] tuToken \r\n\r\n Ejemplo: Bearer 123456abcder",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddResponseCaching();
builder.Services.AddSignalR();

// CORS para el frontend React
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                Configuration["FRONTEND_URL"] ?? "http://localhost:5173",
                "http://localhost:5173",
                "http://localhost:3000"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    var connectionString = Configuration["CONNECTION_STRING"];
    var dbProvider = Configuration["DB_PROVIDER"] ?? "sqlite";

    if (dbProvider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(connectionString))
        option.UseSqlServer(connectionString);
    else
        option.UseSqlite(connectionString ?? "Data Source=nanypet.db");
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepository<Herder>, Repository<Herder>>();
builder.Services.AddScoped<IService<Herder>, Service<Herder>>();
builder.Services.AddScoped<IRepository<Owner>, Repository<Owner>>();
builder.Services.AddScoped<IService<Owner>, Service<Owner>>();
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IGeoService, GeoService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddHttpClient("Wompi");

// JWT + Google OAuth en una sola cadena de autenticación
var secretKey = Configuration["SECRET_KEY"] ?? throw new InvalidOperationException("SECRET_KEY no configurado en variables de entorno.");
var googleClientId = Configuration["CLIENT_ID"];
var googleClientSecret = Configuration["CLIENT_SECRET"];

var authBuilder = builder.Services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/api/signin-google";
});

if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Seed roles y usuario admin inicial
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    foreach (var role in new[] { "Owner", "Herder", "Admin" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = Configuration["ADMIN_EMAIL"];
    var adminPassword = Configuration["ADMIN_PASSWORD"];
    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                FirstName = "Admin",
                LastName = "NanyPet"
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
