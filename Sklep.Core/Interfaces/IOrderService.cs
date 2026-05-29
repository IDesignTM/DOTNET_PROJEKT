using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklep.Core.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(string userId, int productId);
    }
}
