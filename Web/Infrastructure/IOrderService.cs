using System.Collections.Generic;
using Web.Models;

namespace Web.Infrastructure
{
    public interface IOrderService
    {
        IEnumerable<Order> GetOrdersForCompany(int CompanyId);

    }
}
