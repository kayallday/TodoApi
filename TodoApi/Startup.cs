using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using System.Reflection;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// requires using TodoApi.Models; and 
		// using Microsoft.EntityFrameworkCore;
		public void ConfigureServices(IServiceCollection services)
		{
            //commented out because in Swagger tutorial this line of code doesn't exist
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            // Add framework services.
            services.AddMvc();

			services.AddLogging();

			// Some parts of the tutorial have AddSingleton while others have AddScoped? Arguement in comments also AddTransient?
			services.AddSingleton<ITodoRepository, TodoRepository>();

			services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase()).AddSingleton<TodoContext, TodoContext>();

			// Register the Swagger generator, defining one or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info
				{
					Version = "v1",
					Title = "ToDo API",
					Description = "A simple example ASP.NET Core Web API",
					TermsOfService = "None",
					Contact = new Contact { Name = "Kelsey Beard", Email = "kbeard@apterainc.com", Url = "https://twitter.com/kaybeard" },
					License = new License { Name = "Use under LICX", Url = "https://example.com/license" }
				});

				//Set the comments path for the swagger json and ui
				var basePath = PlatformServices.Default.Application.ApplicationBasePath;
				var xmlPath = Path.Combine(basePath, "TodoApi.xml");
				c.IncludeXmlComments(xmlPath);
			});
		}
			

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

			

			// Enable static files middleware. 
			app.UseStaticFiles();
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

			app.UseMvcWithDefaultRoute();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				//c.Asset("index", Assembly.GetEntryAssembly(), "YourWebApiProject.SwaggerExtensions.index.html");
			});

            app.UseMvc();
		}
    }
}
