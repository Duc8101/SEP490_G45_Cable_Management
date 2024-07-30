﻿using API.Services.CableCategories;
using Common.DTO.CableCategoryDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Tests
{
    [TestFixture]
    public class CableCategoryTest : BaseTest
    {
        private Mock<DbSet<CableCategory>> mockDbSet;
        private Mock<ICableCategoryService> mockService;
        private CableCategoryController controller;
        private ControllerContext controllerContext;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            mockDbSet = new Mock<DbSet<CableCategory>>();
            mockService = new Mock<ICableCategoryService>();
            controller = new CableCategoryController(mockService.Object);
            controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext = controllerContext;
        }

        private void SetMockQueryCableCategory()
        {
            List<CableCategory> cableCategories = new List<CableCategory>()
            {
                 new CableCategory(){ CableCategoryId = 1, CableCategoryName = "Category 1" , CreatedAt = DateTime.Now, UpdateAt = DateTime.Now,IsDeleted = false},
                 new CableCategory(){ CableCategoryId = 2, CableCategoryName = "Category 2" , CreatedAt = DateTime.Now, UpdateAt = DateTime.Now,IsDeleted = false}
            };
            mockDbSet.As<IQueryable<CableCategory>>().Setup(c => c.Expression).Returns(cableCategories.AsQueryable().Expression);
            mockDbSet.As<IQueryable<CableCategory>>().Setup(c => c.Provider).Returns(cableCategories.AsQueryable().Provider);
            mockDbSet.As<IQueryable<CableCategory>>().Setup(c => c.ElementType).Returns(cableCategories.AsQueryable().ElementType);
            mockDbContext.Setup(m => m.CableCategories).Returns(mockDbSet.Object);
        }

        /*
         * Input name not exist => return response success: 
         * success : true, code : 200 , message : empty
         * rowCount : 0, list : empty, currentPage : page
         
         */
        [Test]
        public void ListPaged_InputNameNotExist_ReturnsResponseSuccess()
        {
            // arrange
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // act
            ResponseBase trueResult = controller.List(name, page);
            // assert
            Assert.That(trueResult.Code, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.IsTrue(trueResult.Success);
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.IsNotNull(trueData);
            Assert.That(trueData.RowCount, Is.EqualTo(0));
            Assert.That(trueData.List.Count, Is.EqualTo(0));
            Assert.That(trueData.CurrentPage, Is.EqualTo(page));
        }

        /*
            * Input name null or empty => return response success: 
            * success : true, code : 200 , message : empty
            * rowCount : row of currentPage, list : all cable category, currentPage : page
        */

        [TestCase(null)]
        [TestCase("    ")]
        public void ListPaged_InputNameEmptyOrNull_ReturnsResponseSuccess(string? name)
        {
            // arrange
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // act
            ResponseBase trueResult = controller.List(name, page);
            // assert
            Assert.That(trueResult.Code, Is.EqualTo((int)HttpStatusCode.OK));
            Assert.IsTrue(trueResult.Success);
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.IsNotNull(trueData);
            Assert.That(trueData.RowCount, Is.EqualTo(0));
            Assert.That(trueData.CurrentPage, Is.EqualTo(page));
            Assert.That(trueData.List.Count, Is.EqualTo(0));
        }


        [Test]
        public void ListAll_WhenAdmin_ReturnsList()
        {
            List<CableCategoryListDTO> expectedData = new List<CableCategoryListDTO>
            {
                 new CableCategoryListDTO { CableCategoryId = 1, CableCategoryName = "Category 1" },
                 new CableCategoryListDTO { CableCategoryId = 2, CableCategoryName = "Category 2" }
            };
            ResponseBase expectedResult = new ResponseBase(expectedData);
            mockService.Setup(s => s.ListAll()).Returns(expectedResult);
            ResponseBase trueResult = controller.List();
            Assert.That(trueResult, Is.EqualTo(expectedResult));
        }


        /*        [Test]
                public async Task ListPaged_WhenAdmin_ReturnsPagination()
                {
                    // Arrange
                    string? name = "SampleName";
                    Pagination<CableCategoryListDTO> expectedData = new Pagination<CableCategoryListDTO>()
                    {
                        RowCount = 0,
                        CurrentPage = 1,
                        List = new List<CableCategoryListDTO>()
                    };
                    ResponseBase expectedResult = new ResponseBase(expectedData);
                    string expectedContent = JsonSerializer.Serialize(expectedResult);
                    var handler = getHttpMessageHandler(HttpStatusCode.OK, expectedContent);
                    HttpClient client = new HttpClient(handler.Object);
                    string token = SimulateToken(Roles.Admin);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
                    string url = "https://localhost:7107/CableCategory/List/Paged";
                    HttpResponseMessage response = await Get(client, url, new KeyValuePair<string, object>("name", name),
                        new KeyValuePair<string, object>("page", 1));
                    string trueContent = await response.Content.ReadAsStringAsync();
                    ResponseBase<Pagination<CableCategoryListDTO>?>? trueResult = JsonSerializer.Deserialize<ResponseBase<Pagination<CableCategoryListDTO>?>>(trueContent);
                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.NotNull(trueResult);
                   // Assert.That(trueResult.Code, Is.EqualTo((int)HttpStatusCode.OK));
                   // Assert.IsTrue(trueResult.Success);
                }

                private async Task ListPaged_ReturnsUnauthorized(string? token)
                {
                    ResponseBase expectedResult = new ResponseBase("Unauthorized", (int)HttpStatusCode.Unauthorized);
                    string content = JsonSerializer.Serialize(expectedResult);
                    var handler = getHttpMessageHandler(HttpStatusCode.Unauthorized, content);
                    HttpClient client = new HttpClient(handler.Object);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string url = "https://localhost:7107/CableCategory/List/Paged";
                    HttpResponseMessage response = await Get(client, url, new KeyValuePair<string, object>("page", 1));
                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                    string data = await response.Content.ReadAsStringAsync();
                    ResponseBase? trueResult = JsonSerializer.Deserialize<ResponseBase>(data);
                    Assert.NotNull(trueResult);
                    Assert.IsFalse(trueResult.Success);
                }
        */
        /*        [Test]
                public async Task ListPaged_WhenNoLogin_ReturnsUnauthorized()
                {
                    await ListPaged_ReturnsUnauthorized("");
                }
        */
        /*        [Test]
                public async Task ListPaged_WhenInvalidToken_ReturnsUnauthorized()
                {
                    await ListPaged_ReturnsUnauthorized("Invalid token");
                }
        */
        /*        [Test]
                public async Task ListPaged_NotAdmin_ReturnForbidden()
                {
                    ResponseBase result = new ResponseBase("Bạn không có quyền truy cập", (int)HttpStatusCode.Forbidden);
                    string content = JsonSerializer.Serialize(result);
                    var handler = getHttpMessageHandler(HttpStatusCode.Forbidden, content);
                    HttpClient client = new HttpClient(handler.Object);
                    string token = SimulateToken(Roles.Leader);
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
                    string token = SimulateToken(Roles.Admin);
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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    string url = "https://localhost:7107/CableCategory/List/All";
                    HttpResponseMessage response = await Get(client, url);
                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                    string data = await response.Content.ReadAsStringAsync();
                    ResponseBase<object?>? trueResult = JsonSerializer.Deserialize<ResponseBase<object?>>(data);
                    Assert.NotNull(trueResult);
                    Assert.IsFalse(trueResult.Success);
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
                    var handler = getHttpMessageHandler(HttpStatusCode.OK, content);
                    HttpClient client = new HttpClient(handler.Object);
                    string token = SimulateToken(Roles.Staff);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
                    string url = "https://localhost:7107/CableCategory/Create";
                    HttpResponseMessage response = await Post<CableCategoryCreateUpdateDTO?>(client, url, DTO);
                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
                    string token = SimulateToken(Roles.Admin);
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
                }*/

    }
}
