using System;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using System.IO;

[assembly: OwinStartup(typeof(OwinApp.Startup))]

namespace OwinApp
{
	public class Startup
	{
		//The Order of the app.Map, WebApi, app.Run is important
		public void Configuration(IAppBuilder app)
		{
			app.Use((context, next) =>
			{
				var output = context.Get<TextWriter>("host.TraceOutput");
				return next().ContinueWith(result => output.WriteLine("Scheme {0} : Method {1} : Path {2} : MS {3}",
						context.Request.Scheme, context.Request.Method, context.Request.Path, DateTime.Now.Millisecond));
			});

			//http://localhost:50823/test
			app.Map("/test", builder =>
				{
					builder.Run(context =>
						{
							return context.Response.WriteAsync("default route");
						});
				});

			//http://localhost:50823/index
			app.Map("/index", builder =>
			{
				builder.Run(async context =>
				{
					context.Response.Redirect("\\views\\index.html");
				});
			});

			//WebApi
			//http://localhost:50823/api/trades/5
			var config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
			app.UseWebApi(config);

			//Default page routing
			//http://localhost:50823
			app.Run(async context =>
			{
				context.Response.Redirect("\\views\\index.html");
			});
		}
	}
}