using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.ResponseModel
{
    public class RAdminLoginModel
    {

        public Int32 Id { get; set; }

        public string Name { get; set; }

        public string EmailId { get; set; }

        public string Gender { get; set; }

        public string Role { get; set; }

        public int RoleId { get; set; }

        public string CreatedDate { get; set; }

        public string Token { get; set; }

    }
}
