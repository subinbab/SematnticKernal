using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using SemanticAIApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var builder1 = Kernel.CreateBuilder();
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Adding and configuring authentication to use JWT Bearer tokens.
// This sets JWT Bearer as the default authentication scheme and challenge scheme.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding the JwtBearer authentication handler to validate incoming JWT tokens.
.AddJwtBearer(options =>
{
    // Configuring the parameters for JWT token validation.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensure the token's issuer matches the expected issuer.
        ValidateAudience = true, // Ensure the token's audience matches the expected audience.
        ValidateLifetime = true, // Validate that the token has not expired.
        ValidateIssuerSigningKey = true, // Ensure the token is signed by a trusted signing key.
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // The expected issuer, retrieved from configuration (appsettings.json).
        ValidAudience = builder.Configuration["Jwt:Audience"], // The expected audience, retrieved from configuration (appsettings.json).
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // The symmetric key used to sign the JWT, also from configuration.
    };
});
// Adds support for MVC controllers to the application.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configuring JSON serialization options.
        // This disables the default camel-casing of property names, ensuring that property names in the JSON output match the C# model property names.
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
}); ;


var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add Semantic Kernel
builder.Services.AddSingleton<Kernel>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: configuration["DEPLOYMENT_MODEL"],
            endpoint: configuration["AZURE_OPEN_AI_ENDPOINT"],
            apiKey: configuration["AZURE_OPEN_AI_KEY"])
        .Build();
});
var kernel = builder1.Build();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

