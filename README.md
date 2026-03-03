# SalesPatternDemo

API de vendas demonstrando padrões de projeto em .NET.

## Padrões Utilizados

### Strategy Pattern
Cada tipo de cliente (VIP, Interno, Cliente) tem sua própria regra de desconto. O sistema escolhe a estratégia correta em tempo de execução.

```
ISellRule (interface)
    ├── VipRule      → Descontos escalonados (15%-25%)
    ├── InternRule   → 30% uma vez por mês
    └── CustomerRule → Sem desconto
```

### Template Method Pattern
`BaseSellRule` define o esqueleto do algoritmo. Classes filhas sobrescrevem apenas os passos específicos.

```csharp
// BaseSellRule define a ordem:
// 1. Validate() 
// 2. CalculateDiscountAsync()
// 3. OnAfterApplyAsync()
```

### Repository Pattern
Abstrai o acesso a dados. `ICustomerRepository` e `ISaleRepository` isolam a lógica de negócio do Entity Framework.

### Resolver/Factory Pattern
`SellRuleResolver` seleciona qual regra aplicar baseado no tipo de cliente.

## Estrutura

```
SalesApi/
├── Controllers/    → Endpoints da API
├── Services/       → Orquestração de negócio
├── Rules/          → Regras de venda (Strategy)
├── Resolvers/      → Seleção de regras
├── Repositories/   → Acesso a dados
├── Models/         → DTOs, Entities, Enums
├── Mappers/        → AutoMapper profiles
└── Data/           → DbContext (EF Core)
```

## Tecnologias

- .NET 7
- Entity Framework Core (InMemory)
- AutoMapper
- Swagger

## Executar

```bash
cd SalesApi
dotnet run
```

Acesse: `https://localhost:7xxx/swagger`
