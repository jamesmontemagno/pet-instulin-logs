using SQLite;

namespace PetInsulinLogs.Models;

[Table("ShareTokens")]
public class ShareToken
{
    [PrimaryKey]
    public string Token { get; set; } = string.Empty;
    public string PetId { get; set; } = string.Empty;
    public DateTime ExpiryUtc { get; set; }
}
