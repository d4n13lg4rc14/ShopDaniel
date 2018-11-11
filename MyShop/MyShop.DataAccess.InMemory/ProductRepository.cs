using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Runtime.InteropServices.WindowsRuntime;
using MyShop.Core;
using MyShop.Core.Models;

namespace MyShop.DataAccess.InMemory
{
    public class ProductRepository
    {
       //create a local cache memory simulation of a database
       ObjectCache cache = MemoryCache.Default;
       
       //create a list of products to hold the list on the cache
       List<Product> products = new List<Product>();

        //constructor to be called everytime you create a new instance of the Product Repository
        //which will check if the products list is empty, if it is, it will create a new instance of the list
        public ProductRepository()
        {
            products = cache["products"] as List<Product>;

            if (products == null)
            {
                products = new List<Product>();
            }
        }

        //update the list on the cache
        public void Commit()
        {
            cache["products"] = products;
        }

        //insert a new product on the list
        public void Insert(Product p)
        {
            products.Add(p);
        }

        //update an existing product
        public void Update(Product product)
        {
            //find of the existing list the product which you want to update
            Product productToUpdate = products.Find(p => p.Id == product.Id);

            //if the product exists, then you will copy the information from the product we sent in the method
            //will automatically update the product in the underline list
            if (productToUpdate != null)
            {
                productToUpdate = product;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        //method to find an specific product in the list
        public Product Find(string Id)
        {
            Product product = products.Find(p => p.Id == Id);

           
            if (product != null)
            {
               return product;
            }
            else
            {
                throw new Exception("Product not found");
            }

        }

        //method to return a queryable list of products
        public IQueryable<Product> Collection()
        {
            return products.AsQueryable();
        }

        //method to delete an specific product in the list
        public void Delete(string Id)
        {
           
            Product productToDelete = products.Find(p => p.Id == Id);

            
            if (productToDelete != null)
            {
                products.Remove(productToDelete);
            }
            else
            {
                throw new Exception("Product not found");
            }

        }
    
    }
}
