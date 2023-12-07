using Comments_database_manager.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommentsService.Models
{
    [Table("comment")]
    public class Comment
    {
        [Key, Required]
        public long PostgresID { get; set; }
        [Required]
        public long Id { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public long ContentId { get; set; }
        public string ? Content { get; set; }
        public int ? Score { get; set; }

        public void Update(Comment comment)
        {
            this.Id = comment.Id;
            this.AuthorId = comment.AuthorId;
            this.ContentId = comment.ContentId;
            this.Content = comment.Content;
            this.Score = comment.Score;
        }
        public Comment()
        {

        }
        public Comment(CommentDTO dto)
        {
            this.Id = dto.Id;
            this.AuthorId = dto.AuthorId;
            this.ContentId = dto.ContentId;
            this.Content = dto.Content;
            this.Score = dto.Score;
        }
    }
}
