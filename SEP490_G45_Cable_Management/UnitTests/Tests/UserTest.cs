using DataAccess.DTO.UserDTO;

namespace UnitTests.Tests
{
    [TestFixture]
    internal class UserTests
    {
        private UserController controller;
        private Mock<IUserService> userService;

        [SetUp]
        public void SetUp()
        {
            var _mockServiceProvider = new Mock<IServiceProvider>();
            userService = new Mock<IUserService>();
            controller = new UserController(userService.Object);
        }

        [Test]
        public async Task List_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var filter = "SampleFilter";
            int page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List(filter, page);

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task List_NoPrama_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.List();

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Create_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var userCreateDTO = new UserCreateDTO();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Create(userCreateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Update_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var userID = Guid.NewGuid();
            var userUpdateDTO = new UserUpdateDTO();

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            // Act
            var result = await controller.Update(userID, userUpdateDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenUserIsNotAdmin_ReturnsForbiddenResponse()
        {
            // Arrange
            var userID = Guid.NewGuid();  // Replace with an existing user ID

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_STAFF);

            userService.Setup(x => x.Delete(userID, It.IsAny<Guid>()))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Bạn không có quyền truy cập",
                                                    (int)HttpStatusCode.Forbidden));

            // Act
            var result = await controller.Delete(userID);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task Delete_WhenUserLoginIDNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var userID = Guid.NewGuid();  // Replace with an appropriate user ID

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_ADMIN);

            // Act
            var result = await controller.Delete(userID);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy ID"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ChangePassword_WhenUserIsGuest_ReturnsForbiddenResponse()
        {
            // Arrange
            var changePasswordDTO = new ChangePasswordDTO();

            // Act
            var result = await controller.ChangePassword(changePasswordDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không có quyền truy cập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ChangePassword_WhenEmailIsNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var changePasswordDTO = new ChangePasswordDTO();

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_WAREHOUSE_KEEPER);

            // Act
            var result = await controller.ChangePassword(changePasswordDTO);

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message,
                        Is.EqualTo("Không tìm thấy email. Cần xác minh lại thông tin đăng nhập"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Login_WhenUserNotFound_ReturnsConflictResponse()
        {
            // Arrange
            var loginDTO = new LoginDTO { Username = "sample_username", Password = "sample_password" };

            userService.Setup(x => x.Login(loginDTO))
                .ReturnsAsync(new ResponseDTO<TokenDTO?>(null, "Username or password wrong",
                                                         (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Login(loginDTO);

            userService.Verify(x => x.Login(loginDTO));

            // Assert
            Assert.That(result.Data, Is.Null);
            Assert.That(result.Message, Is.EqualTo("Username or password wrong"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Login_WhenUserFound_ReturnsToken()
        {
            // Arrange
            var loginDTO = new LoginDTO { Username = "sample_username", Password = "sample_password" };
            var expectedToken = new TokenDTO { Access_Token = "sample_access_token", Role = "sample_role" };

            userService.Setup(x => x.Login(loginDTO))
                .ReturnsAsync(new ResponseDTO<TokenDTO?>(expectedToken, string.Empty));

            // Act
            var result = await controller.Login(loginDTO);

            userService.Verify(x => x.Login(loginDTO));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Access_Token, Is.EqualTo(expectedToken.Access_Token));
            Assert.That(result.Data.Role, Is.EqualTo(expectedToken.Role));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Login_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            var loginDTO = new LoginDTO { Username = "sample_username", Password = "sample_password" };
            var errorMessage = "Sample error message";

            userService.Setup(x => x.Login(loginDTO)).ThrowsAsync(new Exception(errorMessage));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Login(loginDTO));
        }

        [Test]
        public async Task Create_WhenFieldsAreEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var userCreateDTO = new UserCreateDTO { FirstName = "", LastName = "", Phone = "" };

            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Create(userCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên người dùng không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(userCreateDTO);

            userService.Verify(x => x.Create(userCreateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên người dùng không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenPhoneFieldIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var userCreateDTO = new UserCreateDTO { FirstName = "John", LastName = "Doe", Phone = "" };
            var expectedMessage = "Số điện thoại không được để trống";
            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Create(userCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, expectedMessage, (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(userCreateDTO);

            userService.Verify(x => x.Create(userCreateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo(expectedMessage));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenUserAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            var userCreateDTO =
                new UserCreateDTO
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123456789",
                    UserName = "johndoe123",
                    Email = "johndoe@example.com"
                };
            var expectedMessage = "Email hoặc username đã được sử dụng";
            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Create(userCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, expectedMessage, (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Create(userCreateDTO);

            userService.Verify(x => x.Create(userCreateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo(expectedMessage));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Create_WhenUserCreationIsSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var userCreateDTO =
                new UserCreateDTO
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123456789",
                    UserName = "johndoe123",
                    Email = "johndoe@example.com"
                };
            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Create(userCreateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Tạo thành công"));

            // Act
            var result = await controller.Create(userCreateDTO);

            userService.Verify(x => x.Create(userCreateDTO));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Tạo thành công"));
        }

        [Test]
        public void Create_WhenExceptionThrown_ReturnsErrorResponse()
        {
            // Arrange
            var userCreateDTO =
                new UserCreateDTO
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Phone = "123456789",
                    UserName = "johndoe123",
                    Email = "johndoe@example.com"
                };
            TestHelper.SimulateUserWithRoleWithoutID(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Create(userCreateDTO))
                .ThrowsAsync(new Exception("An error occurred while creating the user."));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Create(userCreateDTO));
        }

        [Test]
        public async Task ForgotPassword_WhenUserNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "johndoe@example.com" };

            userService.Setup(x => x.ForgotPassword(forgotPasswordDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy email trong hệ thống",
                                                    (int)HttpStatusCode.NotFound));

            // Act & Assert
            var result = await controller.ForgotPassword(forgotPasswordDTO);

            userService.Verify(x => x.ForgotPassword(forgotPasswordDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy email trong hệ thống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ForgotPassword_WhenPasswordResetSucceeds_ReturnsSuccessResponse()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "johndoe@example.com" };

            userService.Setup(x => x.ForgotPassword(forgotPasswordDTO))
                .ReturnsAsync(new ResponseDTO<bool>(
                    true, "Đã đổi mật khẩu thành công. Vui lòng kiểm tra email của bạn"));

            // Act
            var result = await controller.ForgotPassword(forgotPasswordDTO);

            userService.Verify(x => x.ForgotPassword(forgotPasswordDTO));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message,
                        Is.EqualTo("Đã đổi mật khẩu thành công. Vui lòng kiểm tra email của bạn"));
        }

        [Test]
        public void ForgotPassword_WhenUserNotFound_ReturnsException()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "nonexistentuser@example.com" };

            userService.Setup(x => x.ForgotPassword(forgotPasswordDTO))
                .ThrowsAsync(new Exception("An error occurred while update password."));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.ForgotPassword(forgotPasswordDTO));

            userService.Verify(x => x.ForgotPassword(forgotPasswordDTO));
        }

        [Test]
        public async Task ChangePassword_WhenUserNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var changePasswordDTO =
                new ChangePasswordDTO
                {
                    CurrentPassword = "currentPassword",
                    NewPassword = "newPassword",
                    ConfirmPassword = "newPassword"
                };

            var email = TestHelper.SimulateUser(controller, RoleConst.STRING_ROLE_LEADER);

            userService.Setup(x => x.ChangePassword(changePasswordDTO, email))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Không tìm thấy thông tin của bạn",
                                                    (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.ChangePassword(changePasswordDTO);

            userService.Verify(x => x.ChangePassword(changePasswordDTO, email));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy thông tin của bạn"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ChangePassword_WhenCurrentPasswordIsIncorrect_ReturnsConflictResponse()
        {
            // Arrange
            var changePasswordDTO =
                new ChangePasswordDTO
                {
                    CurrentPassword = "incorrectPassword",
                    NewPassword = "newPassword",
                    ConfirmPassword = "newPassword"
                };

            var email = TestHelper.SimulateUser(controller, RoleConst.STRING_ROLE_LEADER);

            userService.Setup(x => x.ChangePassword(changePasswordDTO, email))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Mật khẩu hiện tại không chính xác",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.ChangePassword(changePasswordDTO);

            userService.Verify(x => x.ChangePassword(changePasswordDTO, email));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Mật khẩu hiện tại không chính xác"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task ChangePassword_WhenConfirmPasswordDoesNotMatch_ReturnsConflictResponse()
        {
            // Arrange
            var changePasswordDTO =
                new ChangePasswordDTO
                {
                    CurrentPassword = "currentPassword",
                    NewPassword = "newPassword",
                    ConfirmPassword = "differentPassword"
                };

            var email = TestHelper.SimulateUser(controller, RoleConst.STRING_ROLE_LEADER);

            userService.Setup(x => x.ChangePassword(changePasswordDTO, email))
                .ReturnsAsync(new ResponseDTO<bool>(false,
                                                    "Mật khẩu xác nhận không trùng khớp với mật khẩu mới",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.ChangePassword(changePasswordDTO);

            userService.Verify(x => x.ChangePassword(changePasswordDTO, email));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Mật khẩu xác nhận không trùng khớp với mật khẩu mới"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task ChangePassword_WhenSuccessful_ReturnsSuccessResponse()
        {
            // Arrange
            var changePasswordDTO =
                new ChangePasswordDTO
                {
                    CurrentPassword = "currentPassword",
                    NewPassword = "newPassword",
                    ConfirmPassword = "newPassword"
                };

            var email = TestHelper.SimulateUser(controller, RoleConst.STRING_ROLE_LEADER);

            userService.Setup(x => x.ChangePassword(changePasswordDTO, email))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Đổi mật khẩu thành công"));

            // Act
            var result = await controller.ChangePassword(changePasswordDTO);

            userService.Verify(x => x.ChangePassword(changePasswordDTO, email));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Đổi mật khẩu thành công"));
        }

        [Test]
        public void ChangePassword_WhenExceptionThrown_ReturnsInternalError()
        {
            // Arrange
            var changePasswordDTO =
                new ChangePasswordDTO
                {
                    CurrentPassword = "currentPassword",
                    NewPassword = "newPassword",
                    ConfirmPassword = "newPassword"
                };

            var email = TestHelper.SimulateUser(controller, RoleConst.STRING_ROLE_LEADER);

            userService.Setup(x => x.ChangePassword(changePasswordDTO, email))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.ChangePassword(changePasswordDTO));
        }

        [Test]
        public async Task ListPaged_WhenDataRetrievedSuccessfully_ReturnsPagedResult()
        {
            // Arrange
            var filter = "sampleFilter";
            var page = 1;

            var expectedUsers =
                new List<UserListDTO> { new UserListDTO { UserId = Guid.NewGuid(), UserName = "User1" },
                               new UserListDTO { UserId = Guid.NewGuid(), UserName = "User2" } };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.ListPaged(filter, page))
                .ReturnsAsync(new ResponseDTO<PagedResultDTO<UserListDTO>?>(
                    new PagedResultDTO<UserListDTO>(page, expectedUsers.Count,
                                                    PageSizeConst.MAX_USER_LIST_IN_PAGE, expectedUsers),
                    string.Empty));

            // Act
            var result = await controller.List(filter, page);

            userService.Verify(x => x.ListPaged(filter, page));

            // Assert
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Results, Is.EqualTo(expectedUsers));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ListPaged_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var filter = "sampleFilter";
            var page = 1;

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.ListPaged(filter, page))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act and Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.List(filter, page));
        }

        [Test]
        public async Task Update_WhenUserNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userUpdateDTO =
                new UserUpdateDTO
                {
                    UserName = "sampleUserName",
                    Email = "sampleEmail",
                    FirstName = "sampleFirstName",
                    LastName = "sampleLastName",
                    Phone = "samplePhone",
                    RoleId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Update(userId, userUpdateDTO))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy người dùng", (int)HttpStatusCode.NotFound));

            // Act
            var result = await controller.Update(userId, userUpdateDTO);

            userService.Verify(x => x.Update(userId, userUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy người dùng"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Update_WhenUsernameOrEmailExists_ReturnsConflictResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userUpdateDTO =
                new UserUpdateDTO
                {
                    UserName = "sampleUserName",
                    Email = "sampleEmail",
                    FirstName = "sampleFirstName",
                    LastName = "sampleLastName",
                    Phone = "samplePhone",
                    RoleId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Update(userId, userUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Email hoặc username đã được sử dụng",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(userId, userUpdateDTO);

            userService.Verify(x => x.Update(userId, userUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Email hoặc username đã được sử dụng"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenFirstNameIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userUpdateDTO =
                new UserUpdateDTO
                {
                    UserName = "sampleUserName",
                    Email = "sampleEmail",
                    FirstName = "",
                    LastName = "sampleLastName",
                    Phone = "samplePhone",
                    RoleId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Update(userId, userUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Tên người dùng không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(userId, userUpdateDTO);

            userService.Verify(x => x.Update(userId, userUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Tên người dùng không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenPhoneIsEmpty_ReturnsConflictResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userUpdateDTO = new UserUpdateDTO
            {
                UserName = "sampleUserName",
                Email = "sampleEmail",
                FirstName = "sampleFirstName",
                LastName = "sampleLastName",
                Phone = "",
                RoleId = 1
            };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Update(userId, userUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Số điện thoại không được để trống",
                                                    (int)HttpStatusCode.Conflict));

            // Act
            var result = await controller.Update(userId, userUpdateDTO);

            userService.Verify(x => x.Update(userId, userUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Số điện thoại không được để trống"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.Conflict));
        }

        [Test]
        public async Task Update_WhenPhoneIsNotEmpty_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userUpdateDTO =
                new UserUpdateDTO
                {
                    UserName = "sampleUserName",
                    Email = "sampleEmail",
                    FirstName = "sampleFirstName",
                    LastName = "sampleLastName",
                    Phone = "123456789",
                    RoleId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Update(userId, userUpdateDTO))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Chỉnh sửa thành công"));

            // Act
            var result = await controller.Update(userId, userUpdateDTO);

            userService.Verify(x => x.Update(userId, userUpdateDTO));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Chỉnh sửa thành công"));
        }

        [Test]
        public void Update_WhenExceptionOccurs_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userUpdateDTO =
                new UserUpdateDTO
                {
                    UserName = "sampleUserName",
                    Email = "sampleEmail",
                    FirstName = "sampleFirstName",
                    LastName = "sampleLastName",
                    Phone = "123456789",
                    RoleId = 1
                };

            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Update(userId, userUpdateDTO))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await controller.Update(userId, userUpdateDTO));

            userService.Verify(x => x.Update(userId, userUpdateDTO), Times.Once);
        }

        [Test]
        public async Task Delete_WhenUserNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var userLoginId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            userService.Setup(x => x.Delete(userId, userLoginId))
                .ReturnsAsync(
                    new ResponseDTO<bool>(false, "Không tìm thấy user", (int)HttpStatusCode.NotFound));

            // Act & Assert
            var result = await controller.Delete(userId);

            userService.Verify(x => x.Delete(userId, userLoginId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy user"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Delete_WhenDeletingUserWithUserLoginID_ReturnsNotAcceptableResponse()
        {
            // Arrange
            var userId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);
            var userLoginId = userId;

            // Setting up the expected response for the service method call
            userService.Setup(x => x.Delete(userId, userLoginId))
                .ReturnsAsync(new ResponseDTO<bool>(false, "Bạn không thể xóa tài khoản của mình",
                                                    (int)HttpStatusCode.NotAcceptable));

            // Act
            var result = await controller.Delete(userId);

            // Verifying the service method call
            userService.Verify(x => x.Delete(userId, userLoginId));

            // Assert
            Assert.That(result.Data, Is.False);
            Assert.That(result.Message, Is.EqualTo("Bạn không thể xóa tài khoản của mình"));
            Assert.That(result.Code, Is.EqualTo((int)HttpStatusCode.NotAcceptable));
        }

        [Test]
        public async Task Delete_WhenUserFound_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userLoginId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Setting up the expected response for the service method call
            userService.Setup(x => x.Delete(userId, userLoginId))
                .ReturnsAsync(new ResponseDTO<bool>(true, "Xóa thành công"));

            // Act
            var result = await controller.Delete(userId);

            // Verifying the service method call
            userService.Verify(x => x.Delete(userId, userLoginId));

            // Assert
            Assert.That(result.Data, Is.True);
            Assert.That(result.Message, Is.EqualTo("Xóa thành công"));
        }

        [Test]
        public void Delete_WhenExceptionOccurs_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userLoginId = TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Setting up the expected exception for the service method call
            userService.Setup(x => x.Delete(userId, userLoginId))
                .ThrowsAsync(new Exception("An error occurred while deleting the user"));

            // Act and Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await controller.Delete(userId));
            Assert.That(exception.Message, Does.Contain("An error occurred while deleting the user"));
        }

        [Test]
        public async Task ListWarehouseKeeper_WhenExecuted_ReturnsListOfUsers()
        {
            // Arrange
            var expectedList = new List<UserListDTO> {
    new UserListDTO { UserId = Guid.NewGuid(), UserName = "user1", FirstName = "John",
                      LastName = "Doe", Email = "john@example.com", Phone = "1234567890",
                      RoleName = "Warehouse Keeper" },
    new UserListDTO { UserId = Guid.NewGuid(), UserName = "user2", FirstName = "Jane",
                      LastName = "Doe", Email = "jane@example.com", Phone = "9876543210",
                      RoleName = "Warehouse Keeper" }
   };
            TestHelper.SimulateUserWithRoleAndId(controller, RoleConst.STRING_ROLE_ADMIN);

            // Mocking the list of users returned by the service method
            userService.Setup(x => x.ListWarehouseKeeper())
                .ReturnsAsync(new ResponseDTO<List<UserListDTO>>(expectedList, string.Empty));

            // Act
            var result = await controller.List();

            userService.Verify(s => s.ListWarehouseKeeper());

            // Assert
            Assert.That(result.Data, Is.EqualTo(expectedList));
            Assert.That(result.Message, Is.EqualTo(string.Empty));
        }
    }
}
