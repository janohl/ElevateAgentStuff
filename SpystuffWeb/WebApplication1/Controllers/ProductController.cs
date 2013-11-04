using AutoMapper;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductController : BaseController
    {
        IProductService _productService;
        IPathUtilities _pathUtilites;

        public ProductController(IProductService productService, IPathUtilities pathUtilites) : base()
        {
            _productService = productService;
            _pathUtilites = pathUtilites;
        }

        public ActionResult Index()
        {
            //try
            {               
                var viewModel = new ProductListingViewModel();

                viewModel.Products = Mapper.Map<List<ProductViewModel>>(_productService.GetProducts());
                viewModel.Products.ForEach(p => p.Image = _pathUtilites.ToAbsolute(string.Format("~/img/Products/{0}.png", p.ArticleNr))); 

                return View(viewModel);
            }
            //catch(Exception exp)
            //{
            //    Logger.Error(exp);   
            //}

            //return View(); 
        }

        public ViewResult Info(string articleNr)
        {
            //try
            {
                var viewModel = new ProductViewModel();

                viewModel = Mapper.Map<ProductViewModel>(_productService.GetProduct(articleNr));
                viewModel.Image = _pathUtilites.ToAbsolute(string.Format("~/img/Products/{0}.png", viewModel.ArticleNr));

                return View(viewModel);
            }
            //catch (Exception exp)
            //{
            //    Logger.Error(exp);
            //}

           // return View();             
        }
    }
}