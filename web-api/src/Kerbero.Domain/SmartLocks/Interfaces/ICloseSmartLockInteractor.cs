using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Interfaces;

public interface ICloseSmartLockInteractor
{
  Task<Result> Handle(Guid userId, SmartLockProvider smartLockProvider, string smartLockId, int credentialId);
}
