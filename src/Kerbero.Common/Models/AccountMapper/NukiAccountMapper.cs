using Kerbero.Common.Entities;
using Kerbero.Common.Exceptions;

namespace Kerbero.Common.Models.AccountMapper;

public static class NukiAccountMapper
{
	public static NukiAccount MapToEntity(NukiAccountExternalResponseDto nukiAccountExternalResponseDto)
	{
		return new NukiAccount
		{
			Token = nukiAccountExternalResponseDto.Token,
			RefreshToken = nukiAccountExternalResponseDto.RefreshToken,
			TokenExpiringTimeInSeconds = nukiAccountExternalResponseDto.TokenExpiresIn,
			TokenType = nukiAccountExternalResponseDto.TokenType,
			ClientId = nukiAccountExternalResponseDto.ClientId,
			// UTC date?
			ExpiryDate = DateTime.Now.AddSeconds(nukiAccountExternalResponseDto.TokenExpiresIn)
		};
	}
	public static NukiAccountPresentationDto MapToPresentation(NukiAccount nukiAccount)
	{
		return new NukiAccountPresentationDto()
		{
			Id = nukiAccount.Id ?? throw new AccountNotTrackedException(),
			ClientId = nukiAccount.ClientId,
		};
	}
	
}
