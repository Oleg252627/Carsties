using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.Register;

public class RegisterViewModel
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string FullName { get; set; } = null!;
    public string ReturnUrl { get; set; } = null!;
    public string Button { get; set; } = null!;
}