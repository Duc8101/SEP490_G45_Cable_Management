using DataAccess.DTO.IssueDTO;

namespace UnitTests.Tests
{
    [TestFixture]
    public class IssueTest
    {
        private IssueController controller;
        private Mock<IIssueService> issueService;

        [SetUp]
        public void SetUp()
        {
            issueService = new Mock<IIssueService>();
            controller = new IssueController(issueService.Object);
        }

        [Test]
        public async Task List_WhenCalledWithValidParameters_ReturnsOKPagedResult()
        {
            // Arrange
            string filter = "sample filter";
            int page = 1;
            var expectedList = new List<IssueListDTO>
            {
            };

            // Simulate user with admin, leader, or staff role
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
     new Claim(ClaimTypes.Role, RoleConst.ROLE_ADMIN),
    }));

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            controller.ControllerContext = context;

            issueService.Setup(x => x.ListPagedAll(filter, page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<IssueListDTO>>(
                    new PagedResultDTO<IssueListDTO>(page, expectedList.Count, expectedList.Count, expectedList), string.Empty));

            // Act
            var result = await controller.List(filter, page);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Data.Sum, Is.EqualTo(expectedList.Count));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }


        [Test]
        public async Task ListPagedDoing_WhenCalled_ReturnsIssueListDTO()
        {
            // Arrange
            int page = 1;

            var mockList = new List<IssueListDTO> { };

            var mockRowCount = 10;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.ListPagedDoing(page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<IssueListDTO>>(
                    new PagedResultDTO<IssueListDTO>(page, mockRowCount, PageSizeConst.MAX_ISSUE_LIST_IN_PAGE, mockList),
                    string.Empty));

            // Act
            var result = await controller.List(page);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.RowCount, Is.EqualTo(mockRowCount));
        }

        [Test]
        public async Task List_WhenAdminOrLeaderOrStaff_ReturnsListOfIssues()
        {
            // Arrange
            var expectedList = new List<IssueListDTO>
            {
            };

            issueService.Setup(x => x.ListDoing()).ReturnsAsync(new ResponseDTO<List<IssueListDTO>>(expectedList, string.Empty));

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.List();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Data, Is.EqualTo(expectedList));
            Assert.IsEmpty(result.Message);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WhenNotAdminOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var sampleData = new IssueCreateDTO
            {
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenIssueNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var sampleData = new IssueCreateDTO
            {
                IssueName = "",
                IssueCode = "SampleIssueCode",
                Description = "SampleDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "SampleCableRoutingName",
                Group = "SampleGroup"
            };
            Guid CreatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.Create(sampleData, CreatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tên sự vụ không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenIssueCodeIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var sampleData = new IssueCreateDTO
            {
                IssueName = "SampleIssueName",
                IssueCode = "",
                Description = "SampleDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "SampleCableRoutingName",
                Group = "SampleGroup"
            };
            Guid CreatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.Create(sampleData, CreatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Mã sự vụ không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenValidInput_Admin_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleData = new IssueCreateDTO
            {
                IssueName = "SampleIssueName",
                IssueCode = "SampleIssueCode",
                Description = "SampleDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "SampleCableRoutingName",
                Group = "SampleGroup"
            };
            var creatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            // Setup the necessary mocks
            issueService.Setup(x => x.Create(sampleData, creatorId)).ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WhenValidInput_Staff_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleData = new IssueCreateDTO
            {
                IssueName = "SampleIssueName",
                IssueCode = "SampleIssueCode",
                Description = "SampleDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "SampleCableRoutingName",
                Group = "SampleGroup"
            };
            var creatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Setup the necessary mocks
            issueService.Setup(x => x.Create(sampleData, creatorId)).ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenUserNotAuthorized_ReturnsForbiddenResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            var sampleData = new IssueUpdateDTO
            {
                IssueName = "UpdatedIssueName",
                IssueCode = "UpdatedIssueCode",
                Description = "UpdatedDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "UpdatedCableRoutingName",
                Group = "UpdatedGroup"
            };
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.Update(issueId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Update_WhenValidInput_Admin_ReturnsSuccessResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            var sampleData = new IssueUpdateDTO
            {
                IssueName = "UpdatedIssueName",
                IssueCode = "UpdatedIssueCode",
                Description = "UpdatedDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "UpdatedCableRoutingName",
                Group = "UpdatedGroup",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.Update(issueId, sampleData)).ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));
            // Act
            var result = await controller.Update(issueId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }
        [Test]
        public async Task Update_WhenValidInput_Staff_ReturnsSuccessResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            var sampleData = new IssueUpdateDTO
            {
                IssueName = "UpdatedIssueName",
                IssueCode = "UpdatedIssueCode",
                Description = "UpdatedDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "UpdatedCableRoutingName",
                Group = "UpdatedGroup",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            issueService.Setup(x => x.Update(issueId, sampleData)).ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));
            // Act
            var result = await controller.Update(issueId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenIssueNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            var sampleData = new IssueUpdateDTO
            {
                IssueName = "UpdatedIssueName",
                IssueCode = "UpdatedIssueCode",
                Description = "UpdatedDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "UpdatedCableRoutingName",
                Group = "UpdatedGroup",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.Update(issueId, sampleData))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(issueId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenIssueNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            var sampleData = new IssueUpdateDTO
            {
                IssueName = "",
                IssueCode = "UpdatedIssueCode",
                Description = "UpdatedDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "UpdatedCableRoutingName",
                Group = "UpdatedGroup",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.Update(issueId, sampleData))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên sự vụ không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(issueId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tên sự vụ không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenIssueCodeIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            var sampleData = new IssueUpdateDTO
            {
                IssueName = "UpdatedIssueName",
                IssueCode = "",
                Description = "UpdatedDescription",
                CreatedDate = DateTime.Now,
                CableRoutingName = "UpdatedCableRoutingName",
                Group = "UpdatedGroup",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);
            issueService.Setup(x => x.Update(issueId, sampleData))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Mã sự vụ không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(issueId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Mã sự vụ không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Delete_WhenNotAdminLeaderOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.Delete(issueId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenValidIssueId_Admin_ReturnsSuccessResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);
            issueService.Setup(x => x.Delete(issueId)).ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(issueId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }
        [Test]
        public async Task Delete_WhenValidIssueId_Staff_ReturnsSuccessResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);
            issueService.Setup(x => x.Delete(issueId)).ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(issueId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WhenInvalidIssueId_ReturnsNotFoundResponse()
        {
            // Arrange
            var issueId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            issueService.Setup(x => x.Delete(issueId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(issueId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Detail_WhenIssueNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            Guid issueId = Guid.NewGuid();


            var expertResult = new ResponseDTO<List<IssueDetailDTO>>(null, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound);
            issueService.Setup(x => x.Detail(issueId))
                        .ReturnsAsync(expertResult); // Corrected to use ReturnsAsync


            // Act
            var result = await controller.Detail(issueId);

            issueService.Verify(s => s.Detail(issueId));
            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
        [Test]
        public async Task Detail_WhenIssueFound_ReturnsSuccessResponse()
        {
            // Arrange
            Guid issueId = Guid.NewGuid();

            // Create a sample list of IssueDetailDTOs
            var issueDetails = new List<IssueDetailDTO>
    {
        new IssueDetailDTO { /* Populate with sample data */ },
        // Add more IssueDetailDTO items as needed
    };

            var expertResult = new ResponseDTO<List<IssueDetailDTO>>(issueDetails, string.Empty);

            issueService.Setup(x => x.Detail(issueId))
                        .ReturnsAsync(expertResult);

            // Act
            var result = await controller.Detail(issueId);


            issueService.Verify(s => s.Detail(issueId));
            // Assert
            Assert.That(result.Data, Is.EqualTo(issueDetails));
            Assert.That(result.Message, Is.Empty);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Detail_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            Guid issueId = Guid.NewGuid();
            var expectedExceptionMessage = "Simulated exception message";

            issueService.Setup(x => x.Detail(issueId))
                        .ThrowsAsync(new Exception(expectedExceptionMessage));

            // Act
            Assert.ThrowsAsync<Exception>(async () =>
                                            await controller.Detail(issueId));

        }

        // Additional tests can be added based on specific scenarios and conditions.
    }
}
