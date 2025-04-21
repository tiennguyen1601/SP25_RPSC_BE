using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SP25_RPSC.Controllers.Controllers.Middleware;
using SP25_RPSC.Controllers.Extensions;
using SP25_RPSC.Data.Repositories;
using SP25_RPSC.Services.Service;
using SP25_RPSC.Services.Service.AmentyService;
using SP25_RPSC.Services.Service.AuthenticationService;
using SP25_RPSC.Services.Service.BackgroundService;
using SP25_RPSC.Services.Service.ChatService;
using SP25_RPSC.Services.Service.ContractCustomerService;
using SP25_RPSC.Services.Service.CustomerRentRoomDetailRequestServices;
using SP25_RPSC.Services.Service.CustomerService;
using SP25_RPSC.Services.Service.EmailService;
using SP25_RPSC.Services.Service.ExtendContractService;
using SP25_RPSC.Services.Service.FeedbackService;
using SP25_RPSC.Services.Service.Hubs.ChatHub;
using SP25_RPSC.Services.Service.Hubs.NotificationHub;
using SP25_RPSC.Services.Service.JWTService;
using SP25_RPSC.Services.Service.LandlordContractService;
using SP25_RPSC.Services.Service.LandlordService;
using SP25_RPSC.Services.Service.NotificationService;
using SP25_RPSC.Services.Service.OTPService;
using SP25_RPSC.Services.Service.PackageService;
using SP25_RPSC.Services.Service.PaymentService;
using SP25_RPSC.Services.Service.PayOSService;
using SP25_RPSC.Services.Service.PostService;
using SP25_RPSC.Services.Service.RoomRentRequestService;
using SP25_RPSC.Services.Service.RoomServices;
using SP25_RPSC.Services.Service.RoomStayService;
using SP25_RPSC.Services.Service.RoomTypeService;
using SP25_RPSC.Services.Service.TransactionService;
using SP25_RPSC.Services.Service.UserService;
using SP25_RPSC.Services.Utils.DecodeTokenHandler;
using SP25_RPSC.Services.Utils.MapperProfile;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
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
builder.Services.AddScoped<IEmailService,  EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IRoomServices, RoomServices>();
builder.Services.AddScoped<IRoomRentRequestService, RoomRentRequestService>();
builder.Services.AddScoped<IRoomStayService, RoomStayService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ILandlordContractService, LandlordContractService>();
builder.Services.AddScoped<ILandlordService, LandlordService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ICloudinaryStorageService, CloudinaryStorageService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerContractService, CustomerContractService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IAmentyService, AmentyService>(); 
builder.Services.AddScoped<ICustomerRentRoomDetailRequestService, CustomerRentRoomDetailRequestService>();
builder.Services.AddScoped<IExtendContractService, ExtendContractService>();
builder.Services.AddHostedService<ContractExpiryCheckService>();
builder.Services.AddHostedService<ContractAutoDeactivateService>();


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
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
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

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RPSC API",
        Version = "v1",
        Description = "API for RPSC Back End using JWT Bearer authentication"
    });

    options.EnableAnnotations();
    options.MapType<CustomerTypeEnums>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(CustomerTypeEnums))
        .Select(name => (IOpenApiAny)new OpenApiString(name))
        .ToList(),
        Description = "Customer Type: Student or Worker"
    });
});

//-----------------------------------------CONTROLLER-----------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });


var app = builder.Build();
// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
