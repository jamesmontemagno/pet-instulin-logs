using SQLite;

namespace PetInsulinLogs.Models;

[Table("Memberships")]
public class Membership
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string PetId { get; set; } = string.Empty;
    public Role Role { get; set; }
}
