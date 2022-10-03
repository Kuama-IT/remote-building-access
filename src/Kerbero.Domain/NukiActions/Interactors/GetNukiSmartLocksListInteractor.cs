using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Models.Mapper;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class
	GetNukiSmartLocksListInteractor : InteractorAsync<NukiAuthenticatedRequestDto,
		NukiSmartLocksListPresentationDto>
{
	private readonly INukiSmartLockExternalRepository _nukiSmartLockClient;

	public GetNukiSmartLocksListInteractor(INukiSmartLockExternalRepository nukiSmartLockClient)
	{
		_nukiSmartLockClient = nukiSmartLockClient;
	}

	public async Task<Result<NukiSmartLocksListPresentationDto>> Handle(
		NukiAuthenticatedRequestDto nukiAuthenticatedRequestDto)
	{

		var smartLockList = await _nukiSmartLockClient.GetNukiSmartLockList(nukiAuthenticatedRequestDto.KerberoAccountId);
		if (smartLockList.IsFailed)
		{
			return Result.Fail(smartLockList.Errors);
		}
		
		var mapped = NukiSmartLockListMapper.Map(smartLockList.Value);
		return Result.Ok(mapped);
	}
}
