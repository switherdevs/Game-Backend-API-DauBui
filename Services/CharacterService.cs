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

    public async Task<IReadOnlyList<CharacterResponse>> GetAllAsync(int accountId) =>
        await db.Characters
            .Where(character => character.AccountId == accountId)
            .OrderBy(character => character.Id)
            .Select(character => new CharacterResponse(character.Id, character.Name, character.Class, character.Level, character.Hp, character.AccountId))
            .ToListAsync();

    public async Task<CharacterResponse?> GetByIdAsync(int accountId, int characterId)
    {
        var character = await db.Characters.SingleOrDefaultAsync(item => item.Id == characterId && item.AccountId == accountId);
        return character is null ? null : ToResponse(character);
    }

    public async Task<bool> DeleteAsync(int accountId, int characterId)
    {
        var character = await db.Characters.FindAsync(characterId);
        if (character is null || character.AccountId != accountId)
            return false;

        db.Characters.Remove(character);
        await db.SaveChangesAsync();
        return true;
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
