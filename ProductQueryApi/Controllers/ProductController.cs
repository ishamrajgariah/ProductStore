using Microsoft.AspNetCore.Mvc;
using ProductQueryApi.Models;
using ProductQueryApi.Repository;
using System;
using System.Collections.Generic;
using ProductQueryApi.Cache;

namespace ProductQueryApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private IProductRepository productRepository;

        private IProductCache productCache;

        public ProductController( 
            IProductRepository productRepository, IProductCache productCache)
        {
            this.productRepository = productRepository;
            this.productCache = productCache;
        }

        [HttpGet]
        public ICollection<Product> All()
        {
            return this.productCache.GetProducts();
        }


    }
}
