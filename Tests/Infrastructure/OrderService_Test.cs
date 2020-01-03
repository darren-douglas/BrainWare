using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Infrastructure;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using Web.Models;

namespace Tests.Infrastructure
{
    [TestClass]
    public class OrderService_Test
    {
        const string COLUMN_ORDER_NAME = "name";
        const string COLUMN_ORDER_DESCRIPTION = "description";
        const string COLUMN_ORDER_ID = "order_id";
        const string COLUMN_PRODUCT_ID = "product_id";
        const string COLUMN_QUANTITY = "quantity";
        const string COLUMN_ORDER_PRICE = "price";
        const string COLUMN_PRODUCT_NAME = "name";
        const string COLUMN_PRODUCT_PRICE = "price";


        private IOrderService orderService;
        private Mock<IStorage> database = new Mock<IStorage>();

        public OrderService_Test()
        {
            orderService = new OrderService(database.Object);
        }

        [TestMethod]
        public void GetOrdersForCompany_NoValidCompanyId()
        {
            const int CompanyId = 99;

            var dataReader = new Mock<IDataReader>();
            dataReader.SetupSequence(m => m.Read()).Returns(false);

            var args = new Dictionary<string, string>
            {
                { COLUMN_ORDER_NAME, CompanyId.ToString()}
            };

            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), args)).Returns(dataReader.Object);

            // Call the method
            var ordersResult = orderService.GetOrdersForCompany(CompanyId).ToList();
            Assert.IsNotNull(ordersResult);
            Assert.AreEqual(ordersResult.Count, 0);
        }

        [TestMethod]
        public void GetOrdersForCompany_ValidCompanyId_OneOrder()
        {
            const int CompanyId = 1;

            Product expectedProduct = new Product
            {
                Name = "test product name",
                Price = 20.00m
            };

            OrderProduct expectedOrderProduct = new OrderProduct
            {
                OrderId = 1,
                Price = 20.00m,
                Quantity = 5,
                ProductId = 111,
                Product = expectedProduct
            };

            var expectedOrderProducts = new List<OrderProduct>();
            expectedOrderProducts.Add(expectedOrderProduct);

            Order expectedOrder = new Order()
            {
                CompanyName = "test name",
                Description = "test description",
                OrderId = 1,
                OrderProducts = expectedOrderProducts
            };

            var companyReader = new Mock<IDataReader>();
            companyReader.Setup(m => m[COLUMN_ORDER_NAME]).Returns(expectedOrder.CompanyName);
            companyReader.Setup(m => m[COLUMN_ORDER_DESCRIPTION]).Returns(expectedOrder.Description);
            companyReader.Setup(m => m[COLUMN_ORDER_ID]).Returns(expectedOrder.OrderId);
            companyReader.SetupSequence(m => m.Read()).Returns(true).Returns(false);

            var args = new Dictionary<string, string>
            {
                { COLUMN_ORDER_NAME, CompanyId.ToString()}
            };

            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), args)).Returns(companyReader.Object);

            var orderReader = new Mock<IDataReader>();
            orderReader.Setup(m => m[COLUMN_ORDER_ID]).Returns(expectedOrderProduct.OrderId);
            orderReader.Setup(m => m[COLUMN_ORDER_PRICE]).Returns(expectedOrderProduct.Price);
            orderReader.Setup(m => m[COLUMN_PRODUCT_ID]).Returns(expectedOrderProduct.ProductId);
            orderReader.Setup(m => m[COLUMN_PRODUCT_NAME]).Returns(expectedProduct.Name);
            orderReader.Setup(m => m[COLUMN_PRODUCT_PRICE]).Returns(expectedProduct.Price);
            orderReader.Setup(m => m[COLUMN_QUANTITY]).Returns(expectedOrderProduct.Quantity);
            orderReader.SetupSequence(m => m.Read()).Returns(true);

            var orderargs = new Dictionary<string, string>
            {
                { COLUMN_ORDER_ID, "1"}
            };


            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), orderargs)).Returns(orderReader.Object);

            // Call the method
            var ordersResult = orderService.GetOrdersForCompany(CompanyId).ToList();
            Assert.IsNotNull(ordersResult);
            Assert.AreEqual(ordersResult.Count, 1);
            var retOrder = ordersResult.First();
            Assert.AreEqual(retOrder.OrderTotal, expectedProduct.Price * expectedOrderProduct.Quantity);
            Assert.AreEqual(retOrder.OrderId, expectedOrder.OrderId);
            Assert.AreEqual(retOrder.CompanyName, expectedOrder.CompanyName);
            Assert.AreEqual(retOrder.Description, expectedOrder.Description);
            var retOrderProducts = retOrder.OrderProducts.First();
            Assert.AreEqual(retOrderProducts.OrderId, expectedOrderProduct.OrderId);
            Assert.AreEqual(retOrderProducts.ProductId, expectedOrderProduct.ProductId);
            Assert.AreEqual(retOrderProducts.Price, expectedOrderProduct.Price);
            Assert.AreEqual(retOrderProducts.Quantity, expectedOrderProduct.Quantity);
            var retProduct = retOrderProducts.Product;
            Assert.AreEqual(retProduct.Name, expectedProduct.Name);
            Assert.AreEqual(retProduct.Price, expectedProduct.Price);
        }

        [TestMethod]
        public void GetOrdersForCompany_ValidCompanyId_MultiOrder()
        {
            const int CompanyId = 1;

            Product expectedProduct = new Product
            {
                Name = "test product name",
                Price = 20.00m
            };

            OrderProduct expectedOrderProduct = new OrderProduct
            {
                OrderId = 1,
                Price = 20.00m,
                Quantity = 5,
                ProductId = 111,
                Product = expectedProduct
            };

            var expectedOrderProducts = new List<OrderProduct>();
            expectedOrderProducts.Add(expectedOrderProduct);
            expectedOrderProducts.Add(expectedOrderProduct);

            Order expectedOrder = new Order()
            {
                CompanyName = "test name",
                Description = "test description",
                OrderId = 1,
                OrderProducts = expectedOrderProducts
            };

            var companyReader = new Mock<IDataReader>();
            companyReader.Setup(m => m[COLUMN_ORDER_NAME]).Returns(expectedOrder.CompanyName);
            companyReader.Setup(m => m[COLUMN_ORDER_DESCRIPTION]).Returns(expectedOrder.Description);
            companyReader.Setup(m => m[COLUMN_ORDER_ID]).Returns(expectedOrder.OrderId);
            companyReader.SetupSequence(m => m.Read()).Returns(true).Returns(false);

            var args = new Dictionary<string, string>
            {
                { COLUMN_ORDER_NAME, CompanyId.ToString()}
            };

            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), args)).Returns(companyReader.Object);

            var orderReader = new Mock<IDataReader>();
            orderReader.Setup(m => m[COLUMN_ORDER_ID]).Returns(expectedOrderProduct.OrderId);
            orderReader.Setup(m => m[COLUMN_ORDER_PRICE]).Returns(expectedOrderProduct.Price);
            orderReader.Setup(m => m[COLUMN_PRODUCT_ID]).Returns(expectedOrderProduct.ProductId);
            orderReader.Setup(m => m[COLUMN_PRODUCT_NAME]).Returns(expectedProduct.Name);
            orderReader.Setup(m => m[COLUMN_PRODUCT_PRICE]).Returns(expectedProduct.Price);
            orderReader.Setup(m => m[COLUMN_QUANTITY]).Returns(expectedOrderProduct.Quantity);
            orderReader.SetupSequence(m => m.Read()).Returns(true).Returns(true);

            var orderargs = new Dictionary<string, string>
            {
                { COLUMN_ORDER_ID, "1"}
            };


            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), orderargs)).Returns(orderReader.Object);

            // Call the method
            var ordersResult = orderService.GetOrdersForCompany(CompanyId).ToList();
            Assert.IsNotNull(ordersResult);
            Assert.AreEqual(ordersResult.Count, 1);
            var retOrder = ordersResult.First();
            Assert.AreEqual(retOrder.OrderTotal, 2 * expectedProduct.Price * expectedOrderProduct.Quantity);
            Assert.AreEqual(retOrder.OrderId, expectedOrder.OrderId);
            Assert.AreEqual(retOrder.CompanyName, expectedOrder.CompanyName);
            Assert.AreEqual(retOrder.Description, expectedOrder.Description);


            // Check the second product order
            Assert.AreEqual(retOrder.OrderProducts.Count, 2);
            var retOrderProducts = retOrder.OrderProducts.Last();
            Assert.AreEqual(retOrderProducts.OrderId, expectedOrderProduct.OrderId);
            Assert.AreEqual(retOrderProducts.ProductId, expectedOrderProduct.ProductId);
            Assert.AreEqual(retOrderProducts.Price, expectedOrderProduct.Price);
            Assert.AreEqual(retOrderProducts.Quantity, expectedOrderProduct.Quantity);
            var retProduct = retOrderProducts.Product;
            Assert.AreEqual(retProduct.Name, expectedProduct.Name);
            Assert.AreEqual(retProduct.Price, expectedProduct.Price);
        }

        [TestMethod]
        public void getOrderDetails_OneProducts()
        {
            OrderService orderservice = new OrderService(database.Object);
            PrivateObject obj = new PrivateObject(orderservice);

            Product expectedProduct = new Product
            {
                Name = "test product name",
                Price = 20.00m
            };

            OrderProduct expectedOrderProduct = new OrderProduct
            {
                OrderId = 1,
                Price = 20.00m,
                Quantity = 5,
                ProductId = 111,
                Product = expectedProduct
            };

            var expectedOrderProducts = new List<OrderProduct>();
            expectedOrderProducts.Add(expectedOrderProduct);

            var orderReader = new Mock<IDataReader>();
            orderReader.Setup(m => m[COLUMN_ORDER_ID]).Returns(expectedOrderProduct.OrderId);
            orderReader.Setup(m => m[COLUMN_ORDER_PRICE]).Returns(expectedOrderProduct.Price);
            orderReader.Setup(m => m[COLUMN_PRODUCT_ID]).Returns(expectedOrderProduct.ProductId);
            orderReader.Setup(m => m[COLUMN_PRODUCT_NAME]).Returns(expectedProduct.Name);
            orderReader.Setup(m => m[COLUMN_PRODUCT_PRICE]).Returns(expectedProduct.Price);
            orderReader.Setup(m => m[COLUMN_QUANTITY]).Returns(expectedOrderProduct.Quantity);
            orderReader.SetupSequence(m => m.Read()).Returns(true);

            var orderargs = new Dictionary<string, string>
            {
                { COLUMN_ORDER_ID, "1"}
            };


            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), orderargs)).Returns(orderReader.Object);

            object[] args = new object[1] { 1 };

            List<OrderProduct> retVal =  (List <OrderProduct> )obj.Invoke("getOrderDetails", args);
            Assert.IsNotNull(retVal);
            Assert.AreEqual(actual: retVal.Count, expected: 1);
            var retOrderProducts = retVal.First();
            Assert.AreEqual(retOrderProducts.OrderId, expectedOrderProduct.OrderId);
            Assert.AreEqual(retOrderProducts.ProductId, expectedOrderProduct.ProductId);
            Assert.AreEqual(retOrderProducts.Price, expectedOrderProduct.Price);
            Assert.AreEqual(retOrderProducts.Quantity, expectedOrderProduct.Quantity);
            var retProduct = retOrderProducts.Product;
            Assert.AreEqual(retProduct.Name, expectedProduct.Name);
            Assert.AreEqual(retProduct.Price, expectedProduct.Price);
        }

        [TestMethod]
        public void getOrderDetails_MultiProducts()
        {
            OrderService orderservice = new OrderService(database.Object);
            PrivateObject obj = new PrivateObject(orderservice);

            Product expectedProduct = new Product
            {
                Name = "test product name",
                Price = 20.00m
            };

            OrderProduct expectedOrderProduct = new OrderProduct
            {
                OrderId = 1,
                Price = 20.00m,
                Quantity = 5,
                ProductId = 111,
                Product = expectedProduct
            };

            var expectedOrderProducts = new List<OrderProduct>();
            expectedOrderProducts.Add(expectedOrderProduct);

            var orderReader = new Mock<IDataReader>();
            orderReader.Setup(m => m[COLUMN_ORDER_ID]).Returns(expectedOrderProduct.OrderId);
            orderReader.Setup(m => m[COLUMN_ORDER_PRICE]).Returns(expectedOrderProduct.Price);
            orderReader.Setup(m => m[COLUMN_PRODUCT_ID]).Returns(expectedOrderProduct.ProductId);
            orderReader.Setup(m => m[COLUMN_PRODUCT_NAME]).Returns(expectedProduct.Name);
            orderReader.Setup(m => m[COLUMN_PRODUCT_PRICE]).Returns(expectedProduct.Price);
            orderReader.Setup(m => m[COLUMN_QUANTITY]).Returns(expectedOrderProduct.Quantity);
            orderReader.SetupSequence(m => m.Read()).Returns(true).Returns(true);

            var orderargs = new Dictionary<string, string>
            {
                { COLUMN_ORDER_ID, "1"}
            };


            database.Setup(x => x.ExecuteQuery(It.IsAny<string>(), orderargs)).Returns(orderReader.Object);

            object[] args = new object[1] { 1 };

            List<OrderProduct> retVal = (List<OrderProduct>)obj.Invoke("getOrderDetails", args);
            Assert.IsNotNull(retVal);
            Assert.AreEqual(actual: retVal.Count, expected: 2);
            var retOrderProducts = retVal.First();
            Assert.AreEqual(retOrderProducts.OrderId, expectedOrderProduct.OrderId);
            Assert.AreEqual(retOrderProducts.ProductId, expectedOrderProduct.ProductId);
            Assert.AreEqual(retOrderProducts.Price, expectedOrderProduct.Price);
            Assert.AreEqual(retOrderProducts.Quantity, expectedOrderProduct.Quantity);
            var retProduct = retOrderProducts.Product;
            Assert.AreEqual(retProduct.Name, expectedProduct.Name);
            Assert.AreEqual(retProduct.Price, expectedProduct.Price);

            retOrderProducts = retVal.Last();
            Assert.AreEqual(retOrderProducts.OrderId, expectedOrderProduct.OrderId);
            Assert.AreEqual(retOrderProducts.ProductId, expectedOrderProduct.ProductId);
            Assert.AreEqual(retOrderProducts.Price, expectedOrderProduct.Price);
            Assert.AreEqual(retOrderProducts.Quantity, expectedOrderProduct.Quantity);
            retProduct = retOrderProducts.Product;
            Assert.AreEqual(retProduct.Name, expectedProduct.Name);
            Assert.AreEqual(retProduct.Price, expectedProduct.Price);
        }


        [TestMethod]
        public void getOrderDetails_NoProduct()
        {
            OrderService orderservice = new OrderService(database.Object);
            PrivateObject obj = new PrivateObject(orderservice);

            object[] args = new object[1] { 1 };

            List<OrderProduct> retVal =  (List <OrderProduct> )obj.Invoke("getOrderDetails", args);
            Assert.IsNotNull(retVal);
            Assert.AreEqual(expected: retVal.Count, actual: 0);
        }


        [TestMethod]
        public void getOrderTotal_NoProducts()
        {
            OrderService orderservice = new OrderService();
            PrivateObject obj = new PrivateObject(orderservice);
            var orderProducts = new List<OrderProduct>();

            object[] args = new object[1] { orderProducts };

            decimal expectedVal = 0;
            var retVal = obj.Invoke("getOrderTotal", args);
            Assert.AreEqual(expectedVal, retVal);

        }

        [TestMethod]
        public void getOrderTotal_OneProductMulitpleQuantity()
        {
            OrderService orderservice = new OrderService();
            PrivateObject obj = new PrivateObject(orderservice);
            var orderProducts = new List<OrderProduct>();

            var orderProduct1 = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                Quantity = 4,
                Price = 1,
                Product = new Product()
                {
                    Name = "test",
                    Price = 1
                }
            };

            orderProducts.Add(orderProduct1);

            object[] args = new object[1] { orderProducts };

            decimal expectedVal = 4;
            var retVal = obj.Invoke("getOrderTotal", args);
            Assert.AreEqual(expectedVal, retVal);

        }

        [TestMethod]
        public void getOrderTotal_MultipleProducts()
        {
            OrderService orderservice = new OrderService();
            PrivateObject obj = new PrivateObject(orderservice);
            var orderProducts = new List<OrderProduct>();

            var orderProduct1 = new OrderProduct
            {
                OrderId = 1,
                ProductId = 1,
                Quantity = 4,
                Price = 1,
                Product = new Product()
                {
                    Name = "test",
                    Price = 1
                }
            };

            orderProducts.Add(orderProduct1);

            var orderProduct2 = new OrderProduct
            {
                OrderId = 1,
                ProductId = 2,
                Quantity = 3,
                Price = 1,
                Product = new Product()
                {
                    Name = "test",
                    Price = 1
                }
            };

            orderProducts.Add(orderProduct2);


            object[] args = new object[1] { orderProducts };

            decimal expectedVal = 7;
            var retVal = obj.Invoke("getOrderTotal", args);
            Assert.AreEqual(expectedVal, retVal);

        }
    }
}