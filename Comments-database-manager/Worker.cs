using Comments_database_manager.Database;
using Comments_database_manager.Models;
using CommentsService.Models;
using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Linq;
using Comments_database_manager.Communication;

namespace Comments_database_manager
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDBHelper _dbHelper;
        private readonly IRabbitMQManager _rabbitMQManager;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _dbHelper = new DbHelper();
            _rabbitMQManager = new RabbitMQManager();

            //assigning handlers:
            _rabbitMQManager.CreateRequested += AddComment;
            _rabbitMQManager.ReadRequested += GetComments;
            _rabbitMQManager.UpdateRequested += UpdateComment;
            _rabbitMQManager.DeleteRequested += DeleteComment;
        }
        public Worker(ILogger<Worker> logger, IDBHelper dbHelper, IRabbitMQManager rabbitMQManager)
        {
            _logger = logger;
            _dbHelper = dbHelper;
            _rabbitMQManager = rabbitMQManager;

            //assigning handlers:
            _rabbitMQManager.CreateRequested += AddComment;
            _rabbitMQManager.ReadRequested += GetComments;
            _rabbitMQManager.UpdateRequested += UpdateComment;
            _rabbitMQManager.DeleteRequested += DeleteComment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(10000, stoppingToken);
            }
        }

        private void AddComment(Comment comment)
        {
            if (_dbHelper.SaveComment(comment))
                _logger.LogInformation("A comment from userID " + comment.AuthorId + " with id " + comment.Id + " was saved at {time}", DateTimeOffset.Now);
            else 
                _logger.LogError("Failed to save comment from userID " + comment.AuthorId + " with id " + comment.Id + " at {time}", DateTimeOffset.Now);
        }
        private void UpdateComment(Comment comment)
        {
            if (_dbHelper.UpdateComment(comment))
                _logger.LogInformation("A comment from userID " + comment.AuthorId + " with id " + comment.Id + " was updated at {time}", DateTimeOffset.Now);
            else 
                _logger.LogError("Failed to update comment from userID " + comment.AuthorId + " with id " + comment.Id + " at {time}", DateTimeOffset.Now);
        }
        private void DeleteComment(Comment comment)
        {
            if (_dbHelper.DeleteComment(comment.Id))
                _logger.LogInformation("A comment from userID " + comment.AuthorId + " with id " + comment.Id + " was deleted at {time}", DateTimeOffset.Now);
            else 
                _logger.LogError("Failed to delete comment from userID " + comment.AuthorId + " with id " + comment.Id + " at {time}", DateTimeOffset.Now);
        }
        private void GetComments(IModel Channel, BasicDeliverEventArgs props)
        {
            var comments = _dbHelper.GetComments();
            List<CommentDTO> commentDTOs = comments.Select(c => new CommentDTO(c)).ToList();
            _rabbitMQManager.Respond(Channel, props, comments);
        }
    }
}