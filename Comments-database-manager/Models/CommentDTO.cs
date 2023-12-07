using Comments_database_manager.Communication;
using CommentsService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments_database_manager.Models
{
    public class CommentDTO : Comment
    {
        public CommentAction Action { get; set; }
        public CommentDTO(Comment comment)
        {
            this.Content = comment.Content;
            this.AuthorId = comment.AuthorId;
            this.Score = comment.Score;
            this.Id = comment.PostgresID;
            this.ContentId = comment.ContentId;
        }
        public CommentDTO()
        {

        }
    }
}
