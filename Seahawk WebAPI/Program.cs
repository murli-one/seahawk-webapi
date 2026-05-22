using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Seahawk_WebAPI.Helpers;
//using SeaHawkServices.Application.Contract;
using SeaHawkService.Infrastructure.Emails;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Contract;
using SeaHawkServices.Application.Options;
using SeaHawkServices.Application.Services.Implementation;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using SeaHawkServices.Infrastructure.DHL;
using SeaHawkServices.Infrastructure.Emails;
using SeaHawkServices.Infrastructure.FedEx;
using SeaHawkServices.Infrastructure.Reports;
using SeaHawkServices.Infrastructure.Repository;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;


var context = new CustomAssemblyLoadContext();

//context.LoadUnmanagedLibrary(
//    Path.Combine(
//        Directory.GetCurrentDirectory(),
//        "DinkToPdf",
//        "64bit",
//        "libwkhtmltox.dll"
//    )
//);

var libraryPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? Path.Combine(Directory.GetCurrentDirectory(), "DinkToPdf", "64bit", "libwkhtmltox.dll")
    : "/usr/lib/libwkhtmltox.so";

context.LoadUnmanagedLibrary(libraryPath);


//------------------

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Avoid circular reference exceptions when returning EF graphs.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Fix Swagger 500 caused by duplicate DTO class names
    // Example: CareerDto exists in Careers and Home contracts.
    c.CustomSchemaIds(type =>
    {
        var schemaId = type.FullName ?? type.Name;

        return schemaId
            .Replace("+", ".")
            .Replace("`1", "")
            .Replace("`2", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace(",", "_")
            .Replace(" ", "_");
    });

    // Temporary protection if any duplicate route conflict appears.
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    // Keep Swagger focused on API endpoints only.
    c.DocInclusionPredicate((_, apiDesc) =>
        apiDesc.RelativePath != null &&
        apiDesc.RelativePath.StartsWith("api/", StringComparison.OrdinalIgnoreCase));

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key is required in configuration.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Hybrid";
    options.DefaultChallengeScheme = "Hybrid";
})
.AddPolicyScheme("Hybrid", "JWT or Cookie", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authHeader) &&
            authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return JwtBearerDefaults.AuthenticationScheme;
        }

        return IdentityConstants.ApplicationScheme;
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
        policy.RequireClaim("SystemAdmin", "True"));

    options.AddPolicy("OrgOrAdmin", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.HasClaim("SystemAdmin", "True") ||
            ctx.User.HasClaim("VesselUser", "True") ||
            ctx.User.HasClaim("ManagementUser", "True")));
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = "/api/auth/access-denied";
    option.LoginPath = "/api/auth/login";
    option.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
    };
});

builder.Services.Configure<IdentityOptions>(option =>
{
    option.Password.RequiredLength = 6;
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireUppercase = false;
    option.Password.RequireNonAlphanumeric = false;
});

// Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IVesselRepository, VesselRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
builder.Services.AddScoped<IVesselUserRepository, VesselUserRepository>();
builder.Services.AddScoped<IAccountHistoryRepository, AccountHistoryRepository>();
builder.Services.AddScoped<ISmtpService, SmtpService>();

// Services
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IVesselService, VesselService>();
builder.Services.AddScoped<IVesselDetailService, VesselDetailService>();
builder.Services.AddScoped<IAnalysisResultService, AnalysisResultService>();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
builder.Services.AddScoped<ICompanyUserService, CompanyUserService>();
builder.Services.AddScoped<IVesselUserService, VesselUserService>();
builder.Services.AddScoped<IAccountHistoryService, AccountHistoryService>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IXMLReportService, XMLReportService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ILiveDataService, LiveDataService>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ISamplingKitService, SamplingKitService>();
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<IPDFService, PDFService>();
builder.Services.AddScoped<ISampleCollectionsService, SampleCollectionsService>();
builder.Services.AddHttpClient<IPickupService, PickupService>();
builder.Services.AddScoped<IUserLoginHistoryService, UserLoginHistoryService>();
builder.Services.AddHttpContextAccessor();
// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// For Email:-
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton<DropboxTokenService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// For Dropbox:-
builder.Services.Configure<ReportStorageOptions>(builder.Configuration.GetSection("Dropbox"));
builder.Services.AddHttpClient<IDropboxGateway, DropboxGateway>(c => { c.Timeout = TimeSpan.FromMinutes(5); });

builder.Services.AddScoped<ISampleReportRepository, SampleReportRepository>();
builder.Services.AddScoped<ISampleReportService, SampleReportService>();

// For DHL/FedEx
builder.Services.Configure<DhlTrackingOptions>(builder.Configuration.GetSection("DHL_Prod"));
builder.Services.Configure<DhlShipmentOptions>(builder.Configuration.GetSection("DhlShipment"));
builder.Services.AddHttpClient<IDhlShippingService, DhlShippingService>();
builder.Services.Configure<FedExSettings>(builder.Configuration.GetSection("FedEx_Sandbox"));
builder.Services.Configure<FedExRestSettings>(builder.Configuration.GetSection("FedExRest"));
builder.Services.AddHttpClient<IFedExShippingService, FedExShippingService>();
builder.Services.Configure<SMTPSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddHttpClient<IFedExTrackingClient, FedExTrackingClient>();
builder.Services.AddHttpClient<IDhlTrackingClient, DhlRestTrackingClient>((sp, http) =>
{
    var opt = sp.GetRequiredService<IOptions<DhlTrackingOptions>>().Value;
    http.BaseAddress = new Uri(opt.BaseUrl);
    http.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
});
builder.Services.AddScoped<IShipmentTrackingService, ShipmentTrackingService>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In Development many teams run only http locally; avoid redirecting
// to an https port that isn't bound / trusted yet.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Needed for endpoints that store/serve PDFs/labels under wwwroot (FedEx/DHL label flows)
app.UseStaticFiles();

app.UseForwardedHeaders();
app.UseRouting();
app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok", service = "SeaHawk API" }));

app.Run();
