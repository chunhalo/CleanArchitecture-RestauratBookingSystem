using AnnouncementLibrary;
using Domain.Interfaces;
using Domain.Models;
using EasyNetQ;
using Infra_Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra_Data.Repositories
{
    public class AnnounceRepo: IAnnounceRepo
    {
        private readonly RestaurantBookingContext _context;
        public AnnounceRepo(RestaurantBookingContext context)
        {
            _context = context;
        }

        public async Task<PagedList<List<ReturnAnnouncement>>> GetAllAnnouncement(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Announcements
                .Select(x => new ReturnAnnouncement
                {
                    Title = x.Title,
                    Description = x.Description,
                    Username = x.User.UserName,
                    Date = x.Date
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Announcements.CountAsync();

            return new PagedList<List<ReturnAnnouncement>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }
        public async Task PostAnnouncement(AnnouncementAddModel announcementAddModel, string username)
        {
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
            Announcement NewAnnouncement = new Announcement();

            NewAnnouncement.Title = announcementAddModel.Title;
            NewAnnouncement.Description = announcementAddModel.Description;
            NewAnnouncement.Date = DateTime.Now;
            NewAnnouncement.UserId = getUserId.Id;


            _context.Announcements.Add(NewAnnouncement);
            await _context.SaveChangesAsync();
            AnnouncementClass announcementClass = new AnnouncementClass();
            announcementClass.Id = NewAnnouncement.Id;
            announcementClass.Title = NewAnnouncement.Title;
            announcementClass.Description = NewAnnouncement.Description;

            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.PubSub.Publish(announcementClass);


        }
    }
}
