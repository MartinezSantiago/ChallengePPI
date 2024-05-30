using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BL.DTOs
{
    public class CreateOrderDTO
    {
        [JsonIgnore]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "AssetId is required.")]
        public int AssetId { get; set; }


        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Operation is required.")]
        [RegularExpression("^[CV]$", ErrorMessage = "Operation must be 'C' or 'V'.")]
        public char Operation { get; set; }
    }
}
