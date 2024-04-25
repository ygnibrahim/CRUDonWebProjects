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

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
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
                CreatedAt = DateTime.Now,
            };

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index","Products");
        }
    }
}
