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

### Implementação

**1. Interface** — define o contrato:
```csharp
public interface ISellRule
{
    Task<SaleProcessingData> ApplyRuleAsync(SaleProcessingData data);
    string RuleName { get; }
}
```

**2. Estratégia concreta** — VipRule com desconto escalonado:
```csharp
public class VipRule : BaseSellRule
{
    public override string RuleName => "VipRule";
    protected override int DefaultMaxItems => 50;

    protected override Task<decimal> CalculateDiscountAsync(SaleProcessingData data)
    {
        decimal discount = data.OriginalAmount switch
        {
            >= 1000 => 25m,
            >= 500  => 20m,
            _       => 15m
        };

        if (data.ItemsCount >= 10)
            discount += 5m;

        return Task.FromResult(discount);
    }
}
```

**3. Resolver** — fábrica que retorna a estratégia correta:
```csharp
public class SellRuleResolver : ISellRuleResolver
{
    private readonly ICustomerRepository _customerRepository;

    public SellRuleResolver(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public ISellRule Resolve(CustomerType customerType) => customerType switch
    {
        CustomerType.Vip    => new VipRule(),
        CustomerType.Intern => new InternRule(_customerRepository),
        _                   => new CustomerRule()
    };
}
```

**4. Strategy + Resolver juntos** — o Service usa o Resolver para obter a estratégia e aplicá-la:
```csharp
public class SellService : ISellService
{
    private readonly ISellRuleResolver _ruleResolver;
    // ...

    public async Task<SaleResponseDto> ProcessSaleAsync(SaleRequestDto request)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        
        var data = new SaleProcessingData
        {
            Customer = customer,
            OriginalAmount = request.Amount,
            ItemsCount = request.ItemsCount
        };

        // 1. Resolver seleciona a estratégia baseada no tipo
        ISellRule rule = _ruleResolver.Resolve(request.CustomerType);
        
        // 2. Estratégia é aplicada (polimorfismo)
        data = await rule.ApplyRuleAsync(data);

        // O Service não sabe qual regra foi usada — apenas que funcionou
        // ...
    }
}
```

> O Service depende apenas de `ISellRuleResolver` e `ISellRule`. Adicionar um novo tipo de cliente (ex: `StudentRule`) não requer alterações no Service — apenas criar a nova classe e registrar no Resolver.

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
