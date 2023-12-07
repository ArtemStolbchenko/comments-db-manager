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
    public interface IDBHelper
    {
        public List<Comment> GetComments();
        public bool SaveComment(Comment comment);
        public bool UpdateComment(Comment comment);
        public bool DeleteComment(long id);
    }
}
