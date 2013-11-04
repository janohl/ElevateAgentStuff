using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Products
{
    [Serializable]
    public class Product
    {
        [Key]
        public string ArticleNr { get; set; }

        [Data]
        public String Name { get; set; }

        [Data]
        public String Description { get; set; }
    }
}
