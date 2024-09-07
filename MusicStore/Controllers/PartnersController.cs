using Microsoft.AspNetCore.Mvc;
using MusicStore.Service;

namespace MusicStore.Web.Controllers
{
    public class PartnersController : Controller
    {
        private readonly RestaurantService _restaurantService;

        public PartnersController(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }
        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantService.GetAllRestaurantsAsync();

            return View(restaurants);
        }
    }
}
