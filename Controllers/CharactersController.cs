using System.Security.Claims;
using GameApi.DTOs;
using GameApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers;

[ApiController]
[Authorize]
[Route("api/characters")]
public sealed class CharactersController(CharacterService characterService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateCharacterRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId))
            return Unauthorized();

        var character = await characterService.CreateAsync(accountId, request);
        return StatusCode(StatusCodes.Status201Created, character);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<CharacterResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, UpdateCharacterRequest request)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var accountId))
            return Unauthorized();

        var character = await characterService.UpdateAsync(accountId, id, request);
        return character is null
            ? NotFound(new { message = "Không tìm thấy nhân vật của bạn" })
            : Ok(character);
    }
}
