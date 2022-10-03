namespace Kerbero.Domain.NukiActions.Models;

public class NukiSmartLockExternalResponseDto
{
	public int SmartLockId { get; set; }
	public int AccountId { get; set; }
	public int Type { get; set; }
	public int LmType { get; set; }
	public int AuthId { get; set; }
	public string Name { get; set; }
	public bool Favourite { get; set; }
	public NukiSmartLockState State { get; set; }
}
