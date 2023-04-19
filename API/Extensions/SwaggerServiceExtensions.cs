using System.Runtime.CompilerServices;

namespace API.Extensions
{
	public static class SwaggerServiceExtensions
	{
		public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection Services)
		{
			Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
				{
					Title = "BookStore API",
					Version = "v1"
				});
			});

			return Services;
		}

	}

	
}
