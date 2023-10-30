using AutoMapper;
using TFAuto.DAL.Entities.Article;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.Domain.Mappers;

public class ArticleMapper : Profile
{
    public ArticleMapper()
    {
        CreateMap<CreateArticleRequest, Article>();
        CreateMap<Article, CreateArticleResponse>();

        CreateMap<Tag, TagResponse>();

        CreateMap<UpdateArticleRequest, Article>();
        CreateMap<Article, UpdateArticleResponse>();

        CreateMap<Article, GetArticleResponse>();
        CreateMap<Article, GetSingleArticleResponse>();
    }
}
