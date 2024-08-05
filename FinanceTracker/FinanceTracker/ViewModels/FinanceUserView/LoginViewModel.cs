﻿using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ViewModels.FinanceUserView
{
    public class LoginViewModel
    {
        [Required]
        public string EmailOrUserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
