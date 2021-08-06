using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IResRepo
    {
        Task<PagedList<List<ReturnResWithStatus>>> GetAllRestaurants(PaginationFilter paginationFilter);
        Task<PagedList<List<Restaurant>>> GetActiveRestaurants(PaginationFilter paginationFilter);
        Task<int> AddRes(ResAddModel res);
        Task<Restaurant> GetRestaurantById(int ResId);
        Task<List<RestaurantStatus>> GetRestaurantStatuses();
        Task<List<RestaurantStatus>> GetAllRestaurantStatuses();
        Task UpdateRestaurant(RestaurantUpdateRequest restaurantUpdateRequest);
    }
}
