using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Core.Models
{
    public class Basket : BaseEntity
    {
        //set as virtual ICollection - it is important for the entity framework
        //entity framework will know everytime we need to load the basket from the database will automatically load
        //all the items from the basket as well - it is known as Lazy Loading
        public virtual ICollection<BasketItem> BasketItems { get; set; }

        public Basket()
        {
            this.BasketItems = new List<BasketItem>();
        }
    }
}
