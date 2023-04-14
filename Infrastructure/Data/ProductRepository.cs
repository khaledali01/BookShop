using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
	public class ProductRepository : IProductRepository
	{
		private readonly StoreContext context;

		public ProductRepository(StoreContext context)
		{
			this.context = context;
		}

		public async Task<Product> GetProductByIdAsync(int id)
		{
			return await context.Products.FindAsync(id);
		}

		public async Task<IReadOnlyList<Product>> GetProductsAsync()
		{
			return await context.Products.ToListAsync();
		}
	}
}
