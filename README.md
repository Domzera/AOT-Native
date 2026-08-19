# AOT-Native — API de Usuários (Projeto de Estudos)

Projeto pessoal de estudos para explorar **Native AOT (Ahead-Of-Time) compilation** no .NET, usando o padrão **Minimal API**. É um CRUD de usuários com validação de campos, pronto para servir de base para outros projetos.

## 🎯 Objetivo do estudo

Este projeto foi feito para entender na prática como compilar uma API ASP.NET Core em **Native AOT** — uma forma de compilação que gera um executável nativo, com startup mais rápido e menor consumo de memória, sem depender do JIT (Just-In-Time compiler) do .NET em runtime.

## 🤔 Por que não tem "Controllers"?

Essa é a decisão arquitetural mais importante do projeto, então vale explicar:

**Controllers (padrão MVC)** dependem fortemente de **reflection** — o .NET precisa "descobrir" em tempo de execução quais classes são controllers, quais métodos são actions, e fazer o binding de rotas via atributos. Isso funciona muito bem no modelo tradicional (JIT), mas é justamente o tipo de coisa que o **Native AOT não suporta bem (ou não suporta de forma alguma)**, porque no AOT tudo precisa ser resolvido em tempo de compilação, não de execução.

Por isso, o próprio time do ASP.NET Core recomenda o padrão **Minimal API** para projetos compilados com AOT — as rotas são definidas diretamente (`app.MapGet`, `app.MapPost`, etc.), sem a camada de reflection do MVC.

Neste projeto, a pasta `Endpoint/` organiza essas rotas fora do `Program.cs` (que ficaria poluído se tudo estivesse ali), mantendo a mesma filosofia de Minimal API — só que separada por responsabilidade.

**Resumindo:** não usar Controllers aqui não é uma falha ou desvio de padrão — é a adaptação necessária e correta para um projeto compilado em Native AOT.

## 🛠️ Stack Técnica

- **.NET 9** com `PublishAot=true`
- **Minimal APIs**
- **PostgreSQL** (via Npgsql)
- **JWT Bearer Authentication**
- Serialização JSON customizada via `System.Text.Json` Source Generators (`AppJsonSerializerContext`) — necessária porque o AOT também não suporta a serialização JSON baseada em reflection

## 📁 Estrutura do Projeto

```
├── Endpoint/         # Definição das rotas HTTP (substitui os Controllers do MVC)
├── DTO/               # Objetos de transferência de dados (Register, Update, User)
├── Interface/         # Contrato do repositório de usuários
├── Repository/        # Acesso a dados (PostgreSQL)
├── Data/              # DbContext
├── Service/           # Serviços auxiliares
│   ├── AppJsonSerializerContext.cs   # Configuração de serialização JSON compatível com AOT
│   ├── Crypter.cs                     # Criptografia de dados sensíveis
│   ├── ErrorReport.cs                 # Registro de erros em tabela própria no banco
│   └── RegisterValidation.cs          # Validação manual de campos (sem FluentValidation/DataAnnotations em runtime)
└── UserQuerySQL.sql   # Queries SQL de apoio
```

## ✨ Funcionalidades

- Cadastro de usuário com validação manual de campos (sem depender de frameworks de validação baseados em reflection)
- Busca de usuário por ID
- Listagem de todos os usuários / apenas usuários ativos
- Atualização de dados do usuário
- Exclusão lógica (soft delete: altera o campo `ativo` de `1` para `0`, sem apagar o registro)
- Log de erros centralizado, salvo em tabela própria no banco de dados

## 🚀 Como rodar

### Pré-requisitos
- .NET 9 SDK
- PostgreSQL 17 (ou compatível)

### Passos

1. Clone o repositório:
   ```sh
   git clone https://github.com/Domzera/AOT-Native.git
   ```
2. Configure a connection string em `appsettings.json` (`DefaultConnection`)
3. Rode:
   ```sh
   dotnet build
   dotnet run
   ```

### Publicando com Native AOT

```sh
dotnet publish -c Release -r <RID> --self-contained
```
(substitua `<RID>` pelo runtime identifier do seu ambiente, ex: `win-x64`, `linux-x64`)

## 📚 Aprendizados registrados neste projeto

- Diferença prática entre Minimal API e Controllers/MVC
- Por que reflection é o principal obstáculo para compilar em Native AOT
- Como configurar serialização JSON compatível com AOT via Source Generators
- Validação manual de dados sem depender de DataAnnotations

## 📌 Status

Projeto de estudos concluído em sua proposta inicial (CRUD funcional) — usado como referência/base para explorar Native AOT em projetos futuros.
