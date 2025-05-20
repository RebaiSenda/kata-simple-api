using Microsoft.AspNetCore.Mvc;

namespace KataSimpleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ILogger<PostController> _logger;

        public PostController(ILogger<PostController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] PostRequest request)
        {
            _logger.LogInformation("Création d'un nouveau post: {Title}", request.Title);

            // Logique pour créer un post
            var post = new PostResponse
            {
                Id = new Random().Next(1, 1000),
                Title = request.Title,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpGet("{id}")]
        public IActionResult GetPost(int id)
        {
            // Dans un cas réel, récupérer depuis une base de données
            var post = new PostResponse
            {
                Id = id,
                Title = "Post exemple",
                Content = "Contenu du post exemple",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            return Ok(post);
        }
    }

    public class PostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class PostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
