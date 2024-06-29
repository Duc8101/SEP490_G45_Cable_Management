using API.Services.NodeMaterialCategories;
using Common.Base;
using Common.Const;
using Common.DTO.NodeMaterialCategoryDTO;

namespace UnitTests.Tests
{
    [TestFixture]
    public class NodeMaterialCategoryTest
    {
        private NodeMaterialCategoryController controller;
        private Mock<INodeMaterialCategoryService> nodeMaterialCategoryService;

        [SetUp]
        public void SetUp()
        {
            nodeMaterialCategoryService = new Mock<INodeMaterialCategoryService>();
            controller = new NodeMaterialCategoryController(nodeMaterialCategoryService.Object);
        }

        [Test]
        public async Task Update_WhenNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var sampleData = new NodeMaterialCategoryUpdateDTO { };

            // Simulate a user without admin
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Update(nodeId, sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Update_WhenNodeExists_ReturnsSuccessResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var nodeMaterialCategoryUpdateDTO = new NodeMaterialCategoryUpdateDTO
            {
                MaterialCategoryDTOs = new List<MaterialCategoryDTO>
        {
            new MaterialCategoryDTO { OtherMaterialsCategoryId = 1, Quantity = 10 }
        }
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            // Mocking the scenario where getNode returns a valid node
            nodeMaterialCategoryService.Setup(x => x.Update(nodeId, nodeMaterialCategoryUpdateDTO))
                .ReturnsAsync(new ResponseBase<bool>(true, "Update thành công"));

            // Act
            var result = await controller.Update(nodeId, nodeMaterialCategoryUpdateDTO);

            nodeMaterialCategoryService.Verify(x => x.Update(nodeId, nodeMaterialCategoryUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Update thành công"));
        }



        [Test]
        public async Task Update_WhenNodeNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var nodeMaterialCategoryUpdateDTO = new NodeMaterialCategoryUpdateDTO
            {
                MaterialCategoryDTOs = new List<MaterialCategoryDTO>
        {
            new MaterialCategoryDTO { OtherMaterialsCategoryId = 1, Quantity = 10 }
        }
            };
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            // Mocking the scenario where getNode returns null
            nodeMaterialCategoryService.Setup(x => x.Update(nodeId, nodeMaterialCategoryUpdateDTO))
                .ReturnsAsync(new ResponseBase<bool>(false, "Không tìm thấy điểm", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(nodeId, nodeMaterialCategoryUpdateDTO);

            nodeMaterialCategoryService.Verify(x => x.Update(nodeId, nodeMaterialCategoryUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy điểm"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var nodeId = Guid.NewGuid();
            var nodeMaterialCategoryUpdateDTO = new NodeMaterialCategoryUpdateDTO
            {
                MaterialCategoryDTOs = new List<MaterialCategoryDTO>
        {
            new MaterialCategoryDTO { OtherMaterialsCategoryId = 1, Quantity = 10 }
        }
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);


            // Mocking the scenario where an exception is thrown
            nodeMaterialCategoryService.Setup(x => x.Update(nodeId, nodeMaterialCategoryUpdateDTO))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Update(nodeId, nodeMaterialCategoryUpdateDTO));

        }
    }
}
