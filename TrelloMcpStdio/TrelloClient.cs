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
        var qs = new Dictionary<string, string>
        {
            ["idList"] = listId,
            ["name"] = name,
        };
        if (desc is not null) qs["desc"] = desc;
        if (due is not null) qs["due"] = due;

        var response = await _http.PostAsync(
            $"cards?{Auth}&{ToQueryString(qs)}",
            content: null);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TrelloCard>()
            ?? throw new InvalidOperationException("Resposta inválida ao criar card.");
    }

    public async Task<TrelloChecklist> CreateChecklistAsync(string cardId, string name)
    {
        var qs = new Dictionary<string, string>
        {
            ["idCard"] = cardId,
            ["name"] = name,
        };

        var response = await _http.PostAsync(
            $"checklists?{Auth}&{ToQueryString(qs)}",
            content: null);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TrelloChecklist>()
            ?? throw new InvalidOperationException("Resposta inválida ao criar checklist.");
    }

    public async Task<TrelloCheckItem> AddCheckItemAsync(string checklistId, string name, bool? @checked = null)
    {
        var qs = new Dictionary<string, string> { ["name"] = name };
        if (@checked is not null) qs["checked"] = @checked.Value ? "true" : "false";

        var response = await _http.PostAsync(
            $"checklists/{checklistId}/checkItems?{Auth}&{ToQueryString(qs)}",
            content: null);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TrelloCheckItem>()
            ?? throw new InvalidOperationException("Resposta inválida ao adicionar item.");
    }

    public async Task<TrelloCard?> UpdateCardAsync(string cardId, string? name, string? desc)
    {
        var body = new Dictionary<string, string?>();
        if (name is not null) body["name"] = name;
        if (desc is not null) body["desc"] = desc;

        var response = await _http.PutAsync(
            $"cards/{cardId}?{Auth}",
            new FormUrlEncodedContent(body!));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TrelloCard>();
    }

    public async Task<TrelloCard?> MoveCardAsync(string cardId, string targetListId)
    {
        var body = new Dictionary<string, string> { ["idList"] = targetListId };

        var response = await _http.PutAsync(
            $"cards/{cardId}?{Auth}",
            new FormUrlEncodedContent(body));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TrelloCard>();
    }

    public async Task<TrelloCard?> ArchiveCardAsync(string cardId)
    {
        var body = new Dictionary<string, string> { ["closed"] = "true" };

        var response = await _http.PutAsync(
            $"cards/{cardId}?{Auth}",
            new FormUrlEncodedContent(body));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TrelloCard>();
    }

    private static string ToQueryString(Dictionary<string, string> params_) =>
        string.Join("&", params_.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

    public async Task<List<TrelloChecklist>> GetCardChecklistsAsync(string cardId)
    {
        var result = await _http.GetFromJsonAsync<List<TrelloChecklist>>(
            $"cards/{cardId}/checklists?{Auth}");
        return result ?? [];
    }

    public async Task<List<TrelloLabel>> GetBoardLabelsAsync(string boardId)
    {
        var result = await _http.GetFromJsonAsync<List<TrelloLabel>>(
            $"boards/{boardId}/labels?{Auth}");
        return result ?? [];
    }

    public async Task<TrelloLabel> CreateLabelAsync(string boardId, string name, string? color = null)
    {
        var qs = new Dictionary<string, string>
        {
            ["idBoard"] = boardId,
            ["name"] = name,
        };
        if (color is not null) qs["color"] = color;

        var response = await _http.PostAsync(
            $"labels?{Auth}&{ToQueryString(qs)}",
            content: null);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TrelloLabel>()
            ?? throw new InvalidOperationException("Resposta inválida ao criar etiqueta.");
    }

    public async Task AddLabelToCardAsync(string cardId, string labelId)
    {
        var qs = new Dictionary<string, string> { ["value"] = labelId };

        var response = await _http.PostAsync(
            $"cards/{cardId}/idLabels?{Auth}&{ToQueryString(qs)}",
            content: null);

        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveLabelFromCardAsync(string cardId, string labelId)
    {
        var response = await _http.DeleteAsync(
            $"cards/{cardId}/idLabels/{labelId}?{Auth}");

        response.EnsureSuccessStatusCode();
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

public record TrelloChecklist(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("idCard")] string IdCard,
    [property: JsonPropertyName("checkItems")] List<TrelloCheckItem> CheckItems);

public record TrelloCheckItem(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("state")] string State);

public record TrelloLabel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("color")] string? Color,
    [property: JsonPropertyName("idBoard")] string IdBoard);
