using Microsoft.AspNetCore.Mvc;
using Railcar.Data.Services;
using Railcar.Shared.Trips;

namespace Railcar.Endpoints;

// Extension class to group trip-related minimal API endpoints
public static class TripEndpoints
{
    public static void MapTripEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/trips");

        group.MapPost("/upload", UploadTripsFromCsv);
        group.MapGet("/{id:long}", GetTripById);
        group.MapGet("/{id:long}/events", GetTripEvents);
        group.MapGet("", GetTrips);
    }

    private static async Task<IResult> UploadTripsFromCsv(
        HttpRequest request,
        [FromServices] ITripImportService tripImportService,
        CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
        {
            return Results.BadRequest("Expected multipart/form-data.");
        }

        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");

        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("No file uploaded or file is empty.");
        }

        await using var stream = file.OpenReadStream();

        var tripDtos = await tripImportService.ImportTripsAsync(stream, cancellationToken);

        return Results.Ok(tripDtos);
    }

    private static async Task<IResult> GetTripById(
        [FromServices] ITripService tripService,
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var trip = await tripService.GetTripByIdAsync(id, cancellationToken);

        return trip is null 
            ? Results.NotFound() 
            : Results.Ok(trip);
    }

    private static async Task<IResult> GetTripEvents(
        [FromServices] ITripService tripService,
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var eventsForTrip = await tripService.GetTripEventsAsync(id, cancellationToken);

        return Results.Ok(eventsForTrip);
    }

    private static async Task<IResult> GetTrips(
        [FromServices] ITripService tripService,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return Results.BadRequest("pageNumber and pageSize must be positive.");
        }

        var result = await tripService.GetTripsAsync(pageNumber, pageSize, cancellationToken);

        var paginatedResult = new PaginatedTripsResult
        {
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            Items = result.Items
        };

        return Results.Ok(paginatedResult);
    }
}
