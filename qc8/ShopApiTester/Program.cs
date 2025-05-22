using ShopApiTester.Utils;
using Newtonsoft.Json;
using ShopApiTester.Models;

namespace ShopApiTester;

class Program
{
    static async Task Main(string[] args)
    {
        await Print();
    }

    public static async Task Print()
    {
        ApiClient api = new ApiClient();
        var response = api.GetAllProducts().Result.Content.ReadAsStringAsync();
        var products = JsonConvert.DeserializeObject<List<Product>>(await response);
        foreach (var product in products!)
        {
            System.Console.WriteLine(product.Title);
        }
    }
}
