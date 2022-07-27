using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ECommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        private IProductRepository _repo;

        public async Task AddProductAsync(Product product)
        {
            await _repo.AddProduct(product);
        }

        public async Task<Product[]> GetAllProducts()
        {
            return (await _repo.GetAllAsync()).ToArray();
        }

        public async Task<Product> GetProductAsync(Guid productId)
        {
            return await _repo.GetProduct(productId);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repo = new ProductRepository(StateManager);

            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop 1",
                Description = "Laptop Description",
                Price = 150,
                Availbility = 12
            };
            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop 2",
                Description = "Laptop Description",
                Price = 120,
                Availbility = 12
            };
            var product3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Laptop 3",
                Description = "Laptop Description",
                Price = 130,
                Availbility = 12
            };

            await _repo.AddProduct(product3);
            await _repo.AddProduct(product2);
            await _repo.AddProduct(product1);
        }
    }
}
