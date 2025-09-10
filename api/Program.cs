
using System.Text;
using api.Data;
using api.Entities.Enuns;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
JwtConfig(builder);
AuthorizationConfig(builder);
ConfigureServices(builder);

var app = builder.Build();
ConfigureSwagger(app);

app.UseCors("PermitirFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
void JwtConfig(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero 
            };
        });
}

void AuthorizationConfig(WebApplicationBuilder builder)
{
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireRole(UserRole.Admin.ToString()));
        options.AddPolicy("User", policy => policy.RequireRole(UserRole.User.ToString(), UserRole.Admin.ToString()));
    });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddOpenApi();
    builder.Services.AddCors(options=>{
        options.AddPolicy("PermitirFrontend",policy=>{
            policy.WithOrigins("http://localhost:5173","http://localhost:5174", "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITicketService, TicketService>();
    builder.Services.AddControllers();
}

void ConfigureSwagger(WebApplication app)
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Api V1");
    });
}