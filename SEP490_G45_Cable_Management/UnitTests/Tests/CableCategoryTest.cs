using API.Services.CableCategories;
using Common.Const;
using Common.DTO.CableCategoryDTO;
using Common.Paginations;
using System.Net.Http.Headers;
using System.Text.Json;

namespace UnitTests.Tests
{
    [TestFixture]
    public class CableCategoryTest : BaseTest
    {
        private CableCategoryController controller;
        private Mock<ICableCategoryService> service;
        [SetUp]
        public void SetUp()
        {
            service = new Mock<ICableCategoryService>();
            controller = new CableCategoryController(service.Object);
        }

        [Test]
        public async Task ListPaged_WhenAdmin_ReturnsPagination()
        {
            // Arrange
            string? name = "SampleName";
            int page = 1;
            List<CableCategoryListDTO> list = new List<CableCategoryListDTO>();
            var expectedData = new Pagination<CableCategoryListDTO>()
            {
                RowCount = 0,
                CurrentPage = page,
                List = list
            };
            ResponseBase expectedResult = new ResponseBase(expectedData);
            string expectedContent = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.OK, expectedContent);
            HttpClient client = new HttpClient(handler.Object);
            string token = SimulateToken(RoleConst.Admin);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = "https://localhost:7107/CableCategory/List/Paged";
            HttpResponseMessage response = await Get(client, url, new KeyValuePair<string, object>("name", name),
                new KeyValuePair<string, object>("page", 1));
            string trueContent = await response.Content.ReadAsStringAsync();
            ResponseBase<Pagination<CableCategoryListDTO>?>? trueResult = JsonSerializer.Deserialize<ResponseBase<Pagination<CableCategoryListDTO>?>>(trueContent);
            Pagination<CableCategoryListDTO>? trueData = trueResult?.Data;
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.NotNull(trueResult);
            Assert.NotNull(trueResult.Data);
            Assert.NotNull(trueData);
            Assert.That(trueResult.Data.RowCount, Is.EqualTo(trueData.RowCount));
            Assert.IsTrue(trueResult.Data.List.SequenceEqual(list));
            Assert.That(trueResult.Data.CurrentPage, Is.EqualTo(trueData.CurrentPage));
        }

        private async Task ListPaged_ReturnsUnauthorized(string? token)
        {
            ResponseBase expectedResult = new ResponseBase("Unauthorized", (int)HttpStatusCode.Unauthorized);
            string content = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.Unauthorized, content);
            HttpClient client = new HttpClient(handler.Object);
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            string url = "https://localhost:7107/CableCategory/List/Paged";
            HttpResponseMessage response = await Get(client, url, new KeyValuePair<string, object>("page", 1));
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            string data = await response.Content.ReadAsStringAsync();
            ResponseBase<object?>? trueResult = JsonSerializer.Deserialize<ResponseBase<object?>>(data);
            Assert.NotNull(trueResult);
            Assert.IsFalse(trueResult.Success);
            Assert.That(trueResult.Message, Is.EqualTo(expectedResult.Message));
        }

        [Test]
        public async Task ListPaged_WhenNoLogin_ReturnsUnauthorized()
        {
            await ListPaged_ReturnsUnauthorized(null);
        }

        [Test]
        public async Task ListPaged_WhenInvalidToken_ReturnsUnauthorized()
        {
            await ListPaged_ReturnsUnauthorized("Invalid token");
        }

        [Test]
        public async Task ListPaged_NotAdmin_ReturnForbidden()
        {
            ResponseBase result = new ResponseBase("Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
            string content = JsonSerializer.Serialize(result);
            var handler = getHttpMessageHandler(HttpStatusCode.Forbidden, content);
            HttpClient client = new HttpClient(handler.Object);
            string token = SimulateToken(RoleConst.Leader);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = "https://localhost:7107/CableCategory/List/Paged";
            HttpResponseMessage response = await Get(client, url, new KeyValuePair<string, object>("page", 1));
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            string data = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo(data));
        }

        [Test]
        public async Task ListAll_WhenAdmin_ReturnsList()
        {
            List<CableCategoryListDTO> expectedData = new List<CableCategoryListDTO>
            {
                new CableCategoryListDTO { CableCategoryId = 1, CableCategoryName = "Category 1" },
                new CableCategoryListDTO { CableCategoryId = 2, CableCategoryName = "Category 2" }
            };
            ResponseBase expectedResult = new ResponseBase(expectedData);
            string expectedContent = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.OK, expectedContent);
            HttpClient client = new HttpClient(handler.Object);
            string token = SimulateToken(RoleConst.Admin);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = "https://localhost:7107/CableCategory/List/All";
            HttpResponseMessage response = await Get(client, url);
            string trueContent = await response.Content.ReadAsStringAsync();
            ResponseBase<List<CableCategoryListDTO>?>? trueResult = JsonSerializer.Deserialize<ResponseBase<List<CableCategoryListDTO>?>>(trueContent);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.NotNull(trueResult);
            Assert.IsTrue(trueResult.Success);
            Assert.NotNull(trueResult.Data);
            Assert.IsTrue(trueResult.Data.Count == expectedData.Count);
        }

        private async Task ListAll_ReturnsUnauthorized(string? token)
        {
            ResponseBase expectedResult = new ResponseBase("Unauthorized", (int)HttpStatusCode.Unauthorized);
            string content = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.Unauthorized, content);
            HttpClient client = new HttpClient(handler.Object);
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            string url = "https://localhost:7107/CableCategory/List/All";
            HttpResponseMessage response = await Get(client, url);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            string data = await response.Content.ReadAsStringAsync();
            ResponseBase<object?>? trueResult = JsonSerializer.Deserialize<ResponseBase<object?>>(data);
            Assert.NotNull(trueResult);
            Assert.IsFalse(trueResult.Success);
            Assert.That(trueResult.Message, Is.EqualTo(expectedResult.Message));
        }

        [Test]
        public async Task ListAll_WhenNoLogin_ReturnsUnauthorized()
        {
            await ListAll_ReturnsUnauthorized(null);
        }

        [Test]
        public async Task ListAll_WhenInvalidToken_ReturnsUnauthorized()
        {
            await ListAll_ReturnsUnauthorized("Invalid token");
        }

        [Test]
        public void ListAll_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var expectedExceptionMessage = "Simulated exception message";

            // Mock the ListAll method to throw an exception
            service.Setup(x => x.ListAll()).Throws(new Exception(expectedExceptionMessage));
            // Act and Assert
            var exception = Assert.Throws<Exception>(() => controller.List());

            // Verify that the exception has the expected message
            Assert.That(exception.Message, Is.EqualTo(expectedExceptionMessage));
        }

        private async Task Create_ReturnsUnauthorized(string? token)
        {
            CableCategoryCreateUpdateDTO DTO = new CableCategoryCreateUpdateDTO();
            ResponseBase expectedResult = new ResponseBase("Unauthorized", (int)HttpStatusCode.Unauthorized);
            string content = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.Unauthorized, content);
            HttpClient client = new HttpClient(handler.Object);
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            string url = "https://localhost:7107/CableCategory/Create";
            HttpResponseMessage response = await Post<CableCategoryCreateUpdateDTO?>(client, url, DTO);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            string data = await response.Content.ReadAsStringAsync();
            ResponseBase<bool?>? trueResult = JsonSerializer.Deserialize<ResponseBase<bool?>>(data);
            Assert.NotNull(trueResult);
            Assert.IsFalse(trueResult.Success);
            Assert.That(trueResult.Message, Is.EqualTo(expectedResult.Message));
        }

        [Test]
        public async Task Create_WhenNoLogin_ReturnsUnauthorized()
        {
            await Create_ReturnsUnauthorized(null);
        }

        [Test]
        public async Task Create_WhenInvalidToken_ReturnsUnauthorized()
        {
            await Create_ReturnsUnauthorized("Invalid token");
        }

        [Test]
        public async Task Create_NotAdmin_ReturnForbidden()
        {
            CableCategoryCreateUpdateDTO DTO = new CableCategoryCreateUpdateDTO();
            ResponseBase result = new ResponseBase("Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
            string content = JsonSerializer.Serialize(result);
            var handler = getHttpMessageHandler(HttpStatusCode.Forbidden, content);
            HttpClient client = new HttpClient(handler.Object);
            string token = SimulateToken(RoleConst.Staff);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = "https://localhost:7107/CableCategory/Create";
            HttpResponseMessage response = await Post<CableCategoryCreateUpdateDTO?>(client, url, DTO);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }


        [Test]
        public async Task Create_WhenAdmin_ReturnSuccess()
        {
            CableCategoryCreateUpdateDTO DTO = new CableCategoryCreateUpdateDTO()
            {
                CableCategoryName = "New name",
            };
            ResponseBase expectedResult = new ResponseBase(true, "Tạo thành công");
            string content = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.OK, content);
            HttpClient client = new HttpClient(handler.Object);
            string token = SimulateToken(RoleConst.Admin);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = "https://localhost:7107/CableCategory/Create";
            HttpResponseMessage response = await Post<CableCategoryCreateUpdateDTO?>(client, url, DTO);
            string data = await response.Content.ReadAsStringAsync();
            ResponseBase<bool?>? trueResult = JsonSerializer.Deserialize<ResponseBase<bool?>>(data);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.NotNull(trueResult);
            Assert.That(trueResult.Message, Is.EqualTo(expectedResult.Message));
            Assert.IsTrue(trueResult.Success);
        }

        [Test]
        public void Create_WhenCableCategoryNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var sampleData = new CableCategoryCreateUpdateDTO
            {
                CableCategoryName = ""
            };

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { }
            };
            controller.ControllerContext = context;

            service.Setup(x => x.Create(sampleData))
                .Returns(new ResponseBase("Tên cáp không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
            Assert.That(result.Message, Is.EqualTo("Tên cáp không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public void Create_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var sampleData = new CableCategoryCreateUpdateDTO
            {
                CableCategoryName = "hfdsfhdsfhus"
            };
            // Arrange
            var expectedExceptionMessage = "Simulated exception message";

            // Mock the ListAll method to throw an exception
            service.Setup(x => x.Create(sampleData)).Throws(new Exception(expectedExceptionMessage));
            // Act and Assert
            var exception = Assert.Throws<Exception>(() => controller.Create(sampleData));

            // Verify that the exception has the expected message
            Assert.That(exception.Message, Is.EqualTo(expectedExceptionMessage));
        }

    }
}
