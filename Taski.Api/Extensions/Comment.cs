using Taski.Api.Dtos;
using Taski.Api.Entities;

namespace Taski.Api.Extensions;

public static class CommentExtiensions
{
  public static CommentDto AsDto(this Comment comment)
  {
    return new CommentDto
    {
      Id = comment.Id,
      FullName = comment.User.FullName,
      Content = comment.Content,
      CreateDate = comment.CreateDate
    };
  }
}