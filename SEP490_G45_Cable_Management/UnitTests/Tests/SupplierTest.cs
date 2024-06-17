using Common.Const;
using Common.DTO.SupplierDTO;
using Common.Pagination;

namespace UnitTests.Tests
{
    [TestFixture]
    public class SupplierTest
    {
        private Mock<ISupplierService> supplierService;
        private SupplierController controller;
        [SetUp]
        public void SetUp()
        {
            supplierService = new Mock<ISupplierService>();
            controller = new SupplierController(supplierService.Object);
        }

        [Test]
        public async Task List_WhenUserIsNotAdminWarehouseKeeperOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var name = "SampleName";
            int page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.List(name, page);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }



        [Test]
        public async Task Create_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Create(supplierCreateUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenUserIdIsNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO();

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.ROLE_ADMIN);

            // Act
            var result = await controller.Create(supplierCreateUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra lại thông tin đăng nhập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var supplierID = 123;  // Replace with a valid supplier ID
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Update(supplierID, supplierCreateUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var supplierID = 123;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Delete(supplierID);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_Admin_ReturnsListOfSuppliers()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            // Define a sample list of suppliers that the method should return
            var expectedSuppliers = new List<SupplierListDTO> {
    new SupplierListDTO { SupplierId = 1, SupplierName = "Supplier1" },
    new SupplierListDTO { SupplierId = 2, SupplierName = "Supplier2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseDTO<Pagination<SupplierListDTO>?>(
                    new Pagination<SupplierListDTO>(page, expectedSuppliers.Count,
                                                        PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE,
                                                        expectedSuppliers),
                    string.Empty));

            // Act
            var result = await controller.List(name, page);

            supplierService.Verify(x => x.ListPaged(name, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedSuppliers));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_Leader_ReturnsListOfSuppliers()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            // Define a sample list of suppliers that the method should return
            var expectedSuppliers = new List<SupplierListDTO> {
    new SupplierListDTO { SupplierId = 1, SupplierName = "Supplier1" },
    new SupplierListDTO { SupplierId = 2, SupplierName = "Supplier2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_LEADER);

            supplierService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseDTO<Pagination<SupplierListDTO>?>(
                    new Pagination<SupplierListDTO>(page, expectedSuppliers.Count,
                                                        PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE,
                                                        expectedSuppliers),
                    string.Empty));

            // Act
            var result = await controller.List(name, page);

            supplierService.Verify(x => x.ListPaged(name, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedSuppliers));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_WarehouseKeeper_ReturnsListOfSuppliers()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            // Define a sample list of suppliers that the method should return
            var expectedSuppliers = new List<SupplierListDTO> {
    new SupplierListDTO { SupplierId = 1, SupplierName = "Supplier1" },
    new SupplierListDTO { SupplierId = 2, SupplierName = "Supplier2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            supplierService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseDTO<Pagination<SupplierListDTO>?>(
                    new Pagination<SupplierListDTO>(page, expectedSuppliers.Count,
                                                        PageSizeConst.MAX_SUPPLIER_LIST_IN_PAGE,
                                                        expectedSuppliers),
                    string.Empty));

            // Act
            var result = await controller.List(name, page);

            supplierService.Verify(x => x.ListPaged(name, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedSuppliers));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListPaged_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.ListPaged(name, page))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.List(name, page));
        }

        [Test]
        public async Task ListAll_WhenDataRetrievedSuccessfully_ReturnsListOfSuppliers()
        {
            // Arrange\
            var expectedSuppliers = new List<SupplierListDTO>
   { new SupplierListDTO { SupplierId = 1, SupplierName = "Supplier1" },
     new SupplierListDTO { SupplierId = 2, SupplierName = "Supplier2" } };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.ListAll())
                .ReturnsAsync(new ResponseDTO<List<SupplierListDTO>?>(expectedSuppliers, string.Empty));

            // Act
            var result = await controller.List();

            supplierService.Verify(x => x.ListAll());

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.EqualTo(expectedSuppliers));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task Create_WhenSupplierNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var supplierCreateDTO = new SupplierCreateUpdateDTO { SupplierName = "" };

            var creatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Create(supplierCreateDTO, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên nhà cung cấp không được để trống",
                                               (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(supplierCreateDTO);

            supplierService.Verify(x => x.Create(supplierCreateDTO, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên nhà cung cấp không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenSupplierAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            var supplierCreateDTO = new SupplierCreateUpdateDTO { SupplierName = "ExistingSupplier" };

            var creatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Create(supplierCreateDTO, creatorId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(supplierCreateDTO);

            supplierService.Verify(x => x.Create(supplierCreateDTO, creatorId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Nhà cung cấp đã tồn tại"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenSupplierCreationIsSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var supplierCreateDTO = new SupplierCreateUpdateDTO { SupplierName = "NewSupplier" };

            var creatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Create(supplierCreateDTO, creatorId))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Thêm thành công"));

            // Act
            var result = await controller.Create(supplierCreateDTO);

            supplierService.Verify(x => x.Create(supplierCreateDTO, creatorId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Thêm thành công"));
        }

        [Test]
        public void Create_WhenExceptionIsThrownDuringCreation_ReturnsErrorResponse()
        {
            // Arrange
            var supplierCreateDTO = new SupplierCreateUpdateDTO { SupplierName = "NewSupplier" };

            var creatorId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            var errorMessage = "An error occurred while creating the supplier.";

            supplierService.Setup(x => x.Create(supplierCreateDTO, creatorId))
                .ThrowsAsync(new Exception(errorMessage));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Create(supplierCreateDTO));
        }

        [Test]
        public async Task Update_WhenSupplierIdIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            int supplierId = 0;
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO { SupplierName = "UpdatedSupplier" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Update(supplierId, supplierCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp",
                                                    (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(supplierId, supplierCreateUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy nhà cung cấp"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenSupplierNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            int supplierId = 1;
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO { SupplierName = "" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Update(supplierId, supplierCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên nhà cung cấp không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(supplierId, supplierCreateUpdateDTO);

            supplierService.Verify(x => x.Update(supplierId, supplierCreateUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên nhà cung cấp không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenSupplierAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            int supplierId = 1;
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO { SupplierName = "ExistingSupplier" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Update(supplierId, supplierCreateUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Nhà cung cấp đã tồn tại", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(supplierId, supplierCreateUpdateDTO);

            supplierService.Verify(x => x.Update(supplierId, supplierCreateUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Nhà cung cấp đã tồn tại"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenSupplierIsSuccessfullyUpdated_ReturnsSuccessResponse()
        {
            // Arrange
            int supplierId = 1;
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO { SupplierName = "NewSupplier" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Update(supplierId, supplierCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(supplierId, supplierCreateUpdateDTO);

            supplierService.Verify(x => x.Update(supplierId, supplierCreateUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
        }

        [Test]
        public void Update_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            int supplierId = 1;
            var supplierCreateUpdateDTO = new SupplierCreateUpdateDTO { SupplierName = "NewSupplier" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Update(supplierId, supplierCreateUpdateDTO))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Update(supplierId, supplierCreateUpdateDTO));
        }

        [Test]
        public void Delete_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            int supplierId = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Delete(supplierId))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            var result = Assert.ThrowsAsync<Exception>(async () => await controller.Delete(supplierId));

            // Assert
            Assert.That(result.Message, Is.EqualTo("Simulated exception"));
        }

        [Test]
        public async Task Delete_WhenSupplierDoesNotExist_ReturnsNotFoundResponse()
        {
            // Arrange
            int supplierId = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Delete(supplierId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy nhà cung cấp",
                                                    (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(supplierId);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy nhà cung cấp"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenSupplierExists_DeletesSupplierSuccessfully()
        {
            // Arrange
            int supplierId = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            supplierService.Setup(x => x.Delete(supplierId))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(supplierId);

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
        }
    }
}
