using AutoMapper;
using EduSciencePro.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

public class ConfirmationCodeRepository : IConfirmationCodeRepository
{
    private readonly ApplicationDbContext _db;

    public ConfirmationCodeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ConfirmationCode?> GetCodeByEmail(string email)
    {
        ConfirmationCode? codes = await _db.Codes.FirstOrDefaultAsync(c => c.Email == email);
        return codes;
    }

    public async Task<string> Save(string email)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

        ConfirmationCode? tryCode = await GetCodeByEmail(email);
        if (tryCode == null)
        {
            ConfirmationCode conformCode = new ConfirmationCode() { Code = code, Email = email };
            await _db.Codes.AddAsync(conformCode);
            await _db.SaveChangesAsync();
        }
        else
        {
            tryCode.Code = code;
            _db.Codes.Update(tryCode);
            await _db.SaveChangesAsync();
        }
        return code;
    }

    public async Task Delete(string email)
    {
        ConfirmationCode? code = await GetCodeByEmail(email);
        if (code != null)
            _db.Codes.Remove(code);
        await _db.SaveChangesAsync();
    }
}

public interface IConfirmationCodeRepository
{
    Task<ConfirmationCode?> GetCodeByEmail(string email);
    Task<string> Save(string email);
    Task Delete(string email);
}
