using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IRepository<Customer> customers;
        IBasketServices basketServices;
        IOrderService orderService;

        public BasketController(IBasketServices BasketService, IOrderService OrderService, IRepository<Customer> Customers)
        {
            this.basketServices = BasketService;
            this.orderService = OrderService;
            this.customers = Customers;
        }
        // GET: Basket
        public ActionResult Index()
        {
            var model = basketServices.GetBasketItems(this.HttpContext);

            return View(model);
        }

        public ActionResult AddToBasket(string Id)
        {
            basketServices.AddToBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string Id)
        {
            basketServices.RemoveFromBasket(this.HttpContext, Id);

            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var basketSummary = basketServices.GetBasketSummary(this.HttpContext);

            return PartialView(basketSummary);
        }

        //action that will allow the user to see the items it will checkout
        //when the checkout page is loaded, the authentication system checks if the user is logged in
        //the checking is done automatically be decorating the method with "Authorize"
        //once the user is logged in and safe, the checkout page load the fields with the customer information
        [Authorize]
        public ActionResult Checkout()
        {
            Customer customer = customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if (customer != null)
            {
                Order order = new Order()
                {
                    Email = customer.Email,
                    City = customer.City,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PostalCode = customer.PostalCode,
                    Street = customer.Street,
                    State = customer.State
                };
                return View(order);
            }
            else
            {
                return RedirectToAction("Error");
            }
            
        }

        //action that will allow the user to actually checkout after placing the items in the cart
        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            //the basket items are obtained using the method GetBasketItems and passing the httpcontext
            var basketItems = basketServices.GetBasketItems(this.HttpContext);
            //change the record relative to the order status
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;

            //
            //At this point it will happen some kind payment processing, and after the payment being completed, it will advance to the next step
            //

            //after the confirmation of the order, the orderstatus will be changed
            order.OrderStatus = "Payment Processed";
            //by using the orderService, a order will be created and the order will be passed, together with the basket items
            orderService.CreateOrder(order, basketItems);
            //after the order is created, the basket will be cleared from its items
            basketServices.ClearBasket(this.HttpContext);

            return RedirectToAction("Thankyou", new {OrderId = order.Id});
        }

        public ActionResult ThankYou(string OrderId)
        {
            ViewBag.OrderId = OrderId;
            return View();
        }
    }
}