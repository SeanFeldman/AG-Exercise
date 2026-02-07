using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Railcar.Shared.Trips;

namespace Railcar.Client.Pages;

public partial class RailcarTrips(HttpClient httpClient) : ComponentBase
{
    private IBrowserFile? _selectedFile;
    private bool _hasFile;
    private bool _isUploading;
    private string? _statusMessage;
    private List<RailcarTripDto> _trips = new();

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        _selectedFile = e.File;
        _hasFile = _selectedFile is not null;
        _statusMessage = _hasFile
            ? $"Selected file: {_selectedFile!.Name} ({_selectedFile.Size} bytes)"
            : "No file selected.";
    }

    private async Task UploadFile()
    {
        if (_selectedFile is null)
        {
            _statusMessage = "Please select a CSV file first.";
            return;
        }

        _isUploading = true;
        _statusMessage = "Uploading and processing file...";

        try
        {
            using var content = new MultipartFormDataContent();
            await using var stream = _selectedFile.OpenReadStream(long.MaxValue);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            content.Add(fileContent, "file", _selectedFile.Name);

            var response = await httpClient.PostAsync("api/trips/upload", content);

            if (!response.IsSuccessStatusCode)
            {
                _statusMessage =
                    $"Upload failed: {(int)response.StatusCode} {response.ReasonPhrase}";
                return;
            }

            var trips = await response.Content.ReadFromJsonAsync<List<RailcarTripDto>>();

            if (trips is null)
            {
                _statusMessage = "Upload succeeded but no trips were returned.";
                _trips = new List<RailcarTripDto>();
            }
            else
            {
                _trips = trips;
                _statusMessage =
                    $"Upload and processing completed. {_trips.Count} trips returned.";
            }
        }
        catch (Exception ex)
        {
            _statusMessage = $"Error uploading file: {ex.Message}";
        }
        finally
        {
            _isUploading = false;
        }
    }
}
