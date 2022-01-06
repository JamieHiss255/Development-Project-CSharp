﻿using Sparcpoint.DataServices;
using Sparcpoint.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sparcpoint.Services
{
    public class CategoryService: ICategoryService
    {
        ICategoryDataService _categoryDataService;

        public CategoryService(ICategoryDataService categoryDataService) {
            _categoryDataService = categoryDataService;
        }

        public async Task<List<Category>> GetCategories() {
            return await _categoryDataService.GetCategories();
        }

        public async Task CreateCategoryAsync(CreateCategoryRequest req) {

            //eventually want to use a mapper here but start simple

            var category = new Category()
            {
                Name = req.Name,
                Description = req.Description,
                CreatedTimestamp = DateTime.UtcNow
            };

            await _categoryDataService.CreateCategoryAsync(category);
        }
    }
}
