using System.Net;
using System.Text.Json;
using NUnit.Framework;
using ShopApiTester.Models;
using ShopApiTester.Utils;

namespace ShopApiTests.Tests
{
    [TestFixture]
    public class ProductTests
    {
        private ApiClient _apiClient;
        private TestData _testData;
        private int _createdProductId;

        [OneTimeSetUp]
        public void Setup()
        {
            _apiClient = new ApiClient();
            _testData = TestDataLoader.LoadTestData();
        }

        [OneTimeTearDown]
        public async Task Cleanup()
        {
            if (_createdProductId != 0)
            {
                await _apiClient.DeleteProduct(_createdProductId);
            }
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnSuccessStatusCode()
        {
            var response = await _apiClient.GetAllProducts();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Get all products request failed");

            var products = await response.Content.ReadAsStringAsync();
            Assert.That(products, Is.Not.Null.And.Not.Empty, "Products list should not be null or empty");
        }

        [Test]
        public async Task AddValidProduct_ShouldReturnSuccessStatusCode()
        {
            var response = await _apiClient.AddProduct(_testData.ValidProduct);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Add product request failed with valid data");

            var responseContent = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(responseContent);
            _createdProductId = product.Id;

            Assert.That(product.Id, Is.GreaterThan(0), "Product ID should be greater than 0");
            VerifyProductFields(product, _testData.ValidProduct);
        }

        [Test]
        public async Task AddInvalidProduct_ShouldReturnError()
        {
            var response = await _apiClient.AddProduct(_testData.InvalidProduct);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Add product request should fail with invalid data");
        }

        [Test]
        public async Task EditProduct_ShouldUpdateProductData()
        {
            // Arrange
            var addResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var addedProduct = JsonSerializer.Deserialize<Product>(await addResponse.Content.ReadAsStringAsync());
            _createdProductId = addedProduct.Id;

            var productToUpdate = _testData.UpdateProduct;
            productToUpdate.Id = _createdProductId;

            // Act
            var updateResponse = await _apiClient.EditProduct(productToUpdate);

            // Assert
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Edit product request failed");

            var updatedProduct = JsonSerializer.Deserialize<Product>(await updateResponse.Content.ReadAsStringAsync());
            VerifyProductFields(updatedProduct, productToUpdate);
        }

        [Test]
        public async Task DeleteProduct_ShouldRemoveProduct()
        {
            // Arrange
            var addResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var addedProduct = JsonSerializer.Deserialize<Product>(await addResponse.Content.ReadAsStringAsync());
            int productId = addedProduct.Id;

            // Act
            var deleteResponse = await _apiClient.DeleteProduct(productId);

            // Assert
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Delete product request failed");

            var getResponse = await _apiClient.GetAllProducts();
            var products = JsonSerializer.Deserialize<Product[]>(await getResponse.Content.ReadAsStringAsync());
            Assert.That(products.Any(p => p.Id == productId), Is.False, "Product was not deleted");
        }

        private void VerifyProductFields(Product actual, Product expected)
        {
            Assert.That(actual.Category_id, Is.EqualTo(expected.Category_id), "Category_id mismatch");
            Assert.That(actual.Title, Is.EqualTo(expected.Title), "Title mismatch");
            Assert.That(actual.Content, Is.EqualTo(expected.Content), "Content mismatch");
            Assert.That(actual.Price, Is.EqualTo(expected.Price), "Price mismatch");
            Assert.That(actual.Old_price, Is.EqualTo(expected.Old_price), "Old_price mismatch");
            Assert.That(actual.Status, Is.EqualTo(expected.Status), "Status mismatch");
            Assert.That(actual.Keywords, Is.EqualTo(expected.Keywords), "Keywords mismatch");
            Assert.That(actual.Description, Is.EqualTo(expected.Description), "Description mismatch");
            Assert.That(actual.Hit, Is.EqualTo(expected.Hit), "Hit mismatch");

            // Verify alias generation
            if (!string.IsNullOrEmpty(expected.Title))
            {
                Assert.That(actual.Alias, Is.Not.Null.And.Not.Empty, "Alias should be generated from title");
            }
        }
    }
}