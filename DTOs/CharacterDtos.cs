using System.ComponentModel.DataAnnotations;

namespace GameApi.DTOs;

public sealed record CreateCharacterRequest(
    [property: Required, StringLength(50, MinimumLength = 2)] string Name);

public sealed record UpdateCharacterRequest(
    [property: Required, StringLength(50, MinimumLength = 2)] string Name,
    [property: Required, StringLength(30, MinimumLength = 1)] string Class,
    [property: Required, Range(1, 100)] int? Level,
    [property: Required, Range(0, 100000)] int? Hp);

public sealed record CharacterResponse(int Id, string Name, string Class, int Level, int Hp, int AccountId);
