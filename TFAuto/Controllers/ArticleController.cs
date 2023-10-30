using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services;
using TFAuto.Domain.Services.ArticlePage;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;
using TFAuto.Domain.Services.Authentication.Constants;

namespace TFAuto.WebApp.Controllers;

[Route("articles")]
[ApiController]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpPost]
    [Authorize(Policy = PermissionId.EDIT_ARTICLES)]
    [SwaggerOperation(
     Summary = "Create an article",
     Description = "Creates an article")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(CreateArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<CreateArticleResponse>> CreateArticleAsyncs([FromForm] CreateArticleRequest articleRequest)
    {
        var createdArticle = await _articleService.CreateArticleAsync(articleRequest);
        return Ok(createdArticle);
    }

    [HttpPut("{id:Guid}")]
    [Authorize(Policy = PermissionId.MANAGE_ARTICLES)]
    [SwaggerOperation(
     Summary = "Update an article",
     Description = "Updates an article by id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UpdateArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<UpdateArticleResponse>> UpdateArticleAsync([FromRoute] Guid id, [FromForm] UpdateArticleRequest articleRequest)
    {
        var updatedArticle = await _articleService.UpdateArticleAsync(id, articleRequest);
        return Ok(updatedArticle);
    }

    [HttpGet("{id:Guid}")]
    [SwaggerOperation(
     Summary = "Retrieve an article by id",
     Description = "Retrieves an article with tags and an image")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetSingleArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetSingleArticleResponse>> GetArticleAsync([FromRoute] Guid id)
    {
        var retrievedArticle = await _articleService.GetArticleAsync(id);
        return Ok(retrievedArticle);
    }

    [HttpGet]
    [SwaggerOperation(
     Summary = "Retrieve articles with pagination",
     Description = "Retrieves articles by skip and take parameters and sorting")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllArticlesResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetAllArticlesResponse>> GetAllArticlesAsync([FromQuery] GetAllArticlesRequest paginationRequest)
    {
        var retrievedArticles = await _articleService.GetAllArticlesAsync(paginationRequest);
        return Ok(retrievedArticles);
    }

    [HttpPost("{id:Guid}/likes")]
    [Authorize(Policy = PermissionId.READ_ARTICLES)]
    [SwaggerOperation(
    Summary = "Set like by articleId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllArticlesResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetAllArticlesResponse>> SetLikeAsync([FromRoute] Guid id)
    {
        var isLiked = await _articleService.SetLikeAsync(id);
        return Ok(isLiked);
    }

    [HttpDelete("{id:Guid}/likes")]
    [Authorize(Policy = PermissionId.READ_ARTICLES)]
    [SwaggerOperation(
    Summary = "Remove like by articleId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllArticlesResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetAllArticlesResponse>> RemoveLikeAsync([FromRoute] Guid id)
    {
        var isRemoved = await _articleService.RemoveLikeAsync(id);
        return Ok(isRemoved);
    }

    [HttpGet("likes")]
    [Authorize(Policy = PermissionId.READ_ARTICLES)]
    [SwaggerOperation(
    Summary = "Retrieve liked articles with pagination")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllLikedArticlesResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetAllLikedArticlesResponse>> GetAllLikedArticlesAsync([FromQuery] GetAllLikedArticlesRequest paginationRequest)
    {
        var userWhoLikedPages = HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;
        var retrievedArticles = await _articleService.GetAllArticlesAsync(paginationRequest, userWhoLikedPages);
        return Ok(retrievedArticles);
    }

    [HttpGet("authors")]
    [SwaggerOperation(
    Summary = "Retrieve top authors by articles' likes")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetTopAuthorsResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetTopAuthorsResponse>> GetTopAuthorsAsync([FromQuery] GetTopAuthorsRequest paginationRequest)
    {
        var retrievedArticles = await _articleService.GetTopAuthorsAsync(paginationRequest);
        return Ok(retrievedArticles);
    }

    [HttpGet("tags")]
    [SwaggerOperation(
    Summary = "Retrieve top tags by articles' quantity")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetTopAuthorsResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetTopTagsResponse>> GetTopTagsAsync([FromQuery] GetTopTagsRequest paginationRequest)
    {
        var retrievedArticles = await _articleService.GetTopTagsAsync(paginationRequest);
        return Ok(retrievedArticles);
    }
}
