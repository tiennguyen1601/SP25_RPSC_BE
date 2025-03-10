using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SP25_RPSC.Controllers.Controllers.Middleware;
using SP25_RPSC.Data.Repositories;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.MapperProfile;
using System.Text;
using SP25_RPSC.Controllers.Extensions;
using SP25_RPSC.Services.Service.AuthenticationService;
using SP25_RPSC.Services.Service.JWTService;
using SP25_RPSC.Services.Service.OTPService;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Service.UserService;
using SP25_RPSC.Services.Service.PackageService;
using SP25_RPSC.Services.Service.RoomTypeService;
using System.Text.Json.Serialization;
using SP25_RPSC.Services.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddUnitOfWork();


//-----------------------------------------AUTOMAPPER-----------------------------------------

builder.Services.AddAutoMapper(typeof(MapperProfiles).Assembly);

//-----------------------------------------MIDDLEWARE-----------------------------------------

builder.Services.AddSingleton<GlobalExceptionMiddleware>();
//-----------------------------------------SERVICES-----------------------------------------

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IDecodeTokenHandler, DecodeTokenHandler>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped < IEmailService,  EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<ICloudinaryStorageService, CloudinaryStorageService>();


//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//    options.JsonSerializerOptions.WriteIndented = true;
//});
//-----------------------------------------DB-----------------------------------------

builder.Services.AddDbContext<RpscContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

//-----------------------------------------CORS-----------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);

//-----------------------------------------AUTHENTICATION-----------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:JwtKey"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        };
    });
//-----------------------------------------AUTHORIZATION-----------------------------------------
builder.Services.AddAuthorization();
//-----------------------------------------SWAGGER-----------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
