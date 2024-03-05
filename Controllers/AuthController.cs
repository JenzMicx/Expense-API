using Auth_API.Model;
using Auth_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Auth_API.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {

        #region Field
        private readonly IAuthServices _authServices;

        #endregion

        #region DI
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        #endregion

        #region Endpoint

        #region Generate Roles
        //สร้าง endpoint สำหรับการจัดการกับการสร้างบทบาทผู้ใช้ในระบบ
        [HttpPost]
        [Route("type-roles")]
        public async Task<IActionResult> RolesTypes()
        {
            var generateRoles = await _authServices.RolesTypesAsync();
            return Ok(generateRoles);
        }
        #endregion

        #region Register
        //สร้าง endpoint สำหรับการ register 
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var registerAccount = await _authServices.RegisterAsync(registerModel);
            return registerAccount.iscomplete ? Ok(registerAccount) : BadRequest(registerAccount);
            //BadRequest: ใช้เมื่อคำขอไม่ถูกต้องหรือไม่สามารถประมวลผลได้เนื่องจากข้อมูลที่ไม่ถูกต้อง
        }
        #endregion

        #region Login
        //สร้าง endpoint สำหรับการ login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var loginAccount = await _authServices.LoginAsync(loginModel);
            return loginAccount.iscomplete ? Ok(loginAccount) : Unauthorized(loginAccount);
            //Unauthorized: ใช้เมื่อผู้ใช้ไม่มีสิทธิ์ในการเข้าถึง
        }
        #endregion

        #region Add Roles to UserName

        #region Add ADMIN Roles
        [HttpPost]
        [Route("addRoles-Admin")]
        public async Task<IActionResult> AddRolesADMIN([FromBody] AddRolesModel addRolesModel)
        {
            var addRolesAdmin = await _authServices.AddRolesADMINAsync(addRolesModel);
            return addRolesAdmin.iscomplete ? Ok(addRolesAdmin) : BadRequest(addRolesAdmin);
        }
        #endregion

        #region Add OWNER Roles
        [HttpPost]
        [Route("addRoles-Owner")]
        public async Task<IActionResult> AddRolesOWNER([FromBody] AddRolesModel addRolesModel)
        {
            var addRolesOwner = await _authServices.AddRolesOWNERAsync(addRolesModel);
            return addRolesOwner.iscomplete ? Ok(addRolesOwner) : BadRequest(addRolesOwner);
        }
        #endregion

        #endregion

        #endregion
    }
}