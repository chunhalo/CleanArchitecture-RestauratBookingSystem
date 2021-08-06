using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private IResService _resService;
        public RestaurantController(IResService resService)
        {
            _resService = resService;
        }

        [HttpGet]
        [Route("GetAllRestaurants")]
        public async Task<ActionResult<PagedList<List<ReturnResWithStatus>>>> GetAllRestaurants([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<ReturnResWithStatus>> pagedList = await _resService.GetAllRestaurants(paginationFilter);
            return pagedList;
        }

        [HttpGet]
        [Route("ActiveRestaurants")]
        public async Task<ActionResult<PagedList<List<Restaurant>>>> GetActiveRestaurants([FromQuery] PaginationFilter paginationFilter)
        {
            PagedList<List<Restaurant>> pagedList = await _resService.GetActiveRestaurants(paginationFilter);
            return pagedList;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurantById(int id)
        {
            Restaurant res = await _resService.GetRestaurantById(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);

        }


        [HttpGet]
        [Route("GetRestaurantStatuses")]
        public async Task<ActionResult<List<BookingStatus>>> GetRestaurantStatuses()
        {
            var restaurantStatuses = await _resService.GetRestaurantStatuses();
            return Ok(restaurantStatuses);
        }

        [HttpGet]
        [Route("GetAllRestaurantStatuses")]
        public async Task<ActionResult<List<BookingStatus>>> GetAllRestaurantStatuses()
        {
            var restaurantStatuses = await _resService.GetAllRestaurantStatuses();
            return Ok(restaurantStatuses);
        }
        [HttpPost]
        public async Task<ActionResult<int>> PostRes([FromForm] ResAddModel res)
        {

            int resId = await _resService.AddRes(res);
            if (resId != 0)
            {
                return Ok(resId);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromForm] RestaurantUpdateRequest restaurantUpdateRequest)
        {

            await _resService.UpdateRestaurant(restaurantUpdateRequest);
            return Ok();
        }

    }
}
