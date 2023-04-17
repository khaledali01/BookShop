using API.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]

	public class ProductsController : ControllerBase
	{
		private readonly IGenericRepository<Product> productsRepo;
		private readonly IGenericRepository<ProductType> productTypeRepo;
		private readonly IGenericRepository<ProductBrand> productBrandRepo;

		public ProductsController(
			IGenericRepository<Product> productsRepo,
			IGenericRepository<ProductType> productTypeRepo,
			IGenericRepository<ProductBrand> productBrandRepo
			)
		{
			this.productsRepo = productsRepo;
			this.productTypeRepo = productTypeRepo;
			this.productBrandRepo = productBrandRepo;
		}

		[HttpGet]
		public async Task<ActionResult<List<ProductToReturnDto>>> GetProducts()
		{
			var spec = new ProductsWithTypesAndBrandsSpecification();

			var products = await productsRepo.ListAsync(spec);

			return products.Select(product => new ProductToReturnDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				PictureUrl = product.PictureUrl,
				Price = product.Price,
				ProductBrand = product.ProductBrand.Name,
				ProductType = product.ProductType.Name,
			}).ToList();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			var spec = new ProductsWithTypesAndBrandsSpecification(id);

			var product = await productsRepo.GetEntityWithSpec(spec);

			return new ProductToReturnDto
			{
				Id = product.Id,
				Name = product.Name,
				Description = product.Description,
				PictureUrl = product.PictureUrl,
				Price = product.Price,
				ProductBrand = product.ProductBrand.Name,
				ProductType = product.ProductType.Name,
			};
		}

		[HttpGet("brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
		{
			return Ok(await productBrandRepo.ListAllAsync());
		}

		[HttpGet("types")]
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
		{
			return Ok(await productTypeRepo.ListAllAsync());
		}


	}
}