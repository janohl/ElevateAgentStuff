using Models;
using Models.Products;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(IStorage<Product> storage) : base(storage) { }

        public Product GetProduct(string articleNr)
        {
            //try
            //{
                var request = new Request<ProductRequestParameter> { Parameters = new ProductRequestParameter { ArticleNr = articleNr } };
                return storage.Get(request);
            //}
            //catch (Exception exp)
            //{
            //    Logger.Error(exp);
            //}

            //return null;
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                return storage.GetList();
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
            }

            return null;
        }
    }
}
