using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Domain;
using Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using Application;
using Microsoft.EntityFrameworkCore;
using Application.Users;

namespace Tests
{
    public class UserHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMediator _mediator;
        public UserHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UserTestDB")
                .Options;

            _context = new ApplicationDbContext(options);

            var mediatorMock = new Mock<IMediator>();
            _passwordHasher = new PasswordHasher<User>();

            /*mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByUserName.Query>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserByUserName.Query query, CancellationToken _) =>
                {
                    return _context.Users.FirstOrDefault(u => u.UserName == query.UserName);
                });*/

            _mediator = mediatorMock.Object;

        }

        [Fact]
        public async Task RegisterUser_ShouldRegisterUser()
        {
            //Arrange
            var OldUsers = _context.Users;
            _context.RemoveRange(OldUsers);
            await _context.SaveChangesAsync();
            var handler = new RegisterUser.Handler(_context, _mediator, _passwordHasher);
            var command = new RegisterUser.Command { UserName = "username1", Password = "password1" };
            var command2 = new RegisterUser.Command { UserName = "username2", Password = "password2" };

            //Act
            await handler.Handle(command, CancellationToken.None);
            await handler.Handle(command2, CancellationToken.None);

            //Assert
            var users = await _context.Users
                .ToListAsync(CancellationToken.None);

            Assert.True(users.Count() == 2);
            Assert.True(users[0].UserName == command.UserName);
            Assert.True(users[1].UserName == command2.UserName);
        }

        [Fact]
        public async Task LoginUser_ShouldLoginUser()
        {
            //Arrange
            var OldUsers = _context.Users;
            _context.RemoveRange(OldUsers);
            await _context.SaveChangesAsync();

            var RegisterHandler = new RegisterUser.Handler(_context, _mediator, _passwordHasher);
            var RegisterCommand1 = new RegisterUser.Command { UserName = "username1", Password = "password1" };
            var RegisterCommand2 = new RegisterUser.Command { UserName = "username2", Password = "password2" };

            await RegisterHandler.Handle(RegisterCommand1, CancellationToken.None);
            await RegisterHandler.Handle(RegisterCommand2, CancellationToken.None);

            var RegisteredUsers = await _context.Users
                .ToListAsync(CancellationToken.None);

            var LoginHandler = new LoginUser.Handler(_context, _mediator, _passwordHasher);
            var LoginCommand1 = new LoginUser.Command { UserName = "username1", Password = "password1" };
            var LoginCommand2 = new LoginUser.Command { UserName = "username2", Password = "password2" };

            //Act
            var LoginResult1 = await LoginHandler.Handle(LoginCommand1, CancellationToken.None);
            var LoginResult2 = await LoginHandler.Handle(LoginCommand2, CancellationToken.None);

            //Assert
            Assert.True(LoginResult1);
            Assert.True(LoginResult2);

        }
    }
}