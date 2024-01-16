using StockControlApi.Application.Models;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Domain.Entities;
using StockControlApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockControlApi.Controller
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize("Bearer")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAsync()
        {
            return new ResponseHelper().CreateResponse(await _productService.GetAsync());
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> CreateProduct(ProductModel product)
        {
            return new ResponseHelper().CreateResponse(await _productService.CreateProduct(product));
        }

        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> UpdateProduct(ProductModel product)
        {
            return new ResponseHelper().CreateResponse(await _productService.UpdateProduct(product));
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteProduct(Guid Id, string userId)
        {
            return new ResponseHelper().CreateResponse(await _productService.DeleteProduct(Id, userId));
        }

        [HttpGet]
        [Route("get-by-id")]
        public async Task<IActionResult> GetProductById([FromQuery]Guid id)
        {
            return new ResponseHelper().CreateResponse(await _productService.GetProductById(id));
        }
        [HttpGet]
        [Route("get-by-name")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            return new ResponseHelper().CreateResponse(await _productService.GetProductByName(name));
        }

        [HttpGet]
        [Route("get-products")]
        public async Task<IActionResult> GetFilterAndPaginatedProducts(int skip, int take, string? search)
        {
            return new ResponseHelper().CreateResponse(await _productService.GetFilterAndPaginatedProducts(skip,take,search));
        }


    }
}
