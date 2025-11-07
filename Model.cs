namespace EF10_QueryCache;

public class ProductEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
}