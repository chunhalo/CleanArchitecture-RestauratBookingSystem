using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels
{
    public class PagedListModel
    {
        public PagedList<List<ReturnPendingBooking>> pagedListpendingbooking { get; set; }
    }
}
