using P127_Pronia.Models.Base;
using System;
using System.Collections.Generic;

namespace P127_Pronia.Models
{
    public class Order: BaseEntity
    {
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public bool? Status { get; set; }
        public string Address { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
