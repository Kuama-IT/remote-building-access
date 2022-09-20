using FluentAssertions;
using Kerbero.Common.Interactors;
using Kerbero.Common.Interfaces;
using Kerbero.Common.Repositories;
using Moq;

namespace Kerbero.Common.Tests.Interactors;

public class ProvideNukiAuthRedirectUrlTest
{
	private readonly ProvideNukiAuthRedirectUrlInteractor _redirectInteractor;
	private readonly Mock<INukiExternalAuthenticationRepository> _nukiExternalRepository;

	public ProvideNukiAuthRedirectUrlTest()
	{
		_nukiExternalRepository = new Mock<INukiExternalAuthenticationRepository>();
		_redirectInteractor = new ProvideNukiAuthRedirectUrlInteractor(_nukiExternalRepository.Object);
	}

	[Fact]
	public void Handle_Success()
	{
		// Arrange
		var uri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
		                  "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
		                  "&redirect_uri=https://test.com/nuki/code/v7kn_NX7vQ7VjQdXFGK43g" +
		                  "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
		_nukiExternalRepository.Setup(c => c.BuildUriForCode(It.IsAny<string>()))
			.Returns(uri);

		// Act
		var redirectUri = _redirectInteractor.Handle("VALID_CLIENT_ID");

		// Assert
		_redirectInteractor.Should().BeAssignableTo<Interactor<string, Uri>>();
		_nukiExternalRepository.Verify(c => c.BuildUriForCode(It.Is<string>(s => 
			s.Equals("VALID_CLIENT_ID"))));
		redirectUri.Should().BeEquivalentTo(uri);
	}
}
