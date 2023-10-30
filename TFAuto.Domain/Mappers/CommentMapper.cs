using AutoMapper;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.CommentService.DTO;

namespace TFAuto.Domain.Mappers
{
    public class CommentMapper : Profile
    {
        public CommentMapper()
        {
            CreateMap<CreateCommentRequest, Comment>();
            CreateMap<Comment, CreateCommentResponse>();

            CreateMap<UpdateCommentRequest, Comment>();
            CreateMap<Comment, UpdateCommentResponse>();
        }
    }
}