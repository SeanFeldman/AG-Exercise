using CsvHelper.Configuration;

namespace Railcar.Data.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CityCsv
{
    public int CityId { get; init; }
    public string CityName { get; init; } = string.Empty;
    public string TimeZone { get; init; } = string.Empty;
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CityCsvMap : ClassMap<CityCsv>
{
    public CityCsvMap()
    {
        Map(m => m.CityId).Name("City Id");
        Map(m => m.CityName).Name("City Name");
        Map(m => m.TimeZone).Name("Time Zone");
    }
}
