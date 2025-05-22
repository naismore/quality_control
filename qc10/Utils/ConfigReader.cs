using System.IO;
using Newtonsoft.Json;

public class TestConfig
{
    public string BaseUrl { get; set; }
    public UserData CorrectUser { get; set; }
    public UserData IncorrectUser { get; set; }
    public ProductData Product { get; set; }
    public OrderData Order { get; set; }
}

public class UserData
{
    public string Login { get; set; }
    public string Password { get; set; }
}

public class ProductData
{
    public string Name { get; set; }
}

public class OrderData
{
    public string SuccessMessage { get; set; }
}

public static class ConfigReader
{
    public static TestConfig LoadConfig(string path)
    {
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<TestConfig>(json);
    }
}
