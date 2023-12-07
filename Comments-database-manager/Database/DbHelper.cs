using Comments_database_manager.Models;
using CommentsService.Database;
using CommentsService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments_database_manager.Database
{
    public class DbHelper : IDBHelper
    {

        private DataContext _dbContext;

        private DbContextOptions<DataContext> GetAllOptions()
        {
            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            optionsBuilder.UseNpgsql(AppSettings.ConnectionString);

            return optionsBuilder.Options;
        }
       public List<Comment> GetComments()
        {
            using (_dbContext = new DataContext(GetAllOptions()))
            {
                try
                {
                    var comments = _dbContext.Comments.ToList();

                    if (comments == null)
                        throw new InvalidOperationException("No comments found!");

                    return comments;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public bool SaveComment(Comment comment)
        {
            try
            {
                if (CommentExists(comment.Id)) return false;
                using (_dbContext = new DataContext(GetAllOptions()))
                {
                    _dbContext.Comments.Add(comment);
                    _dbContext.SaveChanges();
                }
                return true;
            } catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }
        public bool UpdateComment(Comment comment)
        {
            try
            {
                var id = GetCommentId(comment);
                if (id == -1) return false;

                using (_dbContext = new DataContext(GetAllOptions()))
                {
                    _dbContext.Comments.First(c => c.PostgresID == id)
                                        .Update(comment);

                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }
        public bool DeleteComment(long id)
        {

            try
            {
                if (!CommentExists(id)) return false;

                using (_dbContext = new DataContext(GetAllOptions()))
                {
                    var comment = _dbContext.Comments.First(x => x.Id == id);
                    _dbContext.Comments.Remove(comment);
                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }

        }
        /// <summary>
        /// searches the database for the given comment. Returns the PostgreSQL comment Id if one is found, '-1' otherwise
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        private long GetCommentId(Comment comment)
        {
            using (_dbContext = new DataContext(GetAllOptions()))
            {
                var id = _dbContext.Comments.FirstOrDefault(existingComment => existingComment.Id == comment.Id).PostgresID;
                return (id == 0) ? -1 : id;
            }
        }
        private bool CommentExists(long id)
        {
            using (_dbContext = new DataContext(GetAllOptions()))
            {
                return _dbContext.Comments.Any(comment => comment.Id == id);
            }
        }
    }
}
