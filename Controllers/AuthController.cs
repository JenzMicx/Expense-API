using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth_API.Model;
using Auth_API.Model.Other;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Auth_API.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        //Dependency Injection สร้าง construstor ขึ้นมาเพื่อรับ dependency มาใช้งานใน class นี้
        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        //สร้าง endpoint สำหรับการจัดการกับการสร้างบทบาทผู้ใช้ในระบบ
        [HttpPost]
        [Route("type-roles")]
        public async Task<IActionResult> RolesTypes()
        {
            //TODO_2 ตรวจสอบว่าบทบาท "USER, ADMIN, OWNER" มีอยู่ในฐานข้อมูลหรือไม่
            bool isOwnerExists = await _roleManager.RoleExistsAsync(UserRoles.OWNER);
            bool isAdminExists = await _roleManager.RoleExistsAsync(UserRoles.ADMIN);
            bool isUserExists = await _roleManager.RoleExistsAsync(UserRoles.USER);
            if (isOwnerExists && isAdminExists && isUserExists)
            {
                return Ok("Roles Creating is Already Done");
            }
            //TODO_1 สร้างบทบาท "USER, ADMIN, OWNER" โดยใช้เมธอด CreateAsync ของ RoleManager และส่งชื่อ roles เข้าไปใน IdentityRole constructor
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.OWNER));
            return Ok("Role Creating Done Succesfully");

        }

        //สร้าง endpoint สำหรับการ register 
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            //TODO_1 ค้นหาผู้ใช้จากชื่อผู้ใช้ที่ส่งมาใน registerModel ด้วย UserManager
            var isExistsUser = await _userManager.FindByNameAsync(registerModel.Username);

            //TODO_2 ตรวจสอบว่ามีชื่อผู้ใช้นี้อยู่แล้วหรือไม่ 
            if (isExistsUser != null)
            {
                return BadRequest("Username Already Exists");
            }

            //TODO_3 สร้าง instance ใหม่ของ IdentityUser ด้วยข้อมูลที่ client ส่งมาใน registerModel เพื่อทำการสร้างผู้ใช้ใหม่
            IdentityUser newUser = new IdentityUser()
            {
                Email = registerModel.Email,
                UserName = registerModel.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            //TODO_4 สร้างผู้ใช้ใหม่โดยใช้เมธอด CreateAsync ของ UserManager และรับ password ที่ client ส่งมาใน registerModel เพื่อเข้ารหัสและบันทึกลงในฐานข้อมูล
            var createNewUser = await _userManager.CreateAsync(newUser, registerModel.Password);

            //TODO_5 ตรวจสอบว่าการสร้างผู้ใช้ใหม่สำเร็จหรือไม่
            if (!createNewUser.Succeeded)
            {
                var messageErrors = "User Create Failed Because: ";
                foreach (var e in createNewUser.Errors)
                {
                    messageErrors += "#" + e.Description;
                }
                return BadRequest(messageErrors);
            }

            //TODO_6 กำหนดบทบาท "USER" ให้กับผู้ใช้ใหม่ที่ถูกสร้างขึ้น
            //Setting default "USER" role to new user
            await _userManager.AddToRoleAsync(newUser, UserRoles.USER);
            return Ok("User Created Succesfully");
        }

        //สร้าง endpoint สำหรับการ login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {

            //TODO_1 ค้นหาผู้ใช้จากชื่อผู้ใช้ที่ส่งมาใน loginModel ด้วย UserManager
            var userLogin = await _userManager.FindByNameAsync(loginModel.Username);

            //TODO_2 ตรวจสอบว่ามีผู้ใช้ที่เข้าสู่ระบบโดยใช้ชื่อผู้ใช้ที่รับมาหรือไม่
            if (userLogin is null)
            {
                return Unauthorized("Invaild Credentials");
            }

            //TODO_3 เก็บ role ของผู้ใช้ที่เข้าสู่ระบบได้มาไว้ใน UserRoles เพื่อใช้ในการสร้าง claims ใน token
            var UserRoles = await _userManager.GetRolesAsync(userLogin);

            //TODO_4 เมื่อได้รับบทบาทของผู้ใช้แล้ว เราก็เริ่มสร้างรายการของประกาศ (claims) ซึ่งจะใช้ในการสร้าง token JWT โดยใส่ข้อมูลต่างๆ เช่น ชื่อผู้ใช้, ไอดีผู้ใช้, และบทบาทลงไป
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userLogin.UserName),
                new Claim(ClaimTypes.NameIdentifier, userLogin.Id),
                new Claim("JWTID", Guid.NewGuid().ToString())
            };

            //TODO_5 เพิ่มข้อมูลบทบาท (Role claims) ลงในรายการของประกาศ (claims) เพื่อให้สามารถนำข้อมูลบทบาทมาใช้ในการสร้าง token JWT 
            foreach (var userRole in UserRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            //TODO_6 เมื่อเรามีรายการของประกาศ (claims) ที่ครบถ้วนแล้ว เราก็ใช้เมธอด GenerateNewToken() เพื่อสร้าง token JWT โดยใส่ข้อมูล claims ลงไปใน token และส่ง token นั้นกลับไปยัง client โดยใช้เมธอด Ok()
            var token = GenerateNewToken(authClaims);

            return Ok(token);
        }

        private string GenerateNewToken(List<Claim> Claims)
        {
            //สร้างคีย์สำหรับการเข้ารหัสและถอดรหัส token โดยใช้ค่า Secret ที่กำหนดใน configuration (_configuration) ซึ่งเป็นค่าลับที่ใช้ในการเข้ารหัสข้อมูล
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            //สร้าง object JwtSecurityToken ที่เป็น token JWT
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: Claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

            //เข้ารหัส object JwtSecurityToken เป็น string โดยใช้ JwtSecurityTokenHandler และเมธอด WriteToken() เพื่อให้ได้ token JWT ออกมาเป็น string
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}