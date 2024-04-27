using Microsoft.AspNetCore.Mvc;
using WebProjects.Context;
using WebProjects.Models;

namespace WebProjects.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _environment;
        public ProductsController(AppDbContext appDbContext, IWebHostEnvironment environment)
        {
            _appDbContext = appDbContext;
            _environment = environment;
        }


        public IActionResult Index()
        {
            var products=_appDbContext.Products.OrderByDescending(p=>p.Id).ToList();
            return View(products);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFileName == null)
            {
                ModelState.AddModelError("ImageFileName", "The image file is required");
            }
            if(!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("ddMMyyyy");
            newFileName += Path.GetExtension(productDto.ImageFileName!.FileName);

            string imageFullPath = _environment.WebRootPath + "/products/" + newFileName;
            using(var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFileName.CopyTo(stream);
            }
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index","Products");
        }

        public IActionResult Edit(int id)
        {
            var product = _appDbContext.Products.Find(id);
            if(product== null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("dd/MM/yyyy");


            return View(productDto);
        }
        [HttpPost]
        public IActionResult Edit (int id,ProductDto productDto)
        {
            var product = _appDbContext.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("dd/MM/yyyy");
                return View(productDto);
            }

            string newFile = product.ImageFileName;
            if (productDto.ImageFileName != null)
            {
                newFile = DateTime.Now.ToString("ddMMyyyy");
                newFile += Path.GetExtension(productDto.ImageFileName.FileName);
                
                string imageFullPath = _environment.WebRootPath +"/products/" + newFile;
                using(var stream=System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFileName.CopyTo(stream);
                }

                string oldImagePath = _environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImagePath);
            }
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;

            _appDbContext.SaveChanges();
            return RedirectToAction("Index","Products");
        }

        public IActionResult Delete(int id)
        {
            var product = _appDbContext.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = _environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            _appDbContext.Products.Remove(product);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

    }
}
