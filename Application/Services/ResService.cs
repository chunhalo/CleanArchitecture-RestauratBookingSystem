using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ResService : IResService
    {
        private readonly IResRepo _resRepo;
        public ResService(IResRepo resRepo)
        {
            _resRepo = resRepo;
        }
        public async Task<int> AddRes(ResAddModel res)
        {
            var getInt = await _resRepo.AddRes(res);
            return getInt;

        }

        public async Task<PagedList<List<Restaurant>>> GetActiveRestaurants(PaginationFilter paginationFilter)
        {
            var pagedList = await _resRepo.GetActiveRestaurants(paginationFilter);
            return pagedList;
        }

        public async Task<PagedList<List<ReturnResWithStatus>>> GetAllRestaurants(PaginationFilter paginationFilter)
        {
            var pagedList = await _resRepo.GetAllRestaurants(paginationFilter);
            return pagedList;
        }

        public async Task<List<RestaurantStatus>> GetAllRestaurantStatuses()
        {
            var statusList = await _resRepo.GetAllRestaurantStatuses();
            return statusList;
        }

        public async Task<Restaurant> GetRestaurantById(int ResId)
        {
            var res = await _resRepo.GetRestaurantById(ResId);
            return res;
        }

        public async Task<List<RestaurantStatus>> GetRestaurantStatuses()
        {
            var statusList = await _resRepo.GetRestaurantStatuses();
            return statusList;
        }

        public async Task UpdateRestaurant(RestaurantUpdateRequest restaurantUpdateRequest)
        {
            await _resRepo.UpdateRestaurant(restaurantUpdateRequest);
        }
    }
}
