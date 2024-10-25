using TechLap.API.Models;

namespace TechLap.API.DTOs.Responses.ProductRespones;

public class SearchProductsRespones
{
    public bool IsSuccess { get; set; }
    public List<Product> Products { get; set; }
}