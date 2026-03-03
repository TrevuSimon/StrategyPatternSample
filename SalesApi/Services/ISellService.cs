using SalesApi.Models.DTOs;

namespace SalesApi.Services;

public interface ISellService
{
    Task<SaleResponseDto> ProcessSaleAsync(SaleRequestDto request);
}
