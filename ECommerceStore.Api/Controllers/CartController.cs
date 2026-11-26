using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ECommerceStore.Api.DTOs;
using ECommerceStore.Domain.Entities;
using ECommerceStore.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var items = await _context.CartItems
            .AsNoTracking()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Id)
            .Select(c => ToDto(c))
            .ToListAsync();

        return Ok(new CartDto
        {
            Items = items,
            TotalPrice = items.Sum(i => i.TotalPrice)
        });
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartItemDto>> AddItem(AddToCartDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) return NotFound(new { message = "Product not found." });

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        var newQuantity = (item?.Quantity ?? 0) + dto.Quantity;
        if (newQuantity > product.Stock)
        {
            return BadRequest(new { message = "Not enough stock." });
        }

        if (item == null)
        {
            item = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Product = product
            };
            _context.CartItems.Add(item);
        }
        else
        {
            item.Quantity = newQuantity;
            item.Product = product;
        }

        await _context.SaveChangesAsync();
        return Ok(ToDto(item));
    }

    [HttpPut("items/{productId:int}")]
    public async Task<ActionResult<CartItemDto>> UpdateItem(int productId, UpdateCartItemDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var item = await _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (item == null) return NotFound(new { message = "Cart item not found." });
        if (dto.Quantity > item.Product!.Stock)
        {
            return BadRequest(new { message = "Not enough stock." });
        }

        item.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();
        return Ok(ToDto(item));
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (item == null) return NotFound(new { message = "Cart item not found." });

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var items = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }

    private static CartItemDto ToDto(CartItem item)
    {
        return new CartItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product!.Name,
            UnitPrice = item.Product.Price,
            Quantity = item.Quantity,
            TotalPrice = item.Product.Price * item.Quantity
        };
    }
}