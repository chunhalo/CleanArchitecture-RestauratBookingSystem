using Application.Interfaces;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private IBookService _bookService;
        public BookingController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("PendingBooking")]
        public async Task<ActionResult<PagedListModel>> GetPendingBooking([FromQuery] PaginationFilter paginationFilter)
        {


            PagedListModel pagedList = await _bookService.GetPendingBooking(paginationFilter);
            return Ok(pagedList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingWithIntStatusViewModel>> GetBookingById(int id)
        {
            BookingWithIntStatusViewModel booking = await _bookService.GetBookingById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);

        }

        [HttpPost]
        [Route("BookingWithIdPageList")]
        public async Task<ActionResult<PagedListModel>> GetBookingByIdWithPageList([FromForm] string searchText, [FromForm] string choice, [FromQuery] PaginationFilter paginationFilter)
        {
            if (choice == "bookId")
            {
                PagedListModel pagedList = await _bookService.GetBookingByIdWithPageList(searchText, paginationFilter);
                return Ok(pagedList);
            }
            else
            {
                PagedListModel pagedList = await _bookService.GetBookingByUsernameWithPageList(searchText, paginationFilter);
                return Ok(pagedList);
            }
        }

        [AllowAnonymous]
        [EnableCors("AnotherPolicy")]
        [HttpPut]
        [Route("UpdateDateTime")]
        public async Task<ActionResult<CheckExistBooking>> UpdateBookingDateTime([FromForm] UpdateBookingDateTime updateBookingDateTime)
        {
            var checkExisting = await _bookService.UpdateBookingDateTime(updateBookingDateTime);
            return Ok(checkExisting);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("UpdateRequestStatus")]
        public async Task<ActionResult> UpdateBookingRequestStatus([FromForm] UpdateBookingRequestStatus updateBookingRequestStatus)
        {
            await _bookService.UpdateBookingRequestStatus(updateBookingRequestStatus);
            return Ok();
        }



        [HttpGet]
        [Route("GetBookingStatus")]
        public async Task<ActionResult<BookingStatusesViewModel>> GetBookingStatus()
        {
            BookingStatusesViewModel bookingStatuses = await _bookService.GetBookingStatuses();
            return Ok(bookingStatuses);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("SearchPending")]
        public async Task<ActionResult<PagedListModel>> SearchPendingBooking([FromForm] string searchText, [FromQuery] PaginationFilter paginationFilter)
        {
            PagedListModel pagedList = await _bookService.searchPendingBooking(searchText, paginationFilter);
            return pagedList;
        }

        [HttpGet]
        [Route("Allbooking")]
        public async Task<ActionResult<PagedListModel>> GetBookingWithPageList([FromQuery] PaginationFilter paginationFilter)
        {
            PagedListModel pagedList = await _bookService.GetBookingWithPageList(paginationFilter);
            return pagedList;
        }



        [HttpPost]
        public async Task<ActionResult<ResponseViewModel>> AddBooking([FromForm] AddBookingModel addBookingModel)
        {
            var identity = User.Identity as ClaimsIdentity;
            ResponseViewModel responseViewModel = new ResponseViewModel();
            if (identity != null)
            {
                //IEnumerable<Claim> claims = identity.Claims;
                string username = User.FindFirstValue(ClaimTypes.Name);

                responseViewModel = await _bookService.AddBooking(addBookingModel, username);

            }

            return Ok(responseViewModel);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("SetConfirmBooking")]
        public async Task<ActionResult<ResponseViewModel>> SetConfirmBooking([FromForm] int BookId)
        {
            ResponseViewModel response = await _bookService.SetConfirmBooking(BookId);
            if (response.response.Status == "Modified" || response.response.Status == "Success")
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }

        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        [Route("SetConfirmDate")]
        public async Task<ActionResult<ResponseViewModel>> SetConfirmDate([FromForm] int BookId)
        {
            ResponseViewModel response = await _bookService.SetConfirmDate(BookId);
            if (response.response.Status == "Modified" || response.response.Status == "Success")
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }

        }

        [HttpPut]
        [Route("CancelBooking")]
        public async Task<ActionResult> CancelBooking([FromForm] int BookId)
        {
            await _bookService.CancelBooking(BookId);
            return Ok();


        }

    }
}
