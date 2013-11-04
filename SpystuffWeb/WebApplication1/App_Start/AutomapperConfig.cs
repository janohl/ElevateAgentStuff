using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using BLL;
using Models;
using Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1
{
    public class AutomapperConfig
    {
        public static void RegisterAutomapper()
        {
            Mapper.CreateMap<Product, ProductViewModel>();
        }
    }
}