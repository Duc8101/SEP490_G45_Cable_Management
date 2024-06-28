using API.Services.Cables;
using Common.Base;
using Common.Const;
using Common.DTO.CableDTO;
using Common.Pagination;

namespace UnitTests.Tests
{
    [TestFixture]
    public class CableTest
    {
        private CableController controller;
        private Mock<ICableService> cableService;

        [SetUp]
        public void SetUp()
        {
            cableService = new Mock<ICableService>();
            controller = new CableController(cableService.Object);
        }

        [Test]
        public async Task List_Returns_Forbidden_When_User_Is_Not_Admin_Or_WarehouseKeeper_Or_Leader()
        {
            // Arrange

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);
            // Act
            var result = await controller.List("filter", 1, true, 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
            Assert.IsNull(result.Data);  // Assuming Data is nullable in your implementation
        }

        [Test]
        public async Task List_Returns_Data_When_User_Is_Admin()
        {
            // Arrange
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            var expectedPagedResult = new Pagination<CableListDTO>(1, 10, 5, new List<CableListDTO>());  // Provide expected paged result here
            cableService.Setup(x => x.ListPaged(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<int>()))
                .ReturnsAsync(new ResponseBase<Pagination<CableListDTO>?>(expectedPagedResult, string.Empty));

            // Act
            var result = await controller.List("filter", 1, true, 1);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }
        [Test]
        public async Task List_Returns_Data_When_User_Is_WarehouseKeeper()
        {
            // Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, RoleConst.ROLE_WAREHOUSE_KEEPER) }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;
            var expectedPagedResult = new Pagination<CableListDTO>(1, 10, 5, new List<CableListDTO>());  // Provide expected paged result here
            cableService.Setup(x => x.ListPaged(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<int>()))
                .ReturnsAsync(new ResponseBase<Pagination<CableListDTO>?>(expectedPagedResult, string.Empty));

            // Act
            var result = await controller.List("filter", 1, true, 1);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }
        [Test]
        public async Task List_Returns_Data_When_User_Is_Leader()
        {
            // Arrange
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, RoleConst.ROLE_LEADER) }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;
            var expectedPagedResult = new Pagination<CableListDTO>(1, 10, 5, new List<CableListDTO>());  // Provide expected paged result here
            cableService.Setup(x => x.ListPaged(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<int>()))
                .ReturnsAsync(new ResponseBase<Pagination<CableListDTO>?>(expectedPagedResult, string.Empty));

            // Act
            var result = await controller.List("filter", 1, true, 1);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task ListPaged_WhenCalled_ReturnsPagedResultDTO()
        {
            // Arrange
            string filter = "";
            int warehouseId = 1;
            bool isExportedToUse = true;
            int page = 1;

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, RoleConst.ROLE_WAREHOUSE_KEEPER) }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            var expectedResult = new Pagination<CableListDTO>(1, 10, 5, new List<CableListDTO>());

            cableService.Setup(x => x.ListPaged(filter, warehouseId, isExportedToUse, page))
                .ReturnsAsync(new ResponseBase<Pagination<CableListDTO>>(expectedResult, string.Empty));

            // Act
            var result = await controller.List(filter, warehouseId, isExportedToUse, page);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Data, Is.EqualTo(expectedResult));
            Assert.IsTrue(result.Success);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task ListAll_WhenCalled_ReturnsListOfCableListDTO()
        {
            // Arrange
            int warehouseId = 1;

            var expectedList = new List<CableListDTO> {
    new CableListDTO { CableId = Guid.NewGuid(), WarehouseId = 1, SupplierId = 1, StartPoint = 1, EndPoint = 2, Length = 100,
                       YearOfManufacture = 2023, Code = "SampleCode1", Status = "SampleStatus1", IsExportedToUse = true,
                       CableCategoryId = 1, IsInRequest = false },
    new CableListDTO { CableId = Guid.NewGuid(), WarehouseId = 1, SupplierId = 2, StartPoint = 2, EndPoint = 3, Length = 200,
                       YearOfManufacture = 2022, Code = "SampleCode2", Status = "SampleStatus2", IsExportedToUse = false,
                       CableCategoryId = 2, IsInRequest = true }
   };

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, RoleConst.ROLE_WAREHOUSE_KEEPER) }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            cableService.Setup(x => x.ListAll(warehouseId)).ReturnsAsync(new ResponseBase<List<CableListDTO>>(expectedList, string.Empty));

            // Act
            var result = await controller.List(1);

            Assert.IsNotNull(result);
            Assert.That(result.Data.Count, Is.EqualTo(expectedList.Count));

            foreach (var cableListDTO in result.Data)
            {
                var expectedCableListDTO =
                    expectedList.FirstOrDefault(x => x.CableId == cableListDTO.CableId);  // Assuming you have an ID property to match

                Assert.That(cableListDTO.WarehouseId, Is.EqualTo(expectedCableListDTO.WarehouseId));
                Assert.That(cableListDTO.SupplierId, Is.EqualTo(expectedCableListDTO.SupplierId));
            }

            Assert.IsTrue(result.Success);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WhenAdminOrWarehouseKeeperWithoutID_ReturnsNotFoundResponse()
        {
            // Arrange
            var sampleData = new CableCreateUpdateDTO { };

            // Simulate user with admin or warehouse keeper role
            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.ROLE_ADMIN);

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateSevice_WhenAdminOrWarehouseKeeper_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleData = new CableCreateUpdateDTO
            {
                WarehouseId = 1,
                SupplierId = 2,
                StartPoint = 2,
                EndPoint = 3,
                YearOfManufacture = 2022,
                Code = "SampleCode",
                Status = "SampleStatus",
                CableCategoryId = 3
            };

            // Simulate user with admin role and specific ID
            Guid userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin"), new Claim("UserID", userId.ToString()) }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            // Mock the necessary methods
            cableService.Setup(x => x.Create(sampleData, userId))
                .ReturnsAsync(new ResponseBase<bool>(true, "Thêm thành công"));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Thêm thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenNotAdminOrWarehouseKeeper_ReturnsForbiddenResponse()
        {
            // Arrange
            var cableId = Guid.NewGuid();
            var sampleData = new CableCreateUpdateDTO
            {
                WarehouseId = 1
            };

            // Simulate user without admin role or specific ID
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, RoleConst.ROLE_STAFF)

            }));

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            controller.ControllerContext = context;

            // Act
            var result = await controller.Update(cableId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Update_WhenCableNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var sampleCableID = Guid.NewGuid();
            var sampleData = new CableCreateUpdateDTO
            {
                WarehouseId = 1,
                SupplierId = 1,
                StartPoint = 0,
                EndPoint = 10,
                YearOfManufacture = 2023,
                Code = "SampleCode",
                Status = "SampleStatus",
                CableCategoryId = 1
            };

            // Simulate user with admin role and specific ID
            Guid userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, RoleConst.ROLE_ADMIN), new Claim("UserID", userId.ToString()) }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            var expectedResponse = new ResponseBase<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound);
            cableService.Setup(x => x.Update(sampleCableID, sampleData)).ReturnsAsync(expectedResponse);

            // Act
            var result = await controller.Update(sampleCableID, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Data, Is.EqualTo(expectedResponse.Data));
            Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            Assert.That(result.Code, Is.EqualTo(expectedResponse.Code));
        }

        [Test]
        public async Task Update_WhenValidInput_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleCableID = Guid.NewGuid();
            var sampleData = new CableCreateUpdateDTO
            {
                WarehouseId = 1,
                SupplierId = 1,
                StartPoint = 0,
                EndPoint = 10,
                YearOfManufacture = 2023,
                Code = "SampleCode",
                Status = "SampleStatus",
                CableCategoryId = 1
            };

            // Simulate user with admin role and specific ID
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            cableService.Setup(x => x.Update(sampleCableID, sampleData)).ReturnsAsync(new ResponseBase<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(sampleCableID, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WhenNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            Guid cableId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_LEADER);

            // Act
            var result = await controller.Delete(cableId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenCableExists_ReturnsSuccessResponse()
        {
            // Arrange
            var cableId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            cableService.Setup(x => x.Delete(cableId)).ReturnsAsync(new ResponseBase<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(cableId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }


        [Test]
        public async Task Delete_WhenCableDoesNotExist_Admin_ReturnsNotFoundResponse()
        {
            // Arrange
            var cableId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            cableService.Setup(x => x.Delete(cableId)).ReturnsAsync(new ResponseBase<bool>(false, "Không tìm thấy cáp", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(cableId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy cáp"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
    }
}
