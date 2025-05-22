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
                await TryDeleteProduct(_createdProductId);
            }
        }

        #region Helper Methods
        private async Task TryDeleteProduct(int id)
        {
            try
            {
                await _apiClient.DeleteProduct(id);
            }
            catch
            {
                System.Console.WriteLine("Couldn't delete product");
            }
        }

        private void VerifyProductFields(Product actual, Product expected)
        {
            Assert.That(actual.Category_id, Is.EqualTo(expected.Category_id), $"Category_id mismatch. Expected: {expected.Category_id}, Actual: {actual.Category_id}");
            Assert.That(actual.Title, Is.EqualTo(expected.Title), $"Title mismatch. Expected: {expected.Title}, Actual: {actual.Title}");
            Assert.That(actual.Content, Is.EqualTo(expected.Content), $"Content mismatch. Expected: {expected.Content}, Actual: {actual.Content}");
            Assert.That(actual.Price, Is.EqualTo(expected.Price), $"Price mismatch. Expected: {expected.Price}, Actual: {actual.Price}");
            Assert.That(actual.Old_price, Is.EqualTo(expected.Old_price), $"Old_price mismatch. Expected: {expected.Old_price}, Actual: {actual.Old_price}");
            Assert.That(actual.Status, Is.EqualTo(expected.Status), $"Status mismatch. Expected: {expected.Status}, Actual: {actual.Status}");
            Assert.That(actual.Keywords, Is.EqualTo(expected.Keywords), $"Keywords mismatch. Expected: {expected.Keywords}, Actual: {actual.Keywords}");
            Assert.That(actual.Description, Is.EqualTo(expected.Description), $"Description mismatch. Expected: {expected.Description}, Actual: {actual.Description}");
            Assert.That(actual.Hit, Is.EqualTo(expected.Hit), $"Hit mismatch. Expected: {expected.Hit}, Actual: {actual.Hit}");
        }
        #endregion

        #region GET Products Tests
        [Test]
        public async Task GetAllProducts_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _apiClient.GetAllProducts();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "GET /api/products should return 200 OK");

            var products = JsonSerializer.Deserialize<Product[]>(await response.Content.ReadAsStringAsync());
            Assert.That(products, Is.Not.Null, "Products list should not be null");
            Assert.That(products.Length, Is.GreaterThanOrEqualTo(0), "Products list may be empty but should exist");
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnValidProductStructure()
        {
            // Act
            var response = await _apiClient.GetAllProducts();
            var products = JsonSerializer.Deserialize<Product[]>(await response.Content.ReadAsStringAsync());

            // Assert
            if (products.Length > 0)
            {
                var sampleProduct = products[0];
                Assert.That(sampleProduct.Id, Is.GreaterThan(0), "Product ID should be positive");
                Assert.That(sampleProduct.Title, Is.Not.Null.And.Not.Empty, "Product title should not be empty");
                Assert.That(sampleProduct.Category_id, Is.InRange(1, 15), "Category_id should be between 1 and 15");
            }
        }
        #endregion

        #region POST AddProduct Tests
        [Test]
        public async Task AddProduct_WithValidData_ShouldCreateProduct()
        {
            // Act
            var response = await _apiClient.AddProduct(_testData.ValidProduct);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK for valid product");

            var createdProduct = JsonSerializer.Deserialize<Product>(await response.Content.ReadAsStringAsync());
            _createdProductId = createdProduct.Id;

            Assert.That(createdProduct.Id, Is.GreaterThan(0), "Product ID should be assigned");
            VerifyProductFields(createdProduct, _testData.ValidProduct);
            Assert.That(createdProduct.Alias, Is.Not.Null.And.Not.Empty, "Alias should be generated");
        }

        [Test]
        public async Task AddProduct_WithDuplicateTitle_ShouldGenerateUniqueAlias()
        {
            // Arrange - create first product
            var firstResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var firstProduct = JsonSerializer.Deserialize<Product>(await firstResponse.Content.ReadAsStringAsync());
            _createdProductId = firstProduct.Id;

            // Act - create second product with same title
            var secondResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var secondProduct = JsonSerializer.Deserialize<Product>(await secondResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.That(secondProduct.Alias, Is.Not.EqualTo(firstProduct.Alias),
                "Duplicate titles should generate unique aliases");

            // Cleanup
            await TryDeleteProduct(secondProduct.Id);
        }

        [Test]
        public async Task AddProduct_WithEmptyTitle_ShouldFail()
        {
            // Arrange
            var invalidProduct = new Product { Title = "" };

            // Act
            var response = await _apiClient.AddProduct(invalidProduct);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not accept product with empty title");
        }

        [Test]
        public async Task AddProduct_WithInvalidCategory_ShouldFail()
        {
            // Arrange
            var invalidProduct = _testData.ValidProduct;
            invalidProduct.Category_id = 20;

            // Act
            var response = await _apiClient.AddProduct(invalidProduct);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not accept product with invalid category");
        }

        [Test]
        public async Task AddProduct_WithNegativePrice_ShouldFail()
        {
            // Arrange
            var invalidProduct = _testData.ValidProduct;
            invalidProduct.Price = -100;

            // Act
            var response = await _apiClient.AddProduct(invalidProduct);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not accept product with negative price");
        }

        [Test]
        public async Task AddProduct_WithInvalidStatus_ShouldFail()
        {
            // Arrange
            var invalidProduct = _testData.ValidProduct;
            invalidProduct.Status = -2;

            // Act
            var response = await _apiClient.AddProduct(invalidProduct);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not accept product with invalid status");
        }
        #endregion

        #region POST EditProduct Tests
        [Test]
        public async Task EditProduct_ShouldUpdateAllFields()
        {
            // Arrange - create product
            var createResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var createdProduct = JsonSerializer.Deserialize<Product>(await createResponse.Content.ReadAsStringAsync());
            _createdProductId = createdProduct.Id;

            // Act - update product
            var updateData = _testData.UpdateProduct;
            updateData.Id = _createdProductId;
            var updateResponse = await _apiClient.EditProduct(updateData);

            // Assert
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK for successful update");

            var updatedProduct = JsonSerializer.Deserialize<Product>(await updateResponse.Content.ReadAsStringAsync());
            VerifyProductFields(updatedProduct, updateData);
        }

        [Test]
        public async Task EditProduct_WithNonexistentId_ShouldFail()
        {
            // Arrange
            var nonexistentId = 999999;
            var updateData = _testData.ValidProduct;
            updateData.Id = nonexistentId;

            // Act
            var response = await _apiClient.EditProduct(updateData);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should fail when editing nonexistent product");
        }

        [Test]
        public async Task EditProduct_WithInvalidData_ShouldFail()
        {
            // Arrange - create product
            var createResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var createdProduct = JsonSerializer.Deserialize<Product>(await createResponse.Content.ReadAsStringAsync());
            _createdProductId = createdProduct.Id;

            // Act - try invalid update
            var invalidUpdate = createdProduct;
            invalidUpdate.Title = "";
            invalidUpdate.Category_id = 20;
            var response = await _apiClient.EditProduct(invalidUpdate);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should not accept invalid update data");
        }
        #endregion

        #region GET DeleteProduct Tests
        [Test]
        public async Task DeleteProduct_ShouldRemoveProductFromSystem()
        {
            // Arrange - create product
            var createResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var createdProduct = JsonSerializer.Deserialize<Product>(await createResponse.Content.ReadAsStringAsync());
            int productId = createdProduct.Id;

            // Act - delete product
            var deleteResponse = await _apiClient.DeleteProduct(productId);

            // Assert
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should return 200 OK for successful deletion");

            // Verify product is gone
            var getResponse = await _apiClient.GetAllProducts();
            var products = JsonSerializer.Deserialize<Product[]>(await getResponse.Content.ReadAsStringAsync());
            Assert.That(products.Any(p => p.Id == productId), Is.False,
                "Product should not exist after deletion");
        }

        [Test]
        public async Task DeleteProduct_WithNonexistentId_ShouldFail()
        {
            // Arrange
            int nonexistentId = 999999;

            // Act
            var response = await _apiClient.DeleteProduct(nonexistentId);

            // Assert
            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Should fail when deleting nonexistent product");
        }

        [Test]
        public async Task DeleteProduct_Twice_ShouldFailOnSecondAttempt()
        {
            // Arrange - create and delete product
            var createResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var createdProduct = JsonSerializer.Deserialize<Product>(await createResponse.Content.ReadAsStringAsync());
            int productId = createdProduct.Id;

            var firstDelete = await _apiClient.DeleteProduct(productId);
            Assert.That(firstDelete.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "First deletion should succeed");

            // Act - try to delete again
            var secondDelete = await _apiClient.DeleteProduct(productId);

            // Assert
            Assert.That(secondDelete.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK),
                "Second deletion should fail");
        }
        #endregion

        #region Edge Cases
        [Test]
        public async Task AddProduct_WithMaxLengthFields_ShouldSucceed()
        {
            // Arrange
            var longString = new string('a', 1000);
            var product = _testData.ValidProduct;
            product.Title = longString;
            product.Content = longString;
            product.Keywords = longString;
            product.Description = longString;

            // Act
            var response = await _apiClient.AddProduct(product);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should accept long field values");

            var createdProduct = JsonSerializer.Deserialize<Product>(await response.Content.ReadAsStringAsync());
            _createdProductId = createdProduct.Id;

            Assert.That(createdProduct.Title, Is.EqualTo(longString), "Title should preserve long value");
        }

        [Test]
        public async Task AddProduct_WithMinimalValidData_ShouldSucceed()
        {
            // Arrange
            var minimalProduct = new Product
            {
                Title = "Minimal Product",
                Category_id = 1,
                Price = 1
            };

            // Act
            var response = await _apiClient.AddProduct(minimalProduct);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "Should accept minimal valid product");

            var createdProduct = JsonSerializer.Deserialize<Product>(await response.Content.ReadAsStringAsync());
            _createdProductId = createdProduct.Id;

            Assert.That(createdProduct.Title, Is.EqualTo("Minimal Product"), "Title should be preserved");
            Assert.That(createdProduct.Alias, Is.Not.Null.And.Not.Empty, "Alias should be generated");
        }

        [Test]
        public async Task EditProduct_ShouldPreserveAliasWhenTitleChanges()
        {
            // Arrange - create product
            var createResponse = await _apiClient.AddProduct(_testData.ValidProduct);
            var createdProduct = JsonSerializer.Deserialize<Product>(await createResponse.Content.ReadAsStringAsync());
            _createdProductId = createdProduct.Id;
            string originalAlias = createdProduct.Alias;

            // Act - update title but keep same ID
            var updateData = createdProduct;
            updateData.Title = "Updated Title";
            var updateResponse = await _apiClient.EditProduct(updateData);

            // Assert
            var updatedProduct = JsonSerializer.Deserialize<Product>(await updateResponse.Content.ReadAsStringAsync());
            Assert.That(updatedProduct.Alias, Is.EqualTo(originalAlias),
                "Alias should not change when editing product");
        }
        #endregion
    }
}