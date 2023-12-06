using DataAccess.DTO.StatisticDTO;
using DataAccess.Entity;

namespace UnitTests.Tests
{
    [TestFixture]
    public class StatisticTest
    {
        private StatisticController controller;
        private Mock<IStatisticService> statisticService;

        [SetUp]
        public void SetUp()
        {
            statisticService = new Mock<IStatisticService>();
            controller = new StatisticController(statisticService.Object);
        }

        [Test]
        public async Task
        MaterialFluctuationPerYear_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            int? materialCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_STAFF);  // Simulate a regular employee

            // Act
            var result = await controller.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CableFluctuationPerYear_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            int? cableCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_STAFF);  // Simulate a regular employee

            // Act
            var result = await controller.CableFluctuationPerYear(cableCategoryID, warehouseID, year);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task CableCategory_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            int? warehouseID = 1;

            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_STAFF);  // Simulate a regular employee

            // Act
            var result = await controller.CableCategory(warehouseID);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task MaterialCategory_WhenUserIsNotAdminOrLeader_ReturnsForbiddenResponse()
        {
            // Arrange
            int? warehouseID = 1;

            TestHelper.SimulateUserWithRoleAndId(
                controller, RoleConst.STRING_ROLE_STAFF);  // Simulate a regular employee

            // Act
            var result = await controller.MaterialCategory(warehouseID);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task MaterialFluctuationPerYear_WhenCategoryIsChosen_Admin_ReturnsData()
        {
            // Arrange
            int? materialCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            // Define a sample data object that the method should return
            var expectedData = new MaterialFluctuationPerYear
            {
                MaterialName = "SampleMaterialName",
                WarehouseId = warehouseID,
            };

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_ADMIN);  // Simulate an admin

            statisticService.Setup(x => x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year))
                .ReturnsAsync(new ResponseDTO<MaterialFluctuationPerYear?>(expectedData, ""));

            // Act
            var result = await controller.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year);

            statisticService.Verify(x =>
                                        x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo(""));
        }
        [Test]
        public async Task MaterialFluctuationPerYear_WhenCategoryIsChosen_Warehousekeeper_ReturnsData()
        {
            // Arrange
            int? materialCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            // Define a sample data object that the method should return
            var expectedData = new MaterialFluctuationPerYear
            {
                MaterialName = "SampleMaterialName",
                WarehouseId = warehouseID,
            };

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulate an admin

            statisticService.Setup(x => x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year))
                .ReturnsAsync(new ResponseDTO<MaterialFluctuationPerYear?>(expectedData, ""));

            // Act
            var result = await controller.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year);

            statisticService.Verify(x =>
                                        x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo(""));
        }

        [Test]
        public async Task MaterialFluctuationPerYear_WhenCategoryIsChosen_Leader_ReturnsData()
        {
            // Arrange
            int? materialCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            // Define a sample data object that the method should return
            var expectedData = new MaterialFluctuationPerYear
            {
                MaterialName = "SampleMaterialName",
                WarehouseId = warehouseID,
            };

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_LEADER);  // Simulate an admin

            statisticService.Setup(x => x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year))
                .ReturnsAsync(new ResponseDTO<MaterialFluctuationPerYear?>(expectedData, ""));

            // Act
            var result = await controller.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year);

            statisticService.Verify(x =>
                                        x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo(""));
        }
        [Test]
        public void MaterialFluctuationPerYear_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            int? materialCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_ADMIN);  // Simulate an admin

            statisticService.Setup(x => x.MaterialFluctuationPerYear(materialCategoryID, warehouseID, year))
                .ThrowsAsync(new Exception("Sample exception message"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.MaterialFluctuationPerYear(
                                              materialCategoryID, warehouseID, year));
        }

        [Test]
        public async Task CableFluctuationPerYear_WhenCableCategoryNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            int? cableCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_ADMIN);  // Simulate an admin

            statisticService.Setup(x => x.CableFluctuationPerYear(cableCategoryID, warehouseID, year))
                .ReturnsAsync(new ResponseDTO<CableFluctuationPerYear?>(null, "Không tìm thấy cáp",
                                                                        (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.CableFluctuationPerYear(cableCategoryID, warehouseID, year);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy cáp"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }


        [Test]
        public async Task CableFluctuationPerYear_WhenCableCategoryIsFound_Warehousekeeper_ReturnsFluctuationData()
        {
            // Arrange
            int? cableCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            // Create a sample CableFluctuationPerYear object for the expected result
            var expectedData = new CableFluctuationPerYear
            {
                CableName = "SampleCableName",
                WarehouseId = warehouseID,
                LengthInJanuary = 10,
            };

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);  // Simulate an admin

            statisticService.Setup(x => x.CableFluctuationPerYear(cableCategoryID, warehouseID, year))
                .ReturnsAsync(new ResponseDTO<CableFluctuationPerYear?>(expectedData, string.Empty));

            // Act
            var result = await controller.CableFluctuationPerYear(cableCategoryID, warehouseID, year);

            statisticService.Verify(s => s.CableFluctuationPerYear(cableCategoryID, warehouseID, year));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            // Add assertions for each property in the CableFluctuationPerYear object
            Assert.That(result.Data.CableName, Is.EqualTo(expectedData.CableName));
            Assert.That(result.Data.WarehouseId, Is.EqualTo(expectedData.WarehouseId));
            Assert.That(result.Data.LengthInJanuary, Is.EqualTo(expectedData.LengthInJanuary));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task CableFluctuationPerYear_WhenCableCategoryIsFound_Leader_ReturnsFluctuationData()
        {
            // Arrange
            int? cableCategoryID = 1;
            int? warehouseID = 1;
            int? year = 2023;

            // Create a sample CableFluctuationPerYear object for the expected result
            var expectedData = new CableFluctuationPerYear
            {
                CableName = "SampleCableName",
                WarehouseId = warehouseID,
                LengthInJanuary = 10,
            };

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_LEADER);  // Simulate an admin

            statisticService.Setup(x => x.CableFluctuationPerYear(cableCategoryID, warehouseID, year))
                .ReturnsAsync(new ResponseDTO<CableFluctuationPerYear?>(expectedData, string.Empty));

            // Act
            var result = await controller.CableFluctuationPerYear(cableCategoryID, warehouseID, year);

            statisticService.Verify(s => s.CableFluctuationPerYear(cableCategoryID, warehouseID, year));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            // Add assertions for each property in the CableFluctuationPerYear object
            Assert.That(result.Data.CableName, Is.EqualTo(expectedData.CableName));
            Assert.That(result.Data.WarehouseId, Is.EqualTo(expectedData.WarehouseId));
            Assert.That(result.Data.LengthInJanuary, Is.EqualTo(expectedData.LengthInJanuary));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }



        [Test]
        public void CableFluctuationPerYear_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            int? cableCategoryID = null;
            int? warehouseID = null;
            int? year = null;

            TestHelper.SimulateUserWithRoleAndId(controller,
                                                 RoleConst.STRING_ROLE_ADMIN);  // Simulate an admin

            statisticService.Setup(x => x.CableFluctuationPerYear(cableCategoryID, warehouseID, year))
                .ThrowsAsync(new Exception("Sample exception message"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(
                async () => await controller.CableFluctuationPerYear(cableCategoryID, warehouseID, year));
        }

        [Test]
        public async Task
        CableCategory_WhenCableCategoriesAreRetrievedSuccessfully_Admin_ReturnsListOfCableCategoryStatistics()
        {
            // Arrange
            int? warehouseID = null;

            // Define a sample list of cable categories that the method should return
            var expectedCableCategories = new List<CableCategoryStatistic> {
    new CableCategoryStatistic { CableCategoryId = 1, CableCategoryName = "Category1",
                                 SumOfLength = 100 },
    new CableCategoryStatistic { CableCategoryId = 2, CableCategoryName = "Category2",
                                 SumOfLength = 150 }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            statisticService.Setup(x => x.CableCategory(warehouseID))
                .ReturnsAsync(
                    new ResponseDTO<List<CableCategoryStatistic>?>(expectedCableCategories, string.Empty));

            // Act
            var result = await controller.CableCategory(warehouseID);

            statisticService.Verify(x => x.CableCategory(warehouseID));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedCableCategories.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public async Task
            CableCategory_WhenCableCategoriesAreRetrievedSuccessfully_Warehousekeeper_ReturnsListOfCableCategoryStatistics()
        {
            // Arrange
            int? warehouseID = null;

            // Define a sample list of cable categories that the method should return
            var expectedCableCategories = new List<CableCategoryStatistic> {
    new CableCategoryStatistic { CableCategoryId = 1, CableCategoryName = "Category1",
                                 SumOfLength = 100 },
    new CableCategoryStatistic { CableCategoryId = 2, CableCategoryName = "Category2",
                                 SumOfLength = 150 }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            statisticService.Setup(x => x.CableCategory(warehouseID))
                .ReturnsAsync(
                    new ResponseDTO<List<CableCategoryStatistic>?>(expectedCableCategories, string.Empty));

            // Act
            var result = await controller.CableCategory(warehouseID);

            statisticService.Verify(x => x.CableCategory(warehouseID));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedCableCategories.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public async Task
    CableCategory_WhenCableCategoriesAreRetrievedSuccessfully_Leader_ReturnsListOfCableCategoryStatistics()
        {
            // Arrange
            int? warehouseID = null;

            // Define a sample list of cable categories that the method should return
            var expectedCableCategories = new List<CableCategoryStatistic> {
    new CableCategoryStatistic { CableCategoryId = 1, CableCategoryName = "Category1",
                                 SumOfLength = 100 },
    new CableCategoryStatistic { CableCategoryId = 2, CableCategoryName = "Category2",
                                 SumOfLength = 150 }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            statisticService.Setup(x => x.CableCategory(warehouseID))
                .ReturnsAsync(
                    new ResponseDTO<List<CableCategoryStatistic>?>(expectedCableCategories, string.Empty));

            // Act
            var result = await controller.CableCategory(warehouseID);

            statisticService.Verify(x => x.CableCategory(warehouseID));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedCableCategories.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }


        [Test]
        public void CableCategory_WhenExceptionIsThrown_ReturnsErrorResponse()
        {
            // Arrange
            int? warehouseID = null;  // Replace with a valid warehouse ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            statisticService.Setup(x => x.CableCategory(warehouseID))
                .ThrowsAsync(new Exception("Sample exception message"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.CableCategory(warehouseID));
        }

        [Test]
        public async Task
        MaterialCategory_WhenValidWarehouseID_Admin_ReturnsListOfMaterialCategoryStatistics()
        {
            // Arrange
            int? warehouseID = null;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            var expectedStatistics = new List<OtherMaterialCategoryStatistic> {
    new OtherMaterialCategoryStatistic { CategoryId = 1, CategoryName = "Category1",
        SumOfQuantity = 10 },
    new OtherMaterialCategoryStatistic { CategoryId = 2, CategoryName = "Category2",
                                          SumOfQuantity = 15 }
   };

            statisticService.Setup(x => x.MaterialCategory(warehouseID))
                .ReturnsAsync(new ResponseDTO<List<OtherMaterialCategoryStatistic>?>(expectedStatistics,
                                                                                      string.Empty));

            // Act
            var result = await controller.MaterialCategory(warehouseID);

            statisticService.Verify(x => x.MaterialCategory(warehouseID));
            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedStatistics.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public async Task
        MaterialCategory_WhenValidWarehouseID_Warehousekeeper_ReturnsListOfMaterialCategoryStatistics()
        {
            // Arrange
            int? warehouseID = null;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            var expectedStatistics = new List<OtherMaterialCategoryStatistic> {
    new OtherMaterialCategoryStatistic { CategoryId = 1, CategoryName = "Category1",
        SumOfQuantity = 10 },
    new OtherMaterialCategoryStatistic { CategoryId = 2, CategoryName = "Category2",
                                          SumOfQuantity = 15 }
   };

            statisticService.Setup(x => x.MaterialCategory(warehouseID))
                .ReturnsAsync(new ResponseDTO<List<OtherMaterialCategoryStatistic>?>(expectedStatistics,
                                                                                      string.Empty));

            // Act
            var result = await controller.MaterialCategory(warehouseID);

            statisticService.Verify(x => x.MaterialCategory(warehouseID));
            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedStatistics.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public async Task
    MaterialCategory_WhenValidWarehouseID_Leader_ReturnsListOfMaterialCategoryStatistics()
        {
            // Arrange
            int? warehouseID = null;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_LEADER);

            var expectedStatistics = new List<OtherMaterialCategoryStatistic> {
    new OtherMaterialCategoryStatistic { CategoryId = 1, CategoryName = "Category1",
        SumOfQuantity = 10 },
    new OtherMaterialCategoryStatistic { CategoryId = 2, CategoryName = "Category2",
                                          SumOfQuantity = 15 }
   };

            statisticService.Setup(x => x.MaterialCategory(warehouseID))
                .ReturnsAsync(new ResponseDTO<List<OtherMaterialCategoryStatistic>?>(expectedStatistics,
                                                                                      string.Empty));

            // Act
            var result = await controller.MaterialCategory(warehouseID);

            statisticService.Verify(x => x.MaterialCategory(warehouseID));
            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Count, Is.EqualTo(expectedStatistics.Count));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
        [Test]
        public void MaterialCategory_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            int? warehouseID = null;  // Replace with a valid warehouse ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            statisticService.Setup(x => x.MaterialCategory(warehouseID))
                .ThrowsAsync(new Exception("Sample exception message"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.MaterialCategory(warehouseID));
        }

        [Test]
        public async Task Route_WhenRouteNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var routeId = Guid.NewGuid();

            statisticService.Setup(x => x.Route(routeId))
                            .ReturnsAsync(new ResponseDTO<List<RouteStatistic>?>(null, "Không tìm thấy tuyến", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Route(routeId);
            statisticService.Verify(x => x.Route(routeId));

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy tuyến"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Route_WhenRouteFound_ReturnsSuccessResponse()
        {
            // Arrange
            var routeId = Guid.NewGuid();

            // Create a sample list of RouteStatistic
            var routeStatistics = new List<RouteStatistic>
    {
        new RouteStatistic {  },

    };

            statisticService.Setup(x => x.Route(routeId))
                            .ReturnsAsync(new ResponseDTO<List<RouteStatistic>?>(routeStatistics, string.Empty));

            // Act
            var result = await controller.Route(routeId);

            statisticService.Verify(x => x.Route(routeId));

            // Assert
            Assert.That(result.Data, Is.EqualTo(routeStatistics));
            Assert.That(result.Message, Is.Empty);
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task Route_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var routeId = Guid.NewGuid();
            var expectedExceptionMessage = "Simulated exception message";

            statisticService.Setup(x => x.Route(routeId))
                            .ThrowsAsync(new Exception(expectedExceptionMessage));

            // Act

            Assert.ThrowsAsync<Exception>(async () => await controller.Route(routeId));


        }

    }
}
