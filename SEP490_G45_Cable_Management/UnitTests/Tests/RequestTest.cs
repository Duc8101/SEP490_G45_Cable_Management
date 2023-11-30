using DataAccess.DTO.CableDTO;
using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.DTO.RequestDTO;

namespace UnitTests.Tests
{
    [TestFixture]
    public class RequestTest
    {
        private RequestController controller;
        private Mock<IRequestService> requestService;

        [SetUp]
        public void SetUp()
        {
            requestService = new Mock<IRequestService>();
            controller = new RequestController(requestService.Object);
        }

        [Test]
        public async Task List_WhenStaffCreatorIDNull_ReturnsNotFoundResponse()
        {
            // Arrange
            string name = "SampleName";
            string status = "SampleStatus";
            int? RequestCategoryID = null;
            int page = 1;

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List(name, RequestCategoryID, status, page);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task List_WhenUserNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            string name = "SampleName";
            string status = "SampleStatus";
            int page = 1;
            TestHelper.SimulateUserWithRoleAndId(controller, "SampleUserRole");

            // Act
            var result = await controller.List(name,null, status, page);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenUserNotWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var requestCreateExportDTO = new RequestCreateExportDTO();
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_ADMIN);  // Simulating a user with a role other than
                                                           // warehouse keeper or staff

            // Act
            var result = await controller.Create(requestCreateExportDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CreateRequestExport_WhenStaffCreatorIdNull_ReturnsFalseResponse()
        {
            // Arrange
            var requestCreateExportDTO = new RequestCreateExportDTO();
            TestHelper.SimulateUserWithRoleWithoutID(
                controller,
                RoleConst
                    .STRING_ROLE_STAFF);  // Simulating a user with the staff role but with a null creator ID

            // Act
            var result = await controller.Create(requestCreateExportDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenUserNotWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var requestCreateRecoveryDTO =
                new RequestCreateRecoveryDTO();  // Add relevant data to the DTO if necessary
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_ADMIN);  // Simulating a user with a role other than
                                                           // warehouse keeper or staff

            // Act
            var result = await controller.Create(requestCreateRecoveryDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenWarehouseKeeperCreatorIdNull_ReturnsFalseResponse()
        {
            // Arrange
            var requestCreateRecoveryDTO =
                new RequestCreateRecoveryDTO();  // Add relevant data to the DTO if necessary
            TestHelper.SimulateUserWithRoleWithoutID(
                controller,
                RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulating a user with the warehouse keeper role
                                                          // but with a null creator ID

            // Act
            var result = await controller.Create(requestCreateRecoveryDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenUserNotWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var requestCreateDeliverDTO =
                new RequestCreateDeliverDTO();  // Add relevant data to the DTO if necessary
            TestHelper.SimulateUserWithRoleAndId(
                controller,
                "SampleUserRole");  // Simulating a user with a role other than warehouse keeper or staff

            // Act
            var result = await controller.Create(requestCreateDeliverDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenWarehouseKeeperCreatorIdNull_ReturnsFalseResponse()
        {
            // Arrange
            var requestCreateDeliverDTO =
                new RequestCreateDeliverDTO();  // Add relevant data to the DTO if necessary
            TestHelper.SimulateUserWithRoleWithoutID(
                controller,
                RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulating a user with the warehouse keeper role
                                                          // but with a null creator ID

            // Act
            var result = await controller.Create(requestCreateDeliverDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
        }

        [Test]
        public async Task Reject_WhenUserNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var requestId = Guid.NewGuid();  // Replace with a valid request ID
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_STAFF);  // Simulating a user with a role other than admin

            // Act
            var result = await controller.Reject(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Reject_WhenAdminRejectorIdNull_ReturnsFalseResponse()
        {
            // Arrange
            var requestId = Guid.NewGuid();  // Replace with a valid request ID
            TestHelper.SimulateUserWithRoleWithoutID(
                controller,
                RoleConst.STRING_ROLE_ADMIN);  // Simulating an admin user with a null Rejector ID

            // Act
            var result = await controller.Reject(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
        }

/*        [Test]
        public async Task SuggestionCable_WhenUserNotWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var suggestion = new SuggestionCableDTO();  // Replace with a valid SuggestionCableDTO object
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_ADMIN);  // Simulating a user with a role other than
                                                           // warehouse keeper or staff

            // Act
            var result = await controller.SuggestionCable(suggestion);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }
*/
        [Test]
        public async Task Create_CancelInside_WhenUserNotWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var cancelInsideDTO = new RequestCreateCancelInsideDTO();  // Replace with a valid
                                                                       // RequestCreateCancelInsideDTO object
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_LEADER);  // Simulating a user with a role other than
                                                            // warehouse keeper or staff

            // Act
            var result = await controller.Create(cancelInsideDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_CancelInside_WhenWarehouseKeeperIDIsNull_ReturnsFalseResponse()
        {
            // Arrange
            var cancelInsideDTO = new RequestCreateCancelInsideDTO();  // Replace with a valid
                                                                       // RequestCreateCancelInsideDTO object
            TestHelper.SimulateUserWithRoleWithoutID(
                controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulating a warehousekeeper user

            // Act
            var result = await controller.Create(cancelInsideDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
        }

        [Test]
        public async Task Create_CancelOutside_WhenUserNotWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var cancelOutsideDTO =
                new RequestCreateCancelOutsideDTO();  // Replace with a valid RequestCreateCancelOutsideDTO
                                                      // object
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_ADMIN);  // Simulating a user with a role other than
                                                           // warehouse keeper or staff

            // Act
            var result = await controller.Create(cancelOutsideDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_CancelOutside_WhenWarehouseKeeperIDIsNull_ReturnsFalseResponse()
        {
            // Arrange
            var cancelOutsideDTO =
                new RequestCreateCancelOutsideDTO();  // Replace with a valid RequestCreateCancelOutsideDTO
                                                      // object
            TestHelper.SimulateUserWithRoleWithoutID(
                controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulating a warehousekeeper user

            // Act
            var result = await controller.Create(cancelOutsideDTO);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
        }

        [Test]
        public async Task Delete_WhenUserNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var requestId = Guid.NewGuid();  // Replace with a valid request ID
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_LEADER);  // Simulating a user with a role other than admin

            // Act
            var result = await controller.Delete(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Detail_WhenUserNotAdminOrWarehouseKeeperOrStaff_ReturnsForbiddenResponse()
        {
            // Arrange
            var requestId = Guid.NewGuid();  // Replace with a valid request ID
            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_LEADER);  // Simulating a user with a role other than
                                                            // warehouse keeper or staff

            // Act
            var result = await controller.Detail(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task List_WhenAdmin_ReturnsSuccessResponse()
        {
            // Arrange
            string name = "SampleName";
            string status = "SampleStatus";
            int page = 1;
            var resultSample = new PagedResultDTO<RequestListDTO>(
                currentPage: 1, pageSize: PageSizeConst.MAX_REQUEST_LIST_IN_PAGE, rowCount: 10,
                results: new List<RequestListDTO> {
        new RequestListDTO { RequestId = Guid.NewGuid(), RequestName = "Sample Request",
                             Content = "Sample content", CreatorName = "Sample Creator",
                             ApproverName = "Sample Approver", Status = "Sample Status",
                             RequestCategoryName = "Sample Request Category" },
                });

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var expectedResponse =
                new ResponseDTO<PagedResultDTO<RequestListDTO>?>(resultSample, string.Empty);

            requestService.Setup(x => x.List(name,null, status, creatorId, page)).ReturnsAsync(expectedResponse);

            // Act
            var result = await controller.List(name, null,status, page);

            requestService.Verify(s => s.List(name, null, status, creatorId, page), Times.Once);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.EqualTo(expectedResponse.Data));
            Assert.That(result.Message, Is.EqualTo(expectedResponse.Message));
            Assert.That(result.Code, Is.EqualTo(expectedResponse.Code));
        }

        [Test]
        public void List_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            string name = "SampleName";
            string status = "SampleStatus";
            int page = 1;

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.List(name,null, status, creatorId, page))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.List(name,null, status, page));
        }

        [Test]
        public void CreateRequestExport_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange

            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),  // Replace with a valid Issue ID
                CableExportDTOs =
                 new List<CableExportDeliverDTO> {
         new CableExportDeliverDTO { CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100 },
                 },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 },
                 }
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Create(requestCreateExportDTOSample));
        }

        [Test]
        public async Task CreateRequestExport_WhenRequestNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableExportDTOs = new List<CableExportDeliverDTO> { new CableExportDeliverDTO {
     CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100
    } },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 }
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateExportDTOSample);

            requestService.Verify(s => s.CreateRequestExport(requestCreateExportDTOSample, creatorId));

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tên yêu cầu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestExport_WhenIssueNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableExportDTOs = new List<CableExportDeliverDTO> { new CableExportDeliverDTO {
     CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100
    } },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 },
                RequestCategoryId = RequestCategoryConst.CATEGORY_EXPORT
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Create(requestCreateExportDTOSample);

            requestService.Verify(s => s.CreateRequestExport(requestCreateExportDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateRequestExport_WhenIssueDone_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableExportDTOs = new List<CableExportDeliverDTO> { new CableExportDeliverDTO {
     CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100
    } },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 },
                RequestCategoryId = RequestCategoryConst.CATEGORY_EXPORT
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Sự vụ với mã SampleIssueCode đã được chấp thuận",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateExportDTOSample);

            requestService.Verify(s => s.CreateRequestExport(requestCreateExportDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Sự vụ với mã SampleIssueCode đã được chấp thuận"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestExport_WhenCableInvalid_ReturnsErrorResponse()
        {
            // Arrange
            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableExportDTOs = new List<CableExportDeliverDTO> { new CableExportDeliverDTO {
     CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100
    } },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 },
                RequestCategoryId = RequestCategoryConst.CATEGORY_EXPORT
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var errorMessage = "Invalid cables detected";
            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, errorMessage, (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateExportDTOSample);

            requestService.Verify(s => s.CreateRequestExport(requestCreateExportDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo(errorMessage));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }
        [Test]
        public async Task CreateRequestExport_WhenMaterialInvalid_ReturnsErrorResponse()
        {
            // Arrange
            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableExportDTOs = new List<CableExportDeliverDTO> { new CableExportDeliverDTO {
     CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100
    } },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 },
                RequestCategoryId = RequestCategoryConst.CATEGORY_EXPORT
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var errorMessage = "Invalid materials detected";
            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, errorMessage, (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateExportDTOSample);

            requestService.Verify(s => s.CreateRequestExport(requestCreateExportDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo(errorMessage));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }
        [Test]
        public async Task CreateRequestExport_WhenRequestCreationSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var requestCreateExportDTOSample = new RequestCreateExportDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableExportDTOs = new List<CableExportDeliverDTO> { new CableExportDeliverDTO {
     CableId = Guid.NewGuid(), StartPoint = 0, EndPoint = 100
    } },
                OtherMaterialsExportDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 },
                RequestCategoryId = RequestCategoryConst.CATEGORY_EXPORT
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var successMessage = "Tạo yêu cầu thành công";
            requestService.Setup(x => x.CreateRequestExport(requestCreateExportDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(true, successMessage));

            // Act
            var result = await controller.Create(requestCreateExportDTOSample);

            requestService.Verify(s => s.CreateRequestExport(requestCreateExportDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo(successMessage));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenRequestNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateRecoveryDTOSample =
                new RequestCreateRecoveryDTO
                {
                    RequestName = "",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid()
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateRecoveryDTOSample);

            requestService.Verify(s => s.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên yêu cầu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenRequestCategoryIdIsNotRecovery_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateRecoveryDTOSample = new RequestCreateRecoveryDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId =
                 100  // Replace with a value not equal to RequestCategoryConst.CATEGORY_RECOVERY
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không phải yêu cầu thu hồi",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateRecoveryDTOSample);

            requestService.Verify(s => s.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không phải yêu cầu thu hồi"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenIssueNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestCreateRecoveryDTOSample =
                new RequestCreateRecoveryDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid(),
                    RequestCategoryId = RequestCategoryConst.CATEGORY_RECOVERY
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Create(requestCreateRecoveryDTOSample);

            requestService.Verify(s => s.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenIssueIsDone_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateRecoveryDTOSample =
                new RequestCreateRecoveryDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid(),
                    RequestCategoryId = RequestCategoryConst.CATEGORY_RECOVERY
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false,
                                                    "Sự vụ với mã <SampleIssueCode> đã được chấp thuận",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateRecoveryDTOSample);

            requestService.Verify(s => s.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Sự vụ với mã <SampleIssueCode> đã được chấp thuận"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestRecovery_WhenRequestCreationIsSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var requestCreateRecoveryDTOSample =
                new RequestCreateRecoveryDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid(),
                    RequestCategoryId = RequestCategoryConst.CATEGORY_RECOVERY
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo yêu cầu thành công"));

            // Act
            var result = await controller.Create(requestCreateRecoveryDTOSample);

            requestService.Verify(s => s.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Tạo yêu cầu thành công"));
        }

        [Test]
        public void CreateRequestRecovery_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestCreateRecoveryDTOSample =
                new RequestCreateRecoveryDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid(),
                    RequestCategoryId = RequestCategoryConst.CATEGORY_RECOVERY
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestRecovery(requestCreateRecoveryDTOSample, creatorId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Create(requestCreateRecoveryDTOSample));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenRequestNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample =
                new RequestCreateDeliverDTO
                {
                    RequestName = "",
                    Content = "SampleContent",
                    RequestCategoryId = RequestCategoryConst.CATEGORY_DELIVER,
                    DeliverWareHouseID = 1
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateDeliverDTOSample);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên yêu cầu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenInvalidRequestCategory_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample =
                new RequestCreateDeliverDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    RequestCategoryId = 1,
                    DeliverWareHouseID = 1
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không phải yêu cầu chuyển kho",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateDeliverDTOSample);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không phải yêu cầu chuyển kho"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenWarehouseNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample =
                new RequestCreateDeliverDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    RequestCategoryId = RequestCategoryConst.CATEGORY_DELIVER,
                    DeliverWareHouseID = 1
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy kho nhận", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Create(requestCreateDeliverDTOSample);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy kho nhận"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenCableInvalid_ReturnsInvalidResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                RequestCategoryId = RequestCategoryConst.CATEGORY_DELIVER,
                DeliverWareHouseID = 1,
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var invalidResponse =
                new ResponseDTO<bool>(false, "Invalid cable", (int)HttpStatusCode.BadRequest);
            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ReturnsAsync(invalidResponse);

            // Act
            var result = await controller.Create(requestCreateDeliverDTOSample);

            // Assert
            Assert.That(result.Data, Is.EqualTo(invalidResponse.Data));
            Assert.That(result.Message, Is.EqualTo(invalidResponse.Message));
            Assert.That(result.Code, Is.EqualTo(invalidResponse.Code));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenMaterialInvalid_ReturnsInvalidResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                RequestCategoryId = RequestCategoryConst.CATEGORY_DELIVER,
                DeliverWareHouseID = 1,
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var invalidResponse =
                new ResponseDTO<bool>(false, "Invalid material", (int)HttpStatusCode.BadRequest);
            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ReturnsAsync(invalidResponse);

            // Act
            var result = await controller.Create(requestCreateDeliverDTOSample);

            // Assert
            Assert.That(result.Data, Is.EqualTo(invalidResponse.Data));
            Assert.That(result.Message, Is.EqualTo(invalidResponse.Message));
            Assert.That(result.Code, Is.EqualTo(invalidResponse.Code));
        }

        [Test]
        public async Task CreateRequestDeliver_WhenRequestCreationSucceeds_ReturnsSuccessResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                RequestCategoryId = RequestCategoryConst.CATEGORY_DELIVER,
                DeliverWareHouseID = 1,
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var successResponse = new ResponseDTO<bool>(true, "Tạo yêu cầu thành công");
            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ReturnsAsync(successResponse);

            // Act
            var result = await controller.Create(requestCreateDeliverDTOSample);

            // Assert
            Assert.That(result.Data, Is.EqualTo(successResponse.Data));
            Assert.That(result.Message, Is.EqualTo(successResponse.Message));
            Assert.That(result.Code, Is.EqualTo(successResponse.Code));
        }

        [Test]
        public void CreateRequestDeliver_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                RequestCategoryId = RequestCategoryConst.CATEGORY_DELIVER,
                DeliverWareHouseID = 1,
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestDeliver(requestCreateDeliverDTOSample, creatorId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Create(requestCreateDeliverDTOSample));
        }

        [Test]
        public async Task Approve_WhenRequestIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid approver ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound));
            // Act
            var result = await controller.Approve(requestID);

            requestService.Setup(s => s.Approve(requestID, approverID, approverName));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy yêu cầu"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Approve_WhenRequestStatusIsNotPending_ReturnsConflictResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ReturnsAsync(new ResponseDTO<bool>(false,
                                                    "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Approve(requestID);

            requestService.Setup(s => s.Approve(requestID, approverID, approverName));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Approve_WhenRequestCategoryMatches_ReturnsExpectedResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Request approved successfully"));

            // Act
            var result = await controller.Approve(requestID);

            requestService.Setup(s => s.Approve(requestID, approverID, approverName));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Request approved successfully"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Approve_WhenRequestCategoryIsRecovery_ReturnsExpectedResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Request approved successfully"));

            // Act
            var result = await controller.Approve(requestID);

            requestService.Setup(s => s.Approve(requestID, approverID, approverName));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Request approved successfully"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Approve_WhenRequestCategoryIsCancelOutside_ReturnsExpectedResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Request approved successfully"));

            // Act
            var result = await controller.Approve(requestID);

            requestService.Setup(s => s.Approve(requestID, approverID, approverName));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Request approved successfully"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Approve_WhenRequestCategoryNotSupported_ReturnsConflictResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var unsupportedRequestCategory = "UnsupportedCategory";

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ReturnsAsync(new ResponseDTO<bool>(false,
                                                    $"Không hỗ trợ yêu cầu {unsupportedRequestCategory}",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Approve(requestID);

            requestService.Setup(s => s.Approve(requestID, approverID, approverName));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo($"Không hỗ trợ yêu cầu {unsupportedRequestCategory}"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public void Approve_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var approverName = "Last First";

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            var approverID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Approve(requestID, approverID, approverName))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Approve(requestID));
        }

        [Test]
        public async Task Reject_WhenRequestIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();
            var rejectorID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            requestService.Setup(x => x.Reject(requestID, rejectorID))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Reject(requestID);

            requestService.Verify(s => s.Reject(requestID, rejectorID));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy yêu cầu"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Reject_WhenRequestStatusIsNotPending_ReturnsConflictResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();
            var rejectorID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            requestService.Setup(x => x.Reject(requestID, rejectorID))
                .ReturnsAsync(new ResponseDTO<bool>(false,
                                                    "Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Reject(requestID);

            requestService.Verify(s => s.Reject(requestID, rejectorID));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Yêu cầu đã được xác nhận chấp thuận hoặc bị từ chối"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Reject_WhenRequestRejectionIsSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var rejectorID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            requestService.Setup(x => x.Reject(requestID, rejectorID))
                .ReturnsAsync(new ResponseDTO<bool>(true, string.Empty));

            // Act
            var result = await controller.Reject(requestID);

            requestService.Verify(s => s.Reject(requestID, rejectorID));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Reject_WhenExceptionIsThrown_ReturnsErrorResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();  // Replace with a valid request ID
            var rejectorID = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var requestCreateDeliverDTOSample = new RequestCreateDeliverDTO();

            requestService.Setup(x => x.Reject(requestID, rejectorID))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Reject(requestID));
        }

        [Test]
        public async Task CreateCancelInside_WhenRequestNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = String.Empty,
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên yêu cầu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateCancelInside_WhenRequestCategoryIsNotCancelInside_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId =
                 123,  // Replace with a value that is not RequestCategoryConst.CATEGORY_CANCEL_INSIDE
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không phải yêu cầu hủy trong kho",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không phải yêu cầu hủy trong kho"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateCancelInside_WhenIssueIsNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_INSIDE,
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task CreateCancelInside_WhenIssueIsMarkedAsDone_ReturnsNotAcceptableResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),  // Replace with a valid Issue ID
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_INSIDE,
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Sự vụ với mã SampleIssueCode đã được chấp thuận",
                                                    (int)HttpStatusCode.NotAcceptable));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Sự vụ với mã SampleIssueCode đã được chấp thuận"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotAcceptable));
        }

        [Test]
        public async Task CreateCancelInside_WhenInvalidCableExists_ReturnsInvalidCableResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),  // Replace with a valid Issue ID
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_INSIDE,
                CableCancelInsideDTOs =
                 new List<CableCancelInsideDTO> { new CableCancelInsideDTO { CableId = Guid.NewGuid(),
                                                                    WarehouseId = 1 } },
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "One or more cables are invalid",
                                                    (int)HttpStatusCode.BadRequest));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("One or more cables are invalid"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateCancelInside_WhenInvalidMaterialExists_ReturnsInvalidMaterialResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_INSIDE,
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs =
                 new List<OtherMaterialsExportDeliverCancelInsideDTO> {
         new OtherMaterialsExportDeliverCancelInsideDTO { WarehouseId = 1, OtherMaterialsId = 1,
                                                          Quantity = 5 }
                 }
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "One or more materials are invalid",
                                                    (int)HttpStatusCode.BadRequest));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("One or more materials are invalid"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task CreateCancelInside_WhenRequestIsValid_ReturnsSuccessResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),  // Replace with a valid Issue ID
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_INSIDE,
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo yêu cầu thành công"));

            // Act
            var result = await controller.Create(requestCreateCancelInsideDTOSample);

            requestService.Verify(s => s.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Tạo yêu cầu thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public void CreateCancelInside_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestCreateCancelInsideDTOSample = new RequestCreateCancelInsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_INSIDE,
                CableCancelInsideDTOs = new List<CableCancelInsideDTO>(),
                OtherMaterialsCancelInsideDTOs = new List<OtherMaterialsExportDeliverCancelInsideDTO>()
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelInside(requestCreateCancelInsideDTOSample, creatorId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Create(requestCreateCancelInsideDTOSample));
        }

        [Test]
        public async Task CreateCancelOutside_WhenRequestNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateCancelOutsideDTOSample =
                new RequestCreateCancelOutsideDTO
                {
                    RequestName = "",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid()
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên yêu cầu không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateCancelOutsideDTOSample);

            requestService.Verify(s =>
                                      s.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên yêu cầu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateCancelOutside_WhenRequestCategoryIsNotCancelOutside_ReturnsConflictResponse()
        {
            // Arrange
            var requestCreateCancelOutsideDTOSample =
                new RequestCreateCancelOutsideDTO
                {
                    RequestName = "SampleRequest",
                    Content = "SampleContent",
                    IssueId = Guid.NewGuid(),
                    RequestCategoryId = 1
                };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không phải yêu cầu hủy ngoài kho",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(requestCreateCancelOutsideDTOSample);

            requestService.Verify(s =>
                                      s.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không phải yêu cầu hủy ngoài kho"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task CreateCancelOutside_WhenIssueIsNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestCreateCancelOutsideDTOSample = new RequestCreateCancelOutsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy sự vụ", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Create(requestCreateCancelOutsideDTOSample);

            requestService.Verify(s =>
                                      s.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự vụ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
        [Test]
        public async Task CreateCancelOutside_WhenIssueIsDone_ReturnsNotAcceptableResponse()
        {
            // Arrange
            var requestCreateCancelOutsideDTOSample = new RequestCreateCancelOutsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(
                    false,
                    "Sự vụ với mã " + requestCreateCancelOutsideDTOSample.IssueId + " đã được chấp thuận",
                    (int)HttpStatusCode.NotAcceptable));

            // Act
            var result = await controller.Create(requestCreateCancelOutsideDTOSample);

            requestService.Verify(s =>
                                      s.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Sự vụ với mã " + requestCreateCancelOutsideDTOSample.IssueId +
                                   " đã được chấp thuận"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotAcceptable));
        }

        [Test]
        public async Task CreateCancelOutside_WhenRequestIsSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var requestCreateCancelOutsideDTOSample = new RequestCreateCancelOutsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo yêu cầu thành công"));

            // Act
            var result = await controller.Create(requestCreateCancelOutsideDTOSample);

            requestService.Verify(s =>
                                      s.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Tạo yêu cầu thành công"));
        }

        [Test]
        public void CreateCancelOutside_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestCreateCancelOutsideDTOSample = new RequestCreateCancelOutsideDTO
            {
                RequestName = "SampleRequest",
                Content = "SampleContent",
                IssueId = Guid.NewGuid(),
                RequestCategoryId = RequestCategoryConst.CATEGORY_CANCEL_OUTSIDE
            };

            var creatorId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            requestService.Setup(x => x.CreateRequestCancelOutside(requestCreateCancelOutsideDTOSample, creatorId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Create(requestCreateCancelOutsideDTOSample));
        }

        [Test]
        public async Task Delete_WhenRequestIsNullOrStatusNotPending_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Delete(requestID))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy yêu cầu", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(requestID);

            requestService.Verify(s => s.Delete(requestID));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy yêu cầu"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenRequestIsDeleted_ReturnsSuccessResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Delete(requestID))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(requestID);

            requestService.Verify(s => s.Delete(requestID));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
        }

        [Test]
        public async Task Delete_WhenExceptionOccurs_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Delete(requestID))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Sample error message",
                                                    (int)HttpStatusCode.InternalServerError));

            // Act
            var result = await controller.Delete(requestID);

            requestService.Verify(s => s.Delete(requestID));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Sample error message"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        [Test]
        public async Task Detail_WhenRequestIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Detail(requestID))
                .ReturnsAsync(new ResponseDTO<RequestDetailDTO?>(null, "Không tìm thấy yêu cầu",
                                                                 (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Detail(requestID);

            requestService.Verify(s => s.Detail(requestID));

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy yêu cầu"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Detail_WhenRequestIsNotNull_ReturnsRequestDetailDTO()
        {
            // Arrange
            var requestID = Guid.NewGuid();

            // Simulate a user with the required role and ID
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var requestDetailDTO = new RequestDetailDTO
            {
                RequestId = requestID,
            };
            requestService.Setup(x => x.Detail(requestID))
                .ReturnsAsync(new ResponseDTO<RequestDetailDTO?>(requestDetailDTO, string.Empty));

            // Act
            var result = await controller.Detail(requestID);

            // Verify
            requestService.Verify(s => s.Detail(requestID));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.RequestId, Is.EqualTo(requestID));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Detail_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestID = Guid.NewGuid();

            // Simulate a user with the required role and ID
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            requestService.Setup(x => x.Detail(requestID)).ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Detail(requestID));
        }
    }
}
