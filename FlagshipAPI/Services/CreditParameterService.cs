using FlagshipAPI.Data;
using FlagshipAPI.Models;
using Microsoft.EntityFrameworkCore;

public class CreditParameterService
{
    private readonly AppDbContext _db;

    public CreditParameterService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasOverlapAsync(CreditParameter newParam, int? ignoreId = null)
    {
        var newStart = newParam.EffectiveStart;
        var newEnd = newParam.EffectiveEnd ?? DateTime.MaxValue;

        return await _db.CreditParameters
             .AnyAsync(existing =>
                 existing.Product == newParam.Product &&
                 existing.Key == newParam.Key &&
                 !(newEnd < (existing.EffectiveStart) ||
                   (existing.EffectiveEnd ?? DateTime.MaxValue) < newStart)
             );

    }

    private static bool IntervalsOverlap(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
    {
        if (endA < startB || endB < startA) return false;
        return true;
    }
}
