namespace ForumSystem.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using ForumSystem.Data.Models;

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
                //TODO: Need to fix thease in to the LAYOUT.

                ("Diets", "https://www.jagranjosh.com/imported/images/E/Articles/Diet-Plans-for-people-1.jpg"),
                ("Workouts", "https://workoutsofficial.com/wp-content/uploads/2019/06/workoutsofficial_logo.png"),
                ("Articles", "https://www.kindpng.com/picc/m/458-4584108_clip-art-people-playing-volleyball-player-volleyball-logo.png"),
                ("Supplements", "https://cdn2.vectorstock.com/i/1000x1000/48/56/fitness-club-logo-vector-20684856.jpg"),
                ("For Beginners", "https://previews.123rf.com/images/wannen19/wannen191809/wannen19180900002/108328455-football-logo-designs.jpg"),
                ("Nutritions", "https://cdn.pixabay.com/photo/2019/11/28/15/20/basketball-logo-4659382_960_720.png"),
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
