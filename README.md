# SalesPatternDemo

API de vendas demonstrando o **Strategy Pattern** em .NET.

## Strategy Pattern

Cada tipo de cliente possui sua própria regra de desconto, selecionada em tempo de execução.

**Vantagens:**
- Elimina `if/else` extensos — cada regra é uma classe
- Fácil extensão — basta criar nova classe implementando `ISellRule`
- Testável isoladamente

### Estrutura

```
ISellRule (interface)
    │
    └── BaseSellRule (Template Method)
            ├── VipRule      → 15-30% escalonado
            ├── InternRule   → 30% uma vez/mês
            └── CustomerRule → 0%
```

### Exemplo: VipRule

```csharp
public class VipRule : BaseSellRule
{
    protected override Task<decimal> CalculateDiscountAsync(SaleProcessingData data)
    {
        decimal discount = data.OriginalAmount switch
        {
            >= 1000 => 25m,
            >= 500  => 20m,
            _       => 15m
        };
        return Task.FromResult(discount);
    }
}
```

### Seleção da Estratégia

```csharp
// SellRuleResolver
public ISellRule Resolve(CustomerType type) => type switch
{
    CustomerType.Vip    => _vipRule,
    CustomerType.Intern => _internRule,
    _                   => _customerRule
};
```

## Outros Padrões

| Padrão | Uso |
|--------|-----|
| Template Method | `BaseSellRule` define o algoritmo; filhas sobrescrevem passos |
| Repository | Abstrai acesso a dados (EF Core) |
| Resolver/Factory | Seleciona a estratégia por tipo de cliente |

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
