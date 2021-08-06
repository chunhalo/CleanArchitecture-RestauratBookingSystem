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
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [EnableCors("AnotherPolicy")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ListTableViewModel>> GetAllTableByRestaurantId(int id)
        {
            ListTableViewModel listTableViewModel = await _tableService.GetAllTablesByRestaurantId(id);
            if (listTableViewModel.tableList == null)
            {
                return NotFound();
            }
            return Ok(listTableViewModel);
        }

        [HttpGet]
        [Route("GetTableByResId")]
        public async Task<ActionResult<ListTableViewModel>> GetTableById([FromQuery] int id)
        {
            ListTableViewModel listTableViewModel = await _tableService.GetTableByResId(id);

            return Ok(listTableViewModel);

        }

        [HttpGet]
        [Route("GetQtyTableByResId")]
        public async Task<ActionResult<ListTableViewModel>> GetQtyTableById([FromQuery] int id)
        {
            ListTableViewModel listTableViewModel = await _tableService.GetQtyTableByResId(id);

            return Ok(listTableViewModel);

        }

        [EnableCors("AnotherPolicy")]
        [HttpPost]
        public async Task<ActionResult<ListTableViewModel>> GetTableWithDateTime([FromForm] BookDateWithResId bookDateWithResId)
        {
            ListTableViewModel listTableViewModel = await _tableService.GetTableWithDateTime(bookDateWithResId);
            if (listTableViewModel.tableList == null)
            {
                return NotFound();
            }
            return Ok(listTableViewModel);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut]
        public async Task<ActionResult> UpdateTable([FromBody] List<Table> tableList)
        {
            await _tableService.UpdateTable(tableList);
            return Ok();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("DeleteTable")]
        public async Task<ActionResult> DeleteTable([FromForm] int tableId)
        {
            await _tableService.DeleteTable(tableId);
            return Ok();
        }
    }
}
