using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModels
{
    public class BookViewModel
    {
        public IEnumerable<Booking> Booking { get; set; }
    }
}
