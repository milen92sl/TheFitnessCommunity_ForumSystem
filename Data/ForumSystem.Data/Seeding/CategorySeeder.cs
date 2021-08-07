﻿namespace ForumSystem.Data.Seeding
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
                ("Diets", "https://www.jagranjosh.com/imported/images/E/Articles/Diet-Plans-for-people-1.jpg"),
                ("Workouts", "https://workoutsofficial.com/wp-content/uploads/2019/06/workoutsofficial_logo.png"),
                ("Articles", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTQaS_7FYFeEz5HGLcZNne1BPkZJhkhz7L-UQ&usqp=CAU"),
                ("Supplements", "https://www.nautilusplus.com/content/uploads/2017/10/Supplements.jpg"),
                ("For Beginners", "https://cdn.workoutuni.com/wp-content/uploads/2019/08/gym-for-beginners.jpg"),
                ("Nutritions", "https://previews.123rf.com/images/sonulkaster/sonulkaster1711/sonulkaster171100367/90237498-fitness-food-poster-of-sports-healthy-diet-food-nutrition-icons-vector-flat-design-of-protein-drink-.jpg"),
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
