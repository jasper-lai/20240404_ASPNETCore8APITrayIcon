namespace MyMvcSample.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyMvcSample.ViewModels;


    public class ProductController : Controller
    {

        private readonly ILogger<ProductController> _logger;
        private readonly List<ProductViewModel> _products;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
            _products =
            [
                new ProductViewModel { Id = 1, Name = "(MVC)夏威夷豆塔", UnitPrice = 40},
                new ProductViewModel { Id = 2, Name = "(MVC)養生豆塔", UnitPrice = 40},
                new ProductViewModel { Id = 3, Name = "(MVC)杏仁巧克力豆塔", UnitPrice = 40},
                new ProductViewModel { Id = 4, Name = "(MVC)原味奶酪", UnitPrice = 50},
                new ProductViewModel { Id = 5, Name = "(MVC)原味瑪德蓮", UnitPrice = 30},
                new ProductViewModel { Id = 6, Name = "(MVC)費南雪", UnitPrice = 30}
            ];
        }

        [HttpGet]
        public IActionResult Query()
        {
            var product = _products[0];
            return View(product);
        }

        [HttpPost]
        public IActionResult Query(string name = "豆塔")
        {
            var result = _products.Where(x => x.Name.Contains(name)).ToList();
            return Json(result);
        }
    }
}
