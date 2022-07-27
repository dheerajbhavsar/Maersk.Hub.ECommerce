using ECommerce.API.Models;
using ECommerce.ProductCatalog.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace ECommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductCatalogService _service;

    public ProductsController()
    {
        var proxy = ServiceProxy.Create<IProductCatalogService>(
            new Uri("fabric:/Maersk.Hub.ECommerce/ECommerce.ProductCatalog"),
            new ServicePartitionKey(0));

        _service = proxy;
    }

    [HttpGet]
    public async Task<IEnumerable<ApiProduct>> GetAsync()
    {
        var products = await _service.GetAllProducts();

        return products.Select(p => new ApiProduct
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            IsAvailbility = p.Availbility > 0
        });
    }

    [HttpPost]
    public async Task PostAsync(ApiProduct product)
    {
        var newProduct = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Availbility = 100
        };

        await _service.AddProductAsync(newProduct);
    }
}
