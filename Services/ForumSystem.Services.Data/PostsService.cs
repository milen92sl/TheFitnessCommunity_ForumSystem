namespace ForumSystem.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using ForumSystem.Data.Common.Repositories;
    using ForumSystem.Data.Models;
    using ForumSystem.Services.Mapping;
    using Microsoft.AspNetCore.Http;

    public class PostsService : IPostsService
    {
        private readonly IDeletableEntityRepository<Post> postsRepository;
        private readonly IRepository<Image> imagesRepository;

        public PostsService(IDeletableEntityRepository<Post> postsRepository, IRepository<Image> imagesRepository)
        {
            this.postsRepository = postsRepository;
            this.imagesRepository = imagesRepository;
        }

        public async Task<int> CreateAsync(string title, string content, int categoryId, string userId)
        {
            if (content == null)
            {
                content = "This post is empty!";
            }

            var post = new Post
            {
                CategoryId = categoryId,
                Content = content,
                Title = title,
                UserId = userId,
            };

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();
            return post.Id;
        }

        public async Task Delete(Post post)
        {
            var imagesToDelete = this.imagesRepository.All().Where(i => i.PostId == post.Id);

            foreach (var image in imagesToDelete)
            {
                this.imagesRepository.Delete(image);
                this.imagesRepository.SaveChangesAsync();
            }

            this.postsRepository.HardDelete(post);
            await this.postsRepository.SaveChangesAsync();
        }

        public async Task<int> Edit(Post post)
        {
            this.postsRepository.Update(post);
            await this.postsRepository.SaveChangesAsync();
            return post.Id;
        }

        public IEnumerable<T> GetAll<T>(string search = null, int? take = null, int skip = 0, string userName = null)
        {
            var allPosts = this.postsRepository
                .All()
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                allPosts = allPosts.Take(take.Value);
            }

            if (search != null)
            {
                allPosts = allPosts.Where(a => a.Title.Contains(search));
            }

            if (userName != null)
            {
                allPosts = allPosts.Where(a => a.User.UserName == userName);
            }

            return allPosts.To<T>();
        }

        public IEnumerable<T> GetByCategoryId<T>(int categoryId, int? take = null, int skip = 0)
        {
            var query = this.postsRepository
                .All()
                .OrderByDescending(x => x.CreatedOn)
                .Where(c => c.CategoryId == categoryId).Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.To<T>()
                .ToList();
        }

        public T GetById<T>(int id)
        {
            var post = this.postsRepository.All().Where(x => x.Id == id)
                .To<T>().FirstOrDefault();
            return post;
        }

        public int GetCount(string userName = null)
        {
            var allPostCount = this.postsRepository.All().Count();

            if (userName != null)
            {
                allPostCount = this.postsRepository.All().Where(u => u.User.UserName == userName).Count();
            }

            return allPostCount;
        }

        public int GetCountByCategoryId(int categoryId)
        {
            var categoryCount = this.postsRepository.All().Count(x => x.CategoryId == categoryId);
            return categoryCount;
        }

        public string GetUserNameByPostId(int id)
        {
            var postUserName = this.postsRepository.All().Where(p => p.Id == id).FirstOrDefault().User.UserName;

            return postUserName;
        }

        public async Task<IEnumerable<string>> UploadAsync(Cloudinary cloudinary, ICollection<IFormFile> files)
        {
            List<string> imagesUrl = new List<string>();

            foreach (var file in files)
            {
                byte[] destinationImage;

                using (var image = new MemoryStream())
                {
                    await file.CopyToAsync(image);

                    destinationImage = image.ToArray();
                }

                using (var destinationStrem = new MemoryStream(destinationImage))
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, destinationStrem),
                    };

                    var result = await cloudinary.UploadAsync(uploadParams);

                    if (result.Error == null)
                    {
                        var imgUrl = result.Uri.AbsoluteUri;

                        imagesUrl.Add(imgUrl);
                    }
                }
            }

            return imagesUrl;
        }

        public async Task<int> AddImageInBase(IEnumerable<string> images, int postId)
        {
            foreach (var image in images)
            {
                var imageUrl = new Image
                {
                    Url = image,
                    PostId = postId,
                };
                await this.imagesRepository.AddAsync(imageUrl);
                await this.imagesRepository.SaveChangesAsync();
            }

            return postId;
        }
    }
}
