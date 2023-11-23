using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace SamWarehouse.Models
    {
    public class Item
        {
        public int Id { get; set; }

        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string ItemName { get; set; }

        public string Unit { get; set; }

        [Range(0, 500)]
        public double ItemPrice { get; set; }

        public int itemCode { get; set; }
        public string Description { get; set; }

        // public HttpPostAttribute base ImagePath { get; set; }
        public IEnumerable<SelectListItem> ProductListItems { get; set; }

        }
    }
