using BL.DTOs;
using BL.Services.Order.Strategies.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Order.Strategies
{
    public class FCITotalAmountCalculationStrategy : ITotalAmountCalculationStrategy
    {
        public decimal CalculateTotalAmount(Asset asset, CreateOrderDTO orderDto)
        {
            return asset.UnitPrice * orderDto.Quantity;
        }
    }
}

