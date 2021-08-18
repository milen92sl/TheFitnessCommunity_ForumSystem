namespace ForumSystem.Services.Tests
{
    using System;
    using System.Threading.Tasks;

    using ForumSystem.Data;
    using ForumSystem.Data.Models;
    using ForumSystem.Data.Repositories;
    using ForumSystem.Services.Data;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class VotesServicesTests
    {
        [Fact]
        public async Task TwoDownVotesShouldCountOnce()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var mockRepository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));
            var service = new VotesService(mockRepository);

            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", true);
            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", true);
            var votes = service.GetVotes(0);

            Assert.Equal(1, votes);
        }

        [Fact]
        public async Task UpVoteAndDownVoteFromSamePersonShouldNegativeOne()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var mockRepository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));
            var service = new VotesService(mockRepository);

            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", true);
            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", false);
            var votes = service.GetVotes(0);

            Assert.Equal(-1, votes);
        }

        [Fact]
        public async Task TwoUpVotesAndOneDownVoteFromThreeUsersShouldBeOne()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var mockRepository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));
            var service = new VotesService(mockRepository);

            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", true);
            await service.VoteAsync(0, "175a2793-b1c8-4489-829e-549925137c57", true);
            await service.VoteAsync(0, "1767ea4e-f3a3-47ac-bdb3-41b71a7f5e7e", false);
            var votes = service.GetVotes(0);

            Assert.Equal(1, votes);
        }

        [Fact]
        public async Task TwoUpVotesAndOneDownVoteFromTwoUsersShouldBeZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var mockRepository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));
            var service = new VotesService(mockRepository);

            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", true);
            await service.VoteAsync(0, "175a2793-b1c8-4489-829e-549925137c57", true);
            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", false);
            var votes = service.GetVotes(0);

            Assert.Equal(0, votes);
        }

        [Fact]
        public async Task TwoDownVoteAndOneUpVoteFromTwoUsersShouldBeZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            var mockRepository = new EfRepository<Vote>(new ApplicationDbContext(options.Options));
            var service = new VotesService(mockRepository);

            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", false);
            await service.VoteAsync(0, "175a2793-b1c8-4489-829e-549925137c57", false);
            await service.VoteAsync(0, "0a9ec75c-0560-4e5b-94d4-c44429bb7379", true);
            var votes = service.GetVotes(0);

            Assert.Equal(0, votes);
        }
    }
}
