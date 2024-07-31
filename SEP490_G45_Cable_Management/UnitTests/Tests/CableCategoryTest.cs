using API.Services.CableCategories;
using Common.DTO.CableCategoryDTO;
using Common.Entity;
using Common.Paginations;
using DataAccess.DAO;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
            /*            Assert.That(trueData.RowCount, Is.EqualTo(0));
                        Assert.That(trueData.List.Count, Is.EqualTo(0));
                        Assert.That(trueData.CurrentPage, Is.EqualTo(page));*/
        }

        [Test]
        public void ListPaged_InputNameNotExist_ReturnsSuccessTrue()
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
            Assert.IsTrue(trueResult.Success);
        }

        [Test]
        public void ListPaged_InputNameNotExist_ReturnsMessageEmpty()
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
            Assert.That(trueResult.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListPaged_InputNameNotExist_ReturnsDataNotNull()
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
            Assert.IsNotNull(trueResult.Data);
        }

        [Test]
        public void ListPaged_InputNameNotExist_ReturnsPaginationTrueRowCount()
        {
            // arrange
            string? name = "SampleName";
            int page = 1;
            int expectedRowCount = 0;
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListPaged(name, page);
            mockService.Setup(s => s.ListPaged(name, page)).Returns(expectedResult);
            // act
            ResponseBase trueResult = controller.List(name, page);
            // assert
            Pagination<CableCategoryListDTO>? trueData = (Pagination<CableCategoryListDTO>?)trueResult.Data;
            Assert.That(trueData?.RowCount, Is.EqualTo(expectedRowCount));
        }


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
            Assert.IsNotNull(trueResult.Data);
        }


        [Test]
        public void ListAll__ReturnsExpectedList()
        {
            // arrange
            SetMockQueryCableCategory();
            DAOCableCategory daoCableCategory = new DAOCableCategory(mockDbContext.Object);
            CableCategoryService service = new CableCategoryService(mapper, daoCableCategory);
            ResponseBase expectedResult = service.ListAll();
            List<CableCategory> expectedList = mockDbContext.Object.CableCategories.OrderByDescending(c => c.UpdateAt).ToList();
            List<CableCategoryListDTO> expectedData = mapper.Map<List<CableCategoryListDTO>>(expectedList);
            mockService.Setup(s => s.ListAll()).Returns(expectedResult);
            // act
            ResponseBase trueResult = controller.List();
            // assert
            List<CableCategoryListDTO>? trueData = (List<CableCategoryListDTO>?) trueResult.Data;
            Assert.IsTrue(trueData?.SequenceEqual(expectedData));
        }


        /*      

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
