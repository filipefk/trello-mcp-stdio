# Trello MCP

Servidor [MCP (Model Context Protocol)](https://modelcontextprotocol.io/) para integração com o Trello, escrito em C# com .NET 10. Permite que agentes de IA listem boards, colunas e cards, além de criar novos cards diretamente no Trello.
Construído baseado na documentação da API do Trello: [https://developer.atlassian.com/cloud/trello/rest/](https://developer.atlassian.com/cloud/trello/rest/)

## Ferramentas disponíveis

| Ferramenta | Descrição |
|---|---|
| `GetBoards` | Lista todos os boards da conta Trello autenticada |
| `GetBoardLists` | Lista as colunas (listas) de um board pelo ID |
| `GetCard` | Busca um card pelo ID |
| `GetCardsOnList` | Lista todos os cards de uma coluna pelo ID |
| `CreateCard` | Cria um card em uma coluna, com título, descrição e data de entrega opcionais |
| `CreateChecklist` | Cria um checklist em um card pelo ID |
| `AddCheckItem` | Adiciona um item a um checklist existente |
| `GetCardChecklists` | Lista todos os checklists de um card, incluindo seus itens |
| `GetBoardIdByName` | Retorna o ID de um board buscando pelo nome |
| `GetListIdByName` | Retorna o ID de uma coluna pelo nome, dado o ID do board |
| `GetListIdByBoardNameAndListName` | Retorna o ID de uma coluna buscando pelos nomes do board e da coluna |

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Chave de API e token do Trello (veja as instruções abaixo)

## Obtendo as credenciais do Trello

Para usar o servidor, você precisará de uma chave de API `TRELLO_API_KEY` e um token `TRELLO_TOKEN` do Trello. Siga os passos abaixo para obtê-los:

### 1. Criar um aplicativo no Trello

1. Faça login no Trello e acesse [https://trello.com/power-ups/admin/](https://trello.com/power-ups/admin/)
2. Clique no botão **"Novo"** para criar um novo aplicativo
3. Preencha os campos solicitados — o campo "URL de conector Iframe" não é obrigatório
4. Clique em **"Gerar nova chave de API"**
5. O valor exibido no campo **"Chave de API"** é o que você usará como `TRELLO_API_KEY`

### 2. Gerar o token de autenticação

Monte a URL abaixo substituindo os placeholders pelos dados do seu aplicativo:

```
https://trello.com/1/authorize?expiration=never&scope=read,write&response_type=token&name=[nome+do+app]&key=[Chave de API]
```

> **Atenção:** no parâmetro `name`, substitua espaços por `+` (ex.: `Meu App` → `Meu+App`).

1. Com o Trello aberto no browser, acesse a URL montada acima
2. Leia as permissões solicitadas e clique em **"Permitir"**
3. Copie o token exibido na página seguinte, que será o valor da variável `TRELLO_TOKEN`

## Build

Gere o executável com `dotnet publish`. O binário será criado em `TrelloMcpStdio/bin/Release/net10.0/`:

**Windows:**
```bash
dotnet publish TrelloMcpStdio/TrelloMcpStdio.csproj -c Release -r win-x64 --self-contained false
```

**macOS:**
```bash
dotnet publish TrelloMcpStdio/TrelloMcpStdio.csproj -c Release -r osx-x64 --self-contained false
```

**Linux:**
```bash
dotnet publish TrelloMcpStdio/TrelloMcpStdio.csproj -c Release -r linux-x64 --self-contained false
```

## Integração com Claude Desktop

Após o build e com as credenciais do Trello em mãos (veja a seção anterior), registre o servidor via console substituindo `<sua-chave>` e `<seu-token>` pelos valores obtidos:

**Windows:**
```bash
claude mcp add trello -e TRELLO_API_KEY=<sua-chave> -e TRELLO_TOKEN=<seu-token> -- "C:\caminho\para\TrelloMcpStdio\bin\Release\net10.0\win-x64\publish\TrelloMcpStdio.exe"
```

**macOS/Linux:**
```bash
claude mcp add trello -e TRELLO_API_KEY=<sua-chave> -e TRELLO_TOKEN=<seu-token> -- /caminho/para/TrelloMcpStdio/bin/Release/net10.0/linux-x64/publish/TrelloMcpStdio
```

Isso equivale a adicionar a seguinte entrada no `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "trello": {
      "command": "C:\\caminho\\para\\TrelloMcpStdio\\bin\\Release\\net10.0\\win-x64\\publish\\TrelloMcpStdio.exe",
      "env": {
        "TRELLO_API_KEY": "<sua-chave>",
        "TRELLO_TOKEN": "<seu-token>"
      }
    }
  }
}
```

## Arquitetura

O servidor usa `stdio` como transporte MCP, padrão para integração com clientes como o Claude Desktop.

```
TrelloMcpStdio/
├── Program.cs          # Bootstrap via IHost; registra TrelloClient e sobe o servidor MCP
├── TrelloClient.cs     # Wrapper sobre a API REST do Trello; define os models
└── Tools/
    ├── BoardTools.cs      # Ferramentas GetBoards, GetBoardLists, GetBoardIdByName, GetListIdByName e GetListIdByBoardNameAndListName
    ├── CardTools.cs       # Ferramentas GetCard, GetCardsOnList e CreateCard
    └── ChecklistTools.cs  # Ferramentas CreateChecklist, AddCheckItem e GetCardChecklists
```

## Dependências

- [`ModelContextProtocol`](https://www.nuget.org/packages/ModelContextProtocol) 1.4.0
- `Microsoft.Extensions.Hosting` 10.0.0
- `Microsoft.Extensions.Http` 10.0.0
