using Application.ViewModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITableService
    {
        Task<ListTableViewModel> GetAllTablesByRestaurantId(int resId);

        Task<ListTableViewModel> GetTableWithDateTime(BookDateWithResId bookDateWithResId);
        Task UpdateTable(List<Table> tableList);
        Task DeleteTable(int tableId);
        Task<ListTableViewModel> GetTableByResId(int resId);
        Task<ListTableViewModel> GetQtyTableByResId(int resId);
    }
}
