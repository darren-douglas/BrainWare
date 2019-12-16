using System;
using System.Collections.Generic;

namespace Web.Infrastructure
{
    using System.Data;
    using Models;
    using System.Data.SqlClient;

    public class OrderService
    {
        private const string DB_CONNECTION_STRING = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=BrainWAre;Integrated Security=SSPI;AttachDBFilename=e:\\BrainWare\\Web\\App_Data\\BrainWare.mdf";

        public List<Order> GetOrdersForCompany(int CompanyId)
        {
            // These should be defined centraly to match the schema names for the columns
            const string COLUMN_ORDER_NAME = "name";
            const string COLUMN_ORDER_DESCRIPTION = "description";
            const string COLUMN_ORDER_ID = "order_id";

            var values = new List<Order>();
            Database connection = new Database(DB_CONNECTION_STRING);

            try
            {
                // Open the connection to the database.
                connection.init();

                // Get the orders
                var sql = "SELECT c.name, o.description, o.order_id FROM company c INNER JOIN [order] o on c.company_id=o.company_id";
                SqlDataReader reader = (SqlDataReader)connection.ExecuteReader(sql);

                try
                {
                    // If we have results process them into an list of objects
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                CompanyName = reader[COLUMN_ORDER_NAME].ToString(),
                                Description = reader[COLUMN_ORDER_DESCRIPTION].ToString(),
                                OrderId = (int)reader[COLUMN_ORDER_ID],
                                OrderProducts = new List<OrderProduct>()
                            };

                            // Load the order product details.
                            order.OrderProducts = getOrderDetails(order.OrderId);

                            // Calculate the total of the order
                            order.OrderTotal = getOrderTotal(order.OrderProducts);

                            values.Add(order);
                        }
                    }

                }
                finally
                {
                    // Close the reader
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Normally Log an error 
                Console.Write("Exception caught - " + ex.Message);
            }
            finally
            {
                // Close the connection to the database
                connection.uninit();
            }

            return values;
        }

        /// <summary>
        /// Return the order details for the order id specified
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns>List of OrderProduct objects</returns>
        private List<OrderProduct> getOrderDetails(Int32 OrderId)
        {
            // These should be defined centraly to match the schema names for the columns
            const string COLUMN_ORDER_ID = "order_id";
            const string COLUMN_PRODUCT_ID = "product_id";
            const string COLUMN_QUANTITY = "quantity";
            const string COLUMN_PRODUCT_NAME = "name";
            const string COLUMN_PRODUCT_PRICE = "price";

            var orderProducts = new List<OrderProduct>();

            Database connection = new Database(DB_CONNECTION_STRING);
            try
            {
                // Open the connection to the database
                connection.init();

                // Create the SQL statement for the requested order id
                var args = new Dictionary<string, string>
                {
                    { COLUMN_ORDER_ID, OrderId.ToString()}
                };

                var sql =
                    "SELECT op.price, op.order_id, op.product_id, op.quantity, p.name, p.price FROM orderproduct op INNER JOIN product p on op.product_id=p.product_id WHERE op.order_id = @" + COLUMN_ORDER_ID;
                SqlDataReader reader = (SqlDataReader)connection.ExecuteReader(sql, args);

                try
                {
                    // If there are results load them into a list of order product objects
                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var orderProduct = new OrderProduct
                            {
                                OrderId = (int)reader[COLUMN_ORDER_ID],
                                ProductId = (int)reader[COLUMN_PRODUCT_ID],
                                Quantity = (int)reader[COLUMN_QUANTITY],
                                Product = new Product()
                                {
                                    Name = reader[COLUMN_PRODUCT_NAME].ToString(),
                                    Price = (Decimal)reader[COLUMN_PRODUCT_PRICE]
                                }
                            };

                            orderProducts.Add(orderProduct);
                        }
                    }
                }
                finally
                {
                    // Close the reader
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // Normally Log an error 
                Console.Write("Exception caught - " + ex.Message);
            }
            finally
            {
                // Close the connection the database
                connection.uninit();
            }

            return orderProducts;
        }

        /// <summary>
        /// Return the total of the order.
        /// </summary>
        /// <param name="orderProducts"></param>
        /// <returns>The total of the items in the order</returns>
        private Decimal getOrderTotal(List<OrderProduct> orderProducts)
        {
            Decimal OrderTotal = 0;

            // Total up all the items in the order
            foreach (var orderproduct in orderProducts)
            {
                OrderTotal += (orderproduct.Product.Price * orderproduct.Quantity);
            }

            return OrderTotal;
        }
    }
}