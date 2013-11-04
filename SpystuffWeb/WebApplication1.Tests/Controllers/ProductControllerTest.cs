using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1;
using WebApplication1.Controllers;
using WebApplication1.Models;
using BLL;
using Repositories;
using Moq;
using Models.Products;
using Models;

namespace WebApplication1.Tests.Controllers
{
    [TestClass]
    public class ProductControllerTest
    {
        private Mock<IPathUtilities> _mockPathUtilites;

        [TestInitialize]
        public void Initialize()
        {
            AutomapperConfig.RegisterAutomapper();
            _mockPathUtilites = new Mock<IPathUtilities>();
        }

        [TestMethod]
        public void Index()
        {
            var mockStorage = new Mock<IStorage<Product>>();
            Product p = new Product { ArticleNr = "1", Name = "name", Description = "desc" };
            mockStorage.Setup(r => r.GetList(null)).Returns(new List<Product> { p });

            var controller = new ProductController(new ProductService(mockStorage.Object), _mockPathUtilites.Object);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as ProductListingViewModel;
            Assert.IsTrue(model.Products.Any());
        }

        [TestMethod]
        public void Info()
        {
            var mockStorage = new Mock<IStorage<Product>>();
            Product p = new Product { ArticleNr = "1", Name = "name", Description = "desc" };
            mockStorage.Setup(r => r.Get(It.IsAny<Request>())).Returns(p);

            var controller = new ProductController(new ProductService(mockStorage.Object), _mockPathUtilites.Object);

            ViewResult result = controller.Info("12345-21") as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as ProductViewModel;
            Assert.AreEqual(p.ArticleNr, model.ArticleNr);
        }
    }
}
