using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.Services
{
    public class BasketService
    {
        //give the context for both tables in our database - Product and Basket
        IRepository<Product> productContext;

        IRepository<Basket> basketContext;

        //constant value that will be assign as cookie name
        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContext, IRepository<Basket> BasketContext)
        {
            this.productContext = ProductContext;
            this.basketContext = BasketContext;
        }

        /// <summary>
        /// Internal method - have acces to the users httpcontext. So it will force the consuming service to send the users context
        /// it will also take a boolean, because sometimes we want the basket to be created if not exists othertimes we wont want that
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="createIfNull"></param>
        /// <returns></returns>
        private Basket GetBaskets(HttpContextBase httpContext, bool createIfNull) 
        {
            //try to read the cookie - by acessing http context. It uses an method that take a string as argument
            //the argument will be our constant BasketSessionName
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            //create a new basket
            Basket basket = new Basket();

            //check if the cookie actually exists
            //if the users visited before will have read the cookie, otherwise the cookie we attemped the get will be null
            if (cookie != null)
            {
                //if it has a got a cookie, we will attempt to get the cookie name by using cookie.value
                string basketId = cookie.Value;
                //do an additional checking to see if the value we read from the cookie is not actually null
                if (!string.IsNullOrEmpty(basketId))
                {
                    //if not null then we try to load the basket from the basketContext
                    basket = basketContext.Find(basketId);
                }
                else
                {
                    //if the basket id was null, we need to check to see if we want to create the basket
                    if (createIfNull)
                    {
                        //if yes, we will create a new basket using a method
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            //if the cookie was null
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }  
            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
           //create new basket
           Basket basket = new Basket();
           //insert into the database
           basketContext.Insert(basket);
           basketContext.Commit();
           
           //after we need to write a cookie into the users machine
           HttpCookie cookie = new HttpCookie(BasketSessionName);
           //value for the cookie will be the basket id
           cookie.Value = basket.Id;
           //set an experition for the cookie, you can change according to the business logic
           cookie.Expires = DateTime.Now.AddDays(1);
           //finally we need to add that cookie to the httpContext response, because we are sending it back to the user
           httpContext.Response.Cookies.Add(cookie);
           //return the basket just created
           return basket;

        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            //we need to get a basket, if we are adding an item to it
            //in this situation, we have to make sure the basket exists, so the createIfNull parameter is always passed as true
            Basket basket = GetBaskets(httpContext, true);

            //we need to see if there already an basket item in the users basket with this same product id
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            //if there is not item with the same id in the basket, you will create a new item
            if (item == null)
            {
                item = new BasketItem() {BasketId = basket.Id, ProductId = productId, Quantity = 1};
                //and add this new item to the list of basketItems
                basket.BasketItems.Add(item);
            }
            else
            {
                //if there is already a item with the productid in the cart, you will increase the qty by one
                //entity framework holds the item in the cache, so thats why you dont need to commit the change right after
                item.Quantity = item.Quantity + 1;
            }

            basketContext.Commit();
        }

        //different here from the previous method, is that we will remove the item based in its id, not based in the product id like the previous one
        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBaskets(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                basket.BasketItems.Remove(item);
                basketContext.Commit();
            }
        }
    }
}
