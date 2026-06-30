using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

[McpServerToolType]
public static class LabelTools
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    [McpServerTool, Description("Lista todas as etiquetas de um board do Trello.")]
    public static async Task<string> GetBoardLabels(
        TrelloClient client,
        [Description("ID do board no Trello")] string boardId)
    {
        var labels = await client.GetBoardLabelsAsync(boardId);
        return labels.Count == 0
            ? "Nenhuma etiqueta encontrada neste board."
            : JsonSerializer.Serialize(labels, JsonOpts);
    }

    [McpServerTool, Description("Cria uma nova etiqueta em um board do Trello. Cores válidas: yellow, purple, blue, red, green, orange, black, sky, pink, lime. Omita a cor para criar sem cor.")]
    public static async Task<string> CreateLabel(
        TrelloClient client,
        [Description("ID do board onde a etiqueta será criada")] string boardId,
        [Description("Nome da etiqueta")] string name,
        [Description("Cor da etiqueta (opcional): yellow, purple, blue, red, green, orange, black, sky, pink, lime")] string? color = null)
    {
        var label = await client.CreateLabelAsync(boardId, name, color);
        return $"Etiqueta criada com sucesso!\n{JsonSerializer.Serialize(label, JsonOpts)}";
    }

    [McpServerTool, Description("Aplica uma etiqueta existente a um card do Trello.")]
    public static async Task<string> AddLabelToCard(
        TrelloClient client,
        [Description("ID do card")] string cardId,
        [Description("ID da etiqueta a aplicar")] string labelId)
    {
        await client.AddLabelToCardAsync(cardId, labelId);
        return "Etiqueta aplicada ao card com sucesso.";
    }

    [McpServerTool, Description("Remove uma etiqueta de um card do Trello.")]
    public static async Task<string> RemoveLabelFromCard(
        TrelloClient client,
        [Description("ID do card")] string cardId,
        [Description("ID da etiqueta a remover")] string labelId)
    {
        await client.RemoveLabelFromCardAsync(cardId, labelId);
        return "Etiqueta removida do card com sucesso.";
    }
}
