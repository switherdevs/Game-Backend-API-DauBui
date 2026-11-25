using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameApi.Services;

public sealed class AuthService(AppDbContext db, IConfiguration configuration)
{
    private readonly PasswordHasher<Account> passwordHasher = new();

    public async Task<AccountResponse?> RegisterAsync(RegisterRequest request)
    {
        if (await db.Accounts.AnyAsync(account => account.Username == request.Username))
            return null;

        var account = new Account { Username = request.Username, PasswordHash = string.Empty };
        account.PasswordHash = passwordHasher.HashPassword(account, request.Password);
        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        return new AccountResponse(account.Id, account.Username);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var account = await db.Accounts.SingleOrDefaultAsync(item => item.Username == request.Username);
        if (account is null || passwordHasher.VerifyHashedPassword(account, account.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return null;

        var keyText = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: [new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()), new Claim(ClaimTypes.Name, account.Username)],
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), account.Username);
    }
}
