using Common.DTO.CableCategoryDTO;
using Common.Paginations;
using System.Net.Http.Headers;
using System.Text.Json;

namespace UnitTests.Tests
{
    [TestFixture]
    public class CableCategoryTest : BaseTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public async Task ListPaged_WhenAdmin_ReturnsPagination()
        {
            // Arrange
            string? name = "SampleName";
            int page = 1;
            List<CableCategoryListDTO> data = new List<CableCategoryListDTO>();
            var expectedData = new Pagination<CableCategoryListDTO>()
            {
                RowCount = 0,
                CurrentPage = page,
                Data = data
            };
            ResponseBase expectedResult = new ResponseBase(expectedData);
            string expectedContent = JsonSerializer.Serialize(expectedResult);
            var handler = getHttpMessageHandler(HttpStatusCode.OK, expectedContent);
            HttpClient client = new HttpClient(handler.Object);
            string token = SimulateToken(Role.Admin);
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
            Assert.IsTrue(trueResult.Data.Data.SequenceEqual(data));
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
            string token = SimulateToken(Role.Leader);
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
            string token = SimulateToken(Role.Admin);
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


        /*        [Test]
                public void ListAll_WhenUserHasPermission_ReturnsListOfCableCategory()
                {
                    // Arrange
                    var expectedData = new List<CableCategoryListDTO>
                    {
                        new CableCategoryListDTO { CableCategoryId = 1, CableCategoryName = "Category 1" },
                        new CableCategoryListDTO { CableCategoryId = 2, CableCategoryName = "Category 2" }
                    };

                    // Simulate user with admin role
                    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, Role.Admin.ToString()) }));

                    var context = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = user
                        }
                    };
                    controller.ControllerContext = context;

                    service.Setup(x => x.ListAll())
                        .Returns(new ResponseBase(expectedData));

                    // Act
                    var result = controller.List();

                    // Assert
                    Assert.IsNotNull(result);
                    Assert.IsNotNull(result.Data);
                    Assert.That(((List<CableCategoryListDTO>)result.Data).Count, Is.EqualTo(expectedData.Count));
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

                [Test]
                public void Create_WhenCableCategoryNameIsEmpty_ReturnsConflictResponse()
                {
                    // Arrange
                    var sampleData = new CableCategoryCreateUpdateDTO
                    {
                        CableCategoryName = ""
                    };
                    // Simulate user with admin role
                    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Role, Role.Admin.ToString())
                    }));

                    var context = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = user }
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
                public void Create_WhenCableCategoryCreated_ReturnsSuccessResponse()
                {
                    // Arrange
                    var sampleData = new CableCategoryCreateUpdateDTO
                    {
                        CableCategoryName = "Sample Cable Category Name"
                    };
                    // Simulate user with admin role
                    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Role, Role.Admin.ToString())
                    }));

                    var context = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = user }
                    };
                    controller.ControllerContext = context;
                    service.Setup(x => x.Create(sampleData))
                        .Returns(new ResponseBase(true, "Tạo thành công"));
                    // Act
                    var result = controller.Create(sampleData);
                    // Assert
                    Assert.IsNotNull(result);
                    Assert.IsTrue(result.Success);
                    Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
                    *//*Assert.AreEqual((int)HttpStatusCode.OK, result.Code);*//*
                }

                [Test]
                public void Create_WhenCallNotByAdmin_ReturnsForbiddenResponse()
                {
                    // Arrange
                    var sampleData = new CableCategoryCreateUpdateDTO
                    {
                        CableCategoryName = "Sample Cable Category Name"
                    };
                    // Simulate user with staff role
                    var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Role, Role.Staff.ToString())
                    }));

                    var context = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext { User = user }
                    };
                    controller.ControllerContext = context;
                    // Act
                    var result = controller.Create(sampleData);
                    // Assert
                    Assert.IsNotNull(result);
                    Assert.IsFalse(result.Success);
                    Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
                    Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
                }
        */
    }
}
