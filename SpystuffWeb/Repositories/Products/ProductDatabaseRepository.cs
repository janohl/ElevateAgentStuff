using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using Models.Products;
   
namespace Repositories.Products
{
    public class ProductDatabaseRepository<T> : DatabaseRepository<T> where T : new()
    {

        public override T Get(Request request)
        {
            return base.Get(request);
        }


    }
}
