using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

[McpServerToolType]
public static class CardTools
{
    private static readonly JsonSerializerOptions JsonOpts = new() { WriteIndented = true };

    [McpServerTool, Description("Busca um card do Trello pelo ID.")]
    public static async Task<string> GetCard(
        TrelloClient client,
        [Description("ID do card no Trello")] string cardId)
    {
        var card = await client.GetCardAsync(cardId);
        return card is null
            ? "Card não encontrado."
            : JsonSerializer.Serialize(card, JsonOpts);
    }

    [McpServerTool, Description("Lista todos os cards de uma lista (coluna) do Trello.")]
    public static async Task<string> GetCardsOnList(
        TrelloClient client,
        [Description("ID da lista (coluna) no Trello")] string listId)
    {
        var cards = await client.GetCardsOnListAsync(listId);
        return cards.Count == 0
            ? "Nenhum card encontrado nesta lista."
            : JsonSerializer.Serialize(cards, JsonOpts);
    }

    [McpServerTool, Description("Cria um card em uma lista do Trello.")]
    public static async Task<string> CreateCard(
        TrelloClient client,
        [Description("ID da lista onde o card será criado")] string listId,
        [Description("Título do card")] string name,
        [Description("Descrição do card (opcional)")] string? desc = null,
        [Description("Data de entrega no formato ISO 8601, ex: 2026-07-01T12:00:00Z (opcional)")] string? due = null)
    {
        var card = await client.CreateCardAsync(listId, name, desc, due);
        return $"Card criado com sucesso!\n{JsonSerializer.Serialize(card, JsonOpts)}";
    }

    [McpServerTool, Description("Atualiza o nome e/ou descrição de um card existente no Trello.")]
    public static async Task<string> UpdateCard(
        TrelloClient client,
        [Description("ID do card")] string cardId,
        [Description("Novo nome do card (null para não alterar)")] string? name = null,
        [Description("Nova descrição do card (null para não alterar)")] string? desc = null)
    {
        var card = await client.UpdateCardAsync(cardId, name, desc);
        return card is null ? "Card não encontrado." : JsonSerializer.Serialize(card, JsonOpts);
    }

    [McpServerTool, Description("Move um card para outra coluna (lista) do Trello.")]
    public static async Task<string> MoveCard(
        TrelloClient client,
        [Description("ID do card")] string cardId,
        [Description("ID da lista de destino")] string targetListId)
    {
        var card = await client.MoveCardAsync(cardId, targetListId);
        return card is null ? "Card não encontrado." : JsonSerializer.Serialize(card, JsonOpts);
    }

    [McpServerTool, Description("Arquiva um card do Trello (equivale a fechá-lo).")]
    public static async Task<string> ArchiveCard(
        TrelloClient client,
        [Description("ID do card a arquivar")] string cardId)
    {
        var card = await client.ArchiveCardAsync(cardId);
        return card is null ? "Card não encontrado." : JsonSerializer.Serialize(card, JsonOpts);
    }
}
