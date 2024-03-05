using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth_API.Model.Expenses;

namespace Auth_API.Services.Interfaces
{
    public interface IExpenseServices
    {
        IEnumerable<ExpenseModel> GetExpenses();

        IEnumerable<ExpenseModel> Search(string _search);
        ExpenseModel GetId(int id);

        void Add(ExpenseModel _expenseModel);

        int Update(ExpenseModel _expenseModel);
        void Delete(int id);

    }
}