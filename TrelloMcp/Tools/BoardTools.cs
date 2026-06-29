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

    [McpServerTool, Description("Retorna o ID de um board do Trello buscando pelo nome (comparação sem distinção de maiúsculas/minúsculas). Use esta ferramenta apenas quando já tiver o nome do board e precisar exclusivamente do ID do board. Se o objetivo final for obter o ID de uma coluna, prefira GetListIdByBoardNameAndListName.")]
    public static async Task<string> GetBoardIdByName(
        TrelloClient client,
        [Description("Nome do board no Trello")] string boardName)
    {
        var boards = await client.GetBoardsAsync();
        var board = boards.FirstOrDefault(b => b.Name.Equals(boardName, StringComparison.OrdinalIgnoreCase));
        return board is null
            ? $"Board '{boardName}' não encontrado."
            : board.Id;
    }

    [McpServerTool, Description("Retorna o ID de uma coluna (lista) de um board do Trello buscando pelo nome da coluna (comparação sem distinção de maiúsculas/minúsculas). Use esta ferramenta apenas quando já tiver o ID do board em mãos. Se tiver apenas o nome do board, prefira GetListIdByBoardNameAndListName.")]
    public static async Task<string> GetListIdByName(
        TrelloClient client,
        [Description("ID do board no Trello")] string boardId,
        [Description("Nome da coluna (lista) no Trello")] string listName)
    {
        var lists = await client.GetBoardListsAsync(boardId);
        var list = lists.FirstOrDefault(l => l.Name.Equals(listName, StringComparison.OrdinalIgnoreCase));
        return list is null
            ? $"Coluna '{listName}' não encontrada neste board."
            : list.Id;
    }

    [McpServerTool, Description("Retorna o ID de uma coluna (lista) buscando pelo nome do board e pelo nome da coluna, sem precisar de nenhum ID. Prefira esta ferramenta sempre que o usuário informar apenas os nomes do board e da coluna (comparações sem distinção de maiúsculas/minúsculas).")]
    public static async Task<string> GetListIdByBoardNameAndListName(
        TrelloClient client,
        [Description("Nome do board no Trello")] string boardName,
        [Description("Nome da coluna (lista) no Trello")] string listName)
    {
        var boards = await client.GetBoardsAsync();
        var board = boards.FirstOrDefault(b => b.Name.Equals(boardName, StringComparison.OrdinalIgnoreCase));
        if (board is null)
            return $"Board '{boardName}' não encontrado.";

        var lists = await client.GetBoardListsAsync(board.Id);
        var list = lists.FirstOrDefault(l => l.Name.Equals(listName, StringComparison.OrdinalIgnoreCase));
        return list is null
            ? $"Coluna '{listName}' não encontrada no board '{boardName}'."
            : list.Id;
    }
}
