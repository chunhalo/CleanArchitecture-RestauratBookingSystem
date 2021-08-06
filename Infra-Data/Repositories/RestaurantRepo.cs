using Domain.Interfaces;
using Domain.Models;
using Infra_Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra_Data.Repositories
{
    public class RestaurantRepo : IResRepo
    {
        private readonly RestaurantBookingContext _context;
        public static IWebHostEnvironment _environment;
        public RestaurantRepo(IWebHostEnvironment environment, RestaurantBookingContext context)
        {
            _environment = environment;
            _context = context;
        }
        public async Task<int> AddRes(ResAddModel res)
        {
            try
            {
                
                var resExist = _context.Restaurants.Where(x => x.Name == res.Name).FirstOrDefault();
                if (resExist != null)
                {
                    return 0;
                }
                Restaurant newRes = new Restaurant();
                if (res.Image != null)
                {
                    var fileName = Path.GetFileName(res.Image.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images\\Restaurant\\", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))

                    {
                        await res.Image.CopyToAsync(fileStream);
                    }

                    newRes.Name = res.Name;
                    newRes.Address = res.Address;
                    newRes.Phone = res.Phone;
                    newRes.Image = fileName;

                    newRes.Status = 2;
                    newRes.Description = res.Description;
                    newRes.OperationStart = res.OperationStart;
                    newRes.OperationEnd = res.OperationEnd;
                    _context.Restaurants.Add(newRes);
                    await _context.SaveChangesAsync();

                }

                List<Table> table1 = new List<Table>();
                Table newtable = new Table();
                newtable.TableNo = 1;
                newtable.ResId = newRes.ResId;
                newtable.Accommodate = 0;
                newtable.Status = 1;
                table1.Add(newtable);
                TableRepo tablerepo = new TableRepo();
                await tablerepo.UpdateTable(table1);


                return newRes.ResId;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public async Task<PagedList<List<Restaurant>>> GetActiveRestaurants(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Restaurants.Where(x => x.Status == 1).Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Restaurants.Where(x => x.Status == 1).CountAsync();

            return new PagedList<List<Restaurant>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<PagedList<List<ReturnResWithStatus>>> GetAllRestaurants(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Restaurants.Include(x => x.StatusNavigation)
                .Select(x => new ReturnResWithStatus
                {
                    ResId = x.ResId,
                    Address = x.Address,
                    Phone = x.Phone,
                    Image = x.Image,
                    Description = x.Description,
                    Name = x.Name,
                    OperationEnd = x.OperationEnd,
                    OperationStart = x.OperationStart,
                    restaurantStatus = x.StatusNavigation
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Restaurants.CountAsync();

            return new PagedList<List<ReturnResWithStatus>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<Restaurant> GetRestaurantById(int ResId)
        {
            var restaurant = await _context.Restaurants.FindAsync(ResId);
            return restaurant;
        }

        public async Task<List<RestaurantStatus>> GetRestaurantStatuses()
        {
            var restaurantStatuses = await _context.RestaurantStatuses.Where(x => x.StatusName == "active" || x.StatusName == "inactive").ToListAsync();
            return restaurantStatuses;
        }

        public async Task<List<RestaurantStatus>> GetAllRestaurantStatuses()
        {
            var restaurantStatuses = await _context.RestaurantStatuses.ToListAsync();
            return restaurantStatuses;
        }

        public async Task UpdateRestaurant(RestaurantUpdateRequest restaurantUpdateRequest)
        {
            if (_context.Restaurants.Any(x => x.ResId == restaurantUpdateRequest.ResId))
            {
                Restaurant UpdatedRestaurant = new Restaurant();
                if (restaurantUpdateRequest.ImageFile != null)
                {
                    var a = _environment.WebRootPath;
                    var fileName = Path.GetFileName(restaurantUpdateRequest.ImageFile.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "images\\Restaurant\\", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))

                    {
                        await restaurantUpdateRequest.ImageFile.CopyToAsync(fileStream);
                    }
                    restaurantUpdateRequest.Image = fileName;
                }
                var getDbRes = _context.Restaurants.Where(x => x.ResId == restaurantUpdateRequest.ResId).AsNoTracking().FirstOrDefault();

                UpdatedRestaurant.ResId = restaurantUpdateRequest.ResId;
                UpdatedRestaurant.Name = restaurantUpdateRequest.Name;
                UpdatedRestaurant.Address = restaurantUpdateRequest.Address;
                UpdatedRestaurant.Phone = restaurantUpdateRequest.Phone;
                UpdatedRestaurant.Description = restaurantUpdateRequest.Description;
                UpdatedRestaurant.OperationStart = restaurantUpdateRequest.OperationStart;
                UpdatedRestaurant.OperationEnd = restaurantUpdateRequest.OperationEnd;
                UpdatedRestaurant.Image = restaurantUpdateRequest.Image;
                UpdatedRestaurant.Status = restaurantUpdateRequest.Status;

                try
                {
                    _context.Entry(UpdatedRestaurant).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
