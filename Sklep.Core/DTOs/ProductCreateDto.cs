using Microsoft.AspNetCore.Http;

namespace Sklep.Core.DTOs;

public class ProductCreateDto : ProductBaseDto
{
    public List<int> SelectedTags { get; set; } = new List<int>();
    public List<IFormFile> Images { get; set; } = new List<IFormFile>();
}