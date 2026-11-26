namespace ECommerceStore.Api.DTOs;

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}