using Microsoft.AspNetCore.Http;
using P127_Pronia.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P127_Pronia.Models
{
    public class Slider:BaseEntity
    {
        public string Image { get; set; }
        [Required(ErrorMessage ="Zehmet olmasa endirim faizini qeyd edin")]
        public string Discount { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Article { get; set; }
        [Required]
        public string ButtonUrl { get; set; }
        [Required]
        public byte Order { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
