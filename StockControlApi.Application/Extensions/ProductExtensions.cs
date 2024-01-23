using Microsoft.EntityFrameworkCore;
using StockControlApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockControlApi.Application.Extensions
{
    public static class ProductExtensions
    {
        //public static IQueryable<Product> ApplyFilter(this IQueryable<Product> items, ProjectFilterModel model)
        //{
        //    items = items.AsNoTracking();
        //    items = model.Name != null ? items.Where(x => x.Name.Contains(model.Name)) : items;
        //    items = model.Active != null ? items.Where(x => x.Active == model.Active) : items;
        //    items = model.CategoriesId != null ? items.Where(x => x.Categories.Any(c => model.CategoriesId.Contains(c.CausesInterestId.ToString()))) : items;
        //    return items;
        //}
        //public static IQueryable<ProjectReturnModel> MapToReturnDTO(this IQueryable<Product> admin)
        //{
        //    return admin.Select(x => new ProjectReturnModel(x));
        //}
    }
}
