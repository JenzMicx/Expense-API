using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Auth_API.Model.Entities
{
    public class AddUserFieldModel : IdentityUser
    {
        //เพิ่ม field ใน database ของ IdentityUser ที่มาจาก .net โดยสร้าง class ที่สืบทอดมาจาก IdentityUser อีกที
        public string FristName { get; set; }
        public string LastName { get; set; }
    }
}