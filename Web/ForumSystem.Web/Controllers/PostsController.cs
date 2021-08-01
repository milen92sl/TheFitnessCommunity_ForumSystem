namespace ForumSystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using ForumSystem.Data.Models;
    using ForumSystem.Services.Data;
    using ForumSystem.Services.Mapping;
    using ForumSystem.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class PostsController : Controller
    {
        private readonly IPostsService postsService;
        private readonly ICategoriesService categoriesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly Cloudinary cloudinary;
        private const int ItemsPerPage = 5;

        public PostsController(
            IPostsService postsService,
            ICategoriesService categoriesService,
            UserManager<ApplicationUser> userManager,
            Cloudinary cloudinary)
        {
            this.postsService = postsService;
            this.categoriesService = categoriesService;
            this.userManager = userManager;
            this.cloudinary = cloudinary;
        }

        public IActionResult All(int page = 1, string search = null)
        {
            var viewmodel = new PostAllModel();
            viewmodel.Posts = this.postsService.GetAll<PostViewModel>(search, ItemsPerPage, (page - 1) * ItemsPerPage);
            var count = this.postsService.GetCount();
            viewmodel.Search = search;
            viewmodel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewmodel.PagesCount == 0)
            {
                viewmodel.PagesCount = 1;
            }

            viewmodel.CurrentPage = page;

            return this.View(viewmodel);
        }

        [Authorize]
        public IActionResult ByUser(int id, int page = 1, string search = null)
        {
            var user = this.HttpContext.User.Identity.Name;

            var viewmodel = new PostAllModel();
            viewmodel.Posts = this.postsService.GetAll<PostViewModel>(search, ItemsPerPage, (page - 1) * ItemsPerPage, user);
            var count = this.postsService.GetCount(user);
            viewmodel.Search = search;
            viewmodel.PagesCount = (int)Math.Ceiling((double)count / ItemsPerPage);

            if (viewmodel.PagesCount == 0)
            {
                viewmodel.PagesCount = 1;
            }

            viewmodel.CurrentPage = page;

            return this.View(viewmodel);
        }

        public IActionResult ById(int id)
        {
            var postViewModel = this.postsService.GetById<PostViewModel>(id);
            if (postViewModel == null)
            {
                return this.NotFound();
            }

            if (postViewModel.VotesCount < 0)
            {
                postViewModel.VotesCount = 0;
            }

            return this.View(postViewModel);
        }

        [Authorize]
        public IActionResult Create()
        {
            var categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            var viewModel = new PostCreateInputModel
            {
                Categories = categories,
            };
            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateInputModel input)
        {
            var post = AutoMapperConfig.MapperInstance.Map<PostCreateInputModel>(input);

            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var files = input.Files;

            var postId = await this.postsService.CreateAsync(input.Title, input.Content, input.CategoryId, user.Id);

            if (files != null)
            {
                var urlOfProducts = await this.postsService.UploadAsync(this.cloudinary, files);
                await this.postsService.AddImageInBase(urlOfProducts, postId);

            }

            return this.RedirectToAction(nameof(this.ById), new { id = postId });
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var postViewModel = this.postsService.GetById<PostEditModel>(id);

            if (postViewModel == null)
            {
                return this.NotFound();
            }

            var categories = this.categoriesService.GetAll<CategoryDropDownViewModel>();
            postViewModel.Categories = categories;
            var date = postViewModel.CreatedOn;

            postViewModel.CreatedOn = date;

            var postUserName = postViewModel.UserUserName;
            var currentUser = this.HttpContext.User.Identity.Name;

            if (postUserName != currentUser)
            {
                return this.BadRequest("Failed to edit the post");
            }

            return this.View(postViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(PostEditModel input)
        {
            var post = AutoMapperConfig.MapperInstance.Map<Post>(input);

            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var currentPost = this.postsService.GetById<PostEditModel>(input.Id);

            var user = await this.userManager.GetUserAsync(this.User);
            post.UserId = user.Id;
            post.CreatedOn = currentPost.CreatedOn;
            var postId = await this.postsService.Edit(post);
            return this.RedirectToAction(nameof(this.ById), new { id = postId });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var post = this.postsService.GetById<PostDeleteModel>(id);

            var postUserName = post.UserUserName;
            var currentUser = this.HttpContext.User.Identity.Name;

            if (postUserName != currentUser)
            {
                return this.BadRequest("Failed to delete the post");
            }

            var postToDelete = AutoMapperConfig.MapperInstance.Map<PostDeleteModel>(post);
            return this.View(postToDelete);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(PostDeleteModel input)
        {
            var post = AutoMapperConfig.MapperInstance.Map<Post>(input);

            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            await this.postsService.Delete(post);

            return this.RedirectToAction("All");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var urlOfProducts = await this.postsService.UploadAsync(this.cloudinary, files);

            return this.Redirect("All");
        }
    }
}
