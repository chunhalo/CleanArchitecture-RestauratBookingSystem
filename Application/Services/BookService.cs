using AnnouncementLibrary;
using Application.Interfaces;
using Application.ViewModels;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BookService : IBookService
    {
        public IBookRepo _bookRepo;
        public BookService(IBookRepo bookRepo)
        {
            _bookRepo = bookRepo;
        }

        public async Task<ResponseViewModel> AddBooking(AddBookingModel addBookingModel, string username)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _bookRepo.AddBooking(addBookingModel, username);
            return responseViewModel;
        }

        public async Task CancelBooking(int BookId)
        {

           await _bookRepo.CancelBooking(BookId);
        }

        public async Task<BookingWithIntStatusViewModel> GetBookingById(int bookId)
        {
            BookingWithIntStatusViewModel bookingWithIntStatusViewModel = new BookingWithIntStatusViewModel();
            bookingWithIntStatusViewModel.returnBookingWithIntStatus =  await _bookRepo.GetBookingById(bookId);
            return bookingWithIntStatusViewModel;
        }

        public async Task<PagedListModel> GetBookingByIdWithPageList(string searchText, PaginationFilter paginationFilter)
        {
            PagedListModel pagedListModel = new PagedListModel();
            pagedListModel.pagedListpendingbooking = await _bookRepo.GetBookingByIdWithPageList(searchText,paginationFilter);
            return pagedListModel;

        }

        public async Task<PagedListModel> GetBookingByUsernameWithPageList(string searchText, PaginationFilter paginationFilter)
        {
            PagedListModel pagedListModel = new PagedListModel();
            pagedListModel.pagedListpendingbooking = await _bookRepo.GetBookingByUsernameWithPageList(searchText, paginationFilter);
            return pagedListModel;
        }

        public async Task<BookingStatusesViewModel> GetBookingStatuses()
        {
            BookingStatusesViewModel bookingStatusesViewModel = new BookingStatusesViewModel();
            bookingStatusesViewModel.bookingstatuses = await _bookRepo.GetBookingStatuses();
            return bookingStatusesViewModel;
        }

        public async Task<PagedListModel> GetBookingWithPageList(PaginationFilter paginationFilter)
        {
            PagedListModel pagedListModel = new PagedListModel();
            pagedListModel.pagedListpendingbooking = await _bookRepo.GetBookingWithPageList(paginationFilter);
            return pagedListModel;
        }

        public async Task<PagedListModel> GetPendingBooking(PaginationFilter paginationFilter)
        {
            PagedListModel pagedListModel = new PagedListModel();
            pagedListModel.pagedListpendingbooking = await _bookRepo.GetPendingBooking(paginationFilter);
            return pagedListModel;
        }

        public async Task<PagedListModel> searchPendingBooking(string searchText, PaginationFilter paginationFilter)
        {
            PagedListModel pagedListModel = new PagedListModel();
            pagedListModel.pagedListpendingbooking = await _bookRepo.searchPendingBooking(searchText, paginationFilter);
            return pagedListModel;
        }


        public async Task<ResponseViewModel> SetConfirmBooking(int BookId)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _bookRepo.SetConfirmBooking(BookId);
            return responseViewModel;
        }

        public async Task<ResponseViewModel> SetConfirmDate(int BookId)
        {
            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.response = await _bookRepo.SetConfirmDate(BookId);
            return responseViewModel;
        }

        public async Task<CheckExistBooking> UpdateBookingDateTime(UpdateBookingDateTime updateBookingDateTime)
        {
            CheckExistBooking checkExistBooking = await _bookRepo.UpdateBookingDateTime(updateBookingDateTime);
            return checkExistBooking;

        }

        public async Task UpdateBookingRequestStatus(UpdateBookingRequestStatus updateBookingRequestStatus)
        {
            await _bookRepo.UpdateBookingRequestStatus(updateBookingRequestStatus);
        }
        //public BookViewModel GetBooking()
        //{
        //    return new BookViewModel()
        //    {
        //        Booking = _bookRepo.GetBooking()
        //    };
        //}
    }
}
