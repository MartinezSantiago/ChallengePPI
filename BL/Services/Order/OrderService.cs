using AutoMapper;
using BL.DTOs;
using BL.Enums;
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
        private Dictionary<AssetTypeEnum, ITotalAmountCalculationStrategy> _calculationStrategies;

        public OrderService(
            IOrderRepository orderRepository,
            IAssetRepository assetRepository,
            IAssetTypeRepository assetTypeRepository,
            IOrderStatusRepository orderStatusRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _assetRepository = assetRepository;
            _assetTypeRepository = assetTypeRepository;
            _orderStatusRepository = orderStatusRepository;
            _mapper = mapper;
            _calculationStrategies = new Dictionary<AssetTypeEnum, ITotalAmountCalculationStrategy>();
        }

        public async Task InitializeAsync()
        {
            _calculationStrategies = await GetCalculationStrategiesAsync();
        }
        private async Task<Dictionary<AssetTypeEnum, ITotalAmountCalculationStrategy>> GetCalculationStrategiesAsync()
        {
            var assetTypes = await _assetTypeRepository.GetAllAsync();
            var strategies = new Dictionary<AssetTypeEnum, ITotalAmountCalculationStrategy>();

            foreach (var assetType in assetTypes)
            {
                if (!Enum.TryParse(assetType.Description, out AssetTypeEnum assetTypeEnum))
                {
                    throw new InvalidOperationException("Invalid asset type.");
                }

                ITotalAmountCalculationStrategy strategy;

                switch (assetTypeEnum)
                {
                    case AssetTypeEnum.MF:
                        strategy = new MFTotalAmountCalculationStrategy();
                        break;
                    case AssetTypeEnum.Bond:
                        strategy = new BondTotalAmountCalculationStrategy();
                        break;
                    case AssetTypeEnum.Stock:
                        strategy = new StockTotalAmountCalculationStrategy();
                        break;
                    default:
                        throw new InvalidOperationException("Invalid asset type.");
                }

                strategies.Add(assetTypeEnum, strategy);
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

            if (!Enum.TryParse(assetType.Description, out AssetTypeEnum assetTypeEnum))
            {
                throw new InvalidOperationException("Invalid asset type.");
            }

            if (!_calculationStrategies.TryGetValue(assetTypeEnum, out var strategy))
            {
                throw new InvalidOperationException("Strategy not found for asset type.");
            }

            order.TotalAmount = strategy.CalculateTotalAmount(asset, orderDto);

            order.StatusId = (int)OrderStatusEnum.InProcess;

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
                throw new InvalidOperationException("Order not found.");
            }

            var newStatus = await _orderStatusRepository.GetByIdAsync(orderStatusUpdateDto.NewStatusId);
            if (newStatus == null)
            {
                throw new InvalidOperationException("New order status not found.");
            }

            if (order.StatusId != (int)OrderStatusEnum.InProcess)
            {
                throw new InvalidOperationException("Only can update status when status is 'In process'");
            }

            if (newStatus.Id == (int)OrderStatusEnum.InProcess)
            {
                throw new InvalidOperationException("Only can update status to 'Exectued' or 'Cancelled'");
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
