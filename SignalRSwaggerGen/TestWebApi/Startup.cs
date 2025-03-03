using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using TestWebApi.Hubs;

namespace TestWebApi
{
	public class Filter : IOperationFilter, IParameterFilter, IRequestBodyFilter, IOperationProcessor
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
		}
		public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
		{
		}
		public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
		{
		}

		public bool Process(OperationProcessorContext context)
		{
			return true;
		}
	}

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

			services.AddOpenApiDocument(settings =>
			{
				settings.DocumentName = "controllers";
				settings.Description = "Description.NSwag";
				settings.Title = "Title.NSwag";
				settings.Version = "v1";

				var securityScheme = new NSwag.OpenApiSecurityScheme
				{
					In = NSwag.OpenApiSecurityApiKeyLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = NSwag.OpenApiSecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = JwtBearerDefaults.AuthenticationScheme
				};
				var securityScheme2 = new NSwag.OpenApiSecurityScheme
				{
					In = NSwag.OpenApiSecurityApiKeyLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = NSwag.OpenApiSecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Basic"
				};
				settings.AddSecurity(securityScheme.Scheme, securityScheme);
				settings.AddSecurity(securityScheme2.Scheme, securityScheme2);

				settings.AddSignalRSwaggerGen(o =>
				{
					o.UseXmlComments("TestWebApi.xml");
					o.DisregardOtherSecurityRequirements = true;
					o.AddOperationProcessor(new Filter());
					o.HubMethodsScan = SignalRSwaggerGen.Enums.HubMethodsScan.Default;
				});
			});
			services.AddOpenApiDocument(settings =>
			{
				settings.DocumentName = "hubs";
				settings.Description = "Description.NSwag";
				settings.Title = "Title.NSwag";
				settings.Version = "v1";

				var securityScheme = new NSwag.OpenApiSecurityScheme
				{
					In = NSwag.OpenApiSecurityApiKeyLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = NSwag.OpenApiSecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = JwtBearerDefaults.AuthenticationScheme
				};
				var securityScheme2 = new NSwag.OpenApiSecurityScheme
				{
					In = NSwag.OpenApiSecurityApiKeyLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = NSwag.OpenApiSecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Basic"
				};
				settings.AddSecurity(securityScheme.Scheme, securityScheme);
				settings.AddSecurity(securityScheme2.Scheme, securityScheme2);

				settings.AddSignalRSwaggerGen(o =>
				{
					o.UseXmlComments("TestWebApi.xml");
					o.DisregardOtherSecurityRequirements = true;
					o.AddOperationProcessor(new Filter());
					o.HubMethodsScan = SignalRSwaggerGen.Enums.HubMethodsScan.Default;
				});
			});

			//services.AddSwaggerGen(options =>
			//{
			//	var apiInfo = new OpenApiInfo { Title = "TestWebApi", Version = "v1" };
			//	options.SwaggerDoc("controllers", apiInfo);
			//	options.SwaggerDoc("hubs", apiInfo);
			//	options.IncludeXmlComments("TestWebApi.xml", true);

			//	var securityScheme = new OpenApiSecurityScheme
			//	{
			//		In = ParameterLocation.Header,
			//		Description = "Please enter a valid token",
			//		Name = "Authorization",
			//		Type = SecuritySchemeType.Http,
			//		BearerFormat = "JWT",
			//		Scheme = JwtBearerDefaults.AuthenticationScheme
			//	};
			//	var securityScheme2 = new OpenApiSecurityScheme
			//	{
			//		In = ParameterLocation.Header,
			//		Description = "Please enter a valid token",
			//		Name = "Authorization",
			//		Type = SecuritySchemeType.Http,
			//		BearerFormat = "JWT",
			//		Scheme = "Basic"
			//	};
			//	options.AddSecurityDefinition(securityScheme.Scheme, securityScheme);
			//	options.AddSecurityDefinition(securityScheme2.Scheme, securityScheme2);
			//	options.AddSecurityRequirement(new OpenApiSecurityRequirement
			//	{
			//		{
			//			new OpenApiSecurityScheme
			//			{
			//				Reference = new OpenApiReference
			//				{
			//					Type = ReferenceType.SecurityScheme,
			//					Id = securityScheme.Scheme,
			//				}
			//			},
			//			Array.Empty<string>()
			//		}
			//	});

			//	options.AddSignalRSwaggerGen(o =>
			//	{
			//		o.UseXmlComments("TestWebApi.xml");
			//		o.DisregardOtherSecurityRequirements = true;
			//		o.AddOperationFilter(new Filter());
			//		o.AddParameterFilter(new Filter());
			//		o.AddRequestBodyFilter(new Filter());
			//		//o.HubMethodsScan = SignalRSwaggerGen.Enums.HubMethodsScan.IncludeInherited;
			//		//o.IgnoreMethodsInheritedFromTypes(typeof(Hub), typeof(IAsyncDisposable), typeof(IDisposable));
			//	});
			//});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage()
					//.UseSwagger()
					//.UseSwaggerUI(options =>
					//{
					//	options.SwaggerEndpoint("/swagger/controllers/swagger.json", "REST API");
					//	options.SwaggerEndpoint("/swagger/hubs/swagger.json", "SignalR");
					//})
					.UseOpenApi()
					.UseSwaggerUi()
					;
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
