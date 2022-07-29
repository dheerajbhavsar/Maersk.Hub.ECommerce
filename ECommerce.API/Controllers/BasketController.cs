using ECommerce.API.Models;
using ECommerce.ProductCatalog.Model;
using ECommerce.UserActor.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {

        [HttpGet("{userId}")]
        public async Task<ApiBasket> GetAsync(string userId)
        {
            var actor = GetActor(userId);
            var products = await actor.GetBasket();

            return new ApiBasket
            {
                UserId = userId,
                Items = products.Select(
                    p => new ApiBasketItem
                    {
                        ProductId = p.ProductId.ToString(),
                        Quantity = p.Quantity
                    }).ToArray()
            };
        }

        [HttpPost("userId")]
        public async Task AddAsync(string userId, ApiBasketAddRequest request)
        {
            var actor = GetActor(userId);
            await actor.AddToBasket(request.ProductId, request.Quantity);
        }

        [HttpDelete("{userId}")]
        public async Task DeleteAsync(string userId)
        {
            var actor = GetActor(userId);
            await actor.ClearBasket();
        }

        private static IUserActor GetActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(
                new ActorId(userId),
                new Uri("fabric:/Maersk.Hub.ECommerce/UserActorService"));
        }


    }


}
