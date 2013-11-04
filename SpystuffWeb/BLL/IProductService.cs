using Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IProductService
    {
        Product GetProduct(string articleNr);

        IEnumerable<Product> GetProducts();
    }
}
