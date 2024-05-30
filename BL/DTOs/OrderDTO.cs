using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int AssetId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public char Operation { get; set; }
        public int StatusId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
