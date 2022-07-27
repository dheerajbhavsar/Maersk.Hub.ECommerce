using Microsoft.ServiceFabric.Services.Remoting;

namespace ECommerce.ProductCatalog.Model;

public interface IProductCatalogService : IService
{
    Task<Product[]> GetAllProducts();

    Task AddProductAsync(Product product);

    Task<Product> GetProductAsync(Guid productId);
}
