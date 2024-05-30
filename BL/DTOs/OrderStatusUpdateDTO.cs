using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.DTOs
{
    public class OrderStatusUpdateDTO
    {
        public int OrderId { get; set; }
        public int NewStatusId { get; set; }
    }
}
