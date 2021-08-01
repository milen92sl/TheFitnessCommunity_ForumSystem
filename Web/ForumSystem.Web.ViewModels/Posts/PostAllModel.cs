using ForumSystem.Data.Models;
using ForumSystem.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSystem.Web.ViewModels.Posts
{
    public class PostAllModel
    {
        public IEnumerable<PostViewModel> Posts { get; set; }

        public string Search { get; set; }

        public string Title { get; set; }

        public int CurrentPage { get; set; }

        public int PagesCount { get; set; }
    }
}
