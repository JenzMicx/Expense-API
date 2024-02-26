using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        //สร้าง endpoint ที่ใช้สำหรับการจัดการกับการสร้างบทบาทผู้ใช้ในระบบ
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
    }
}