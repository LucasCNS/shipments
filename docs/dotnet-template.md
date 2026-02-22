# Clean Architecture .NET — Template de Estrutura

> Substitua `{Project}` pelo nome do projeto e `{Entity}` pela entidade principal.

```
src/
├── {Project}.sln
│
├── {Project}.Api/
│   ├── {Project}.Api.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── {Project}.Api.http
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Controllers/
│   │   ├── Extensions.cs
│   │   └── V1/
│   │       └── {Entity}sController.cs
│   └── Swagger/
│       └── ConfigureSwaggerOptions.cs
│
├── {Project}.Application/
│   ├── {Project}.Application.csproj
│   ├── DependencyInjection.cs
│   ├── Repositories/
│   │   └── I{Entity}Repository.cs
│   └── UseCases/
│       ├── IUseCase.cs
│       ├── Page.cs
│       ├── Create{Entity}/
│       │   ├── ICreate{Entity}UseCase.cs
│       │   ├── Create{Entity}UseCase.cs
│       │   ├── Create{Entity}Input.cs
│       │   └── Create{Entity}Output.cs
│       ├── Delete{Entity}ById/
│       │   ├── IDelete{Entity}ByIdUseCase.cs
│       │   ├── Delete{Entity}ByIdUseCase.cs
│       │   ├── Delete{Entity}ByIdInput.cs
│       │   └── Delete{Entity}ByIdOutput.cs
│       ├── Get{Entity}ById/
│       │   ├── IGet{Entity}ByIdUseCase.cs
│       │   ├── Get{Entity}ByIdUseCase.cs
│       │   ├── Get{Entity}ByIdInput.cs
│       │   └── Get{Entity}ByIdOutput.cs
│       ├── Get{Entity}sPaged/
│       │   ├── IGet{Entity}sPagedUseCase.cs
│       │   ├── Get{Entity}sPagedUseCase.cs
│       │   ├── Get{Entity}sPagedInput.cs
│       │   └── Get{Entity}sPagedOutput.cs
│       └── Update{Entity}/
│           ├── IUpdate{Entity}UseCase.cs
│           ├── Update{Entity}UseCase.cs
│           ├── Update{Entity}Input.cs
│           └── Update{Entity}Output.cs
│
├── {Project}.Domain/
│   ├── {Project}.Domain.csproj
│   ├── Models/
│   │   ├── IValidatable.cs
│   │   ├── Extensions.cs
│   │   └── {Entity}.cs
│   └── Results/
│       └── Error.cs
│
├── {Project}.Infrastructure/
│   ├── {Project}.Infrastructure.csproj
│   ├── DependencyInjection.cs
│   └── Persistence/
│       └── Repositories/
│           ├── {Entity}Repository.cs
│           └── Models/
│               └── {Entity}Document.cs
│
└── {Project}.UnitTests/
    ├── {Project}.UnitTests.csproj
    ├── Application/
    │   └── UseCases/
    │       ├── Create{Entity}UseCaseTests.cs
    │       ├── Delete{Entity}ByIdUseCaseTests.cs
    │       ├── Get{Entity}ByIdUseCaseTests.cs
    │       ├── Get{Entity}sPagedUseCaseTests.cs
    │       └── Update{Entity}UseCaseTests.cs
    └── Domain/
        └── Models/
            └── {Entity}Tests.cs
```

---

## Modelo — `Error`

```
Error
├── Code                   : string   — identificador do erro (ex: "EntityNotFound")
├── Message                : string   — descrição legível do erro
└── CorrespondingStatusCode: int      — HTTP status correspondente (400, 404, 409…)
```

Retornado como `Error?` nos Outputs dos use cases. Quando `null`, a operação foi bem-sucedida.

Exposto na API como RFC 7807 `ProblemDetails`:

```
ProblemDetails
├── status    → Error.CorrespondingStatusCode
├── title     → "API error"
├── detail    → Error.Message
├── instance  → request path
└── extensions
    └── errorCode → Error.Code
```
