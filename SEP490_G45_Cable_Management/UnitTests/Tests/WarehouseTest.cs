using Common.Base;
using Common.Const;
using Common.DTO.WarehouseDTO;
using Common.Pagination;

namespace UnitTests.Tests
{
    [TestFixture]
    public class WarehouseTest
    {
        private WarehouseController controller;
        private Mock<IWarehouseService> warehouseService;

        [SetUp]
        public void SetUp()
        {
            warehouseService = new Mock<IWarehouseService>();
            controller = new WarehouseController(warehouseService.Object);
        }

        [Test]
        public async Task List_WhenUserIsNotAdminWarehouseKeeperOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var name = "SampleName";  // Replace with a valid name or leave it as null for testing
            int page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.List(name, page);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }


        [Test]
        public async Task Create_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var warehouseCreateUpdateDTO = new WarehouseCreateUpdateDTO { };
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Create(warehouseCreateUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenAdminCreatorIDIsNull_ReturnsNotFoundResponse()
        {
            // Arrange
            var warehouseCreateUpdateDTO = new WarehouseCreateUpdateDTO { };

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.ROLE_ADMIN);

            // Act
            var result = await controller.Create(warehouseCreateUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy ID của bạn. Vui lòng kiểm tra thông tin đăng nhập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var warehouseID = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Delete(warehouseID);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_Admin_ReturnsListOfWarehouses()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            // Define a sample list of warehouses that the method should return
            var expectedWarehouses = new List<WarehouseListDTO> {
    new WarehouseListDTO { WarehouseId = 1, WarehouseName = "Warehouse1" },
    new WarehouseListDTO { WarehouseId = 2, WarehouseName = "Warehouse2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseBase<Pagination<WarehouseListDTO>?>(
                    new Pagination<WarehouseListDTO>(page, expectedWarehouses.Count,
                                                         PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE,
                                                         expectedWarehouses),
                    string.Empty));

            // Act
            var result = await controller.List(name, page);

            warehouseService.Verify(x => x.ListPaged(name, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedWarehouses));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_Warehousekeeper_ReturnsListOfWarehouses()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            // Define a sample list of warehouses that the method should return
            var expectedWarehouses = new List<WarehouseListDTO> {
    new WarehouseListDTO { WarehouseId = 1, WarehouseName = "Warehouse1" },
    new WarehouseListDTO { WarehouseId = 2, WarehouseName = "Warehouse2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            warehouseService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseBase<Pagination<WarehouseListDTO>?>(
                    new Pagination<WarehouseListDTO>(page, expectedWarehouses.Count,
                                                         PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE,
                                                         expectedWarehouses),
                    string.Empty));

            // Act
            var result = await controller.List(name, page);

            warehouseService.Verify(x => x.ListPaged(name, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedWarehouses));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_Leader_ReturnsListOfWarehouses()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            // Define a sample list of warehouses that the method should return
            var expectedWarehouses = new List<WarehouseListDTO> {
    new WarehouseListDTO { WarehouseId = 1, WarehouseName = "Warehouse1" },
    new WarehouseListDTO { WarehouseId = 2, WarehouseName = "Warehouse2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_LEADER);

            warehouseService.Setup(x => x.ListPaged(name, page))
                .ReturnsAsync(new ResponseBase<Pagination<WarehouseListDTO>?>(
                    new Pagination<WarehouseListDTO>(page, expectedWarehouses.Count,
                                                         PageSizeConst.MAX_WAREHOUSE_LIST_IN_PAGE,
                                                         expectedWarehouses),
                    string.Empty));

            // Act
            var result = await controller.List(name, page);

            warehouseService.Verify(x => x.ListPaged(name, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedWarehouses));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }


        [Test]
        public void ListPaged_WhenExceptionOccurs_ReturnsNull()
        {
            // Arrange
            var name = "SampleName";
            var page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.ListPaged(name, page))
                .ThrowsAsync(new Exception("Sample exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.List(name, page));
        }

        [Test]
        public async Task ListAll_WhenDataRetrievedSuccessfully_ReturnsListOfWarehouses()
        {
            // Arrange
            var expectedWarehouses = new List<WarehouseListDTO> {
    new WarehouseListDTO { WarehouseId = 1, WarehouseName = "Warehouse1" },
    new WarehouseListDTO { WarehouseId = 2, WarehouseName = "Warehouse2" }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            warehouseService.Setup(x => x.ListAll())
                .ReturnsAsync(new ResponseBase<List<WarehouseListDTO>?>(expectedWarehouses, string.Empty));

            // Act
            var result = await controller.List();

            warehouseService.Verify(x => x.ListAll());

            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedWarehouses));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListAll_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_WAREHOUSE_KEEPER);

            warehouseService.Setup(x => x.ListAll()).ThrowsAsync(new Exception("Some error message"));

            // Act and Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await controller.List());
            Assert.That(ex.Message, Is.EqualTo("Some error message"));
        }

        [Test]
        public async Task Update_WhenWarehouseNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var warehouseId = 1;
            var warehouseUpdateDTO = new WarehouseCreateUpdateDTO { WarehouseName = "NewWarehouse" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Update(warehouseId, warehouseUpdateDTO))
                .ReturnsAsync(
                    new ResponseBase<bool>(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(warehouseId, warehouseUpdateDTO);

            warehouseService.Verify(x => x.Update(warehouseId, warehouseUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy kho"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenWarehouseNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var warehouseId = 1;
            var warehouseUpdateDTO = new WarehouseCreateUpdateDTO { WarehouseName = String.Empty };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Update(warehouseId, warehouseUpdateDTO))
                .ReturnsAsync(new ResponseBase<bool>(false, "Tên kho không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(warehouseId, warehouseUpdateDTO);

            warehouseService.Verify(x => x.Update(warehouseId, warehouseUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên kho không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenWarehouseUpdatedSuccessfully_ReturnsTrueResponse()
        {
            // Arrange
            var warehouseId = 1;
            var warehouseUpdateDTO = new WarehouseCreateUpdateDTO { WarehouseName = "SampleWarehouse" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Update(warehouseId, warehouseUpdateDTO))
                .ReturnsAsync(new ResponseBase<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(warehouseId, warehouseUpdateDTO);

            warehouseService.Verify(x => x.Update(warehouseId, warehouseUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
        }

        [Test]
        public void Update_WhenWarehouseUpdateFails_ThrowsException()
        {
            // Arrange
            var warehouseId = 1;
            var warehouseUpdateDTO = new WarehouseCreateUpdateDTO { WarehouseName = "SampleWarehouse" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Update(warehouseId, warehouseUpdateDTO))
                .ThrowsAsync(new Exception("Some error occurred during warehouse update"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () =>
                                              await controller.Update(warehouseId, warehouseUpdateDTO));
        }

        [Test]
        public void Delete_WhenWarehouseNotFound_ThrowsException()
        {
            // Arrange
            var warehouseId = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Delete(warehouseId)).ThrowsAsync(new Exception(""));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Delete(warehouseId));
        }

        [Test]
        public async Task Delete_WhenWarehouseNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            int warehouseId = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Delete(warehouseId))
                .ReturnsAsync(
                    new ResponseBase<bool>(false, "Không tìm thấy kho", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Delete(warehouseId);

            warehouseService.Verify(x => x.Delete(warehouseId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy kho"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenWarehouseDeletedSuccessfully_ReturnsTrue()
        {
            // Arrange
            int warehouseId = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            warehouseService.Setup(x => x.Delete(warehouseId))
                .ReturnsAsync(new ResponseBase<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(warehouseId);

            warehouseService.Verify(x => x.Delete(warehouseId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
        }
    }
}
