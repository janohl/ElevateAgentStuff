using Models;
using Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IProductAdminService : IProductService
    {
        Product Edit(string articleNr, User user);

        void Save(Product product);

        void CancelEdit(string articleNr, User user);
    }
}
