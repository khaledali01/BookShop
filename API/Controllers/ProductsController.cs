using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class ProductsController : BaseApiController
	{
		private readonly IGenericRepository<Product> productsRepo;
		private readonly IGenericRepository<ProductType> productTypeRepo;
		private readonly IGenericRepository<ProductBrand> productBrandRepo;
		private readonly IMapper mapper;

		public ProductsController(
			IGenericRepository<Product> productsRepo,
			IGenericRepository<ProductType> productTypeRepo,
			IGenericRepository<ProductBrand> productBrandRepo, 
			IMapper mapper
			)
		{
			this.productsRepo = productsRepo;
			this.productTypeRepo = productTypeRepo;
			this.productBrandRepo = productBrandRepo;
			this.mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
			[FromQuery] ProductSpecParams productParams)
		{
			var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

			var countSpec = new ProductWithFiltersForCountSpecification(productParams);

			var totalItems = await productsRepo.CountAsync(countSpec);

			var products = await productsRepo.ListAsync(spec);

			var data = mapper
				.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

			return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex, 
				productParams.PageSize,
				totalItems,
				data
				));
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			var spec = new ProductsWithTypesAndBrandsSpecification(id);

			var product = await productsRepo.GetEntityWithSpec(spec);

			if (product == null) return NotFound(new ApiResponse(404));

			return mapper.Map<Product, ProductToReturnDto>(product);
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