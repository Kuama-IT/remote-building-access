using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using Kerbero.Domain.SmartLockKeys.Dtos;
using Kerbero.WebApi;
using Kerbero.WebApi.Models.Requests;

namespace Kerbero.Integration.Tests.SmartLockKeys;

public class CreateSmartLockKeyIntegrationTests: IDisposable
{
	private readonly KerberoWebApplicationFactory<Program> _application;
	private readonly HttpTest _httpTest;

	public CreateSmartLockKeyIntegrationTests()
	{
		_application = new KerberoWebApplicationFactory<Program>();
		_application.Server.PreserveExecutionContext = true; // fixture for Flurl
		_httpTest = new HttpTest();
	}
	
	public void Dispose()
	{
		_httpTest.Dispose();
	}

	[Fact]
	public async Task CreateSmartLockKeyBySmartLockId_ValidParameters()
	{
		var rawResponse = await File.ReadAllTextAsync("JsonData/get-nuki-smartlock-response.json");
		_httpTest.RespondWith(rawResponse);

		var (loggedClient, user) = await _application.CreateUserAndAuthenticateClient();
		
		var tNukiCredential = await _application.CreateNukiCredential(IntegrationTestsUtils.GetNukiCredential(), user.Id);

		var expiryDate = DateTime.Now.AddDays(7).ToUniversalTime();

		var response = await loggedClient.PostAsJsonAsync("/api/smart-lock-keys/",
			new CreateSmartLockKeyRequest("VALID_ID", expiryDate, tNukiCredential.Id, "nuki"));

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		response.IsSuccessStatusCode.Should().BeTrue();
		var smartLockKeyResponse = await response.Content.ReadFromJsonAsync<SmartLockKeyDto>();
		smartLockKeyResponse.Should().NotBeNull();
		smartLockKeyResponse!.CreationDate.Should().BeCloseTo(DateTime.Now.ToUniversalTime(), TimeSpan.FromSeconds(5));
		smartLockKeyResponse.ExpiryDate.Should().Be(expiryDate);
	}
	
	[Fact]
	public async Task CreateSmartLockKeyBySmartLockId_NoCredentialsFound()
	{
		var (loggedClient, _) = await _application.CreateUserAndAuthenticateClient();
		
		var expiryDate = DateTime.Now.AddDays(7);

		var response = await loggedClient.PostAsJsonAsync("/api/smart-lock-keys/",
			new CreateSmartLockKeyRequest("VALID_ID", expiryDate, 0, "nuki"));
		
		response.IsSuccessStatusCode.Should().BeFalse();
		response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
	}
}
