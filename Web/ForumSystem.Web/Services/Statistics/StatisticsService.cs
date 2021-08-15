namespace ForumSystem.Web.Services.Statistics
{
    using System.Linq;

    using ForumSystem.Data;

    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext data;

        public StatisticsService(ApplicationDbContext data)
            => this.data = data;

        public StatisticsServiceModel Total()
        {
            var totalUsers = this.data.Users.Count();
            var totalPosts = this.data.Posts.Count();
            var totalVotes = this.data.Votes.Count();
            var totalComments = this.data.Comments.Count();

            return new StatisticsServiceModel
            {
                TotalUsers = totalUsers,
                TotalPosts = totalPosts,
                TotalVotes = totalVotes,
                TotalComments = totalComments,
            };
        }
    }
}
