using System.ComponentModel.DataAnnotations;

namespace GameApi.DTOs;

public sealed class CreateCharacterRequest
{
    [Required(ErrorMessage = "Tên nhân vật không được để trống")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên nhân vật phải từ 2 đến 50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_\s\u00C0-\u1EF9-]+$", ErrorMessage = "Tên nhân vật chỉ chứa chữ cái, số, khoảng trắng hoặc dấu gạch nối")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lớp nhân vật không được để trống")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Lớp nhân vật tối đa 30 ký tự")]
    [RegularExpression(@"^(Warrior|Mage|Archer)$", ErrorMessage = "Lớp nhân vật phải là Warrior, Mage hoặc Archer")]
    public string Class { get; set; } = string.Empty;
}

public sealed class UpdateCharacterRequest
{
    [Required(ErrorMessage = "Tên nhân vật không được để trống")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Tên nhân vật phải từ 2 đến 50 ký tự")]
    [RegularExpression(@"^[a-zA-Z0-9_\s\u00C0-\u1EF9-]+$", ErrorMessage = "Tên nhân vật chỉ chứa chữ cái, số, khoảng trắng hoặc dấu gạch nối")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lớp nhân vật không được để trống")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Lớp nhân vật tối đa 30 ký tự")]
    [RegularExpression(@"^(Warrior|Mage|Archer)$", ErrorMessage = "Lớp nhân vật phải là Warrior, Mage hoặc Archer")]
    public string Class { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cấp độ không được để trống")]
    [Range(1, 100, ErrorMessage = "Cấp độ phải từ 1 đến 100")]
    public int? Level { get; set; }

    [Required(ErrorMessage = "HP không được để trống")]
    [Range(0, 100000, ErrorMessage = "HP phải từ 0 đến 100000")]
    public int? Hp { get; set; }
}

public sealed record CharacterResponse(int Id, string Name, string Class, int Level, int Hp, int AccountId);
