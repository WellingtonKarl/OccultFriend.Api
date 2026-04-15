<h1 align="center">🎁 Amigo Oculto API</h1>

<p align="center">
  <img src="https://img.shields.io/static/v1?label=API&message=OCCULTFRIEND&color=33eb1f&style=for-the-badge&logo=ghost"/>
  <img src="https://img.shields.io/badge/.NET-6.0-512BD4?style=for-the-badge&logo=dotnet"/>
  <img src="https://img.shields.io/badge/SQL%20Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server"/>
  <img src="https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white"/>
</p>

<p align="center">
  Aplicação para gerenciar sorteios de Amigo Oculto com suporte a participantes adultos e crianças, envio de e-mails automáticos e upload de imagens.
</p>

---

## Arquitetura

```mermaid
graph TD
    Client["🌐 Cliente (HTTP)"]

    subgraph API["OccultFriend.API"]
        Controller["FriendController\n(REST Endpoints)"]
        Auth["JWT Middleware\n(Autenticação)"]
        Swagger["Swagger UI"]
        Health["Health Checks\n/monitor"]
    end

    subgraph Service["OccultFriend.Service"]
        FriendSvc["ServicesFriend\n(Lógica de Sorteio)"]
        TokenSvc["TokenService\n(JWT)"]
        EmailSvc["EmailServices\n(Envio de E-mails)"]
        ImgSvc["ImgbbService\n(Upload de Imagens)"]
        Template["EmailTemplate\n(Templates HTML)"]
    end

    subgraph Domain["OccultFriend.Domain"]
        Entities["Entities\n(Friend, FriendDto)"]
        Interfaces["Interfaces\n(IRepositoriesFriend,\nIServicesFriend, etc.)"]
    end

    subgraph Repository["OccultFriend.Repository"]
        SqlRepo["RepositoriesFriend\n(SQL Server + Dapper)"]
        MongoRepo["FriendRepository\n(MongoDB)"]
    end

    subgraph External["Serviços Externos"]
        SMTP["📧 SMTP Server"]
        ImgBB["🖼️ ImgBB API"]
        SqlServer["🗄️ SQL Server"]
        Mongo["🍃 MongoDB"]
    end

    Client -->|"HTTPS"| Controller
    Controller --> Auth
    Controller --> FriendSvc
    Controller --> TokenSvc
    Controller --> ImgSvc
    FriendSvc --> EmailSvc
    FriendSvc --> SqlRepo
    FriendSvc --> MongoRepo
    EmailSvc --> Template
    EmailSvc --> SMTP
    ImgSvc --> ImgBB
    SqlRepo --> SqlServer
    MongoRepo --> Mongo
    Service --> Domain
    Repository --> Domain
```

---

## Fluxo do Sorteio

```mermaid
sequenceDiagram
    actor Admin
    participant API
    participant ServicesFriend
    participant Repository
    participant EmailServices
    participant SMTP

    Admin->>API: GET /friend/draw?childPlay=bool
    API->>API: Valida JWT Token
    API->>ServicesFriend: Draw(childWillPlay)

    ServicesFriend->>Repository: GetAll()
    Repository-->>ServicesFriend: Lista de participantes

    opt childPlay = true
        ServicesFriend->>Repository: Childdrens()
        Repository-->>ServicesFriend: Lista de crianças
    end

    ServicesFriend->>ServicesFriend: Fisher-Yates Shuffle\n(embaralha sorteio)
    ServicesFriend->>ServicesFriend: Valida auto-sorteio

    alt Sorteio sem repetição
        loop Para cada participante adulto
            ServicesFriend->>EmailServices: SendDrawEmail(tirador, tirado)
            EmailServices->>SMTP: Envia e-mail com template HTML
        end
        loop Para cada criança
            ServicesFriend->>EmailServices: SendResponsibleEmail(responsável, criança, tirado)
            EmailServices->>SMTP: Envia e-mail ao responsável
        end
        ServicesFriend-->>API: Sucesso
        API-->>Admin: 200 OK
    else Auto-sorteio detectado
        ServicesFriend->>EmailServices: SendDuplicateEmail(duplicados)
        EmailServices->>SMTP: Notifica admin
        ServicesFriend-->>API: Erro
        API-->>Admin: 400 Bad Request
    end
```

---

## Fluxo de Autenticação

```mermaid
sequenceDiagram
    actor User
    participant API
    participant TokenService
    participant Repository

    User->>API: POST /friend/login\n{name, password}
    API->>Repository: Get(name, password)
    Repository-->>API: Friend | null

    alt Credenciais válidas
        API->>TokenService: GenerateToken(friend)
        TokenService-->>API: JWT Token (30 min)
        API-->>User: 200 OK + Token
    else Credenciais inválidas
        API-->>User: 400 Bad Request
    end

    User->>API: GET /friend (Authorization: Bearer <token>)
    API->>API: Valida JWT
    API-->>User: 200 OK + Lista de amigos
```

---

## Modelo de Dados

```mermaid
erDiagram
    FRIEND {
        int Id PK
        varchar50 Name
        varchar50 Password
        varcharMAX Description
        varchar50 Email
        smalldatetime Data
        varchar2000 ImagePath
        bit IsChildreen
    }
```

---

## Endpoints da API

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| `POST` | `/friend/login` | Não | Autenticação, retorna JWT |
| `POST` | `/friend` | Não | Cadastro de participante (multipart) |
| `GET` | `/friend` | Sim | Lista todos os participantes |
| `GET` | `/friend/{id}` | Sim | Busca participante por ID |
| `GET` | `/friend/draw?childPlay=bool` | Sim | Executa o sorteio |
| `PUT` | `/friend` | Sim | Atualiza participante |
| `DELETE` | `/friend/{id}` | Sim | Remove participante |
| `GET` | `/monitor` | Não | Health check UI |

---

## Seleção de Banco de Dados

```mermaid
flowchart LR
    Config["appsettings.json"]
    Check{"ConnectionStrings\n:connection\npreenchida?"}
    SQL["SQL Server\n+ Dapper"]
    Mongo["MongoDB\nDriver"]
    Interface["IRepositoriesFriend"]

    Config --> Check
    Check -->|"Sim"| SQL
    Check -->|"Não"| Mongo
    SQL --> Interface
    Mongo --> Interface
```

---

## Tecnologias

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server 2019 (local)](https://www.microsoft.com/pt-br/sql-server/sql-server-2019)
- [MongoDB (produção)](https://docs.mongodb.com/guides/)
- [Dapper ORM](https://dapper-tutorial.net/step-by-step-tutorial)
- [JWT Bearer Authentication](https://docs.microsoft.com/pt-br/aspnet/core/security/authentication/jwt-authn)
- [ImgBB API](https://api.imgbb.com/)
- [xUnit + Moq (testes)](https://xunit.net/)
- [Swagger / Swashbuckle](https://swagger.io/)
- [Azure Pipelines (CI/CD)](https://azure.microsoft.com/pt-br/products/devops/pipelines)

---

## Autor

<a href="https://github.com/WellingtonKarl">
 <img style="border-radius: 50%;" src="https://avatars.githubusercontent.com/u/76018356?v=4" width="100px;" alt=""/>
 <br />
 <sub><b>Wellington Karl</b></sub>
</a>

Feito com ❤️ por Wellington Karl 👋🏽

[![Linkedin Badge](https://img.shields.io/badge/-Wellington-blue?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/wellingtonkarl/)](https://www.linkedin.com/in/wellingtonkarl/)
[![Gmail Badge](https://img.shields.io/badge/-wellington.regiskarl@gmail.com-c14438?style=flat-square&logo=Gmail&logoColor=white&link=mailto:wellington.regiskarl@gmail.com)](mailto:wellington.regiskarl@gmail.com)
