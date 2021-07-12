using System.IO;
using AspNetCoreRateLimit;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Repository.DataShaping;
using WideWorldImportersWebApi.ActionFilters;
using WideWorldImportersWebApi.Extensions;
using WideWorldImportersWebApi.Utility;

namespace WideWorldImporters.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/NLog.config"));
            Configuration = configuration;
            WebHostingEnvironment = environment;

            // todo - comment this line out if not using Hibernating Rhinos EF Profiler
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<AppConfigSettings>().Bind(Configuration.GetSection("AppConfigSettings"));

            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLoggerService();
            services.ConfigureSqlContext(Configuration, WebHostingEnvironment);
            services.ConfigureRepositoryManager();

            services.AddAutoMapper(typeof(Startup));

            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateSupplierCategoryExistsAttribute>();
            services.AddScoped<ValidateSupplierCategoryForSupplierExistsAttribute>();
            services.AddScoped<ValidateSupplierTransactionExistsAttribute>();

            services.AddScoped<IDataShaper<SupplierTransactionDto>, DataShaper<SupplierTransactionDto>>();
            services.AddScoped<ValidateMediaTypeAttribute>();

            services.AddScoped<SupplierTransactionLinks>();

            services.ConfigureVersioning();

            services.ConfigureResponseCaching();
            services.ConfigureHttpCacheHeaders();

            services.AddMemoryCache();

            services.ConfigureRateLimitingOptions(Configuration);
            services.AddHttpContextAccessor();

            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            services.ConfigureSwagger();

            services.AddControllers(config =>
                                    {
                                        config.RespectBrowserAcceptHeader = true;
                                        config.ReturnHttpNotAcceptable = true;
                                        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile {Duration = 120});
                                    })
                    .AddNewtonsoftJson()
                    .AddXmlDataContractSerializerFormatters()
                    .AddCustomCSVFormatter();

            services.AddHealthChecks();

            services.AddCustomMediaTypes();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.ConfigureExceptionHandler(logger);

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
                                    {
                                        ForwardedHeaders = ForwardedHeaders.All
                                    });

            app.UseResponseCaching();
            app.UseHttpCacheHeaders();

            app.UseIpRateLimiting();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "WideWorldImporters API v1"); });

            app.UseEndpoints(endpoints =>
                             {
                                 endpoints.MapHealthChecks("/healthcheck");
                                 endpoints.MapControllers();
                             });
        }
    }
}