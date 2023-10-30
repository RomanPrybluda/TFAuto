using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.CommentService;
using TFAuto.Domain.Services.CommentService.DTO;
using TFAuto.Domain.Services.LikeService;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("comments")]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;

        public CommentController(ICommentService commentService, ILikeService likeService)
        {
            _commentService = commentService;
            _likeService = likeService;
        }

        [HttpPost]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(CreateCommentResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<CreateCommentResponse>> AddCommentAsync([FromQuery] CreateCommentRequest commentCreate)
        {
            var createdComment = await _commentService.AddCommentAsync(commentCreate);
            return Ok(createdComment);
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UpdateCommentResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<UpdateCommentResponse>> UpdateCommentAsync([Required] Guid id, [FromQuery] UpdateCommentRequest commentUpdate)
        {
            var updatedComment = await _commentService.UpdateCommentAsync(id, commentUpdate);
            return Ok(updatedComment);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Policy = PermissionId.DELETE_COMMENT)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> DeleteCommentAsync([Required] Guid id, [FromQuery] DeleteCommentRequest commentDelete)
        {
            await _commentService.DeleteCommentAsync(id, commentDelete);
            return NoContent();
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetCommentResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetCommentResponse>> GetAllCommentsAsync([Required] Guid articleId, [FromQuery] GetCommentsPaginationRequest paginationRequest)
        {
            var comments = await _commentService.GetAllCommentsAsync(articleId, paginationRequest);
            return Ok(comments);
        }

        [HttpPost("{id:Guid}/like")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<bool>> GiveLikeCommentAsync(Guid id, [Required] Guid userId)
        {
            var like = await _likeService.GiveLikeCommentAsync(id, userId);
            return Ok(like);
        }

        [HttpDelete("{id:Guid}/like")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<bool>> RemoveLikeCommentAsync(Guid id, [Required] Guid userId)
        {
            var unlike = await _likeService.RemoveLikeCommentAsync(id, userId);
            return Ok(unlike);
        }
    }
}