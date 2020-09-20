using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BandAPI.DbContexts;
using BandAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace BandAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // the WebAPI is still a MVC application
            services.AddControllers(setupAction =>
            {
                // If the return type is not acceptable (application/json || application/xml)
                setupAction.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(setupAction => // The first one will be default (XML)
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // For JsonPatchDocument
            })
            .AddXmlDataContractSerializerFormatters(); // adding the XML support

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // Adding autoMapper service

            services.AddScoped<IBandAlbumRepository, BandAlbumRepository>(); // Adding the service for the repository 

            services.AddScoped<IPropertyMappingService, PropertyMappingService>(); // Adding the ordering service

            services.AddScoped<IPropertyValidationService, PropertyValidationService>(); // Addding the validation of the requiring fields

            services.AddCors(setupActions => setupActions.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));


            services.AddDbContext<BandAlbumContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                } // specify the connection string
            ); // adding the dependency of DbContext
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async c =>
                    {
                        c.Response.StatusCode = 500;
                        await c.Response.WriteAsync("Something went wrong, try again later.");

                    });
                });
            }

            // app.UseHttpsRedirection(); // only need this for https, we're currently working for HTTP only



            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
