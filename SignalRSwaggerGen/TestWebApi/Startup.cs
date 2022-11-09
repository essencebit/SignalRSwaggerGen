using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace TestWebApi
{
	internal class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(options =>
			{
				var apiInfo = new OpenApiInfo { Title = "TestWebApi", Version = "v1" };
				options.SwaggerDoc("controllers", apiInfo);
				options.SwaggerDoc("hubs", apiInfo);
				options.IncludeXmlComments("TestWebApi.xml", true);

				var securityScheme = new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = JwtBearerDefaults.AuthenticationScheme
				};
				var securityScheme2 = new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Basic"
				};
				options.AddSecurityDefinition(securityScheme.Scheme, securityScheme);
				options.AddSecurityDefinition(securityScheme2.Scheme, securityScheme2);
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = securityScheme.Scheme,
							}
						},
						Array.Empty<string>()
					}
				});

				options.AddSignalRSwaggerGen(o =>
				{
					o.UseXmlComments("TestWebApi.xml");
					o.DisregardOtherSecurityRequirements = true;
				});
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage()
					.UseSwagger()
					.UseSwaggerUI(options =>
					{
						options.SwaggerEndpoint("/swagger/controllers/swagger.json", "REST API");
						options.SwaggerEndpoint("/swagger/hubs/swagger.json", "SignalR");
					});
			}

			app.UseHttpsRedirection()
				.UseRouting()
				.UseAuthentication()
				.UseAuthorization()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapControllers();
				});
		}
	}
}
