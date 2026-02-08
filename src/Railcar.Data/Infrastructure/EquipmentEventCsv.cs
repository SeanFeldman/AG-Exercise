using System;
using CsvHelper.Configuration;

namespace Railcar.Data.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class EquipmentEventCsv
{
    public string EquipmentId { get; init; } = string.Empty;
    public string EventCode { get; init; } = string.Empty;
    public DateTime EventTime { get; init; }
    public int CityId { get; init; }
}

internal sealed class EquipmentEventCsvMap : ClassMap<EquipmentEventCsv>
{
    public EquipmentEventCsvMap()
    {
        Map(m => m.EquipmentId).Name("Equipment Id");
        Map(m => m.EventCode).Name("Event Code");
        Map(m => m.EventTime).Name("Event Time");
        Map(m => m.CityId).Name("City Id");
    }
}
