using DataAccess.DTO.RouteDTO;

namespace UnitTests.Tests
{
    [TestFixture]
    public class RouteTest
    {
        private RouteController controller;
        private Mock<IRouteService> routeService;

        [SetUp]
        public void SetUp()
        {
            routeService = new Mock<IRouteService>();
            controller = new RouteController(routeService.Object);
        }

        [Test]
        public async Task List_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var name = "TestName";  // Replace with the desired value
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List(name);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ListPaged_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var page = 1;  // Replace with the desired value
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List(page);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var routeCreateDTO = new RouteCreateDTO { };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Create(routeCreateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var routeID = Guid.NewGuid();
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.Delete(routeID);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ListAll_WhenRoutesAreRetrievedSuccessfully_ReturnsListOfRoutes()
        {
            // Arrange
            var name = "SampleName";  // Replace with a valid name or leave it as null for testing

            // Define a sample list of routes that the method should return
            var expectedRoutes = new List<RouteListDTO> {
    new RouteListDTO { RouteId = Guid.NewGuid(), RouteName = "Route1" },
    new RouteListDTO { RouteId = Guid.NewGuid(), RouteName = "Route2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.ListAll(name))
                .ReturnsAsync(new ResponseDTO<List<RouteListDTO>?>(expectedRoutes, string.Empty));

            // Act
            var result = await controller.List(name);

            routeService.Verify(x => x.ListAll(name));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedRoutes.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListAll_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            var name = "SampleName";

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.ListAll(name)).ThrowsAsync(new Exception("Sample exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.List(name));
        }

        [Test]
        public async Task ListPaged_WhenRoutesAreRetrievedSuccessfully_ReturnsPagedListOfRoutes()
        {
            // Arrange
            var page = 1;

            // Define a sample list of routes that the method should return
            var expectedRoutes = new List<RouteListDTO> {
    new RouteListDTO { RouteId = Guid.NewGuid(), RouteName = "Route1" },
    new RouteListDTO { RouteId = Guid.NewGuid(), RouteName = "Route2" }
   };
            var expectedRowCount = 10;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.ListPaged(page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<RouteListDTO>?>(
                    new PagedResultDTO<RouteListDTO>(page, expectedRowCount,
                                                     PageSizeConst.MAX_ROUTE_LIST_IN_PAGE, expectedRoutes),
                    string.Empty));

            // Act
            var result = await controller.List(page);

            routeService.Verify(x => x.ListPaged(page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.PageCount, Is.EqualTo(page));
            Assert.That(result.Data.RowCount, Is.EqualTo(expectedRowCount));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListPaged_WhenExceptionIsThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var page = 1;  // Replace with a valid page number for testing

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.ListPaged(page)).ThrowsAsync(new Exception("Sample exception"));

            // Act and Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await controller.List(page));
            Assert.That(ex.Message, Does.Contain("Sample exception"));
        }

        [Test]
        public async Task Create_WhenRouteNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var routeCreateDTO = new RouteCreateDTO { RouteName = String.Empty };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Create(routeCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên tuyến không được để trống",
                                               (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(routeCreateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên tuyến không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenRouteNameExists_ReturnsConflictResponse()
        {
            // Arrange
            var routeCreateDTO = new RouteCreateDTO { RouteName = "SampleRoute" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Create(routeCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên tuyến đã tồn tại", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(routeCreateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên tuyến đã tồn tại"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenRouteCreationIsSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var routeCreateDTO = new RouteCreateDTO { RouteName = "NewRoute" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Create(routeCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(routeCreateDTO);

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
        }

        [Test]
        public void Create_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            var routeCreateDTO = new RouteCreateDTO { RouteName = "NewRoute" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Create(routeCreateDTO))
                .Throws(new Exception("An error occurred while creating the route."));

            // Act and Assert
            Assert.Throws<Exception>(async () => await controller.Create(routeCreateDTO));
        }

        [Test]
        public async Task Delete_WhenRouteIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            var routeID = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Delete(routeID))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy tên tuyến", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(routeID);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy tên tuyến"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenRouteIsDeleted_ReturnsSuccessResponse()
        {
            // Arrange
            var routeID = Guid.NewGuid();  // Replace with the valid route ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Delete(routeID))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(routeID);

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
        }

        [Test]
        public void Delete_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var routeID = Guid.NewGuid();  // Replace with the valid route ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            routeService.Setup(x => x.Delete(routeID)).ThrowsAsync(new Exception("Some exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Delete(routeID));
        }
    }
}
