using Microsoft.AspNetCore.Mvc;
using SalesApi.Models.DTOs;
using SalesApi.Services;

namespace SalesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISellService _sellService;

    public SalesController(ISellService sellService)
    {
        _sellService = sellService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessSale([FromBody] SaleRequestDto request)
    {
        var result = await _sellService.ProcessSaleAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("sample-customers")]
    public IActionResult GetSampleCustomers() => Ok(new
    {
        Vip = new { Id = "11111111-1111-1111-1111-111111111111", Type = 1 },
        Intern = new { Id = "22222222-2222-2222-2222-222222222222", Type = 2 },
        Customer = new { Id = "33333333-3333-3333-3333-333333333333", Type = 0 }
    });
}
