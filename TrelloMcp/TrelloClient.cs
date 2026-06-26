using System.Net.Http.Json;
using System.Text.Json.Serialization;

public class TrelloClient
{
    private readonly HttpClient _http;
    private readonly string _key;
    private readonly string _token;

    public TrelloClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.trello.com/1/");

        _key = Environment.GetEnvironmentVariable("TRELLO_API_KEY")
            ?? throw new InvalidOperationException("TRELLO_API_KEY não definida.");
        _token = Environment.GetEnvironmentVariable("TRELLO_TOKEN")
            ?? throw new InvalidOperationException("TRELLO_TOKEN não definida.");
    }

    private string Auth => $"key={_key}&token={_token}";

    public async Task<List<TrelloBoard>> GetBoardsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<TrelloBoard>>(
            $"members/me/boards?fields=name,id&{Auth}");
        return result ?? [];
    }

    public async Task<List<TrelloList>> GetBoardListsAsync(string boardId)
    {
        var result = await _http.GetFromJsonAsync<List<TrelloList>>(
            $"boards/{boardId}/lists?fields=name,id&{Auth}");
        return result ?? [];
    }

    public async Task<TrelloCard?> GetCardAsync(string cardId)
    {
        return await _http.GetFromJsonAsync<TrelloCard>(
            $"cards/{cardId}?fields=name,desc,due,idList,url&{Auth}");
    }

    public async Task<List<TrelloCard>> GetCardsOnListAsync(string listId)
    {
        var result = await _http.GetFromJsonAsync<List<TrelloCard>>(
            $"lists/{listId}/cards?fields=name,desc,due,url&{Auth}");
        return result ?? [];
    }

    public async Task<TrelloCard> CreateCardAsync(string listId, string name, string? desc = null, string? due = null)
    {
        var body = new Dictionary<string, string>
        {
            ["idList"] = listId,
            ["name"] = name,
        };
        if (desc is not null) body["desc"] = desc;
        if (due is not null) body["due"] = due;

        var response = await _http.PostAsync(
            $"cards?{Auth}",
            new FormUrlEncodedContent(body));

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TrelloCard>()
            ?? throw new InvalidOperationException("Resposta inválida ao criar card.");
    }
}

public record TrelloBoard(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name);

public record TrelloList(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name);

public record TrelloCard(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("desc")] string? Desc,
    [property: JsonPropertyName("due")] string? Due,
    [property: JsonPropertyName("idList")] string? IdList,
    [property: JsonPropertyName("url")] string? Url);
