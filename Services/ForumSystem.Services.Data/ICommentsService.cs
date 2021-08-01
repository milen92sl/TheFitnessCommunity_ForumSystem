using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ForumSystem.Services.Data
{
    public interface ICommentsService
    {
        Task Create(int postId, string userId, string content, int? parentId = null);

        bool IsInPostId(int commentId, int postId);
    }
}
