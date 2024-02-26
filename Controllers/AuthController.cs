using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Auth_API.Model;
using Auth_API.Model.Other;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        //สร้าง endpoint สำหรับการจัดการกับ register 
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            //TODO_1 ค้นหาผู้ใช้จากชื่อผู้ใช้ที่ส่งมาใน registerModel ด้วย UserManager
            var isExistsUser = await _userManager.FindByNameAsync(registerModel.Username);

            //TODO_2 ตรวจสอบว่ามีชื่อผู้ใช้นี้อยู่แล้วหรือไม่ 
            if (isExistsUser != null){
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
    }
}