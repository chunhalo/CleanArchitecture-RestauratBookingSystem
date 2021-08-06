using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IBookRepo
    {
        //IEnumerable<Booking> GetBooking();
        Task<PagedList<List<ReturnPendingBooking>>> GetPendingBooking(PaginationFilter paginationFilter);
        Task<Response> AddBooking(AddBookingModel addBookingModel, string username);
        Task<Response> SetConfirmBooking(int BookId);
        Task<Response> SetConfirmDate(int BookId);
        Task<PagedList<List<ReturnPendingBooking>>> searchPendingBooking(string searchText, PaginationFilter paginationFilter);

        Task<ReturnBookingWithIntStatus> GetBookingById(int BookId);
        Task<PagedList<List<ReturnPendingBooking>>> GetBookingByIdWithPageList(string searchText, PaginationFilter paginationFilter);
        Task<PagedList<List<ReturnPendingBooking>>> GetBookingByUsernameWithPageList(string searchText, PaginationFilter paginationFilter);
        Task<PagedList<List<ReturnPendingBooking>>> GetBookingWithPageList(PaginationFilter paginationFilter);
        Task<List<BookingStatus>> GetBookingStatuses();
        Task<CheckExistBooking> UpdateBookingDateTime(UpdateBookingDateTime updateBookingDateTime);
        Task UpdateBookingRequestStatus(UpdateBookingRequestStatus updateBookingRequestStatus);
        Task CancelBooking(int BookId);
    }
}
