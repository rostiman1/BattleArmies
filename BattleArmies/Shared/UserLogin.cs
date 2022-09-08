using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleArmies.Shared
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter an Email Address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter the Password")]
        public string? Password { get; set; }
    }
}
