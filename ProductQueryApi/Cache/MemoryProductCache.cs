using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using ProductQueryApi.Models;

namespace ProductQueryApi.Cache
{
    public class MemoryProductCache : IProductCache
    {
        private IMemoryCache _cache;


        public MemoryProductCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IList<Product> GetProducts()
        {
            return new List<Product>();
        }

        public void Put(Product product)
        {
            _cache.Set(product.ProductId, product,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
        }

        public Product Get(Guid productId)
        {
            return _cache.Get<Product>(productId);
        }
    }
}

