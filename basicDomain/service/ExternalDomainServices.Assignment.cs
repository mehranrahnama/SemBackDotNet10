#nullable enable
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ExternalDomainServices
{
    // Both listeners update tables in this database. Apply their effects in the
    // planning transaction so nothing is externally visible before commit.
    public async Task ChangeFacilitatorAsync(PlaningLevel level, long? newFacilitator, long? oldFacilitator, long? newResponsible, long? oldResponsible, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(level);
        RequireTransaction();
        long? facilitator = newFacilitator is not null && newFacilitator != oldFacilitator ? newFacilitator : null;
        long? manager = newResponsible is not null && newResponsible != oldResponsible ? newResponsible : null;
        if (facilitator is null && manager is null) return;
        var levelId = level.Id;
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE [dbo].[tblactionplan] SET [payeshgar] = COALESCE({facilitator}, [payeshgar]), [modir] = COALESCE({manager}, [modir]) WHERE [levelid] = {levelId}", ct);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE [dbo].[tblactionplancatalog] SET [payeshgar] = COALESCE({facilitator}, [payeshgar]), [modir] = COALESCE({manager}, [modir]) WHERE [levelid] = {levelId}", ct);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE m SET m.[tashilgar] = COALESCE({facilitator}, m.[tashilgar]), m.[modir] = COALESCE({manager}, m.[modir]) FROM [dbo].[tbldimmeasure] m INNER JOIN [dbo].[tbldimmeasurecatalog] c ON c.[dimmeasurecatalogid] = m.[dimmeasurecatalogid] WHERE c.[levelid] = {levelId}", ct);
        await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE [dbo].[tbldimmeasurecatalog] SET [tashilgar] = COALESCE({facilitator}, [tashilgar]), [modir] = COALESCE({manager}, [modir]) WHERE [levelid] = {levelId}", ct);
    }
}
