using Common.Base;
using Common.Const;
using Common.DTO.CableCategoryDTO;
using Common.Pagination;

namespace UnitTests.Tests
{
    [TestFixture]
    public class CableCategoryTest
    {
        private CableCategoryController controller;
        private Mock<ICableCategoryService> cableCategoryService;
        private Mock<HttpContext> contextMock;
        private CableCategoryCreateUpdateDTO modelCU = new CableCategoryCreateUpdateDTO();

        [SetUp]
        public void SetUp()
        {
            cableCategoryService = new Mock<ICableCategoryService>();
            contextMock = new Mock<HttpContext>();
            var user = new ClaimsPrincipal();
            var listClaim = new List<Claim> { new Claim(ClaimTypes.Role, RoleConst.ROLE_ADMIN), new Claim("userid", "1") };
            user.AddIdentity(new ClaimsIdentity(listClaim));
            contextMock.Setup(x => x.User).Returns(user);
            controller = new CableCategoryController(cableCategoryService.Object);
        }

        [Test]
        public async Task ListPaged_WhenAdmin_ReturnsPagedResultDTO()
        {
            // Arrange
            string? name = "SampleName";
            int page = 1;
            var expectedResult = new Pagination<CableCategoryListDTO>(1, 1, 12, new List<CableCategoryListDTO>());

            // Simulate user with admin role
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            cableCategoryService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseBase<Pagination<CableCategoryListDTO>>(expectedResult, string.Empty));

            // Act
            var result = await controller.List(name, page);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Data, Is.EqualTo(expectedResult));
            Assert.IsTrue(result.Success);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task List_WhenUserHasPermission_ReturnsListOfCableCategories()
        {
            // Arrange
            var expectedList =
                new List<CableCategoryListDTO> { new CableCategoryListDTO { CableCategoryId = 1, CableCategoryName = "Category 1" },
                                        new CableCategoryListDTO { CableCategoryId = 2, CableCategoryName = "Category 2" } };

            // Simulate user with admin role
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            cableCategoryService.Setup(x => x.ListAll())
                .ReturnsAsync(new ResponseBase<List<CableCategoryListDTO>?>(expectedList, string.Empty));

            // Act
            var result = await controller.List();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(expectedList.Count, result.Data.Count);
        }

        [Test]
        public async Task Create_WhenCableCategoryNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var sampleData = new CableCategoryCreateUpdateDTO { CableCategoryName = "" };

            // Simulate user with admin role
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            cableCategoryService.Setup(x => x.Create(sampleData))
                .ReturnsAsync(new ResponseBase<bool>(false, "Tên cáp không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tên cáp không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenCableCategoryCreated_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleData = new CableCategoryCreateUpdateDTO { CableCategoryName = "Sample Cable Category Name" };
            // Simulate user with admin role
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            cableCategoryService.Setup(x => x.Create(sampleData)).ReturnsAsync(new ResponseBase<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công")); /*
    Assert.AreEqual((int)HttpStatusCode.OK, result.Code);*/
        }

        [Test]
        public async Task AddCableCategory_WhenCallNotByAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var sampleData = new CableCategoryCreateUpdateDTO { CableCategoryName = "Sample Cable Category Name" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);
            // Act
            var result = await controller.Create(sampleData);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateCableCategory_WhenCallNotByAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var sampleData = new CableCategoryCreateUpdateDTO { CableCategoryName = "Updated Sample Cable Category Name" };
            int cableCategoryId = 1;  // Sample cable category ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);
            // Act
            var result = await controller.Update(cableCategoryId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task UpdateCableCategory_WhenCallByAdmin_RetrieveService()
        {
            controller.ControllerContext.HttpContext = contextMock.Object;
            cableCategoryService.Setup(x => x.Update(2, modelCU)).ReturnsAsync(new ResponseBase<bool>(true, "Chỉnh sửa thành công"));
            var result = await controller.Update(2, modelCU);
            cableCategoryService.Verify(s => s.Update(2, modelCU));
            Assert.NotNull(result);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.True);
        }
        [Test]
        public void ListAll_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var expectedExceptionMessage = "Simulated exception message";

            // Mock the ListAll method to throw an exception
            cableCategoryService.Setup(x => x.ListAll())
                               .ThrowsAsync(new Exception(expectedExceptionMessage));

            // Act and Assert
            var exception = Assert.ThrowsAsync<Exception>(async () =>

                await controller.List()
            );

            // Verify that the exception has the expected message
            Assert.That(exception.Message, Is.EqualTo(expectedExceptionMessage));
        }

        [Test]
        public void Create_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var cableCategoryCreateUpdateDTO = new CableCategoryCreateUpdateDTO
            {
                CableCategoryName = "TestCableCategory"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            var expectedExceptionMessage = "Simulated exception message";

            cableCategoryService.Setup(x => x.Create(cableCategoryCreateUpdateDTO))
                               .ThrowsAsync(new Exception(expectedExceptionMessage));


            // Act and Assert
            var exception = Assert.ThrowsAsync<Exception>(async () =>

                await controller.Create(cableCategoryCreateUpdateDTO)
            );

            // Verify that the exception has the expected message
            Assert.That(exception.Message, Is.EqualTo(expectedExceptionMessage));

        }

    }
}
