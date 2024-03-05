using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth_API.DbContext;
using Auth_API.Migrations;
using Auth_API.Model.Expenses;
using Auth_API.Services.Interfaces;

namespace Auth_API.Services
{
    public class ExpenseServices : IExpenseServices
    {
        private readonly ApplicationDbContext _context;

        //DI
        public ExpenseServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(ExpenseModel _expenseModel)
        {
            _context.ExpenseDB.Add(_expenseModel);
        }

        public void Delete(int id)
        {
            
        }

        public IEnumerable<ExpenseModel> GetExpenses()
        {
            var expenses = _context.ExpenseDB.ToList();
            return expenses;
        }

        public ExpenseModel GetId(int id)
        {
            var GetID = _context.ExpenseDB.Find(id);
            return GetID;
        }

        public IEnumerable<ExpenseModel> Search(string _search)
        {
            var filter = GetExpenses().Where(s=>s.Title.Contains(_search)).ToList();
            return filter;
        }

        public int Update(ExpenseModel _expenseModel)
        {
            _context.Entry(_expenseModel).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return 1;
        }
    }
}