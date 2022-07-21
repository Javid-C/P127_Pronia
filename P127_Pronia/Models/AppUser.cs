using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace P127_Pronia.Models
{
    public class AppUser:IdentityUser
    {
        public string Firstname{ get; set; }
        public string Lastname { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<Order> Order { get; set; }
    }
}
