using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth_API.Model.Expenses
{
    public class ExpenseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string DateTime { get; set; }
        public double Amount { get; set; }
        public string Category { get; set; }
    }
}