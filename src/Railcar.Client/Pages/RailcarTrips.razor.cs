using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Railcar.Shared.Trips;

namespace Railcar.Client.Pages;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class RailcarTrips(HttpClient httpClient) : ComponentBase
{
    private IBrowserFile? _selectedFile;
    private bool _hasFile;
    private List<RailcarTripDto> _trips = [];
    private long? _selectedTripId;
    private List<RailcarTripEventDto> _selectedTripEvents = [];
    private bool _isLoadingEvents;

    private bool IsUploading { get; set; }
    private bool HasTripsToShow => _trips.Count > 0;
    private string? StatusMessage { get; set; }
    private bool UploadingDisabled => !_hasFile || IsUploading;

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        _selectedFile = e.File;
        _hasFile = _selectedFile is not null;
        StatusMessage = _hasFile
            ? $"Selected file: {_selectedFile!.Name} ({_selectedFile.Size} bytes)"
            : "No file selected.";
    }

    private async Task UploadFile()
    {
        if (_selectedFile is null)
        {
            StatusMessage = "Please select a CSV file first.";
            return;
        }

        IsUploading = true;
        StatusMessage = "Uploading and processing file...";

        try
        {
            using var content = new MultipartFormDataContent();
            await using var stream = _selectedFile.OpenReadStream(long.MaxValue);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "file", _selectedFile.Name);

            // TODO: the logic of talking to the API endpoints could be extracted to a local service but for simplicity doing it here
            var response = await httpClient.PostAsync("api/trips/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                StatusMessage = $"Upload failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}";
                return;
            }

            var trips = await response.Content.ReadFromJsonAsync<List<RailcarTripDto>>();

            if (trips is null)
            {
                _trips = [];
                StatusMessage = "Upload succeeded but no trips were returned.";
            }
            else
            {
                _trips = trips;
                StatusMessage = $"Upload and processing completed. {_trips.Count} trips returned.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error uploading file: {ex.Message}";
        }
        finally
        {
            IsUploading = false;
        }
    }

    private async Task SelectTrip(long id)
    {
        _selectedTripId = id;
        _selectedTripEvents = [];
        _isLoadingEvents = true;

        try
        {
            var response = await httpClient.GetAsync($"api/trips/{id}/events");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                StatusMessage = $"Failed to load events for trip {id}: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}";
                return;
            }

            var eventsForTrip = await response.Content.ReadFromJsonAsync<List<RailcarTripEventDto>>();
            _selectedTripEvents = eventsForTrip ?? [];
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading events for trip {id}: {ex.Message}";
        }
        finally
        {
            _isLoadingEvents = false;
        }
    }
}