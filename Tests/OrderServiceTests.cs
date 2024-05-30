using AutoMapper;
using BL.DTOs;
using BL.Enums;
using BL.Services.Order;
using BL.Services.Order.Strategies.Interfaces;
using BL.Services.Order.Strategies;
using DAL.Models;
using DAL.Repository.Interfaces;
using Moq;
using NUnit.Framework.Internal.Commands;
using System.Reflection;

namespace Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IAssetRepository> _mockAssetRepository;
        private Mock<IAssetTypeRepository> _mockAssetTypeRepository;
        private Mock<IOrderStatusRepository> _mockOrderStatusRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockAssetRepository = new Mock<IAssetRepository>();
            _mockAssetTypeRepository = new Mock<IAssetTypeRepository>();
            _mockOrderStatusRepository = new Mock<IOrderStatusRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateOrderDTO, Order>();
                cfg.CreateMap<OrderDTO, Order>();
                cfg.CreateMap<Order, OrderDTO>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        [Test]
        public async Task CreateOrderAsync_ValidAsset_ValidAssetType_ReturnsOk()
        {
            // Arrange
            var orderService = new OrderService(_mockOrderRepository.Object, _mockAssetRepository.Object,
                                                _mockAssetTypeRepository.Object, _mockOrderStatusRepository.Object,
                                                _mapper);
            int assetTypeStock = (int)AssetTypeEnum.Stock;
            var orderDto = new CreateOrderDTO
            {
                AssetId = assetTypeStock,
                Quantity = 10,
                Price = 100,
                Operation = 'C',
                AccountId = 1
            };

            var asset = new Asset { Id = 1, Ticker = "AAPL", Name = "Apple", AssetTypeId = assetTypeStock, UnitPrice = 177.97m };
            var assetTypes = new List<AssetType>
            {
                new AssetType { Id = 1, Description = "Stock" },
                new AssetType { Id = 2, Description = "Bond" },
                new AssetType { Id = 3, Description = "MF" },
            };
            var defaultStatus = new OrderStatus { Id = 1, StatusDescription = "In process" };


            _mockAssetTypeRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(assetTypes);
            _mockAssetRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(asset);
            _mockAssetTypeRepository.Setup(repo => repo.GetByIdAsync(assetTypeStock)).ReturnsAsync(assetTypes.FirstOrDefault(x=>x.Id== assetTypeStock));

            // Act
            await orderService.CreateOrderAsync(orderDto);

            // Assert
            _mockOrderRepository.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public async Task GetOrderByIdAsync_ExistingOrderId_ReturnsOrderDTO()
        {
            // Arrange
            var orderService = new OrderService(_mockOrderRepository.Object, _mockAssetRepository.Object,
                                                _mockAssetTypeRepository.Object, _mockOrderStatusRepository.Object,
                                                _mapper);

            var orderId = 1;
            var userId = 1;
            var order = new Order { Id = 1, AccountId = userId, AssetId = 1, Quantity = 10, Price = 100, Operation = "C", StatusId = 1, TotalAmount = 1000 };

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await orderService.GetOrderByIdAsync(orderId, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OrderDTO>(result);
            Assert.That(result.Id, Is.EqualTo(orderId));
            Assert.That(result.Quantity, Is.EqualTo(10));
        }
    }
}