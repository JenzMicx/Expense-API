![image](https://github.com/JenzMicx/Expense-API/assets/142468203/f79e6f00-c243-4d88-b96f-00167e0e7dbd)

Migrations(VScode)

dotnet ef migrations add secordAuth --context AuthDbContext

dotnet ef migrations add secordExpense --context ExpenseDbContext

dotnet ef database update  --context ExpenseDbContext

dotnet ef database update  --context AuthDbContext
