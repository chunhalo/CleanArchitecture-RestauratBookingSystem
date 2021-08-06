using AnnouncementLibrary;
using Domain.Interfaces;
using Domain.Models;
using Infra_Data.Context;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Infra_Data.Repositories
{
    public class BookRepo: IBookRepo
    {
        private readonly RestaurantBookingContext _context;
        
        public BookRepo(RestaurantBookingContext context)
        {
            
            _context = context;
        }
        //public IEnumerable<Booking> GetBooking()
        //{
        //    return _context.Booking;
        //}

        public async Task<PagedList<List<ReturnPendingBooking>>> GetPendingBooking(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.OrderBy(x => x.StartDate)
                .Where(x => x.Status == 1)
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.Where(x => x.Status == 1).CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<Response> AddBooking(AddBookingModel addBookingModel, string username)
        {

            var getstartDate = Convert.ToDateTime(addBookingModel.StartDate);
            var getendDate = Convert.ToDateTime(addBookingModel.EndDate);
            var getUserId = _context.AspNetUsers.Where(x => x.UserName == username).FirstOrDefault();
            var getUserBookingList = _context.Bookings.Where(x => x.UserId == getUserId.Id && x.StartDate.Date == getstartDate.Date).ToList();
            if (getUserBookingList.Count != 0)
            {
                Response response = new Response();
                response.Status = "AlreadyBooked";
                return response;
            }

            var findbooking = _context.Bookings.Where(x => x.TableId == addBookingModel.TableId && x.Status != 4 && x.Status != 5)
                .Where(x => (getstartDate >= x.StartDate && getstartDate <= x.EndDate) || (getendDate >= x.StartDate && getendDate < x.EndDate)
                || (x.StartDate >= getstartDate && x.StartDate <= getendDate) || (x.EndDate >= getstartDate && x.EndDate <= getendDate)).FirstOrDefault();

            if (findbooking == null)
            {

                Booking booking = new Booking();
                booking.ResId = addBookingModel.ResId;
                booking.StartDate = Convert.ToDateTime(addBookingModel.StartDate);
                booking.EndDate = Convert.ToDateTime(addBookingModel.EndDate);
                booking.TableId = addBookingModel.TableId;
                booking.Request = addBookingModel.Request;
                booking.UserId = getUserId.Id;
                booking.Status = 1;
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                var bus = RabbitHutch.CreateBus("host=localhost");
                var getbooking = _context.Bookings.Include(x => x.Table).Include(x => x.Res).Where(x => x.BookId == booking.BookId).FirstOrDefault();
                BookingClass bookingClass = new BookingClass();
                bookingClass.BookId = getbooking.BookId;
                bookingClass.StartDate = getbooking.StartDate;
                bookingClass.EndDate = getbooking.EndDate;
                bookingClass.Request = getbooking.Request;
                bookingClass.TableNo = getbooking.Table.TableNo;
                bookingClass.ResName = getbooking.Res.Name;
                bookingClass.Email = getbooking.User.Email;
                bus.PubSub.Publish(bookingClass, "Add");

                Response response = new Response();
                response.Status = "Success";
                return response;
            }
            else
            {
                Response response = new Response();

                response.Status = "BookedByOther";
                return response;
            }


        }

        public async Task<Response> SetConfirmBooking(int BookId)
        {
            var getBooking = _context.Bookings.Include(x => x.Table).Include(x => x.Res).Include(x => x.User).Where(x => x.BookId == BookId).FirstOrDefault();
            Response response = new Response();
            if (getBooking != null)
            {

                if (getBooking.Status != 1)
                {
                    response.Status = "Modified";
                    response.Message = $"Booking Id,{getBooking.BookId} has been cancelled by user, so it is not approved";
                }
                else
                {
                    getBooking.Status = 2;
                    _context.Bookings.Update(getBooking);
                    await _context.SaveChangesAsync();
                    response.Status = "Success";
                    response.Message = "Booking has been approved";
                    var bus = RabbitHutch.CreateBus("host=localhost");
                    BookingClass bookingClass = new BookingClass();
                    bookingClass.BookId = getBooking.BookId;
                    bookingClass.StartDate = getBooking.StartDate;
                    bookingClass.EndDate = getBooking.EndDate;
                    bookingClass.Request = getBooking.Request;
                    bookingClass.TableNo = getBooking.Table.TableNo;
                    bookingClass.ResName = getBooking.Res.Name;
                    bookingClass.Email = getBooking.User.Email;
                    bus.PubSub.Publish(bookingClass, "Confirm");
                }
                //EmailHelper emailHelper = new EmailHelper();
                //bool emailResponse = emailHelper.SendConfirmBookingEmail(getBooking.User.Email, getBooking);
            }
            else
            {
                response.Status = "Not Found";
                response.Message = "Booking Not Found";
            }
            return response;
        }

        public async Task<Response> SetConfirmDate(int BookId)
        {
            var getBooking = await _context.Bookings.FindAsync(BookId);
            Response response = new Response();
            if (getBooking != null)
            {
                if (getBooking.Status != 1)
                {
                    response.Status = "Modified";
                    response.Message = $"Booking Id,{getBooking.BookId} has been cancelled by user, so it is not validated";
                }
                else
                {
                    response.Status = "Success";
                    response.Message = "Booking has been validated";
                    getBooking.ConfirmDate = DateTime.Now;
                    getBooking.Status = 3;
                    _context.Bookings.Update(getBooking);
                    await _context.SaveChangesAsync();

                }

            }
            else
            {
                response.Status = "Not Found";
                response.Message = "Booking Not Found";
            }
            return response;
        }

        public async Task CancelBooking(int BookId)
        {
            var getBooking = await _context.Bookings.FindAsync(BookId);
            if (getBooking != null)
            {
                getBooking.Status = 4;
                _context.Bookings.Update(getBooking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> searchPendingBooking(string searchText, PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.Include(x => x.StatusNavigation).Include(x => x.User).
                Include(x => x.Res).Include(x => x.Table).OrderBy(x => x.StartDate)
                .Where(x => x.Status == 1 && x.BookId == Convert.ToInt32(searchText))
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.Where(x => x.Status == 1 && x.BookId == Convert.ToInt32(searchText)).CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<ReturnBookingWithIntStatus> GetBookingById(int bookId)
        {
            var booking = _context.Bookings.Include(x => x.StatusNavigation)
                    .Include(x => x.Res).Include(x => x.Table)
                    .Where(x => x.BookId == bookId)
                    .Select(x => new ReturnBookingWithIntStatus
                    {
                        bookingId = x.BookId,
                        startDate = x.StartDate,
                        endDate = x.EndDate,
                        res = x.Res,
                        username = x.User.UserName,
                        table = x.Table,
                        status = x.Status,
                        request = x.Request
                    }).FirstOrDefault();
            return booking;
        }

        public async Task<List<BookingStatus>> GetBookingStatuses()
        {
            var bookingstatuses = await _context.BookingStatuses.Where(x => x.StatusName != "expire").ToListAsync();
            return bookingstatuses;
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> GetBookingByIdWithPageList(string searchText, PaginationFilter paginationFilter)
        {
            int convertedBookingId;
            bool result = Int32.TryParse(searchText, out convertedBookingId);
            if (result)
            {
                var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                    .Where(x => x.BookId == convertedBookingId)
                    .Select(x => new ReturnPendingBooking
                    {
                        bookingId = x.BookId,
                        startDate = x.StartDate,
                        endDate = x.EndDate,
                        resName = x.Res.Name,
                        username = x.User.UserName,
                        tableNo = x.Table.TableNo,
                        status = x.StatusNavigation.StatusName,
                        request = x.Request
                    })
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize).ToListAsync();
                var totalRecords = await _context.Bookings.Where(x => x.BookId == convertedBookingId).CountAsync();

                return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
            }
            else
            {
                var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                    .Where(x => x.BookId == 0)
                    .Select(x => new ReturnPendingBooking
                    {
                        bookingId = x.BookId,
                        startDate = x.StartDate,
                        endDate = x.EndDate,
                        resName = x.Res.Name,
                        username = x.User.UserName,
                        tableNo = x.Table.TableNo,
                        status = x.StatusNavigation.StatusName,
                        request = x.Request
                    })
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize)
                    .ToListAsync();
                var totalRecords = await _context.Bookings.Where(x => x.Status == 1).CountAsync();

                return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
            }
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> GetBookingByUsernameWithPageList(string searchText, PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                .Where(x => x.User.UserName == searchText)
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.Where(x => x.User.UserName == searchText).CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<PagedList<List<ReturnPendingBooking>>> GetBookingWithPageList(PaginationFilter paginationFilter)
        {
            var pagedata = await _context.Bookings.OrderByDescending(x => x.StartDate)
                .Select(x => new ReturnPendingBooking
                {
                    bookingId = x.BookId,
                    startDate = x.StartDate,
                    endDate = x.EndDate,
                    resName = x.Res.Name,
                    username = x.User.UserName,
                    tableNo = x.Table.TableNo,
                    status = x.StatusNavigation.StatusName,
                    request = x.Request
                })
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize).ToListAsync();
            var totalRecords = await _context.Bookings.CountAsync();

            return new PagedList<List<ReturnPendingBooking>>(pagedata, paginationFilter.PageNumber, paginationFilter.PageSize, totalRecords);
        }

        public async Task<CheckExistBooking> UpdateBookingDateTime(UpdateBookingDateTime updateBookingDateTime)
        {

            var getstartDate = Convert.ToDateTime(updateBookingDateTime.startDate);
            var getendDate = Convert.ToDateTime(updateBookingDateTime.endDate);
            CheckExistBooking checkExistBooking = new CheckExistBooking();
            var findbooking = _context.Bookings.Where(x => x.TableId == updateBookingDateTime.tableId && x.Status != 4 && x.Status != 5)
                .Where(x => (getstartDate >= x.StartDate && getstartDate <= x.EndDate) || (getendDate >= x.StartDate && getendDate < x.EndDate)
                || (x.StartDate >= getstartDate && x.StartDate <= getendDate) || (x.EndDate >= getstartDate && x.EndDate <= getendDate)).FirstOrDefault();

            if (findbooking == null)
            {
                var booking = _context.Bookings.Where(x => x.BookId == updateBookingDateTime.bookingId).FirstOrDefault();
                booking.StartDate = getstartDate;
                booking.EndDate = getendDate;
                booking.TableId = updateBookingDateTime.tableId;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();

                checkExistBooking.checkExist = true;
                return checkExistBooking;
            }
            else
            {
                checkExistBooking.checkExist = false;
                return checkExistBooking;
            }
        }

        public async Task UpdateBookingRequestStatus(UpdateBookingRequestStatus updateBookingRequestStatus)
        {
            var booking = _context.Bookings.Where(x => x.BookId == updateBookingRequestStatus.bookingId).FirstOrDefault();
            booking.Request = updateBookingRequestStatus.request;

            if (booking.Status == 3 && updateBookingRequestStatus.StatusId != 3)
            {
                booking.ConfirmDate = null;
            }
            else if (booking.Status != 3 && updateBookingRequestStatus.StatusId == 3)
            {
                booking.ConfirmDate = DateTime.Now;
            }
            booking.Status = updateBookingRequestStatus.StatusId;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

    }
}
