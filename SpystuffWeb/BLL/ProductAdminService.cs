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
    public class ProductAdminService : ProductService, IProductAdminService
    {
        public ProductAdminService(IStorage<Product> storage) : base(storage) { }

        public Product Edit(string articleNr, User user)
        {
            throw new NotImplementedException();
        }

        public void Save(Product product)
        {
            throw new NotImplementedException();
        }

        public void CancelEdit(string articleNr, User user)
        {
            throw new NotImplementedException();
        }
    }
}
