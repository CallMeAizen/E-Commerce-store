using ECommerceStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products = new()
    {
        new() { Id = 1, Name = "Headphones", Price = 79.99m, Stock = 10 },
        new() { Id = 2, Name = "Keyboard", Price = 49.99m, Stock = 25 },
        new() { Id = 3, Name = "Mouse", Price = 29.99m, Stock = 50 }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(_products);
    }
}