using System.Text;
using System.Text.Json;
using ShopApiTester.Models;

namespace ShopApiTester.Utils
{
    public class ApiClient
    {
        private static readonly string BASE_URL = "http://shop.qatl.ru/";
        private readonly HttpClient _client;

        public ApiClient()
        {
            _client = new HttpClient();
        }

        public async Task<HttpResponseMessage> GetAllProducts()
        {
            return await _client.GetAsync($"{BASE_URL}api/products");
        }

        public async Task<HttpResponseMessage> DeleteProduct(int id)
        {
            return await _client.GetAsync($"{BASE_URL}api/deleteproduct?id={id}");
        }

        public async Task<HttpResponseMessage> AddProduct(Product product)
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"{BASE_URL}api/addproduct", content);
        }

        public async Task<HttpResponseMessage> EditProduct(Product product)
        {
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _client.PostAsync($"{BASE_URL}api/editproduct", content);
        }
    }
}