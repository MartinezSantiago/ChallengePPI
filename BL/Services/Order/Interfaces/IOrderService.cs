using BL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Order.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> GetOrderByIdAsync(int id,int userId);
        Task<List<OrderDTO>> GetOrdersByUserIdAsync(int userId);
        Task CreateOrderAsync(CreateOrderDTO order);
        Task UpdateOrderStatusAsync(OrderStatusUpdateDTO orderStatusUpdateDto,int userId);
        Task DeleteOrderAsync(int id,int userId);
    }
}
