using CsvHelper.Configuration;

namespace Railcar.Data.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class EventCodeDefinitionCsv
{
    public string EventCode { get; init; } = string.Empty;
    public string EventDescription { get; init; } = string.Empty;
    public string LongDescription { get; init; } = string.Empty;
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class EventCodeDefinitionCsvMap : ClassMap<EventCodeDefinitionCsv>
{
    public EventCodeDefinitionCsvMap()
    {
        Map(m => m.EventCode).Name("Event Code");
        Map(m => m.EventDescription).Name("Event Description");
        Map(m => m.LongDescription).Name("Long Description");
    }
}
