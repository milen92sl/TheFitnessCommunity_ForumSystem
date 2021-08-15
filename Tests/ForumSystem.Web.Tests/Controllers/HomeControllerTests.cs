namespace ForumSystem.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    using HttpStatusCode = System.Net.HttpStatusCode;

    public class HomeControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> server;

        public HomeControllerTests(WebApplicationFactory<Startup> server)
        {
            this.server = server;
        }

        [Fact]
        public async Task IndexPageShouldReturnStatusCode200WithTitle()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>", responseContent);
        }

        [Fact]
        public async Task AccountManagePageRequiresAuthorization()
        {
            var client = this.server.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync("Identity/Account/Manage");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
