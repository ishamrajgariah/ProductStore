using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductQueryApi.Models;
using StackExchange.Redis;

namespace ProductQueryApi.Cache.RedisProductCache
{
    public class RedisProductCache : IProductCache
    {
        private IConnectionMultiplexer connection;

        public RedisProductCache(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connection = connectionMultiplexer;
        }

        public IList<Product> GetProducts()
        {
            IDatabase db = connection.GetDatabase();

            EndPoint endPoint = connection.GetEndPoints().First();
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: "*").ToArray();

            List<RedisValue> vals = new List<RedisValue>();
            
            foreach (var key in keys)
            {
               vals.AddRange(db.HashValues(key, CommandFlags.None).ToList());
            }

            return ConvertRedisValsToLocationList(vals);
        }

        public void Put(Product product)
        {
            IDatabase db = connection.GetDatabase();
            db.HashSet(product.ProductId.ToString(), product.ProductId.ToString(), product.ToJsonString());
        }

        public Product Get(Guid productId)
        {
            IDatabase db = connection.GetDatabase();

            var value = (string)db.HashGet(productId.ToString(), productId.ToString());
            Product product = Product.FromJsonString(value);
            return product;
        }

        private IList<Product> ConvertRedisValsToLocationList(List<RedisValue> vals)
        {
            List<Product> products = new List<Product>();

            for (int x = 0; x < vals.Count; x++)
            {
                string val = (string)vals[x];
                Product product = Product.FromJsonString(val);
                products.Add(product);
            }

            return products;
        }
    }

    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisConnectionMultiplexer(this IServiceCollection services,
            IConfiguration config)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var redisConfig = config.GetSection("redis:configstring").Value;

            services.AddSingleton(typeof(IConnectionMultiplexer), ConnectionMultiplexer.ConnectAsync(redisConfig).Result);
            return services;
        }
    }
}
