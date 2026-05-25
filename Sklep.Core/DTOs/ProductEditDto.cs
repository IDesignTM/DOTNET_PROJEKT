using Microsoft.AspNetCore.Http;

namespace Sklep.Core.DTOs;

public class ProductEditDto : ProductBaseDto
{
    public int Id { get; set; }
    
    public List<int> SelectedTags { get; set; } = new List<int>();
    public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    
    public List<int> ImagesToDelete { get; set; } = new List<int>();
    public List<ProductImageDto> ExistingImages { get; set; } = new List<ProductImageDto>();
}