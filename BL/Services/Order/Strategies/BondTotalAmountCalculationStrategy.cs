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
    public class BondTotalAmountCalculationStrategy : ITotalAmountCalculationStrategy
    {
        public decimal CalculateTotalAmount(Asset asset, CreateOrderDTO orderDto)
        {
            decimal totalAmount = asset.UnitPrice * orderDto.Quantity;
            decimal commission = totalAmount * 0.002M; // 0.2% commission
            decimal taxes = commission * 0.21M; // 21% taxes on commission
            return totalAmount + commission + taxes;
        }
    }
}
