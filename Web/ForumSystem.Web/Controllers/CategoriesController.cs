namespace ForumSystem.Web.Controllers
{
    using System;

    using ForumSystem.Services.Data;
    using ForumSystem.Web.ViewModels.Categories;
    using ForumSystem.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class CategoriesController : Controller
    {
        private const int ItemsPerPage = 5;

        private readonly ICategoriesService categoriesService;
        private readonly IPostsService postsService;

        public CategoriesController(
            ICategoriesService categoriesService,
            IPostsService postsService)
        {
            this.categoriesService = categoriesService;
            this.postsService = postsService;
        }

        public IActionResult ByName(string name, int page = 1)
        {
            var viewModel = this.categoriesService.GetByName<CategoryViewModel>(name);

            viewModel.ForumPosts = this.postsService
                .GetByCategoryId<PostInCategoryViewModel>(viewModel.Id, ItemsPerPage, (page - 1) * ItemsPerPage);

            var count = this.postsService.GetCountByCategoryId(viewModel.Id);

            viewModel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewModel.PagesCount == 0)
            {
                viewModel.PagesCount = 1;
            }

            viewModel.CurrentPage = page;

            return this.View(viewModel);
        }

        public IActionResult All()
        {
            var viewModel = new IndexViewModel();
            var categories = this.categoriesService.GetAll<IndexCategoryViewModel>();
            viewModel.Categories = categories;

            return this.View(viewModel);
        }
    }
}
