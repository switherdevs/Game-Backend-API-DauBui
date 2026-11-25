using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Services;

public sealed class CharacterService(AppDbContext db)
{
    public async Task<CharacterResponse> CreateAsync(int accountId, CreateCharacterRequest request)
    {
        var character = new Character
        {
            Name = request.Name,
            Class = request.Class,
            AccountId = accountId
        };
        db.Characters.Add(character);
        await db.SaveChangesAsync();
        return ToResponse(character);
    }

    public async Task<CharacterResponse?> UpdateAsync(int accountId, int characterId, UpdateCharacterRequest request)
    {
        var character = await db.Characters.SingleOrDefaultAsync(item => item.Id == characterId && item.AccountId == accountId);
        if (character is null)
            return null;

        character.Name = request.Name;
        character.Class = request.Class;
        character.Level = request.Level!.Value;
        character.Hp = request.Hp!.Value;
        await db.SaveChangesAsync();
        return ToResponse(character);
    }

    private static CharacterResponse ToResponse(Character character) =>
        new(character.Id, character.Name, character.Class, character.Level, character.Hp, character.AccountId);
}
