Migrations(VScode)
dotnet ef migrations add secordAuth --context AuthDbContext
dotnet ef migrations add secordExpense --context ExpenseDbContext
dotnet ef database update  --context ExpenseDbContext
dotnet ef database update  --context AuthDbContext
