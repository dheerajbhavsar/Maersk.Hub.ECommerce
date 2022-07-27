using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ECommerce.ProductCatalog;

public class ProductRepository : IProductRepository
{
    private readonly IReliableStateManager _stateManager;

    public ProductRepository(IReliableStateManager stateManager)
    {
        _stateManager = stateManager;
    }

    public async Task AddProduct(Product product)
    {
        var products = await _stateManager
            .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

        using var tx = _stateManager.CreateTransaction();
        await products.AddOrUpdateAsync(tx, product.Id, product, (id, product) => product);

        await tx.CommitAsync();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = await _stateManager
                   .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

        var result = new List<Product>();

        using var tx = _stateManager.CreateTransaction();
        var allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

        using var enumerator = allProducts.GetAsyncEnumerator();
        while (await enumerator.MoveNextAsync(CancellationToken.None))
        {
            KeyValuePair<Guid, Product> kvp = enumerator.Current;
            result.Add(kvp.Value);
        }

        return result;
    }

    public async Task<Product> GetProduct(Guid id)
    {
        var products = await _stateManager
           .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

        using var tx = _stateManager.CreateTransaction();
        ConditionalValue<Product> product = await products.TryGetValueAsync(tx, id);

        return product.HasValue ? product.Value : null;
    }
}
