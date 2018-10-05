using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webtest.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage ="New password required", AllowEmptyStrings =false)]
        [DataType(DataType.Password)]
        [RegularExpression("^.*(?=.{6,})(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).*$", ErrorMessage = "Your password must contain at least one capital letter, one number and one small letter")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage ="New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}