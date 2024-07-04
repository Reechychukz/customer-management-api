using Domain.Entities;
using Infrastructure;
using Infrastructure.Data.DbContext;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation.AspNetCore;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Validations;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using API.Configurations;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
	{
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        public static void ConfigureIisIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequiredLength = 8;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.User.RequireUniqueEmail = true;
            }).AddRoles<Role>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // custom auditing service at db level
            //services.AddScoped<IPersistenceAudit, PersistenceAuditService>();
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IDesignTimeDbContextFactory<AppDbContext>), typeof(DbContextFactoryConfiguration));

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x =>
                {
                    x.MigrationsAssembly("Infrastructure.Data");
                    x.EnableRetryOnFailure();
                })
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(c => c.Log(CoreEventId.DetachedLazyLoadingWarning))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }


        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("ValidIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("ValidAudience").Value,
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSecret))
                };
            });
        }

        public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = false;
            });
            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = false;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddMvcCore().AddApiExplorer();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RemoveVersionFromParameter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }


        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddRepositories();
        }

        /// <summary>
        /// Configure binding of IConfigurations to typed object for better maintainability.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureIOObjects(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfigSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<WemaConfigSettings>(configuration.GetSection("WemaApiSettings"));
        }

        [Obsolete]
        public static void ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest)
                .ConfigureApiBehaviorOptions(o =>
                {
                    //o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
                }).AddFluentValidation(x =>
                    x.RegisterValidatorsFromAssemblyContaining<UserValidator>());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}

