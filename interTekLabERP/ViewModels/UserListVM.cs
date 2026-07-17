namespace interTekLabERP.ViewModels;

public class UserListVM
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }
}