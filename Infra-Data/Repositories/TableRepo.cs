using Domain.Interfaces;
using Domain.Models;
using Infra_Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra_Data.Repositories
{
    public class TableRepo:ITableRepo
    {
        private readonly RestaurantBookingContext _context;
        public TableRepo(RestaurantBookingContext context)
        {
            _context = context;
        }

        public TableRepo()
        {
        }

        public async Task<List<Table>> GetAllTablesByRestaurantId(int resId)
        {

            List<Table> tableList = await _context.Tables.Where(x => x.ResId == resId).OrderBy(x => x.TableNo).ToListAsync();
            return tableList;
        }

        public async Task<List<Table>> GetTableWithDateTime(BookDateWithResId bookDateWithResId)
        {
            var getstartDate = Convert.ToDateTime(bookDateWithResId.startDate);
            var getendDate = Convert.ToDateTime(bookDateWithResId.endDate);
            List<int> bookingList = _context.Bookings
                .Where(x => x.Status != 4 && x.Status != 5 && x.ResId == bookDateWithResId.resId)
                .Where(x => (getstartDate >= x.StartDate && getstartDate <= x.EndDate) || (getendDate >= x.StartDate && getendDate < x.EndDate)
                || (x.StartDate >= getstartDate && x.StartDate <= getendDate) || (x.EndDate >= getstartDate && x.EndDate <= getendDate))
                //.Where(x => getendDate >= x.StartDate && getendDate < x.EndDate)
                .Select(x => x.TableId)
                .ToList();
            List<Table> tableList = await _context.Tables.Where(x => x.ResId == bookDateWithResId.resId && x.Status != 2).ToListAsync();

            foreach (Table t in tableList.ToList())
            {
                foreach (int id in bookingList)
                {
                    if (t.TableId == id)
                    {
                        tableList.Remove(t);
                    }
                }
            }
            return tableList;
        }



        public async Task UpdateTable(List<Table> tableList)
        {
            foreach (Table table in tableList)
            {

                if (table.TableId == 0)
                {
                    Table addTable = new Table
                    {
                        TableNo = table.TableNo,
                        Accommodate = table.Accommodate,
                        ResId = table.ResId,
                        Status = 1
                    };
                    _context.Tables.Add(addTable);
                    var getres = _context.Restaurants.Where(x => x.ResId == table.ResId).FirstOrDefault();
                    getres.Status = 1;
                    _context.Restaurants.Update(getres);

                }
                else
                {
                    var result = _context.Tables.Where(x => x.TableId == table.TableId).FirstOrDefault();
                    if (result.Status == 2)
                    {
                        result.Status = 1;
                    }
                    result.Accommodate = table.Accommodate;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Table>> GetTableByResId(int resId)
        {
            var tableList = await _context.Tables.Where(x => x.ResId == resId && x.Status != 2).OrderBy(x => x.TableNo).ToListAsync();
            if (tableList.Count == 0)
            {
                var gettable1 = _context.Tables.Where(x => x.ResId == resId && x.TableNo == 1).FirstOrDefault();
                gettable1.Status = 1;
                _context.Tables.Update(gettable1);
                await _context.SaveChangesAsync();
                List<Table> tableList1 = new List<Table>();
                tableList1.Add(gettable1);
                return tableList1;
            }
            return tableList;
        }

        public async Task<List<Table>> GetQtyTableByResId(int resId)
        {
            var tableList = await _context.Tables.Where(x => x.ResId == resId && x.Status != 2).OrderBy(x => x.TableNo).ToListAsync();
            return tableList;
        }

        public async Task<List<Table>> GetAllTableByResId(int resId)
        {
            var tableList = await _context.Tables.Where(x => x.ResId == resId && x.Status != 2).OrderBy(x => x.TableNo).ToListAsync();
            return tableList;
        }

        public async Task DeleteTable(int tableId)
        {

            var getTable = _context.Tables.Where(x => x.TableId == tableId).FirstOrDefault();
            getTable.Status = 2;
            getTable.Accommodate = 0;
            _context.Tables.Update(getTable);
            await _context.SaveChangesAsync();

            //var getAmountTable = _context.Tables.Where(x => x.ResId == getTable.ResId && x.Status != 2).AsNoTracking().ToList();
            //var count = getAmountTable.Count;
            //var getRes = _context.Restaurants.Where(x => x.ResId == getTable.ResId).FirstOrDefault();
            //if (getRes.AmountSlot == count)
            //{
            //    getRes.Status = 1;
            //    _context.Restaurants.Update(getRes);
            //    await _context.SaveChangesAsync();
            //}

        }
    }
}
