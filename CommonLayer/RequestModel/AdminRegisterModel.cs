using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonLayer.RequestModel
{
    public class AdminRegisterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [RegularExpression("^([a-zA-Z]{2,})$", ErrorMessage = "First Name should contain atleast 2 or more characters")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required")]
        public string EmailId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(?:m|M|male|Male|f|F|female|Female)$", ErrorMessage = "Not valid Gender eg : Male Or Female")]
        public string Gender { get; set; }

        [Required]
        public string Role { get; set; }

        public string CreatedDate { get; set; }

        public string ModificateDate { get; set; }
    }
}
