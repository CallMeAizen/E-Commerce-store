using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.Api.DTOs;

public class UpdateCartItemDto
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}