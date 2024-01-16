using StockControlApi.Application.Models;
using StockControlApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<ResponseModel> GetAsync();
        Task<ResponseModel> CreateProduct(ProductModel product);
        Task<ResponseModel> DeleteProduct(Guid Id, string userId);
        Task<ResponseModel> UpdateProduct(ProductModel product);
        Task<ResponseModel> GetProductById(Guid Id);
        Task<ResponseModel> GetProductByName(string name);
        Task<ResponseModel> GetFilterAndPaginatedProducts(int skip, int take, string? search);


    }
}
