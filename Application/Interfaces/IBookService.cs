using AnnouncementLibrary;
using Application.ViewModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBookService
    {
        Task<PagedListModel> GetPendingBooking(PaginationFilter paginationFilter);

        Task<ResponseViewModel> AddBooking(AddBookingModel addBookingModel, string username);
        Task<ResponseViewModel> SetConfirmBooking(int BookId);
        Task<ResponseViewModel> SetConfirmDate(int BookId);

        Task CancelBooking(int BookId);

        Task<PagedListModel> searchPendingBooking(string searchText, PaginationFilter paginationFilter);
        Task<BookingWithIntStatusViewModel> GetBookingById(int bookId);
        Task<BookingStatusesViewModel> GetBookingStatuses();
        Task<PagedListModel> GetBookingByIdWithPageList(string searchText, PaginationFilter paginationFilter);
        Task<PagedListModel> GetBookingByUsernameWithPageList(string searchText, PaginationFilter paginationFilter);
        Task<PagedListModel> GetBookingWithPageList(PaginationFilter paginationFilter);
        Task<CheckExistBooking> UpdateBookingDateTime(UpdateBookingDateTime updateBookingDateTime);
        Task UpdateBookingRequestStatus(UpdateBookingRequestStatus updateBookingRequestStatus);

    }
}
