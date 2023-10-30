using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class UpdateArticleRequest
{
    public IFormFile Image { get; set; }

    [Required]
    [MaxLength(195, ErrorMessage = ErrorMessages.ARTICLE_MAX_NAME)]
    [DefaultValue("New name of the article")]
    public string Name { get; set; }

    [Required]
    [DefaultValue("New text of the article")]
    public string Text { get; set; }

    [DefaultValue(new string[] { "#tag4", "#tag5", "#tag6" })]
    public List<string> Tags { get; set; }
}
