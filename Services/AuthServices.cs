using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth_API.Model;
using Auth_API.Model.Entities;
using Auth_API.Model.Other;
using Auth_API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth_API.Services
{
    public class AuthServices : IAuthServices
    {
        #region Field
        private readonly UserManager<AddUserFieldModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        #endregion

        #region DI
        //Dependency Injection สร้าง construstor ขึ้นมาเพื่อรับ dependency มาใช้งานใน class นี้
        public AuthServices(UserManager<AddUserFieldModel> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        #endregion

        #region Method
        //Method for this class
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
        #endregion

        public async Task<AuthServiceResponseModel> AddRolesADMINAsync(AddRolesModel addRolesModel)
        {
            var userLogin = await _userManager.FindByNameAsync(addRolesModel.Username);
            if (userLogin is null)
            {
                // return BadRequest("UserName IS NULL");
                return new AuthServiceResponseModel()
                {
                    iscomplete = false,
                    messageStatus = "UserName IS NULL"
                };
            }
            await _userManager.AddToRoleAsync(userLogin, UserRoles.ADMIN);
            // return Ok("Role: Add already 'ADMIN' roles");
            return new AuthServiceResponseModel()
            {
                iscomplete = true,
                messageStatus = "Role: Add already 'ADMIN' roles"
            };
        }

        public async Task<AuthServiceResponseModel> AddRolesOWNERAsync(AddRolesModel addRolesModel)
        {
            var userLogin = await _userManager.FindByNameAsync(addRolesModel.Username);
            if (userLogin is null)
            {
                // return BadRequest("UserName IS NULL");
                return new AuthServiceResponseModel()
                {
                    iscomplete = false,
                    messageStatus = "UserName IS NULL"
                };
            }
            await _userManager.AddToRoleAsync(userLogin, UserRoles.OWNER);
            // return Ok("Role: Add already 'OWNER' roles");
            return new AuthServiceResponseModel()
            {
                iscomplete = true,
                messageStatus = "Role: Add already 'OWNER' roles"
            };
        }

        public async Task<AuthServiceResponseModel> LoginAsync(LoginModel loginModel)
        {
            //TODO_1 ค้นหาผู้ใช้จากชื่อผู้ใช้ที่ส่งมาใน loginModel ด้วย UserManager
            var userLogin = await _userManager.FindByNameAsync(loginModel.Username);

            //TODO_2 ตรวจสอบว่ามีผู้ใช้ที่เข้าสู่ระบบโดยใช้ชื่อผู้ใช้ที่รับมาหรือไม่
            if (userLogin is null)
            {
                // return Unauthorized("Invaild Credentials"); //Unauthorized คือเด้งออกไปละแสดง message
                return new AuthServiceResponseModel()
                {
                    iscomplete = false,
                    messageStatus = "Invaild Credentials"
                };
            }
            //TODO_2.1 ตรวจสอบว่ารหัสผ่านที่ผู้ใช้ป้อนมา ตรงกับรหัสผ่านของผู้ใช้ที่กำลังล็อกอินอยู่หรือไม่
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(UserLogin, loginModel.Password);
            if (!isPasswordCorrect)
            {
                //  return Unauthorized("Password incorrect");
                return new AuthServiceResponseModel()
                {
                    iscomplete = false,
                    messageStatus = "Password Incorrect"
                };
            }
            //TODO_3 เก็บ role ของผู้ใช้ที่เข้าสู่ระบบได้มาไว้ใน UserRoles เพื่อใช้ในการสร้าง claims ใน token
            var UserRoles = await _userManager.GetRolesAsync(userLogin);

            //TODO_4 เมื่อได้รับบทบาทของผู้ใช้แล้ว เราก็เริ่มสร้างรายการของ claims (information ที่อยู่ในส่วน payload ตอน Decoded) ซึ่งจะใช้ในการสร้าง token JWT โดยใส่ข้อมูลต่างๆ เช่น ชื่อผู้ใช้, ไอดีผู้ใช้, และบทบาทลงไป
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userLogin.UserName),
                    new Claim(ClaimTypes.NameIdentifier, userLogin.Id),
                    new Claim("JWTID", Guid.NewGuid().ToString())
                };

            //TODO_4.1 เพิ่มข้อมูลบทบาท (Role claims) ลงในรายการของ claims (information ที่อยู่ในส่วน payload ตอน Decoded) เพื่อให้สามารถนำข้อมูลบทบาทมาใช้ในการสร้าง token JWT 
            foreach (var userRole in UserRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            //TODO_5 เมื่อเรามีรายการของประกาศ (claims) ที่ครบถ้วนแล้ว เราก็ใช้เมธอด GenerateNewToken() เพื่อสร้าง token JWT โดยใส่ข้อมูล claims ลงไปใน token และส่ง token นั้นกลับไปยัง client โดยใช้เมธอด Ok()
            var token = GenerateNewToken(authClaims);

            // return Ok(token);
            return new AuthServiceResponseModel()
            {
                iscomplete = true,
                messageStatus = token
            };
        }

        public async Task<AuthServiceResponseModel> RegisterAsync(RegisterModel registerModel)
        {
            //TODO_1 ค้นหาผู้ใช้จากชื่อผู้ใช้ที่ส่งมาใน registerModel ด้วย UserManager
            var isExistsUser = await _userManager.FindByNameAsync(registerModel.Username);

            //TODO_2 ตรวจสอบว่ามีชื่อผู้ใช้นี้อยู่แล้วหรือไม่ 
            if (isExistsUser != null)
            {
                // return BadRequest("Username Existed");
                return new AuthServiceResponseModel()
                {
                    iscomplete = false,
                    messageStatus = "Username Existed"
                };
            }

            //TODO_3 สร้าง instance ใหม่ของ AddUserFieldModel ด้วยข้อมูลที่ client ส่งมาใน registerModel เพื่อทำการสร้างผู้ใช้ใหม่
            AddUserFieldModel newUser = new AddUserFieldModel()
            {
                FristName = registerModel.FristName,
                LastName = registerModel.LastName,
                UserName = registerModel.Username,
                Email = registerModel.Email,
                PhoneNumber = registerModel.Phone,
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
                // return BadRequest(messageErrors);
                return new AuthServiceResponseModel()
                {
                    iscomplete = false,
                    messageStatus = messageErrors
                };
            }

            //TODO_6 กำหนดบทบาท "USER" ให้กับผู้ใช้ใหม่ที่ถูกสร้างขึ้น
            //Setting default "USER" role to new user
            await _userManager.AddToRoleAsync(newUser, UserRoles.USER);
            // return Ok("User Created Succesfully");
            return new AuthServiceResponseModel()
            {
                iscomplete = true,
                messageStatus = "User Created Complete"
            };
        }

        public async Task<AuthServiceResponseModel> RolesTypesAsync()
        {
            //TODO_2 ตรวจสอบว่าบทบาท "USER, ADMIN, OWNER" มีอยู่ในฐานข้อมูลหรือไม่
            bool isOwnerExists = await _roleManager.RoleExistsAsync(UserRoles.OWNER);
            bool isAdminExists = await _roleManager.RoleExistsAsync(UserRoles.ADMIN);
            bool isUserExists = await _roleManager.RoleExistsAsync(UserRoles.USER);
            if (isOwnerExists && isAdminExists && isUserExists)
            {
                // return Ok("Roles Creating is Already Done");
                return new AuthServiceResponseModel()
                {
                    iscomplete = true,
                    messageStatus = "Roles Creating Done."
                };
            }
            //TODO_1 สร้างบทบาท "USER, ADMIN, OWNER" โดยใช้เมธอด CreateAsync ของ RoleManager และส่งชื่อ roles เข้าไปใน IdentityRole constructor
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.USER));
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.OWNER));
            // return Ok("Role Creating Done Succesfully");
            return new AuthServiceResponseModel()
            {
                iscomplete = true,
                messageStatus = "Role Creating Done Succesfully"
            };
        }
    }
}