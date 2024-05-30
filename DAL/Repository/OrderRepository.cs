using DAL.Context;
using DAL.Models;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ChallengePpiContext _context;
        public OrderRepository(ChallengePpiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var orders=await _context.Orders.Where(x=>x.AccountId==userId).ToListAsync();
            return orders;
        }
    }
}
