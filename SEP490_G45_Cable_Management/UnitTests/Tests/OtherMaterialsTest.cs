using DataAccess.DTO.OtherMaterialsDTO;
using DataAccess.Entity;

namespace UnitTests.Tests
{
    [TestFixture]
    public class OtherMaterialsTest
    {
        private OtherMaterialsController controller;
        private Mock<IOtherMaterialsService> otherMaterialsService;

        [SetUp]
        public void SetUp()
        {
            otherMaterialsService = new Mock<IOtherMaterialsService>();
            controller = new OtherMaterialsController(otherMaterialsService.Object);
        }

        [Test]
        public async Task List_WhenUserNotAuthorized_ReturnsForbiddenResponse()
        {
            // Arrange
            string filter = "SampleFilter";
            int page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List(filter, null, page);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }


        [Test]
        public async Task List_WhenWarehouseKeeperIDNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            string filter = "SampleFilter";
            int page = 1;

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.List(filter, null, page);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ListPaged_ByWarehouseKeeper_ReturnsPagedResultDTO()
        {
            // Arrange
            string filter = "SampleFilter";
            int page = 1;
            int? warehouseId = 123;  // Sample warehouse ID
            Guid warehouseKeeperId =
                TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            otherMaterialsService.Setup(x => x.ListPaged(filter, warehouseId, warehouseKeeperId, page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(
                    new PagedResultDTO<OtherMaterialsListDTO>(page, 10,
                                                              PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE,
                                                              new List<OtherMaterialsListDTO>(), 100),
                    string.Empty));

            // Act
            var result = await controller.List(filter, warehouseId, page);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.RowCount, Is.EqualTo(10));
            Assert.That(result.Data.Sum, Is.EqualTo(100));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task ListPaged_ByAdmin_ReturnsPagedResultDTO()
        {
            // Arrange
            string filter = "SampleFilter";
            int page = 1;
            int? warehouseId = 123;  // Sample warehouse ID
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.ListPaged(filter, warehouseId, null, page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<OtherMaterialsListDTO>?>(
                    new PagedResultDTO<OtherMaterialsListDTO>(page, 10,
                                                              PageSizeConst.MAX_OTHER_MATERIAL_LIST_IN_PAGE,
                                                              new List<OtherMaterialsListDTO>(), 100),
                    string.Empty));

            // Act
            var result = await controller.List(filter, warehouseId, page);
            otherMaterialsService.Verify(s => s.ListPaged(filter, warehouseId, null, page));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.RowCount, Is.EqualTo(10));
            Assert.That(result.Data.Sum, Is.EqualTo(100));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task Create_WhenNotAdminNorWarehouseKeeper_ReturnsForbiddenResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO = new OtherMaterialsCreateUpdateDTO { };

            TestHelper.SimulateUserWithRoleAndId(
                controller,
                RoleConst.STRING_ROLE_LEADER);  // Set a user role that is not admin or warehouse keeper

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenQuantityIsNegative_ReturnsConflictResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO = new OtherMaterialsCreateUpdateDTO { Quantity = -100 };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Số lượng không hợp lệ", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);
            otherMaterialsService.Verify(s => s.Create(otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Số lượng không hợp lệ"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenCodeIsNull_ReturnsErrorResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO = new OtherMaterialsCreateUpdateDTO
            {
                Unit = "SampleUnit",
                Quantity = 10,
                Code = null,
                WarehouseId = 1,
                Status = "SampleStatus",
                OtherMaterialsCategoryId = 1
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Mã hàng không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);
            otherMaterialsService.Verify(s => s.Create(otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
            Assert.That(result.Message, Is.EqualTo("Mã hàng không được để trống"));
        }

        [Test]
        public async Task Create_WhenStatusIsEmpty_ReturnsErrorResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "      ",
                    OtherMaterialsCategoryId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Trạng thái không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);
            otherMaterialsService.Verify(s => s.Create(otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Trạng thái không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenWarehouseIdIsNull_ReturnsErrorResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = null,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Kho không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);
            otherMaterialsService.Verify(s => s.Create(otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenMaterialIsNull_ReturnsSuccessResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Create(otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Create_WhenMaterialIsNotNull_ReturnsSuccessResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            OtherMaterial existingMaterial = new OtherMaterial { };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Create(otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenMaterialExists_ReturnsSuccessResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            OtherMaterial existingMaterial =
                new OtherMaterial
                {
                    OtherMaterialsId = 1,
                    Unit = "SampleUnit",
                    Quantity = 50,
                    Code = "SampleCode",
                    SupplierId = 1,
                    CreatedAt = new DateTime(2023, 10, 1),
                    UpdateAt = new DateTime(2023, 10, 15),
                    IsDeleted = false,
                    WarehouseId = 1,
                    MaxQuantity = 100,
                    MinQuantity = 10,
                    Status = "Active",
                    OtherMaterialsCategoryId = 1,
                    OtherMaterialsCategory = new OtherMaterialsCategory { },
                    Supplier = new Supplier { },
                    Warehouse = new Warehouse { },
                    NodeMaterials = new List<NodeMaterial> { },
                    RequestOtherMaterials = new List<RequestOtherMaterial> { },
                    TransactionOtherMaterials = new List<TransactionOtherMaterial> { }
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenMaterialIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy vật liệu"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenCodeIsNull_ReturnsConflictResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO = new OtherMaterialsCreateUpdateDTO
            {
                Unit = "SampleUnit",
                Quantity = 10,
                Code = null,
                WarehouseId = 1,
                Status = "SampleStatus",
                OtherMaterialsCategoryId = 1
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Mã hàng không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Mã hàng không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenUnitIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = string.Empty,
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Đơn vị không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Đơn vị không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenStatusIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = string.Empty,
                    OtherMaterialsCategoryId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Trạng thái không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Trạng thái không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenWarehouseIdIsNull_ReturnsConflictResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = null,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Kho không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Kho không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenMaterialAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO
                {
                    Unit = "SampleUnit",
                    Quantity = 10,
                    Code = "SampleCode",
                    WarehouseId = 1,
                    Status = "SampleStatus",
                    OtherMaterialsCategoryId = 1
                };

            OtherMaterial existingMaterial = new OtherMaterial { };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Vật liệu đã tồn tại", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            otherMaterialsService.Verify(s => s.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Vật liệu đã tồn tại"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public void Update_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            var otherMaterialsCreateUpdateDTO = new OtherMaterialsCreateUpdateDTO { };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO))
                .ThrowsAsync(new Exception("An error occurred during the update operation"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(
                async () => await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO));
        }

        [Test]
        public void Create_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var otherMaterialsCreateUpdateDTO = new OtherMaterialsCreateUpdateDTO { };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Create(otherMaterialsCreateUpdateDTO))
                .ThrowsAsync(new Exception("An error occurred during the update operation"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Create(otherMaterialsCreateUpdateDTO));
        }

        [Test]
        public async Task Delete_WhenMaterialExists_ReturnsSuccessResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            OtherMaterial existingMaterial = new OtherMaterial();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Delete(otherMaterialsID))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(otherMaterialsID);

            otherMaterialsService.Verify(s => s.Delete(otherMaterialsID));

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Delete_WhenMaterialIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            int otherMaterialsID = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Delete(otherMaterialsID))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy vật liệu", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(otherMaterialsID);

            otherMaterialsService.Verify(s => s.Delete(otherMaterialsID));

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy vật liệu"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public void Delete_WhenExceptionThrown_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            int otherMaterialsID = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsService.Setup(x => x.Delete(otherMaterialsID))
                .ThrowsAsync(new Exception("Mocked exception"));

            // Act and Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await controller.Delete(otherMaterialsID));
            Assert.That(ex.Message, Is.EqualTo("Mocked exception"));
        }

        [Test]
        public async Task Update_WhenNotAdminOrWarehouseKeeper_ReturnsForbiddenResponse()
        {
            // Arrange
            int otherMaterialsID = 1;
            OtherMaterialsCreateUpdateDTO otherMaterialsCreateUpdateDTO =
                new OtherMaterialsCreateUpdateDTO();  // Add appropriate DTO values

            TestHelper.SimulateUserWithRoleAndId(
                controller,
                RoleConst
                    .STRING_ROLE_LEADER);  // Assuming the current user is not an admin or warehouse keeper

            // Act
            var result = await controller.Update(otherMaterialsID, otherMaterialsCreateUpdateDTO);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenNotAdminOrWarehouseKeeper_ReturnsForbiddenResponse()
        {
            // Arrange
            int otherMaterialsID = 1;

            TestHelper.SimulateUserWithRoleAndId(
                controller,
                RoleConst
                    .STRING_ROLE_STAFF);  // Assuming the current user is not an admin or warehouse keeper

            // Act
            var result = await controller.Delete(otherMaterialsID);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }
    }
}
