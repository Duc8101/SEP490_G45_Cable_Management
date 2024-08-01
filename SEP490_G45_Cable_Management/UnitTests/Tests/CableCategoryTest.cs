using API.Services.CableCategories;
using Common.DTO.CableCategoryDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using System.Globalization;

namespace UnitTests.Tests
{
    [TestFixture]
    public class CableCategoryTest : BaseTest<CableCategory>
    {
        private Mock<ICableCategoryService> mockService;
        private CableCategoryController controller;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            mockService = new Mock<ICableCategoryService>();
            controller = new CableCategoryController(mockService.Object);
            controller.ControllerContext = controllerContext;
        }

        private void SetMockQueryCableCategory()
        {
            string format = "yyyy-MM-dd HH:mm:ss.fffffff";
            List<CableCategory> cableCategories = new List<CableCategory>()
            {
                 new CableCategory(){ CableCategoryId = 1, CableCategoryName = "Category 1" , CreatedAt = DateTime.ParseExact("2023-11-10 19:24:24.4349955", format, CultureInfo.InvariantCulture), UpdateAt = DateTime.ParseExact("2023-11-22 19:24:24.4349955", format, CultureInfo.InvariantCulture), IsDeleted = false},
                 new CableCategory(){ CableCategoryId = 2, CableCategoryName = "Category 2" , CreatedAt = DateTime.ParseExact("2023-11-11 19:24:24.4349955", format, CultureInfo.InvariantCulture), UpdateAt = DateTime.Now, IsDeleted = false},
            };
            mockDbSet.As<IQueryable<CableCategory>>().Setup(c => c.Expression).Returns(cableCategories.AsQueryable().Expression);
            mockDbSet.As<IQueryable<CableCategory>>().Setup(c => c.Provider).Returns(cableCategories.AsQueryable().Provider);
            mockDbSet.As<IQueryable<CableCategory>>().Setup(c => c.ElementType).Returns(cableCategories.AsQueryable().ElementType);
            mockDbContext.Setup(m => m.CableCategories).Returns(mockDbSet.Object);
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsSuccessCode()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Assert.That(trueResult.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsSuccessTrue()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Assert.IsTrue(trueResult.Success);
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsMessageEmpty()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Assert.That(trueResult.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsDataNotNull()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Assert.IsNotNull(trueResult.Data);
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsPaginationTrueRowCount()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            int expectedRowCount = 0;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.That(trueData?.RowCount, Is.EqualTo(expectedRowCount));
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsPaginationTrueNumberPage()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            int numberPage = 0;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.That(trueData?.NumberPage, Is.EqualTo(numberPage));
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsPaginationTrueSum()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.That(trueData?.NumberPage, Is.EqualTo(0));
        }

        [Test]
        public void ListPaged_InputNameNotExistInData_ReturnsExpectedList()
        {
            // --------------------------- arrange ----------------------------
            string? name = "SampleName";
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            List<CableCategory> list = daoCableCategory.getListCableCategory(name, page);
            List<CableCategoryListDTO> expectedList = mapper.Map<List<CableCategoryListDTO>>(list);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.That(trueData?.List, Is.EqualTo(expectedList));
        }


        [TestCase(null)]
        [TestCase("    ")]
        public void ListPaged_InputNameEmptyOrNull_ReturnsCodeSuccess(string? name)
        {
            // --------------------------- arrange ----------------------------
            int page = 1;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // --------------------------- act ----------------------------
            ResponseBase trueResult = controller.List(name, page);
            // --------------------------- assert ----------------------------
            Assert.That(trueResult.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }


        [Test]
        public void ListAll_ReturnsExpectedList()
        {
            // arrange
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListAll();
            List<CableCategory> expectedList = daoCableCategory.getListCableCategory();
            List<CableCategoryListDTO> expectedData = mapper.Map<List<CableCategoryListDTO>>(expectedList);
            mockService.Setup(s => s.ListAll()).Returns(expectedResult);
            // act
            ResponseBase trueResult = controller.List();
            // assert
            List<CableCategoryListDTO>? trueData = (List<CableCategoryListDTO>?)trueResult.Data;
            Assert.That(trueData, Is.EqualTo(expectedData));
        }


        /*
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
