using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core;
using MyShop.Core.Models;

namespace MyShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        //create a local cache memory simulation of a database
        ObjectCache cache = MemoryCache.Default;

        //create a list of products to hold the list on the cache
        List<ProductCategory> productCategories;

        //constructor to be called everytime you create a new instance of the Product Repository
        //which will check if the products list is empty, if it is, it will create a new instance of the list
        public ProductCategoryRepository()
        {
            productCategories = cache["productsCategories"] as List<ProductCategory>;

            if (productCategories == null)
            {
                productCategories = new List<ProductCategory>();
            }
        }

        //update the list on the cache
        public void Commit()
        {
            cache["productsCategories"] = productCategories;
        }

        //insert a new product on the list
        public void Insert(ProductCategory p)
        {
            productCategories.Add(p);
        }

        //update an existing product
        public void Update(ProductCategory productCategory)
        {
            //find of the existing list the product which you want to update
            ProductCategory productCategoryToUpdate = productCategories.Find(p => p.Id == productCategory.Id);

            //if the product exists, then you will copy the information from the product we sent in the method
            //will automatically update the product in the underline list
            if (productCategoryToUpdate != null)
            {
                productCategoryToUpdate = productCategory;
            }
            else
            {
                throw new Exception("Product Category not found");
            }
        }

        //method to find an specific product in the list
        public ProductCategory Find(string Id)
        {
            ProductCategory productCategory = productCategories.Find(p => p.Id == Id);


            if (productCategory != null)
            {
                return productCategory;
            }
            else
            {
                throw new Exception("Product Category not found");
            }

        }

        //method to return a queryable list of products
        public IQueryable<ProductCategory> Collection()
        {
            return productCategories.AsQueryable();
        }

        //method to delete an specific product in the list
        public void Delete(string Id)
        {

            ProductCategory productCategoryToDelete = productCategories.Find(p => p.Id == Id);


            if (productCategoryToDelete != null)
            {
                productCategories.Remove(productCategoryToDelete);
            }
            else
            {
                throw new Exception("Product Category not found");
            }

        }

    }
}
