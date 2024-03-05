using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_API.Model.Expenses
{
    public class ExpenseModel
    {
        [Key]
        public int id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime SpendingDate { get; set; } = DateTime.Now;

        public string Category { get; set; }
    }
}