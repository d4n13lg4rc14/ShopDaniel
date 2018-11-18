using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.ViewModels;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {

        IRepository<Product> context;
        IRepository<ProductCategory> productCategories;

        public HomeController(IRepository<Product> productcontext, IRepository<ProductCategory> productCategoryContext)
        {
            context = productcontext;
            productCategories = productCategoryContext;
        }

        public ActionResult Index(string Category=null) //what this means is 1-you can have a null item, 2-if we dont pass anything in we assume it is null
        {
            List<Product> products;
            List<ProductCategory> categories = productCategories.Collection().ToList();

            if (Category == null)
            {
                products = context.Collection().ToList();
            }
            else
            {
                //this is the reason we originally exposed the Collection as a IQuerible, so we could use a where clause
                //it will allow the construction of a filter, which means, in Entity Frameword, it will convert in a SQL statement that will filter the statement
                products = context.Collection().Where(p => p.Category == Category).ToList();

            }

            ProductListViewModel model = new ProductListViewModel();
            model.Products = products;
            model.ProductCategories = categories;

            return View(model);

        }

        public ActionResult Details(string Id)
        {
            Product product = context.Find(Id);

            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}