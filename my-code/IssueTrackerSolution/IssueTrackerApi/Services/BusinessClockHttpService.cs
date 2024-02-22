namespace IssueTrackerApi.Services;

public class BusinessClockHttpService(HttpClient client)
{
    public async Task<SupportResponse?> GetCurrentSupportInformationAsync()
    {
        var response = await client.GetAsync("/support-info");
        response.EnsureSuccessStatusCode(); //if I get anything out of the range of 200-299, then throw an exception

        var body = await response.Content.ReadFromJsonAsync<SupportResponse>();
        return body;
    }
}

public record SupportResponse(string Name, string Phone);
