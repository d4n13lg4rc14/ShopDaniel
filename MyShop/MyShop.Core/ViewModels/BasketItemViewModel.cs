using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.ViewModels
{
    public class BasketItemViewModel
    {
        public string Id { get; set; }//id of the basket item
        public int Quantity { get; set; } // from the basketitem
        public string ProductName { get; set; } //from the product table
        public decimal Price { get; set; }//from the product table
        public string Image { get; set; }//from the product table

    }
}
