using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductQueryApi.Models;

namespace ProductQueryApi.Cache
{
    public interface IProductCache
    {
        IList<Product> GetProducts();
        
        void Put(Product product);

        Product Get(Guid productId);
    }
}
