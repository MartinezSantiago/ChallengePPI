using AutoMapper;
using BL.DTOs;
using BL.Services.Order.Interfaces;
using BL.Services.Order.Strategies;
using BL.Services.Order.Strategies.Interfaces;
using DAL.Models;
using DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly IMapper _mapper;
        private Dictionary<string, ITotalAmountCalculationStrategy> _calculationStrategies;

        public OrderService(
            IOrderRepository orderRepository,
            IAssetRepository assetRepository,
            IAssetTypeRepository assetTypeRepository,
            IOrderStatusRepository orderStatusRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _assetRepository = assetRepository ?? throw new ArgumentNullException(nameof(assetRepository));
            _assetTypeRepository = assetTypeRepository ?? throw new ArgumentNullException(nameof(assetTypeRepository));
            _orderStatusRepository = orderStatusRepository ?? throw new ArgumentNullException(nameof(orderStatusRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _calculationStrategies = new Dictionary<string, ITotalAmountCalculationStrategy>();
        }
        public async Task InitializeAsync()
        {
            _calculationStrategies = await GetCalculationStrategiesAsync();
        }
        private async Task<Dictionary<string, ITotalAmountCalculationStrategy>> GetCalculationStrategiesAsync()
        {
            var assetTypes = await _assetTypeRepository.GetAllAsync();
            var strategies = new Dictionary<string, ITotalAmountCalculationStrategy>();

            foreach (var assetType in assetTypes)
            {
                // Concatenate strategy name with TotalAmountCalculationStrategy suffix
                var strategyName = assetType.Description + "TotalAmountCalculationStrategy";

                // Dynamically instantiate the strategy using reflection
                var strategyType = Type.GetType("BL.Services.Order.Strategies." + strategyName);
                if (strategyType != null)
                {
                    var strategyInstance = (ITotalAmountCalculationStrategy)Activator.CreateInstance(strategyType);
                    strategies.Add(assetType.Description, strategyInstance);
                }
            }

            return strategies;
        }
        public async Task CreateOrderAsync(CreateOrderDTO orderDto)
        {
            var asset = await _assetRepository.GetByIdAsync(orderDto.AssetId);
            if (asset == null)
            {
                throw new InvalidOperationException("Asset not found.");
            }

            var assetType = await _assetTypeRepository.GetByIdAsync(asset.AssetTypeId);
            if (assetType == null)
            {
                throw new InvalidOperationException("Asset type not found.");
            }


            var order = _mapper.Map<DAL.Models.Order>(orderDto);

            await InitializeAsync();
            var assetTypeDescription = assetType.Description;
            if (!_calculationStrategies.TryGetValue(assetTypeDescription, out var strategy))
            {
                throw new InvalidOperationException("Invalid asset type.");
            }

            order.TotalAmount = strategy.CalculateTotalAmount(asset, orderDto);

            var defaultStatusId = await _orderStatusRepository.GetDefaultOrderStatusAsync();
            order.StatusId = defaultStatusId.Id;

            await _orderRepository.AddAsync(order);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            return order?.AccountId == userId ? _mapper.Map<OrderDTO>(order) : null;
        }

        public async Task<List<OrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task UpdateOrderStatusAsync(OrderStatusUpdateDTO orderStatusUpdateDto, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderStatusUpdateDto.OrderId);
            if (order == null || order.AccountId != userId)
            {
                throw new InvalidOperationException("Order not found or user does not have permission to update this order.");
            }

            var newStatus = await _orderStatusRepository.GetByIdAsync(orderStatusUpdateDto.NewStatusId);
            if (newStatus == null)
            {
                throw new InvalidOperationException("New order status not found.");
            }

            if (order.Status.StatusDescription != "In process")
            {
                throw new InvalidOperationException("Only can update status from 'In process'");
            }

            order.StatusId = newStatus.Id;

            await _orderRepository.UpdateAsync(order);
        }


        public async Task DeleteOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.AccountId != userId)
            {
                throw new InvalidOperationException("Order not found.");
            }

            await _orderRepository.DeleteAsync(order);
        }
    }
}
