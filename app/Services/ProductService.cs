using app.Data;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .ToListAsync();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        return await _context.Products
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task<Product> AddProductAsync(Product product, List<int> tagIds)
    {
        // Add the product
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Add tags to product
        foreach (var tagId in tagIds)
        {
            await _context.Set<ProductTag>().AddAsync(new ProductTag
            {
                ProductId = product.Id,
                TagId = tagId
            });
        }
        
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product, List<int> tagIds)
    {
        // Update product details
        _context.Products.Update(product);
        
        // Remove existing tag associations
        var existingTags = await _context.Set<ProductTag>()
            .Where(pt => pt.ProductId == product.Id)
            .ToListAsync();
        
        _context.Set<ProductTag>().RemoveRange(existingTags);
        
        // Add new tag associations
        foreach (var tagId in tagIds)
        {
            await _context.Set<ProductTag>().AddAsync(new ProductTag
            {
                ProductId = product.Id,
                TagId = tagId
            });
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Product>> GetProductsByTagAsync(int? tagId)
    {
        var query = _context.Products
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .AsQueryable();
            
        if (tagId.HasValue)
        {
            query = query.Where(p => p.ProductTags.Any(pt => pt.TagId == tagId.Value));
        }
        
        return await query.ToListAsync();
    }
}
