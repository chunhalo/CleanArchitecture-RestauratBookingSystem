using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAnnounceRepo
    {
        Task PostAnnouncement(AnnouncementAddModel announcementAddModel, string username);
        Task<PagedList<List<ReturnAnnouncement>>> GetAllAnnouncement(PaginationFilter paginationFilter);
    }
}
