﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Book.Models.ViewModels
{
    public class ProductVM
	{
        public Product Product { get; set; } = null!;

        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; } = null!;
    }
}
