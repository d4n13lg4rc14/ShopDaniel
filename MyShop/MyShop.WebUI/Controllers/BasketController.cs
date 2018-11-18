using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IBasketServices basketServices;

        public BasketController(IBasketServices BasketService)
        {
            this.basketServices = BasketService;
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
    }
}