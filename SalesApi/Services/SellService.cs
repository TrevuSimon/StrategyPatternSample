using AutoMapper;
using SalesApi.Resolvers;
using SalesApi.Models.DTOs;
using SalesApi.Models.Entities;
using SalesApi.Repositories;

namespace SalesApi.Services;

public class SellService : ISellService
{
    private readonly ISellRuleResolver _ruleResolver;
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SellService> _logger;

    public SellService(
        ISellRuleResolver ruleResolver,
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<SellService> logger)
    {
        _ruleResolver = ruleResolver;
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleResponseDto> ProcessSaleAsync(SaleRequestDto request)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
            return SaleResponseDto.Failure($"Customer {request.CustomerId} not found.");

        var data = new SaleProcessingData
        {
            Customer = customer,
            OriginalAmount = request.Amount,
            ItemsCount = request.ItemsCount
        };

        var rule = _ruleResolver.Resolve(request.CustomerType);
        data = await rule.ApplyRuleAsync(data);

        if (!data.IsValid)
            return SaleResponseDto.Failure(data.ValidationError!);

        var sale = await _saleRepository.CreateSaleAsync(_mapper.Map<Sale>(data));

        _logger.LogInformation("Sale {Id} created: {Original} → {Final} ({Rule})",
            sale.Id, sale.OriginalAmount, sale.FinalAmount, sale.RuleApplied);

        return _mapper.Map<SaleResponseDto>(sale);
    }
}
