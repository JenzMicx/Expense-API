using System.Net;
using System.Text;
using Auth_API.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("local");
    options.UseSqlServer(connectionString);
});

//Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>() //ใช้ในการ DB ผู้ใช้และบทบาทใน Identity Framework
.AddEntityFrameworkStores<ApplicationDbContext>() //ใช้ในการจัดการ DB ของ Identity Framework
.AddDefaultTokenProviders(); //ใช้ในการจัดการ token 

//Config Idenfity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 10;
    options.Password.RequireDigit = true; //ตัวเลข
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = true;  //อักษรพิเศษ
    options.SignIn.RequireConfirmedEmail = false; //ยืนยัน email ทุกครั้งที่ Sign In
});

//Add Authentication and JwtBearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //ตรวจสอบ Issuer ของ token ว่าถูกต้องไหม
        ValidateAudience = true, //ตรวจสอบ Audience ของ token ว่าถูกต้องไหม
        // ValidateLifetime = false, //ตรวจสอบว่า token มีอายุหมดอายุหรือยัง
        // ValidateIssuerSigningKey = true, //ตรวจสอบความถูกต้องของคีย์สำหรับเซ็นต์ token
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"], //เป็น URL ของระบบหรือเว็บไซต์ที่ออก Token
        ValidAudience = builder.Configuration["JWT:ValidAudience"], //เป็น URL ของบริการที่จะรับ Token เพื่อใช้งาน
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))

    };
});



//pipeline
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
