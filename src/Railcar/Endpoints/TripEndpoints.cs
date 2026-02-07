using Railcar.Shared.Trips;

namespace Railcar.Endpoints;

// Extension class to group trip-related minimal API endpoints
public static class TripEndpoints
{
    public static void MapTripEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips");

        group.MapPost("/upload", UploadTripsFromCsv);
    }

    // Minimal API action method to handle CSV upload and return processed trips
    private static async Task<IResult> UploadTripsFromCsv(HttpRequest request)
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest("Expected multipart/form-data.");
        }

        var form = await request.ReadFormAsync();
        var file = form.Files.GetFile("file");

        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded or file is empty.");
        }

        // TODO: Replace with real CSV parsing and trip processing logic including:
        // - Parse equipment_events.csv
        // - Map cities to time zones
        // - Convert local times to UTC
        // - Build trips by equipment where W starts and Z ends
        // - Persist Trips and Events with EF Core
        // - Return the newly created Trips

        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        _ = await reader.ReadToEndAsync();

        var mockTrips = new List<RailcarTripDto>
        {
            new()
            {
                EquipmentId = "RC001",
                OriginCityId = 1,
                DestinationCityId = 2,
                StartLocal = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-10), DateTimeKind.Utc),
                EndLocal = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            },
            new()
            {
                EquipmentId = "RC002",
                OriginCityId = 3,
                DestinationCityId = 4,
                StartLocal = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-20), DateTimeKind.Utc),
                EndLocal = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-5), DateTimeKind.Utc)
            }
        };

        return Results.Ok(mockTrips);
    }

}
