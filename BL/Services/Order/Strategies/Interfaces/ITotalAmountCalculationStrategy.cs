using BL.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Order.Strategies.Interfaces
{
    public interface ITotalAmountCalculationStrategy
    {
        decimal CalculateTotalAmount(Asset asset, CreateOrderDTO orderDto);
    }
}
