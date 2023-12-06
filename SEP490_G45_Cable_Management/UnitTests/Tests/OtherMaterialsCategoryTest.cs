using DataAccess.DTO.OtherMaterialsCategoryDTO;
using Moq;

namespace UnitTests.Tests
{
    [TestFixture]
    public class OtherMaterialsCategoryTest
    {
        private OtherMaterialsCategoryController controller;
        private Mock<IOtherMaterialsCategoryService> otherMaterialsCategoryService;

        [SetUp]
        public void SetUp()
        {
            otherMaterialsCategoryService = new Mock<IOtherMaterialsCategoryService>();
            controller = new OtherMaterialsCategoryController(otherMaterialsCategoryService.Object);
        }

        [Test]
        public async Task List_WhenNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);
            var page = 1;

            // Act
            var result = await controller.List(null, page);

            // Assert
            Assert.IsFalse(result.Data != null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ListPaged_ReturnsPagedResultDTO()
        {
            // Arrange
            var expectedList = new List<OtherMaterialsCategoryListDTO> { new OtherMaterialsCategoryListDTO {
                                                                OtherMaterialsCategoryId = 1,
                                                               },
                                                                new OtherMaterialsCategoryListDTO {
                                                                 OtherMaterialsCategoryId = 2,
                                                                } };
            var expectedRowCount = 10;
            var expectedPage = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.ListPaged(null, expectedPage))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<OtherMaterialsCategoryListDTO>>(
                    new PagedResultDTO<OtherMaterialsCategoryListDTO>(
                        expectedPage, expectedRowCount, PageSizeConst.MAX_OTHER_MATERIAL_CATEGORY_LIST_IN_PAGE, expectedList),
                    string.Empty));

            // Act
            var result = await controller.List(null, expectedPage);

            // Assert
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.RowCount, Is.EqualTo(expectedRowCount));
            Assert.That(result.Data.CurrentPage, Is.EqualTo(expectedPage));
        }

        [Test]
        public async Task ListAll_WhenCalled_ReturnsListOfOtherMaterialsCategories()
        {
            // Arrange
            var expectedList = new List<OtherMaterialsCategoryListDTO> { new OtherMaterialsCategoryListDTO {
                                                                OtherMaterialsCategoryId = 1,
                                                               },
                                                                new OtherMaterialsCategoryListDTO {
                                                                 OtherMaterialsCategoryId = 2,
                                                                } };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.ListAll())
                .ReturnsAsync(new ResponseDTO<List<OtherMaterialsCategoryListDTO>?>(expectedList, string.Empty));

            // Act
            var result = await controller.List();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.That(result.Data.Count, Is.EqualTo(expectedList.Count));
        }

        [Test]
        public async Task Create_WhenNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.Create(sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO
            {
                OtherMaterialsCategoryName = ""

            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.Create(sampleDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tên vật liệu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenNameExists_ReturnsConflictResponse()
        {
            // Arrange
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "SampleName" };

            otherMaterialsCategoryService.Setup(x => x.Create(sampleDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict));
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Act
            var result = await controller.Create(sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Loại vật liệu này đã tồn tại"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenNameIsValid_CreatesCategorySuccessfully()
        {
            // Arrange
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "SampleName" };

            otherMaterialsCategoryService.Setup(x => x.Create(sampleDTO)).ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Act
            var result = await controller.Create(sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Update_WhenUserNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            var categoryId = 1;
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "SampleName" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Update(categoryId, sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Update_WhenCategoryNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var categoryId = 1;
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "SampleName" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.Update(categoryId, sampleDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy loại vật liệu này", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(categoryId, sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy loại vật liệu này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenNameEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var categoryId = 1;
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.Update(categoryId, sampleDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên vật liệu không được để trống", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(categoryId, sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Tên vật liệu không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenCategoryAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            var categoryId = 1;
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "Chống rung 1350 (L1 1300mm)" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.Update(categoryId, sampleDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Loại vật liệu này đã tồn tại", (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(categoryId, sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Data);
            Assert.That(result.Message, Is.EqualTo("Loại vật liệu này đã tồn tại"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenCategoryUpdated_ReturnsSuccessResponse()
        {
            // Arrange
            var categoryId = 1;
            var sampleDTO = new OtherMaterialsCategoryCreateUpdateDTO { OtherMaterialsCategoryName = "SampleName" };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            otherMaterialsCategoryService.Setup(x => x.Update(categoryId, sampleDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(categoryId, sampleDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
        }
    }
}
