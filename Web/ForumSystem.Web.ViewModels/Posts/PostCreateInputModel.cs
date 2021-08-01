namespace ForumSystem.Web.ViewModels.Posts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ForumSystem.Data.Models;
    using ForumSystem.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class PostCreateInputModel : IMapTo<Post>, IMapFrom<Post>
    {
        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryDropDownViewModel> Categories { get; set; }

        public ICollection<IFormFile> Files { get; set; }
    }
}
