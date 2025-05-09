using System;
using System.Collections.Generic;

namespace Infosys.Shop3D.Services.Models
{
    // Helper model for login operations
    public class LoginModel
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    // Helper model for save/remove product operations
    public class SaveProductModel
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
