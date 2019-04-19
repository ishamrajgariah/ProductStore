using System;
using Newtonsoft.Json;

namespace ProductQueryApi.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public static Product FromJsonString(string json)
        {
            return JsonConvert.DeserializeObject<Product>(json);
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
