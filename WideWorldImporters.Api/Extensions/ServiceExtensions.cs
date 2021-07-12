using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AspNetCoreRateLimit;
using Contracts;
using Entities;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using Repository;
using WideWorldImportersWebApi.Utility;

namespace WideWorldImportersWebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
                             {
                                 options.AddPolicy("CorsPolicy", builder =>
                                                                     builder.AllowAnyOrigin()
                                                                            .AllowAnyMethod()
                                                                            .AllowAnyHeader());
                             });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services) { services.Configure<IISOptions>(options => { }); }

        public static void ConfigureLoggerService(this IServiceCollection services) { services.AddScoped<ILoggerManager, LoggerManager>(); }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddScoped<RepositoryContext>();

            bool logSqlServer = configuration.GetValue<bool>("AppConfigSettings:LogSqlServer");
            bool deleteLog = configuration.GetValue<bool>("AppConfigSettings:DeleteLog");

            if (deleteLog)
            {
                string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs");
                string fileName = $"{DateTime.Now.Year}-{DateTime.Now.Month:d2}-{DateTime.Now.Day:d2}.log";
                fileName = Path.Combine(filePath, fileName);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }

            if (env.IsDevelopment())
            {
                services.AddDbContext<WideWorldImportersDbContext>(options =>
                                                                   {
                                                                       options.UseSqlServer(configuration.GetConnectionString("sqlConnection"));

                                                                       if (logSqlServer)
                                                                       {
                                                                           ILogger logger = LogManager.GetCurrentClassLogger();
                                                                           options.LogTo(logger.Debug);
                                                                       }
                                                                   });
            }
            else
            {
                services.AddDbContext<WideWorldImportersDbContext>(options => { options.UseSqlServer(configuration.GetConnectionString("sqlConnection")); });
            }

        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) { services.AddScoped<IRepositoryManager, RepositoryManager>(); }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) { return builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter())); }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
                                      {
                                          opt.ReportApiVersions = true;
                                          opt.AssumeDefaultVersionWhenUnspecified = true;
                                          opt.DefaultApiVersion = new ApiVersion(1, 0);
                                          opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                                      });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
                                   {
                                       s.SwaggerDoc("v1", new OpenApiInfo
                                                          {
                                                              Title = "WideWorldImporters API v1",
                                                              Version = "v1",
                                                              Description = "Suppliers API by George McBath",
                                                              TermsOfService = new Uri("https://example.com/terms"),
                                                              Contact = new OpenApiContact
                                                                        {
                                                                            Name = "George McBath",
                                                                            Email = "george@georgemcbath.com",
                                                                            Url = new Uri("https://www.linkedin.com/in/georgemcbath/")
                                                                        },
                                                              License = new OpenApiLicense
                                                                        {
                                                                            Name = "Suppliers API LICX",
                                                                            Url = new Uri("https://www.linkedin.com/in/georgemcbath/")
                                                                        }
                                                          });

                                       var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                                       var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                                       s.IncludeXmlComments(xmlPath);

                                       s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                                         {
                                                                             In = ParameterLocation.Header,
                                                                             Description = "Place to add JWT with Bearer",
                                                                             Name = "Authorization",
                                                                             Type = SecuritySchemeType.ApiKey,
                                                                             Scheme = "Bearer"
                                                                         });

                                       s.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                                {
                                                                    {
                                                                        new OpenApiSecurityScheme
                                                                        {
                                                                            Reference = new OpenApiReference
                                                                                        {
                                                                                            Type = ReferenceType.SecurityScheme,
                                                                                            Id = "Bearer"
                                                                                        },
                                                                            Name = "Bearer"
                                                                        },
                                                                        new List<string>()
                                                                    }
                                                                });
                                   });
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
                                           {
                                               var newtonsoftJsonOutputFormatter = config.OutputFormatters
                                                                                         .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                                               if (newtonsoftJsonOutputFormatter != null)
                                               {
                                                   newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.gmcbath.hateoas+json");
                                                   newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.gmcbath.apiroot+json");
                                               }

                                               var xmlOutputFormatter = config.OutputFormatters
                                                                              .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                                               if (xmlOutputFormatter != null)
                                               {
                                                   xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.gmcbath.hateoas+xml");
                                                   xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.gmcbath.apiroot+xml");
                                               }
                                           });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) { services.AddResponseCaching(); }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddHttpCacheHeaders(
                                         expirationOpt =>
                                         {
                                             expirationOpt.MaxAge = 60;
                                             expirationOpt.CacheLocation = CacheLocation.Private;
                                         },
                                         validationOpt => { validationOpt.MustRevalidate = true; });
        }

        public static void ConfigureRateLimitingOptions(this IServiceCollection services, IConfiguration configuration)
        {
            int rateLimitPerPeriod = 10;

            bool isDebugRateLimitActive = configuration.GetValue<bool>("AppConfigSettings:IsDebugRateLimitActive");
            if (isDebugRateLimitActive)
            {
                rateLimitPerPeriod = 1000;
            }

            var rateLimitRules = new List<RateLimitRule>
                                 {
                                     // 10 requests are allowed per 2 minute period to the entire API
                                     new RateLimitRule
                                     {
                                         Endpoint = "*",
                                         Limit = rateLimitPerPeriod,
                                         Period = "2m"
                                     }
                                 };

            services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}