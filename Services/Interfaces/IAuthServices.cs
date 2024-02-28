using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth_API.Model;

namespace Auth_API.Services.Interfaces
{
    public interface IAuthServices
    {
         Task<AuthServiceResponseModel> RolesTypesAsync();
        Task<AuthServiceResponseModel> RegisterAsync(RegisterModel registerModel);
        Task<AuthServiceResponseModel> LoginAsync(LoginModel loginModel);
        Task<AuthServiceResponseModel> AddRolesADMINAsync(AddRolesModel addRolesModel);
        Task<AuthServiceResponseModel> AddRolesOWNERAsync(AddRolesModel addRolesModel);
    }
}