namespace ForumSystem.Web.ViewModels.Posts
{
    using System.Collections.Generic;

    public class PostAllModel
    {
        public IEnumerable<PostViewModel> Posts { get; set; }

        public string Search { get; set; }

        public string Title { get; set; }

        public int CurrentPage { get; set; }

        public int PagesCount { get; set; }
    }
}
