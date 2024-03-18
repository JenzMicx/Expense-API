using Auth_API.DbContext;
using Auth_API.Model.Expenses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Auth_API.Controllers
{
    [Route("[controller]")]
    public class ExpenseController : Controller
    {
        private readonly ExpenseDbContext _context;

        public ExpenseController(ExpenseDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetExpense()
        {
            if (_context.ExpenseDB == null)
            {
                return NotFound();
            }
            var ExpenseList = await _context.ExpenseDB.ToListAsync();
            return Ok(ExpenseList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            if (_context.ExpenseDB == null)
            {
                return NotFound();
            }
            var expenseId = await _context.ExpenseDB.FindAsync(id);
            if (expenseId == null)
            {
                return NotFound();
            }
            return Ok(expenseId);
        }

        [HttpPost]
        public async Task<ActionResult> CreateExpense([FromBody] ExpenseModel expenseModel)
        {
            // สร้างข้อมูลใหม่โดยใช้ string ที่แปลงไว้แทนที่ DateTime ที่มีปัญหา
            var newExpenseModel = new ExpenseModel
            {
                // Id = expenseModel.Id,
                Title = expenseModel.Title,
                DateTime = expenseModel.DateTime,
                Amount = expenseModel.Amount,
                Category = expenseModel.Category
            };

            _context.ExpenseDB.Add(newExpenseModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new { id = expenseModel.Id }, expenseModel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateExpense([FromBody] ExpenseModel expenseModel, int id)
        {
            //check ก่อนว่า id ที่รับมาตรงกับ id ที่จะแก้ไขใน database ไหม
            if (id != expenseModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(expenseModel).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //DbUpdateConcurrencyException เกิดขึ้นเมื่อมีการอัปเดตข้อมูลในฐานข้อมูลพร้อมกันจากแหล่งที่ไม่เหมือนกัน 
                //ซึ่งส่งผลให้ Entity Framework Core ไม่สามารถดำเนินการอัปเดตข้อมูลได้
                throw;
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteExpense(int id)
        {
            if (_context.ExpenseDB == null)
            {
                return NotFound();
            }
            var ExpenseId = await _context.ExpenseDB.FindAsync(id);
            if (ExpenseId == null)
            {
                return NotFound();
            }
            _context.ExpenseDB.Remove(ExpenseId);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}