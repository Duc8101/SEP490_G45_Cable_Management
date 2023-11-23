using DataAccess.DTO.NodeMaterialCategoryDTO;

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

/*        [Test]
        public async Task Create_WhenNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var sampleData = new NodeMaterialCategoryUpdateDTO { };

            // Simulate a user without admin or leader role
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Create(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }
*/
/*        [Test]
        public async Task Create_ReturnsSuccessResponse()
        {
            // Arrange
            var sampleData = new NodeMaterialCategoryUpdateDTO { };

            var materialCategoryDTOs = new List<MaterialCategoryDTO> { };

            sampleData.MaterialCategoryDTOs = materialCategoryDTOs;

            // Simulate a user with admin or leader role
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Setup the necessary mocks
            nodeMaterialCategoryService.Setup(x => x.Update(sampleData))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Update thành công"));
            // Act
            var result = await controller.Update(sampleData);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Update thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }
*/    }
}
