namespace ForumSystem.Web.ViewModels.Posts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ForumSystem.Data.Models;
    using ForumSystem.Services.Mapping;

    public class PostEditModel : IMapTo<Post>, IMapFrom<Post>
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<CategoryDropDownViewModel> Categories { get; set; }

        public string UserUserName { get; set; }
    }
}
