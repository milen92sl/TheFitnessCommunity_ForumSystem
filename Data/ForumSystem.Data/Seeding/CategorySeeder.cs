namespace ForumSystem.Data.Seeding
{
    using ForumSystem.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CategorySeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext.Categories.Any())
            {
                return;
            }

            var categories = new List<(string Name, string ImageUrl)> 
            {
                ("Boxing", "https://nice-assets.s3-accelerate.amazonaws.com/smart_templates/414f64871bde8a1ab383ad45ce654c3e/assets/77d5fewlka4jw23rff94ynkb87l1yemu.jpg"),
                ("Swimming", "https://cdn1.vectorstock.com/i/1000x1000/90/15/swimming-logo-vector-14239015.jpg"),
                ("Volleyball", "https://www.kindpng.com/picc/m/458-4584108_clip-art-people-playing-volleyball-player-volleyball-logo.png"),
                ("Fitness", "https://cdn2.vectorstock.com/i/1000x1000/48/56/fitness-club-logo-vector-20684856.jpg"),
                ("Football", "https://previews.123rf.com/images/wannen19/wannen191809/wannen19180900002/108328455-football-logo-designs.jpg"),
                ("Basketball", "https://cdn.pixabay.com/photo/2019/11/28/15/20/basketball-logo-4659382_960_720.png"),
            };

            foreach (var category in categories)
            {
                await dbContext.Categories.AddAsync(new Category
                {
                    Name = category.Name,
                    Description = category.Name,
                    Title = category.Name,
                    ImageUrl = category.ImageUrl,
                });
            }
        }
    }
}
