using API.Errors;
using API.Helpers;
using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "BookStore API",
		Version = "v1"
	});
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<StoreContext>(x =>
 x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.InvalidModelStateResponseFactory = actionContext =>
	{
		var errors = actionContext.ModelState
			.Where(e => e.Value.Errors.Count > 0)
			.SelectMany(x => x.Value.Errors)
			.Select(x => x.ErrorMessage).ToArray();

		var errorResponse = new ApiValidation
		{
			Errors = errors
		};

		return new BadRequestObjectResult(errorResponse);
	};
});

var app = builder.Build();



// Configure the HTTP request pipeline.



app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	//app.UseSwaggerUI();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API v1");
	});
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var loggerFactory = services.GetRequiredService<ILoggerFactory>();
	try
	{
		var context = services.GetRequiredService<StoreContext>();
		await context.Database.MigrateAsync();
		await StoreContextSeed.SeedAsync(context, loggerFactory);
	}
	catch (Exception ex)
	{
		var logger = loggerFactory.CreateLogger<Program>();
		logger.LogError(ex, "An Error occured during migration");
	}
	app.Run();
}
