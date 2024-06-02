using DataAccess.DTO.TransactionDTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Tests
{
    [TestFixture]
    internal class TransactionTest
    {
        private Mock<ITransactionService> transactionService;
        private TransactionController controller;

        [SetUp]
        public void SetUp()
        {
            transactionService = new Mock<ITransactionService>();

            controller = new TransactionController(transactionService.Object);
        }

        [Test]
        public async Task List_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var filter = "SampleFilter";
            var warehouseID = 123;
            var page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.List(filter, warehouseID, page);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Detail_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var transactionID = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_STAFF);

            // Act
            var result = await controller.Detail(transactionID);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập trang này"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task List_WhenTransactionHistoryRetrievedSuccessfully_ReturnsListOfTransactions()
        {
            // Arrange
            var filter = "SampleFilter";
            var warehouseId = 1;
            var page = 1;

            // Define a sample list of transaction history that the method should return
            var expectedTransactionHistory = new List<TransactionHistoryDTO> {
    new TransactionHistoryDTO { TransactionId = Guid.NewGuid() },
    new TransactionHistoryDTO { TransactionId = Guid.NewGuid() }
   };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            transactionService.Setup(x => x.List(filter, warehouseId, page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<TransactionHistoryDTO>?>(
                    new PagedResultDTO<TransactionHistoryDTO>(page, expectedTransactionHistory.Count,
                                                              PageSizeConst.MAX_TRANSACTION_LIST_IN_PAGE,
                                                              expectedTransactionHistory),
                    string.Empty));

            // Act
            var result = await controller.List(filter, warehouseId, page);

            transactionService.Verify(x => x.List(filter, warehouseId, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedTransactionHistory));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void List_WhenExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            var filter = "SampleFilter";
            var warehouseId = 1;
            var page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            transactionService.Setup(x => x.List(filter, warehouseId, page))
                .ThrowsAsync(new Exception("Sample exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.List(filter, warehouseId, page));
        }

        [Test]
        public async Task Detail_WhenTransactionHistoryNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            transactionService.Setup(x => x.Detail(transactionId))
                .ReturnsAsync(new ResponseDTO<TransactionDetailDTO?>(null, "Không tìm thấy giao dịch",
                                                                     (int)HttpStatusCode.NotFound));

            // Act & Assert
            var result = await controller.Detail(transactionId);

            transactionService.Verify(x => x.Detail(transactionId));

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy giao dịch"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Detail_WhenTransactionHistoryFound_ReturnsTransactionDetail()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var expectedTransactionDetail =
                new TransactionDetailDTO
                {
                    TransactionId = transactionId,
                    TransactionCategoryName = "SampleCategory",
                    Description = "SampleDescription",
                    CreatedAt = DateTime.Now,
                    WarehouseId = 1,
                    IssueCode = "SampleIssueCode",
                    FromWarehouseName = "SampleFromWarehouse",
                    ToWarehouseName = "SampleToWarehouse",
                    CreatedDate = DateTime.Now,
                    CableTransactions = new List<TransactionCableDTO>(),
                    MaterialsTransaction = new List<TransactionMaterialDTO>()
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            transactionService.Setup(x => x.Detail(transactionId))
                .ReturnsAsync(
                    new ResponseDTO<TransactionDetailDTO?>(expectedTransactionDetail, string.Empty));

            // Act
            var result = await controller.Detail(transactionId);

            transactionService.Verify(x => x.Detail(transactionId));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.EqualTo(expectedTransactionDetail));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Detail_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.ROLE_ADMIN);

            transactionService.Setup(x => x.Detail(transactionId))
                .ThrowsAsync(new Exception("Sample exception"));

            // Act and Assert
            Exception exception =
                Assert.ThrowsAsync<Exception>(async () => await controller.Detail(transactionId));

            // Assert
            Assert.That(exception.Message, Does.Contain("Sample exception"));
        }
    }
}
