namespace ForumSystem.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ForumSystem.Data;
    using ForumSystem.Data.Models;
    using ForumSystem.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class CommentsServicesTests
    {
        [Fact]
        public async Task CreateMethodShouldAddCorrectNewCommentToDbAndToPost()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var commentsService = new EfDeletableEntityRepository<Comment>(dbContext);

            var comment = new Comment
            {
                Content = "testContent",
                UserId = "Milen92sl",
            };

            var commentList = new List<Comment>();
            commentList.Add(comment);

            var postId = Guid.NewGuid().ToString();
            var post = new Post
            {
                Comments = commentList,
            };

            await dbContext.Posts.AddAsync(post);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Milen92sl",
            };

            await dbContext.Users.AddAsync(user);

            var commentToAdd = new Comment
            {
                Content = "testContent",
            };

            await commentsService.AddAsync(commentToAdd);
            await dbContext.SaveChangesAsync();

            Assert.NotNull(dbContext.Comments.FirstOrDefaultAsync());
            Assert.Equal("testContent", dbContext.Comments.FirstAsync().Result.Content);
            Assert.Equal("Milen92sl", dbContext.Comments.FirstAsync().Result.UserId);
        }

        [Fact]
        public async Task IsInPostMethodShouldReturnTrueIfCommentIsInPost()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var commentsService = new EfDeletableEntityRepository<Comment>(dbContext);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Milen92sl",
            };

            var post = new Post
            {
                UserId = user.Id,
            };

            var comment = new Comment
            {
                Content = "testContent",
                UserId = user.Id,
                PostId = post.Id,
            };
            await dbContext.Users.AddAsync(user);
            await dbContext.Posts.AddAsync(post);
            await dbContext.Comments.AddAsync(comment);


            await dbContext.SaveChangesAsync();


            var result = commentsService.All().Count(x => x.PostId == comment.PostId);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CheckAddReplyComment()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var commentsService = new EfDeletableEntityRepository<Comment>(dbContext);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Milen Ivanov",
            };
            await dbContext.Users.AddAsync(user);

            var post = new Post
            {
                Content = "Post's content",
                Title = "Posts's title",
                UserId = user.Id,
            };

            await dbContext.Posts.AddAsync(post);

            var parentComment = new Comment
            {
                Content = "Parent comment content",
                UserId = user.Id,
                PostId = post.Id,
            };

            await dbContext.Comments.AddAsync(parentComment);

            var replyComment = new Comment
            {
                ParentId = parentComment.Id,
                Content = "Reply comment content",
                UserId = user.Id,
                PostId = post.Id,
            };

            await dbContext.Comments.AddAsync(replyComment);

            await dbContext.SaveChangesAsync();

            var replyCommentsCount = dbContext.Comments.Count(x => x.ParentId != null);

            Assert.Equal(1, replyCommentsCount);
        }

        [Fact]
        public async Task CheckAddReplyToReplyComment()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);

            var commentsService = new EfDeletableEntityRepository<Comment>(dbContext);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Milen Ivanov",
            };
            await dbContext.Users.AddAsync(user);

            var post = new Post
            {
                Content = "Post's content",
                Title = "Posts's title",
                UserId = user.Id,
            };

            await dbContext.Posts.AddAsync(post);

            var parentComment = new Comment
            {
                Content = "Parent comment content",
                UserId = user.Id,
                PostId = post.Id,
            };

            await dbContext.Comments.AddAsync(parentComment);

            var firstReplyComment = new Comment
            {
                ParentId = parentComment.Id,
                Content = "Reply comment content",
                UserId = user.Id,
                PostId = post.Id,
            };
            await dbContext.Comments.AddAsync(firstReplyComment);

            var secondReplyComment = new Comment
            {
                ParentId = firstReplyComment.Id,
                Content = "Reply comment content",
                UserId = user.Id,
                PostId = post.Id,
            };
            await dbContext.Comments.AddAsync(secondReplyComment);

            await dbContext.SaveChangesAsync();

            var replyCommentsCount = dbContext.Comments.Count(x => x.ParentId != null);

            Assert.Equal(2, replyCommentsCount);
        }
    }
}
