using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProductQueryApi.Cache;
using ProductQueryApi.Models;
using ProductQueryApi.Queues;
using ProductQueryApi.Repository;

namespace ProductQueryApi.Events
{
    public class NewProductEventProcessor : IEventProcessor
    {
        private ILogger logger;
        private IEventSubscriber subscriber;

        public NewProductEventProcessor(
            ILogger<NewProductEventProcessor> logger,
            IEventSubscriber eventSubscriber,
            IProductRepository productRepository,
            IProductCache productCache
        )
        {
            this.logger = logger;
            this.subscriber = eventSubscriber;
            this.subscriber.ProductAddedEventReceived += (prd) => {


                var newProduct = new Product
                {
                    Name = prd.Name,
                    ProductId = prd.ProductId,
                    Category = prd.Category
                };
                productRepository.Add(newProduct);

                productCache.Put(newProduct);
            };
        }

        public void Start()
        {
            this.subscriber.Subscribe();
        }

        public void Stop()
        {
            this.subscriber.Unsubscribe();
        }
    }
}
