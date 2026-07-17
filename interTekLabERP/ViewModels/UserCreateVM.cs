using System.ComponentModel.DataAnnotations;

namespace interTekLabERP.ViewModels;

public class UserCreateVM
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    public string UserName { get; set; } = "";

    [Required(ErrorMessage = "Şifre zorunludur")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Rol seçiniz")]
    public string Role { get; set; } = "";

    public bool IsActive { get; set; } = true;
}