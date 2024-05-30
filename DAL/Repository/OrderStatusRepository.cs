using DAL.Context;
using DAL.Models;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class OrderStatusRepository : Repository<OrderStatus>, IOrderStatusRepository
    {
        private readonly ChallengePpiContext _context;
        public OrderStatusRepository(ChallengePpiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OrderStatus> GetDefaultOrderStatusAsync()
        {
            return await _context.Set<OrderStatus>().FirstOrDefaultAsync(os => os.StatusDescription == "In process");
        }
    }
}
