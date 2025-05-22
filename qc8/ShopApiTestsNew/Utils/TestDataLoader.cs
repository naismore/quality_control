using System.Text.Json;
using ShopApiTester.Models;

namespace ShopApiTester.Utils
{
    public static class TestDataLoader
    {
        public static TestData LoadTestData()
        {
            var json = File.ReadAllText("..\\Config\\TestData.json");
            return JsonSerializer.Deserialize<TestData>(json);
        }
    }
}

/*
Добавление, удаление, алиасы
*/