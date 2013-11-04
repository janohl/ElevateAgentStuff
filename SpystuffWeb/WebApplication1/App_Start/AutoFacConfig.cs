using Autofac;
using Autofac.Integration.Mvc;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1
{
    public class AutofacConfig
    {
        public static void RegisterServices()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly()).InstancePerHttpRequest();
            builder.RegisterType<ProductService>().As<IProductService>().SingleInstance();

            IContainer container = builder.Build();            
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));   
        }
    }
}