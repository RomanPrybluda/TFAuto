using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TFAuto.DAL.Constant;
using TFAuto.Domain;
using TFAuto.Domain.Configurations;
using TFAuto.Domain.Mappers;
using TFAuto.Domain.Seeds;
using TFAuto.Domain.Services.Admin;
using TFAuto.Domain.Services.ArticlePage;
using TFAuto.Domain.Services.Authentication;
using TFAuto.Domain.Services.Authentication.Constants;
using TFAuto.Domain.Services.Blob;
using TFAuto.Domain.Services.CommentService;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.LikeService;
using TFAuto.Domain.Services.Roles;
using TFAuto.Domain.Services.UserInfo;
using TFAuto.Domain.Services.UserPassword;
using TFAuto.Domain.Services.UserUpdate;
using TFAuto.WebApp.Configurations;
using TFAuto.WebApp.Middleware;

namespace TFAuto.WebApp;

public static class ServicesConfigurations
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        AddCosmosRepository(builder);
        AddBlobStorage(builder.Configuration);
        AddCors(builder);
        ConfigureAuthentication(builder);
        ConfigureAuthorization(builder);
        AddSwagger(builder.Services);
        AddServices(builder.Services);
        AddMappers(builder.Services);
        AddHtttpAccessor(builder);
    }

    private static void AddCosmosRepository(WebApplicationBuilder builder)
    {
        builder.Services.AddCosmosRepository(options =>
        {
            options.CosmosConnectionString = builder.Configuration.GetConnectionString("CosmosDBConnectionString");

            var cosmosSettings = builder.Configuration.GetSection("CosmosDBConnectionSettings").Get<CosmosDBConnectionSettings>();
            options.DatabaseId = cosmosSettings.DatabaseId;
            options.ContainerId = cosmosSettings.ContainerId;
        });
    }

    private static void AddBlobStorage(IConfiguration configuration)
    {
        configuration.GetConnectionString("BlobStorageConnectionString");
        configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>();
    }

    private static void AddCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }

    private static void AddSwagger(IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Access Token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
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
                    new List<string>()
                }
            });
        });
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("jwtSettings"));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = builder.Configuration.GetSection("jwtSettings").Get<JWTSettings>();
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.ValidIssuer,
                ValidAudience = jwtSettings.ValidAudience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var isAccessClaim = context.Principal.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.IS_ACCESS)?.Value;

                    if (!bool.Parse(isAccessClaim))
                    {
                        context.Fail("Unauthorized");
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }

    private static void ConfigureAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            foreach (var permissionId in PermissionIdList.GetPermissions())
            {
                options.AddPolicy(permissionId, policy =>
                {
                    policy.RequireClaim(CustomClaimsType.PERMISSION_ID, permissionId);
                });
            }
        });
    }

    private static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
        serviceCollection.AddScoped<IEmailService, EmailService>();
        serviceCollection.AddScoped<IUserPasswordService, UserPasswordService>();
        serviceCollection.AddScoped<IRoleService, RoleService>();
        serviceCollection.AddScoped<RoleInitializer>();
        serviceCollection.AddScoped<PermissionInitializer>();
        serviceCollection.AddScoped<JWTService>();
        serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
        serviceCollection.AddScoped<IBlobService, BlobService>();
        serviceCollection.AddScoped<IArticleService, ArticleService>();
        serviceCollection.AddScoped<IUserInfoService, UserInfoService>();
        serviceCollection.AddScoped<IUserUpdateInfoService, UserUpdateInfoService>();
        serviceCollection.AddScoped<ILikeService, LikeService>();
        serviceCollection.AddScoped<ICommentService, CommentService>();
        serviceCollection.AddScoped<IAdminService, AdminService>();
    }

    private static void AddMappers(IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(UserMapper));
        serviceCollection.AddAutoMapper(typeof(RoleUserMapper));
        serviceCollection.AddAutoMapper(typeof(ArticleMapper));
        serviceCollection.AddAutoMapper(typeof(UserUpdateInfoMapper));
        serviceCollection.AddAutoMapper(typeof(CommentMapper));
    }

    public static void InitializeSeeds(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
            roleInitializer.InitializeRoles().Wait();

            var permissionInitializer = scope.ServiceProvider.GetRequiredService<PermissionInitializer>();
            permissionInitializer.InitializePermissions().Wait();
        }
    }

    private static void AddHtttpAccessor(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
    }

    public static void RegisterMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}