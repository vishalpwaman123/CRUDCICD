using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.ResponseModel
{
    public class RAdminRegisterModel
    {
        public Int32 AdminId { get; set; }

        public string AdminName { get; set; }

        public string AdminEmailId { get; set; }

        public string Gender { get; set; }

        public string Role { get; set; }

        public string RoleID { get; set; }

        public string CreatedDate { get; set; }
    }
}
