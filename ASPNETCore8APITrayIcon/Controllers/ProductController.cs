// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNETCore8APITrayIcon.Controllers
{
    using ASPNETCore8APITrayIcon.ViewModels;
    using Microsoft.AspNetCore.Http.Json;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Text.Json;

    //[Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly List<ProductViewModel> _products;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _products =
            [
                new ProductViewModel { Id = 1, Name = "(API)夏威夷豆塔", UnitPrice = 40},
                new ProductViewModel { Id = 2, Name = "(API)養生豆塔", UnitPrice = 40},
                new ProductViewModel { Id = 3, Name = "(API)杏仁巧克力豆塔", UnitPrice = 40},
                new ProductViewModel { Id = 4, Name = "(API)原味奶酪", UnitPrice = 50},
                new ProductViewModel { Id = 5, Name = "(API)原味瑪德蓮", UnitPrice = 30},
                new ProductViewModel { Id = 6, Name = "(API)費南雪", UnitPrice = 30}
            ];
            _logger = logger;
        }

        /// <summary>
        /// 依產品名稱, 取得符合條件的產品清單
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// [發佈前] http://localhost:5137/api/Product/QueryProducts
        /// [發佈後] http://localhost:5000/api/Product/QueryProducts
        /// 注意: 只適用於 Postman + Query String
        /// </remarks>
        //[HttpPost("[action]")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> QueryProducts(string name = "豆塔")
        //public async Task<ActionResult<IEnumerable<ProductViewModel>>> QueryProducts([FromForm] string name = "豆塔")
        {
            var result = _products.Where(x => x.Name.Contains(name)).ToList();
            _logger.LogInformation("{name} {@products}", name, result);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 依產品名稱, 取得符合條件的產品清單 (by Ajax Form)
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> QueryProductsByAjaxForm([FromForm] ProductViewModel? query = null)
        {
            List<ProductViewModel> result;

            _logger.LogInformation("查詢條件: {@query}", query);

            if (query != null && !string.IsNullOrWhiteSpace(query.Name))
            {
                // Assuming _products is a queryable data source that can be accessed asynchronously
                result = _products.Where(x => x.Name.Contains(query.Name)).ToList();
                _logger.LogInformation("查詢結果: {@products}", result);
            }
            else
            {
                // 如果沒有給查詢條件, 就回傳全部
                result = _products;
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// 依產品名稱, 取得符合條件的產品清單 (by Ajax Json)
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> QueryProductsByAjaxJson([FromBody] ProductViewModel? query = null)
        {
            List<ProductViewModel> result;

            _logger.LogInformation("查詢條件: {@query}", query);

            if (query != null && !string.IsNullOrWhiteSpace(query.Name))
            {
                // Assuming _products is a queryable data source that can be accessed asynchronously
                result = _products.Where(x => x.Name.Contains(query.Name)).ToList();
                _logger.LogInformation("查詢結果: {@products}", result);
            }
            else
            {
                // 如果沒有給查詢條件, 就回傳全部
                result = _products;
            }

            return await Task.FromResult(result);
        }
    }
}
