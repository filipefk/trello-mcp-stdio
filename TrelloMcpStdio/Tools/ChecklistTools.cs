using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

[McpServerToolType]
public static class ChecklistTools
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    [McpServerTool, Description("Cria um checklist em um card do Trello.")]
    public static async Task<string> CreateChecklist(
        TrelloClient client,
        [Description("ID do card onde o checklist será criado")] string cardId,
        [Description("Nome do checklist")] string name)
    {
        var checklist = await client.CreateChecklistAsync(cardId, name);
        return $"Checklist criado com sucesso!\n{JsonSerializer.Serialize(checklist, JsonOpts)}";
    }

    [McpServerTool, Description("Adiciona um item a um checklist do Trello.")]
    public static async Task<string> AddCheckItem(
        TrelloClient client,
        [Description("ID do checklist")] string checklistId,
        [Description("Nome do item")] string name,
        [Description("Se o item já deve iniciar marcado como concluído (opcional, padrão: false)")] bool? @checked = null)
    {
        var item = await client.AddCheckItemAsync(checklistId, name, @checked);
        return $"Item adicionado com sucesso!\n{JsonSerializer.Serialize(item, JsonOpts)}";
    }

    [McpServerTool, Description("Lista todos os checklists de um card do Trello, incluindo seus itens.")]
    public static async Task<string> GetCardChecklists(
        TrelloClient client,
        [Description("ID do card no Trello")] string cardId)
    {
        var checklists = await client.GetCardChecklistsAsync(cardId);
        return checklists.Count == 0
            ? "Nenhum checklist encontrado neste card."
            : JsonSerializer.Serialize(checklists, JsonOpts);
    }
}
