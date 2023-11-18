using DataAccess.DTO.NodeDTO;
using DataAccess.DTO.NodeMaterialCategoryDTO;

namespace UnitTests.Tests
{
    [TestFixture]
    public class NodeTests
    {
        private Mock<INodeService> nodeService;
        private NodeController controller;
        [SetUp]
        public void SetUp()
        {
            nodeService = new Mock<INodeService>();
            controller = new NodeController(nodeService.Object);
        }

        [Test]
        public async Task List_WhenNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var routeId = Guid.NewGuid();  // Replace with the actual route ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List(routeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task List_WhenValidRouteId_ReturnsListOfNodes()
        {
            // Arrange
            var routeId = Guid.NewGuid();

            var expectedNodes = new List<NodeListDTO> {
    new NodeListDTO { Id = Guid.NewGuid(), NodeMaterialCategoryListDTOs =
                                               new List<NodeMaterialCategoryListDTO> { new NodeMaterialCategoryListDTO {
                                                OtherMaterialsCategoryName = "Category 1", Quantity = 10
                                               } } },
    new NodeListDTO { Id = Guid.NewGuid(), NodeMaterialCategoryListDTOs =
                                               new List<NodeMaterialCategoryListDTO> { new NodeMaterialCategoryListDTO {
                                                OtherMaterialsCategoryName = "Category 2", Quantity = 20
                                               } } }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            nodeService.Setup(x => x.List(routeId)).ReturnsAsync(new ResponseDTO<List<NodeListDTO>?>(expectedNodes, string.Empty));

            // Act
            var result = await controller.List(routeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data, Is.EqualTo(expectedNodes));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task Create_WhenUserNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var sampleData = new NodeCreateDTO { };

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulating a user with a staff role

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenRouteIdNotProvided_ReturnsConflictResponse()
        {
            // Arrange
            var sampleData = new NodeCreateDTO { RouteId = null, NumberOrder = 2 };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            nodeService.Setup(x => x.Create(sampleData))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Bạn chưa chọn tuyến", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn chưa chọn tuyến"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenValidData_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleData = new NodeCreateDTO
            {
                RouteId = Guid.NewGuid(),
                NumberOrder = 1,
                Longitude = 123.45F,
                Latitude = 67.89F,
                Address = "Sample Address",
                NodeCode = "Sample Code",
                NodeNumberSign = "Sample Sign",
                Note = "Sample Note",
                Status = "Sample Status"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            nodeService.Setup(x => x.Create(sampleData)).ReturnsAsync(new ResponseDTO<bool>(true, "Thêm thành công"));

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Thêm thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Detail_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Detail(nodeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
            // Add additional assertions if necessary
        }

        [Test]
        public async Task Detail_WhenNodeExists_ReturnsNodeListDTO()
        {
            var nodeId = Guid.NewGuid();
            var nodeDTO =
                new NodeListDTO
                {
                    Id = nodeId,
                    NodeCode = "Node001",
                    NodeNumberSign = "Sign001",
                    Address = "123 Example Street",
                    Longitude = 123.456F,
                    Latitude = 78.901F,
                    Status = "Active",
                    RouteId = Guid.NewGuid(),
                    Note = "Sample note",
                    NodeMaterialCategoryListDTOs = new List<NodeMaterialCategoryListDTO>() {
                          new NodeMaterialCategoryListDTO { OtherMaterialsCategoryName = "Material1", Quantity = 5 },
                          new NodeMaterialCategoryListDTO { OtherMaterialsCategoryName = "Material2", Quantity = 10 }
                                  }
                };
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);
            nodeService.Setup(x => x.Detail(nodeId)).ReturnsAsync(new ResponseDTO<NodeListDTO?>(nodeDTO, string.Empty));
            var result = await controller.Detail(nodeId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.Id, Is.EqualTo(nodeDTO.Id));
            Assert.That(result.Data.NodeCode, Is.EqualTo(nodeDTO.NodeCode));
            Assert.That(result.Data.NodeNumberSign, Is.EqualTo(nodeDTO.NodeNumberSign));
            Assert.That(result.Data.Address, Is.EqualTo(nodeDTO.Address));
            Assert.That(result.Data.Longitude, Is.EqualTo(nodeDTO.Longitude));
            Assert.That(result.Data.Latitude, Is.EqualTo(nodeDTO.Latitude));
            Assert.That(result.Data.Status, Is.EqualTo(nodeDTO.Status));
            Assert.That(result.Data.RouteId, Is.EqualTo(nodeDTO.RouteId));
            Assert.That(result.Data.Note, Is.EqualTo(nodeDTO.Note));
            Assert.That(result.Data.NodeMaterialCategoryListDTOs.Count, Is.EqualTo(nodeDTO.NodeMaterialCategoryListDTOs.Count));
        }

        [Test]
        public async Task Detail_WhenNodeNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);
            nodeService.Setup(x => x.Detail(nodeId))
                .ReturnsAsync(new ResponseDTO<NodeListDTO?>(null, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Detail(nodeId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy điểm"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var nodeUpdateDTO = new NodeUpdateDTO();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Update(nodeId, nodeUpdateDTO);

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Update_WhenNodeExists_ReturnsSuccessResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var nodeUpdateDTO = new NodeUpdateDTO
            {
                Longitude = 123.456F,
                Latitude = 78.901F,
                Address = "Updated Example Street",
                NodeCode = "UpdatedNode001",
                NodeNumberSign = "UpdatedSign001",
                Note = "Updated note",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            nodeService.Setup(x => x.Update(nodeId, nodeUpdateDTO)).ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(nodeId, nodeUpdateDTO);

            // Assert
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenNodeNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var nodeUpdateDTO = new NodeUpdateDTO
            {
                Longitude = 123.456F,
                Latitude = 78.901F,
                Address = "Updated Example Street",
                NodeCode = "UpdatedNode001",
                NodeNumberSign = "UpdatedSign001",
                Note = "Updated note",
                Status = "UpdatedStatus"
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            nodeService.Setup(x => x.Update(nodeId, nodeUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(nodeId, nodeUpdateDTO);

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy điểm"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Delete(nodeId);

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenNodeNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            nodeService.Setup(x => x.Delete(nodeId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound));
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Act
            var result = await controller.Delete(nodeId);

            // Assert
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy điểm"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenNodeFound_ReturnsSuccessResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            nodeService.Setup(x => x.Delete(nodeId)).ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Act
            var result = await controller.Delete(nodeId);

            // Assert
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
        }
    }
}
