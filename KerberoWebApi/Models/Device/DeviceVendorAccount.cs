using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models.Device;

// DeviceVendorAccount derive from DeviceVendor class, add the information of an Host account
public class DeviceVendorAccount
{
  [Key]
  public int Id { get; set; }
  public string? Token { get; set; }
  public string? RefreshToken { get; set; }
  public string ClientId { get; set; } = null!;
  // TODO verify it is necessary, or the same of token 
  public string? ApiKey { get; set; }

  // Db Foreign keys
  public int HostId { get; set; }
  public Host Host { get; set; } = null!;
  public int DeviceVendorId { get; set; }
  public DeviceVendor DeviceVendor { get; set; } = null!;
}