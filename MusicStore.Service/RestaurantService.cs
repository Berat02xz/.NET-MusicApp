using MusicStore.Domain.RestaurantAppModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Service
{
    public class RestaurantService
    {
        private readonly HttpClient _httpClient;

        public RestaurantService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Restaurant>> GetAllRestaurantsAsync()
        {
            var response = await _httpClient.GetAsync("https://dinemaster.azurewebsites.net/api/RestaurantApi/GetAllRestaurants");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var restaurants = JsonConvert.DeserializeObject<List<Restaurant>>(jsonData);
                return restaurants;
            }

            return new List<Restaurant>(); 
        }

    }
}
