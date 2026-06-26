using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

[McpServerToolType]
public static class BoardTools
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    [McpServerTool, Description("Lista todos os boards disponíveis na conta Trello autenticada.")]
    public static async Task<string> GetBoards(TrelloClient client)
    {
        var boards = await client.GetBoardsAsync();
        return boards.Count == 0
            ? "Nenhum board encontrado."
            : JsonSerializer.Serialize(boards, JsonOpts);
    }

    [McpServerTool, Description("Lista as colunas (listas) de um board do Trello.")]
    public static async Task<string> GetBoardLists(
        TrelloClient client,
        [Description("ID do board no Trello")] string boardId)
    {
        var lists = await client.GetBoardListsAsync(boardId);
        return lists.Count == 0
            ? "Nenhuma lista encontrada neste board."
            : JsonSerializer.Serialize(lists, JsonOpts);
    }
}
