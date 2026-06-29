# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Projeto

Servidor MCP (Model Context Protocol) para integração com o Trello, escrito em C# (.NET 10). Expõe ferramentas que permitem a agentes de IA listar boards, colunas e cards, criar cards e gerenciar checklists dentro de cards.

## Comandos

```bash
# Build
dotnet build TrelloMcpStdio/TrelloMcpStdio.csproj

# Executar (requer variáveis de ambiente)
TRELLO_API_KEY=<key> TRELLO_TOKEN=<token> dotnet run --project TrelloMcpStdio/TrelloMcpStdio.csproj
```

Não há testes automatizados no projeto atualmente.

## Variáveis de Ambiente

Obrigatórias em tempo de execução — sem elas o processo falha na inicialização:

- `TRELLO_API_KEY` — chave da API do Trello
- `TRELLO_TOKEN` — token de autenticação do Trello

## Arquitetura

O servidor usa `stdio` como transporte MCP (padrão para integração com clientes como Claude Desktop).

- **`Program.cs`** — bootstrapping via `IHost`; registra `TrelloClient` (via `HttpClient`) e sobe o servidor MCP com `WithStdioServerTransport()` e `WithToolsFromAssembly()` (descobre ferramentas automaticamente por reflexão).
- **`TrelloClient.cs`** — wrapper sobre a API REST do Trello (`https://api.trello.com/1/`). Também define os records de modelo: `TrelloBoard`, `TrelloList`, `TrelloCard`, `TrelloChecklist`, `TrelloCheckItem`.
- **`Tools/BoardTools.cs`** — ferramentas MCP de boards: `GetBoards`, `GetBoardLists`, `GetBoardIdByName`, `GetListIdByName`, `GetListIdByBoardNameAndListName`.
- **`Tools/CardTools.cs`** — ferramentas MCP de cards: `GetCard`, `GetCardsOnList`, `CreateCard`.
- **`Tools/ChecklistTools.cs`** — ferramentas MCP de checklists: `CreateChecklist`, `AddCheckItem`, `GetCardChecklists`.

## Convenções

- Novas ferramentas MCP são classes `static` com `[McpServerToolType]`; cada método público com `[McpServerTool]` é registrado automaticamente.
- Parâmetros de ferramentas devem ter `[Description(...)]` para que o LLM saiba como usá-los.
- Retorno das ferramentas é sempre `string` (JSON indentado ou mensagem de texto).
- `TrelloClient` é injetado via DI nos métodos de ferramenta — não instanciar diretamente.
