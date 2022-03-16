﻿using DemoWebsite.Core;
using DemoWebsite.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Threading.Tasks;

namespace DemoWebsiteTests
{
    [TestClass]
    public class RewardServiceTest
    {
        [TestMethod]
        public async Task MemberIdIsRequired()
        {
            var customer = Substitute.For<IMemberService>();
            var points = Substitute.For<IRewardCustomerPointsService>();
            var product = Substitute.For<IProductService>();
            var order = Substitute.For<IOrderService>();
            var item = Substitute.For<IRewardItemService>();
            var rewardService = new RewardService(points, customer, item, product, order);

            var result = await rewardService.Redeem("", "52");
            Assert.IsFalse(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));
        }

        [TestMethod]
        public async Task RewardItemMustBeValidRequired()
        {
            var customer = Substitute.For<IMemberService>();
            var points = Substitute.For<IRewardCustomerPointsService>();
            var product = Substitute.For<IProductService>();
            var order = Substitute.For<IOrderService>();
            customer.GetRewardCustomer("4214").Returns(new RewardCustomer());
            var item = Substitute.For<IRewardItemService>();
            var rewardService = new RewardService(points, customer, item, product, order);

            var result = await rewardService.Redeem("4214", "52");
            Assert.IsFalse(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));
        }

        [TestMethod]
        public async Task CustomerMustHaveEnoughPoints()
        {
            var customer = Substitute.For<IMemberService>();
            var points = Substitute.For<IRewardCustomerPointsService>();
            var product = Substitute.For<IProductService>();
            var order = Substitute.For<IOrderService>();
            points.GetPoints("4214").Returns(10);
            var item = Substitute.For<IRewardItemService>();
            item.GetRewardItem("52").Returns(new RewardItem { Points = 11 });
            var rewardService = new RewardService(points, customer, item, product, order);

            var result = await rewardService.Redeem("4214", "52");
            Assert.IsFalse(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));
        }


        [TestMethod]
        public async Task CustomerWithExactPointsCanRedeem()
        {
            var customer = Substitute.For<IMemberService>();
            var points = Substitute.For<IRewardCustomerPointsService>();
            var product = Substitute.For<IProductService>();
            var order = Substitute.For<IOrderService>();
            product.IsInStock(Arg.Any<string>()).Returns(true);
            order.PlaceOrder("52", "4214").Returns(new OrderResponse { Success = true, OrderId = Guid.NewGuid() });

            points.GetPoints("4214").Returns(100);

            customer.GetRewardCustomer("4214").Returns(new RewardCustomer());
            var item = Substitute.For<IRewardItemService>();
            item.GetRewardItem("52").Returns(new RewardItem { Points = 100 });
            var rewardService = new RewardService(points, customer, item, product, order);

            var result = await rewardService.Redeem("4214", "52");
            Assert.IsTrue(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));

            await points.Received().Update("4214", 0);            
        }

        [TestMethod]
        public async Task CustomerWithEnoughPointsCanRedeem()
        {
            var customer = Substitute.For<IMemberService>();
            var points = Substitute.For<IRewardCustomerPointsService>();
            var product = Substitute.For<IProductService>();
            var order = Substitute.For<IOrderService>();
            product.IsInStock(Arg.Any<string>()).Returns(true);
            order.PlaceOrder("52", "4214").Returns(new OrderResponse { Success = true, OrderId = Guid.NewGuid() });
            
            points.GetPoints("4214").Returns(101);

            customer.GetRewardCustomer("4214").Returns(new RewardCustomer());
            var item = Substitute.For<IRewardItemService>();
            item.GetRewardItem("52").Returns(new RewardItem { Points = 100 });
            var rewardService = new RewardService(points, customer, item, product, order);

            var result = await rewardService.Redeem("4214", "52");
            Assert.IsTrue(result.Success);
            Assert.IsTrue(!string.IsNullOrEmpty(result.Message));

            await points.Received().Update("4214", 1);
        }
    }
}
