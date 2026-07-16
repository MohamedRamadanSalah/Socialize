using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Socialize.Application.Auth.Commands.Login;
using Socialize.Application.Auth.Commands.Logout;
using Socialize.Application.Auth.Commands.Refresh;
using Socialize.Application.Auth.Commands.Register;
using Socialize.Application.Common.Exceptions;
using Socialize.Application.Tests.TestSupport;
using Socialize.Infrastructure.Persistence;
using Xunit;

namespace Socialize.Application.Tests.Auth;

public class AuthHandlerTests
{
    private static (AppDbContext Db, FakeTokenService TokenService, FakeJwtService JwtService) CreateSut()
    {
        return (TestDbContextFactory.Create(), new FakeTokenService(), new FakeJwtService());
    }

    [Fact]
    public async Task Register_CreatesUser_AndReturnsTokens()
    {
        var (db, tokenService, jwtService) = CreateSut();
        var handler = new RegisterCommandHandler(db, tokenService, jwtService);

        var result = await handler.Handle(new RegisterCommand("alice", "alice@example.com", "P@ssw0rd123", "Alice A"), default);

        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.UserName.Should().Be("alice");
        (await db.Users.CountAsync()).Should().Be(1);
        (await db.RefreshTokens.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ThrowsConflict()
    {
        var (db, tokenService, jwtService) = CreateSut();
        var handler = new RegisterCommandHandler(db, tokenService, jwtService);
        await handler.Handle(new RegisterCommand("alice", "dup@example.com", "P@ssw0rd123", "Alice A"), default);

        var act = () => handler.Handle(new RegisterCommand("alice2", "dup@example.com", "P@ssw0rd123", "Alice B"), default);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorized()
    {
        var (db, tokenService, jwtService) = CreateSut();
        var registerHandler = new RegisterCommandHandler(db, tokenService, jwtService);
        await registerHandler.Handle(new RegisterCommand("bob", "bob@example.com", "P@ssw0rd123", "Bob B"), default);

        var loginHandler = new LoginCommandHandler(db, tokenService, jwtService);
        var act = () => loginHandler.Handle(new LoginCommand("bob", "wrong-password"), default);

        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndRevokesPrevious()
    {
        var (db, tokenService, jwtService) = CreateSut();
        var registerHandler = new RegisterCommandHandler(db, tokenService, jwtService);
        var registerResult = await registerHandler.Handle(new RegisterCommand("carol", "carol@example.com", "P@ssw0rd123", "Carol C"), default);

        var refreshHandler = new RefreshCommandHandler(db, tokenService, jwtService);
        var refreshResult = await refreshHandler.Handle(new RefreshCommand(registerResult.RefreshToken), default);

        refreshResult.RefreshToken.Should().NotBe(registerResult.RefreshToken);

        var act = () => refreshHandler.Handle(new RefreshCommand(registerResult.RefreshToken), default);
        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken()
    {
        var (db, tokenService, jwtService) = CreateSut();
        var registerHandler = new RegisterCommandHandler(db, tokenService, jwtService);
        var registerResult = await registerHandler.Handle(new RegisterCommand("dave", "dave@example.com", "P@ssw0rd123", "Dave D"), default);

        var logoutHandler = new LogoutCommandHandler(db, tokenService);
        await logoutHandler.Handle(new LogoutCommand(registerResult.RefreshToken), default);

        var refreshHandler = new RefreshCommandHandler(db, tokenService, jwtService);
        var act = () => refreshHandler.Handle(new RefreshCommand(registerResult.RefreshToken), default);
        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }
}
