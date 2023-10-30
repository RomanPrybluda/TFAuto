using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class CreateArticleRequest
{
    [Required]
    public IFormFile Image { get; set; }

    [Required]
    [MaxLength(195, ErrorMessage = ErrorMessages.ARTICLE_MAX_NAME)]
    [DefaultValue("Name of the article")]
    public string Name { get; set; }

    [Required]
    [DefaultValue("Text of the article")]
    public string Text { get; set; }

    [DefaultValue(new string[] { "#tag1", "#tag2", "#tag3" })]
    public List<string> Tags { get; set; }
}
