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
    public class TableService : ITableService
    {
        public ITableRepo _tableRepo;
        public TableService(ITableRepo tableRepo)
        {
            _tableRepo = tableRepo;
        }
        public async Task DeleteTable(int tableId)
        {
            await _tableRepo.DeleteTable(tableId);
        }

        public async Task<ListTableViewModel> GetAllTablesByRestaurantId(int resId)
        {
            ListTableViewModel listTableViewModel= new ListTableViewModel();
            listTableViewModel.tableList = await _tableRepo.GetAllTablesByRestaurantId(resId);
            return listTableViewModel;
        }

        public async Task<ListTableViewModel> GetQtyTableByResId(int resId)
        {
            ListTableViewModel listTableViewModel = new ListTableViewModel();
            listTableViewModel.tableList = await _tableRepo.GetQtyTableByResId(resId);
            return listTableViewModel;
        }

        public async Task<ListTableViewModel> GetTableByResId(int resId)
        {
            ListTableViewModel listTableViewModel = new ListTableViewModel();
            listTableViewModel.tableList = await _tableRepo.GetTableByResId(resId);
            return listTableViewModel;
        }

        public async Task<ListTableViewModel> GetTableWithDateTime(BookDateWithResId bookDateWithResId)
        {
            ListTableViewModel listTableViewModel = new ListTableViewModel();
            listTableViewModel.tableList = await _tableRepo.GetTableWithDateTime(bookDateWithResId);
            return listTableViewModel;
        }

        public async Task UpdateTable(List<Table> tableList)
        {
            await _tableRepo.UpdateTable(tableList);
        }
    }
}
