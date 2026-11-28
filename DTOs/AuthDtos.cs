using System.ComponentModel.DataAnnotations;

namespace GameApi.DTOs;

public sealed class RegisterRequest
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số, dấu gạch dưới, gạch ngang và dấu chấm")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; } = string.Empty;
}

public sealed record AccountResponse(int Id, string Username);
public sealed record LoginResponse(string Token, string Username);