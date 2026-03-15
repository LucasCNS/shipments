# APIs: Shipments & Costs

> Workspace contendo duas APIs RESTful independentes desenvolvidas com .NET 8.0, implementando os princípios da Clean Architecture e as melhores práticas de desenvolvimento de software.

## Sumário

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Stack Tecnológico](#stack-tecnológico)
- [Padrões de Design](#padrões-de-design)
- [Convenções de Commits](#convenções-de-commits)
- [Como Executar](#como-executar)
- [Testes](#testes)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Endpoints da API](#endpoints-da-api)
- [Referências e Links Úteis](#referências-e-links-úteis)

## Visão Geral

Este workspace contém **duas soluções .NET independentes**:

1. **Shipments API** - Gerenciamento de envios com rastreamento de status e atualização de estados
2. **Costs API** - Gerenciamento de custos de envio (em desenvolvimento)

Ambas as APIs implementam a Clean Architecture, são totalmente independentes e podem ser implantadas e escaladas separadamente. Compartilham apenas a infraestrutura de banco de dados PostgreSQL.

### Funcionalidades Principais - Shipments API

- Criar novos envios (Create)
- Listar envios com paginação e filtros (Read)
- Obter detalhes de um envio específico (Read by ID)
- Atualizar dados de envios existentes (Update)
- Atualizar status de envios com máquina de estados
- Validação robusta de dados de entrada
- API versionada (v1.0)
- Documentação Swagger/OpenAPI
- Testes unitários

### Funcionalidades Principais - Costs API

- Gerenciar custos de envio (em desenvolvimento)
- Estrutura preparada para extensão
- Reutiliza mesmos padrões e arquitetura da Shipments API

## Arquitetura

O projeto implementa a **Clean Architecture** em 4 camadas, garantindo separação de responsabilidades, facilidade de manutenção e testabilidade:

```
┌─────────────────────────────────────────────────────────┐
│              Shipments.Api                              │
│        (Camada de Apresentação)                         │
│  Controllers, Routing, Swagger, Versioning              │
└──────────────────────┬──────────────────────────────────┘
                       │ (Dependências)
┌──────────────────────▼──────────────────────────────────┐
│           Shipments.Application                         │
│         (Camada de Aplicação)                           │
│   Use Cases, Validators, Business Rules                │
└──────────────────────┬──────────────────────────────────┘
                       │ (Dependências)
┌──────────────────────▼──────────────────────────────────┐
│            Shipments.Domain                             │
│         (Camada de Domínio)                             │
│   Models, Entities, Value Objects, Contracts           │
└──────────────────────┬──────────────────────────────────┘
                       │ (Dependências)
┌──────────────────────▼──────────────────────────────────┐
│         Shipments.Infrastructure                        │
│       (Camada de Infraestrutura)                        │
│ Repositories, Persistence, External Services           │
└─────────────────────────────────────────────────────────┘
```

### Detalhamento das Camadas

#### 1. Shipments.Api (Camada de Apresentação)

Responsável pela interface HTTP da aplicação, coordenando requisições e respostas.

**Responsabilidades:**
- Definir endpoints REST
- Configurar API Versioning
- Integrar Swagger/OpenAPI
- Configurar Dependency Injection
- Mapear requisições HTTP para Use Cases
- Retornar respostas formatadas ao cliente

**Componentes Principais:**
- Controllers/V1/ShipmentsController.cs - Controller principal da API
- Program.cs - Configuração e bootstrap da aplicação
- appsettings.json - Configurações da aplicação

**Versioning:**
- API v1.0 configurada por padrão
- Suporte futuro para múltiplas versões

---

#### 2. Shipments.Application (Camada de Aplicação)

Encapsula a lógica de negócio e coordena a execução dos casos de uso.

**Responsabilidades:**
- Implementar Use Cases (fluxos de negócio)
- Validar dados de entrada
- Coordenar operações entre domínio e infraestrutura
- Manter regras de aplicação

**Componentes Principais:**

**Use Cases:**
- CreateShipment/ - Criar novo envio
- ListShipments/ - Listar todos os envios
- GetShipmentById/ - Obter envio específico
- UpdateShipment/ - Atualizar envio existente

**Validators:**
- CreateShipmentValidator.cs - Valida criação de envios
- ListShipmentsValidator.cs - Valida listagem/filtros
- GetShipmentByIdValidator.cs - Valida busca por ID
- UpdateShipmentValidator.cs - Valida atualização de envios

**Repositories:**
- IShipmentRepository.cs - Contrato do repositório (Abstração)

---

#### 3. Shipments.Domain (Camada de Domínio)

Define as entidades, regras de negócio e objetos de valor do domínio.

**Responsabilidades:**
- Definir Models e Entities
- Implementar Value Objects
- Especificar contratos (Interfaces)
- Encapsular regras de negócio essenciais

**Componentes Principais:**

**Models:**
- Shipment.cs - Entidade principal de envio
  - Id: Identificador único (GUID)
  - PackageName: Nome do pacote
  - Weight: Peso (decimal)
  - Dimensions: Objetos de valor com dimensões
  
- Dimensions.cs - Value Object para dimensões
  - Length: Comprimento
  - Width: Largura
  - Height: Altura

**Results:**
- Error.cs - Padrão para tratamento de erros

---

#### 4. Shipments.Infrastructure (Camada de Infraestrutura)

Implementa detalhes técnicos de persistência e acesso a dados.

**Responsabilidades:**
- Implementar interfaces de Repository
- Gerenciar persistência de dados
- Configurar acesso a banco de dados ou memória
- Implementar serviços técnicos

**Componentes Principais:**
- ShipmentInMemoryRepository.cs - Implementação em memória
- DependencyInjection.cs - Configuração de injeção de dependência

---

## Stack Tecnológico

| Tecnologia | Versão | Propósito |
|-----------|--------|----------|
| .NET | 8.0 | Framework base |
| C# | 12 | Linguagem de programação |
| ASP.NET Core | 8.0 | Framework web |
| Swashbuckle.AspNetCore | 6.6.2 | Documentação Swagger/OpenAPI |
| Asp.Versioning.Mvc | 8.1.0 | API Versioning |
| Microsoft.Extensions.DependencyInjection | Nativa | IoC Container |
| xUnit | (a adicionar) | Framework de testes |
| Moq | (a adicionar) | Mocking para testes |

## Padrões de Design

### 1. Use Case Pattern

Encapsula lógica de negócio específica em classes independentes.

**Benefícios:**
- Separação clara de responsabilidades
- Facilita testes unitários
- Reutilização de lógica
- Código mais legível

### 2. Repository Pattern

Abstrai a persistência de dados, permitindo trocar implementações facilmente.

```csharp
// Contrato
public interface IShipmentRepository
{
    Task<Shipment> CreateAsync(Shipment shipment);
    Task<Shipment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Shipment>> ListAsync();
    Task<Shipment> UpdateAsync(Shipment shipment);
}

// Implementação
public class ShipmentInMemoryRepository : IShipmentRepository
{
    // Implementação em memória
}
```

**Benefícios:**
- Desacoplamento de detalhes de persistência
- Facilita mudança de banco de dados
- Simplifica testes com mocks

### 3. Dependency Injection

Utiliza o container nativo do ASP.NET Core para gerenciar dependências.

```csharp
// No Program.cs
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddControllers();
```

**Benefícios:**
- Código mais testável
- Baixo acoplamento
- Facilita composição de objetos
- Padrão profissional

### 4. API Versioning

Permite manter múltiplas versões da API simultaneamente.

```csharp
// Configuração
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});
```

**Benefícios:**
- Suporte a evolução da API
- Compatibilidade com clientes antigos
- Transições suaves entre versões

## Convenções de Commits

Este projeto segue a especificação **Conventional Commits** (https://www.conventionalcommits.org/) para manter um histórico de commits limpo e semântico.

### Formato

```
<tipo>(<escopo>): <assunto>

<corpo>

<rodapé>
```

### Tipos de Commits

| Tipo | Descrição | Exemplo |
|------|-----------|---------|
| feat | Nova funcionalidade | feat(shipment): adicionar validação de peso |
| fix | Correção de bug | fix(repository): corrigir erro ao listar envios |
| docs | Mudanças na documentação | docs: atualizar README com exemplos |
| style | Formatação, sem mudança de lógica | style: remover espaços em branco |
| refactor | Refatoração de código | refactor(validator): simplificar validação |
| perf | Melhoria de performance | perf(memoryrepo): otimizar busca por ID |
| test | Adicionar ou atualizar testes | test(shipment): adicionar testes de criação |
| chore | Tarefas de manutenção | chore(deps): atualizar dependências |
| ci | Alterações em CI/CD | ci: configurar pipeline de testes |

### Exemplos Práticos para o Projeto Shipments

#### Exemplo 1: Nova Funcionalidade
```
feat(api): adicionar endpoint DELETE para remover envios

Implementa a exclusão de envios através de novo endpoint.
Adiciona validação para impedir exclusão de envios em trânsito.

Closes #42
```

#### Exemplo 2: Correção de Bug
```
fix(validator): corrigir validação de dimensões negativas

O validator de criação de shipment aceitava dimensões negativas.
Agora rejeita valores menores ou iguais a zero.

Fixes #15
```

#### Exemplo 3: Refatoração
```
refactor(usecase): extrair lógica de validação compartilhada

Move lógica de validação duplicada em múltiplos use cases
para uma classe utilitária reutilizável.

No breaking changes.
```

#### Exemplo 4: Teste
```
test(application): adicionar testes para ListShipmentsUseCase

- Testa listagem sem filtros
- Testa paginação com offset/limit
- Testa filtro por status
```

#### Exemplo 5: Dependências
```
chore(deps): atualizar Swashbuckle.AspNetCore para 6.7.0

Atualiza pacote Swashbuckle.AspNetCore de 6.6.2 para 6.7.0
para corrigir vulnerabilidade de segurança.
```

### Boas Práticas de Commits

**Faça:**
- Commits pequenos e focados
- Use verbos no imperativo ("adicionar", "corrigir", "refatorar")
- Seja descritivo no assunto
- Use corpo do commit para explicar o "por quê"
- Referência issues quando aplicável

**Evite:**
- Commits gigantes com múltiplas mudanças
- Mensagens vagas ("fix stuff", "update")
- Misturar diferentes tipos de mudanças
- Commits de merge sem propósito

## Como Executar

### Pré-requisitos

- .NET SDK 8.0 ou superior (https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker & Docker Compose (opcional, para execução containerizada)
- Visual Studio 2022 ou VS Code (https://code.visualstudio.com/)
- PostgreSQL (local) ou via Docker
- Conhecimento básico de C# e ASP.NET Core

### Execução Local (Desenvolvimento)

#### 1. Clonar o Repositório
```bash
git clone <url-do-repositorio>
cd shipments
```

#### 2. Restaurar Dependências - Shipments API
```bash
cd src/shipments-api
dotnet restore
```

#### 3. Restaurar Dependências - Costs API
```bash
cd src/costs-api
dotnet restore
```

#### 4. Executar a Aplicação Shipments

**Via Linha de Comando:**
```bash
cd src/shipments-api
dotnet run
```

**Via Visual Studio:**
1. Abra src/shipments-api/Shipments.sln
2. Define Shipments.Api como projeto de inicialização
3. Pressione F5 ou clique em "Run"

#### 5. Executar a Aplicação Costs

**Via Linha de Comando:**
```bash
cd src/costs-api
dotnet run
```

**Via Visual Studio:**
1. Abra src/costs-api/Costs.sln
2. Define Costs.Api como projeto de inicialização
3. Pressione F5 ou clique em "Run"

#### 6. Acessar as Aplicações

**Shipments API:**
- HTTP: http://localhost:5067
- HTTPS: https://localhost:7259
- Swagger: https://localhost:7259/swagger/index.html

**Costs API:**
- HTTP: http://localhost:5068
- HTTPS: https://localhost:7260
- Swagger: https://localhost:7260/swagger/index.html

### Execução com Docker Compose

#### Pré-requisitos Docker
- Docker Desktop instalado e executando
- Docker Compose (incluído no Docker Desktop)

#### Passos

1. **Certifique-se que está no diretório raiz do projeto:**
```bash
cd shipments
```

2. **Construir e iniciar todos os serviços:**
```bash
docker-compose up --build
```

3. **Para executar em background:**
```bash
docker-compose up -d --build
```

4. **Visualizar logs:**
```bash
docker-compose logs -f
```

5. **Parar os serviços:**
```bash
docker-compose down
```

#### Acessar as APIs via Docker

**Shipments API:**
- HTTP: http://localhost:8080
- Swagger: http://localhost:8080/swagger/index.html

**Costs API:**
- HTTP: http://localhost:8081
- Swagger: http://localhost:8081/swagger/index.html

**Banco de Dados PostgreSQL:**
- Host: localhost
- Port: 5432
- User: postgres
- Password: postgres
- Databases: shipments, costs

### Configurações de Desenvolvimento

#### Shipments API
Arquivo: `src/shipments-api/Shipments.Api/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=shipments;Username=postgres;Password=postgres;"
  }
}
```

#### Costs API
Arquivo: `src/costs-api/Costs.Api/appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=costs;Username=postgres;Password=postgres;"
  }
}
```


## Testes

### Estrutura de Testes

O projeto inclui:
- Shipments.UnitTests/ - Testes unitários

**Organize os testes:**
```
Shipments.UnitTests/
├── Application/
│   ├── UseCases/
│   │   ├── CreateShipmentUseCaseTests.cs
│   │   ├── ListShipmentsUseCaseTests.cs
│   │   ├── GetShipmentByIdUseCaseTests.cs
│   │   └── UpdateShipmentUseCaseTests.cs
│   └── Validators/
│       ├── CreateShipmentValidatorTests.cs
│       ├── ListShipmentsValidatorTests.cs
│       └── ...
└── Infrastructure/
    └── Repositories/
        └── ShipmentInMemoryRepositoryTests.cs
```

### Executar Testes

**Todos os testes:**
```bash
dotnet test
```

**Testes com Coverage:**
```bash
dotnet test /p:CollectCoverage=true
```

**Testes específicos:**
```bash
dotnet test --filter "FullyQualifiedName~CreateShipmentUseCaseTests"
```

**Watch Mode (reexecuta ao salvar):**
```bash
dotnet watch test
```

### Exemplo de Teste Unitário

```csharp
public class CreateShipmentUseCaseTests
{
    private readonly Mock<IShipmentRepository> _repositoryMock;
    private readonly CreateShipmentUseCase _useCase;

    public CreateShipmentUseCaseTests()
    {
        _repositoryMock = new Mock<IShipmentRepository>();
        _useCase = new CreateShipmentUseCase(_repositoryMock.Object);
    }

    [Fact]
    public async Task Execute_WithValidInput_CreatesShipment()
    {
        // Arrange
        var request = new CreateShipmentRequest 
        { 
            PackageName = "Test Package",
            Weight = 10.5m
        };

        // Act
        var result = await _useCase.Execute(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Package", result.PackageName);
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<Shipment>()), Times.Once);
    }
}
```

## Estrutura do Projeto

```
shipments/
├── docs/
│   ├── projeto-shipments.md          # Documentação do projeto
│   ├── dotnet-template.md            # Molde de implementação
│   ├── curl-examples.md              # Exemplos cURL
│   └── test-list-shipments-guide.md  # Guia de testes
│
├── src/
│   ├── shipments-api/                # API de Shipments (Solução Independente)
│   │   ├── Shipments.sln             # Solution .NET
│   │   │
│   │   ├── Shipments.Api/            # APRESENTAÇÃO
│   │   │   ├── Controllers/
│   │   │   │   └── V1/
│   │   │   │       └── ShipmentsController.cs
│   │   │   ├── Program.cs
│   │   │   ├── Shipments.Api.csproj
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   └── Shipments.Api.http
│   │   │
│   │   ├── Shipments.Application/    # APLICAÇÃO
│   │   │   ├── UseCases/
│   │   │   │   ├── CreateShipment/
│   │   │   │   ├── GetShipmentById/
│   │   │   │   ├── ListShipments/
│   │   │   │   └── UpdateShipment/
│   │   │   ├── Validators/
│   │   │   │   ├── CreateShipmentValidator.cs
│   │   │   │   ├── GetShipmentByIdValidator.cs
│   │   │   │   ├── ListShipmentsValidator.cs
│   │   │   │   └── UpdateShipmentValidator.cs
│   │   │   ├── Repositories/
│   │   │   │   └── IShipmentRepository.cs
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Shipments.Application.csproj
│   │   │
│   │   ├── Shipments.Domain/         # DOMÍNIO
│   │   │   ├── Models/
│   │   │   │   ├── Shipment.cs
│   │   │   │   └── Dimensions.cs
│   │   │   ├── Results/
│   │   │   │   └── Error.cs
│   │   │   └── Shipments.Domain.csproj
│   │   │
│   │   ├── Shipments.Infrastructure/ # INFRAESTRUTURA
│   │   │   ├── Persistence/
│   │   │   │   └── ShipmentInMemoryRepository.cs
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Shipments.Infrastructure.csproj
│   │   │
│   │   └── Shipments.UnitTests/      # TESTES
│   │       ├── Application/
│   │       │   ├── UseCases/
│   │       │   └── Validators/
│   │       └── Shipments.UnitTests.csproj
│   │
│   ├── costs-api/                    # API de Costs (Solução Independente)
│   │   ├── Costs.sln                 # Solution .NET
│   │   │
│   │   ├── Costs.Api/                # APRESENTAÇÃO
│   │   │   ├── Controllers/
│   │   │   │   └── V1/
│   │   │   ├── Properties/
│   │   │   ├── Program.cs
│   │   │   ├── Costs.Api.csproj
│   │   │   ├── appsettings.json
│   │   │   └── appsettings.Development.json
│   │   │
│   │   ├── Costs.Application/        # APLICAÇÃO
│   │   │   ├── UseCases/
│   │   │   ├── Validators/
│   │   │   ├── Repositories/
│   │   │   ├── Services/
│   │   │   ├── Results/
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Costs.Application.csproj
│   │   │
│   │   ├── Costs.Domain/             # DOMÍNIO
│   │   │   ├── Models/
│   │   │   ├── Results/
│   │   │   └── Costs.Domain.csproj
│   │   │
│   │   ├── Costs.Infrastructure/     # INFRAESTRUTURA
│   │   │   ├── Persistence/
│   │   │   │   └── CostsDbContext.cs
│   │   │   ├── Migrations/
│   │   │   ├── DependencyInjection.cs
│   │   │   └── Costs.Infrastructure.csproj
│   │   │
│   │   └── Costs.UnitTests/          # TESTES
│   │       ├── Application/
│   │       │   ├── UseCases/
│   │       │   └── Validators/
│   │       └── Costs.UnitTests.csproj
│
├── scripts/
│   ├── init-db.sql                  # Script inicialização Shipments DB
│   └── init-costs-db.sql            # Script inicialização Costs DB
│
├── collections/                     # Bruno API Client Collections
│   ├── bruno.json
│   └── ...
│
├── docker-compose.yml               # Configuração multi-serviço
├── Dockerfile                       # Build de ambas as APIs
├── curl-tests-list-shipments.sh     # Script de teste cURL
├── test-list-shipments.ps1          # Script PowerShell
├── rest-client-tests.http           # Testes REST Client
├── README.md                        # Este arquivo
└── LICENSE
```

### Características da Estrutura

**Independência das Soluções:**
- Cada API reside em seu próprio diretório (`src/shipments-api/` e `src/costs-api/`)
- Cada solução possui seu próprio `.sln` file
- Nenhuma dependência entre os projetos das duas APIs
- Cada uma pode ser desenvolvida e deployada independentemente

## Endpoints da API

### Base URL
```
https://localhost:5001/v1
```

### Criar Envio
```http
POST /shipments
Content-Type: application/json
Creator: user_name

{
  "packageName": "Eletrônico",
  "weight": 2.5,
  "dimensions": {
    "length": 10,
    "width": 5,
    "height": 8
  },
  "shippingCost": 100,
  "destinationAddress": "Rua Principal, 123"
}
```

**Resposta (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "packageName": "Eletrônico",
  "weight": 2.5,
  "dimensions": {
    "length": 10,
    "width": 5,
    "height": 8
  },
  "shippingCost": 100,
  "destinationAddress": "Rua Principal, 123",
  "dateCreated": "2026-02-24T10:00:00Z",
  "dateLastUpdated": "2026-02-24T10:00:00Z",
  "creator": "user_name",
  "status": "pending"
}
```

### Listar Envios
```http
GET /shipments?offset=0&limit=10
Creator: user_name
```

**Resposta (200 OK):**
```json
{
  "total": 1,
  "offset": 0,
  "limit": 10,
  "results": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "packageName": "Eletrônico",
      "weight": 2.5,
      "dimensions": {...}
    }
  ]
}
```

### Obter Envio por ID
```http
GET /shipments/{id}
Creator: user_name
```

**Resposta (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "packageName": "Eletrônico",
  "weight": 2.5,
  "dimensions": {...}
}
```

### Atualizar Envio
```http
PUT /shipments/{id}
Content-Type: application/json
Creator: user_name

{
  "packageName": "Eletrônico Updated",
  "weight": 3.0,
  "dimensions": {...}
}
```

**Resposta (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "packageName": "Eletrônico Updated",
  "weight": 3.0,
  "dimensions": {...}
}
```

## Referências e Links Úteis

### Clean Architecture
- Clean Architecture - Robert C. Martin (Uncle Bob) - https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- Clean Architecture in .NET - GitHub - https://github.com/ardalis/CleanArchitecture

### Padrões de Design
- Design Patterns - Refactoring.Guru - https://refactoring.guru/design-patterns
- Enterprise Integration Patterns - https://www.enterpriseintegrationpatterns.com/
- Repository Pattern - https://martinfowler.com/eaaCatalog/repository.html
- Use Case Pattern - https://en.wikipedia.org/wiki/Use_case

### .NET e ASP.NET Core
- Documentação Oficial .NET - https://learn.microsoft.com/en-us/dotnet/
- ASP.NET Core Documentation - https://learn.microsoft.com/en-us/aspnet/core/
- Dependency Injection em .NET - https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- API Versioning em ASP.NET Core - https://github.com/dotnet/aspnet-api-versioning

### Conventional Commits
- Conventional Commits Official - https://www.conventionalcommits.org/
- Conventional Commits - PT-BR - https://www.conventionalcommits.org/pt-br/

### Ferramentas Úteis
- Swagger/OpenAPI - https://swagger.io/
- Swashbuckle.AspNetCore - https://github.com/domaindrivendev/Swashbuckle.AspNetCore
- xUnit.net - https://xunit.net/
- Moq - Mocking Framework - https://github.com/moq/moq4

### Segurança e Boas Práticas
- OWASP Top 10 - https://owasp.org/www-project-top-ten/
- Microsoft Security Best Practices - https://learn.microsoft.com/en-us/aspnet/core/security/
- C# Coding Conventions - https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

## Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (git checkout -b feat(escopo): descricao)
3. Commit suas mudanças (git commit -m 'feat(escopo): descricao')
4. Push para a branch (git push origin feat(escopo): descricao)
5. Abra um Pull Request
6. Siga o padrão de Conventional Commits

## Licença

Este projeto é licenciado sob a MIT License - veja o arquivo LICENSE para detalhes.

---

Desenvolvido com dedicação para explorar e demonstrar os princípios de Clean Architecture em .NET
