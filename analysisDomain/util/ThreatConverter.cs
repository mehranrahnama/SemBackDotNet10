using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.util;

public class ThreatConverter : ValueConverter<Threat?, long?>
{
    public ThreatConverter()
        : base(
            threat => threat == null ? (long?)null : threat.Value.GetId(),
            dbData => dbData == null ? (Threat?)null : ThreatExtensions.FromId(dbData))
    {
    }
}
