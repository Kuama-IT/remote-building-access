using FluentAssertions;
using FluentResults;
using Kerbero.Common.Entities;
using Kerbero.Common.Errors;
using Kerbero.Common.Errors.CreateNukiAccountErrors;
using Kerbero.Common.Interactors;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;
using Moq;

namespace Kerbero.Common.Tests.Interactors;

public class CreateNukiAccountInteractorTests
{
	private readonly Mock<INukiPersistentAccountRepository> _repository;
	private readonly CreateNukiAccountInteractor _interactor;
	private readonly Mock<INukiExternalAuthenticationRepository> _nukiClient;

	public CreateNukiAccountInteractorTests()
	{
		_nukiClient = new Mock<INukiExternalAuthenticationRepository>();
		_repository = new Mock<INukiPersistentAccountRepository>();
		_interactor = new CreateNukiAccountInteractor(_repository.Object, _nukiClient.Object);
	}

	// Handle should create an entity from an input DTO and upload it into the DB.
	[Fact]
	public async Task Handle_ReturnASuccessfulResponse_Test()
	{
		// Arrange
		var nukiAccountDto = new NukiAccountExternalResponseDto
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 2592000,
		};
		var nukiAccountEntity = new NukiAccount
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
			It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(() => Task.FromResult(Result.Ok(nukiAccountDto)));
		_repository.Setup(c => 
			c.Create(It.IsAny<NukiAccount>())).Returns(
			async () => { nukiAccountEntity.Id = 1; return await Task.FromResult(Result.Ok(nukiAccountEntity)); });
		
		// Act
		var nukiAccountPresentationDto = await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE"});
		
		// Assert
		_nukiClient.Verify(c => c.GetNukiAccount(
			It.Is<NukiAccountExternalRequestDto>(s => 
				s.ClientId.Contains("VALID_CLIENT_ID") &&
				s.Code.Contains("VALID_CODE"))));
		_repository.Verify(c => c
			.Create(It.Is<NukiAccount>(account => 
				account.Token == nukiAccountEntity.Token &&
				account.RefreshToken == nukiAccountEntity.RefreshToken &&
				account.TokenExpiringTimeInSeconds == nukiAccountEntity.TokenExpiringTimeInSeconds &&
				account.TokenType == nukiAccountEntity.TokenType &&
				account.ClientId == nukiAccountEntity.ClientId)));
		nukiAccountPresentationDto.Should().BeOfType<Result<NukiAccountPresentationDto>>();
		nukiAccountPresentationDto.Value.Should().BeEquivalentTo(new NukiAccountPresentationDto()
		{
			Id = 1,
			ClientId = "VALID_CLIENT_ID"
		});
	}

	[Theory]
	[MemberData(nameof(ExternalErrorToTest))]
	public async Task Handle_ExternalReturnAnError_Test(KerberoError error)
	{
		// Arrange
		
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(async () => await Task.FromResult(Result.Fail(error)));

		// Act 
		var ex = await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });
		// Assert
		ex.IsFailed.Should().BeTrue();
		ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
	}	
	public static IEnumerable<object[]> ExternalErrorToTest =>
		new List<object[]>
		{
			new object[] { new ExternalServiceUnreachableError()},
			new object[] { new UnableToParseResponseError()},
			new object[] { new UnauthorizedAccessError()},
			new object[] { new KerberoError()},
			new object[] { new InvalidParametersError("VALID_CLIENT_ID") }
		};

	[Theory]
	[MemberData(nameof(PersistentErrorToTest))]
	public async Task Handle_PersistentReturnAnError_Test(KerberoError error)
	{
		// Arrange
		var nukiAccountDto = new NukiAccountExternalResponseDto
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 2592000,
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(async () => await Task.FromResult(Result.Ok(nukiAccountDto)));
		_repository.Setup(c => c.Create(It.IsAny<NukiAccount>()))
			.Returns(async () => await Task.FromResult(Result.Fail(error)));

		// Act 
		var ex = await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });
		// Assert
		ex.IsFailed.Should().BeTrue();
		ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
	}	
	public static IEnumerable<object[]> PersistentErrorToTest =>
		new List<object[]>
		{
			new object[] { new DuplicateEntryError("Nuki account")},
			new object[] { new PersistentResourceNotAvailableError()}
		};

}
