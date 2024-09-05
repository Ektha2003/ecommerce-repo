using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;
using WebApplication5.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication5.Controllers
{
    [Authorize(Roles = "ADMIN ")]
    public class AdminController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        public AdminController(ApplicationDBContext dbContext)      
        {
            this._dbContext = dbContext;
            
        }
        public IActionResult Admin()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DashBoard()
        {
            var catagory = await _dbContext.Catagory.ToListAsync();
            return View(catagory);

        }
        [HttpGet]
        public IActionResult AddCatagory(string CatagoryId = "") {
            ViewData["Catagories"] = _dbContext.Catagory.ToList();
            var data = _dbContext.Catagory.FirstOrDefault(x => x.CatagoryId == CatagoryId) ?? new();
            return View(new CatagoryViewModel { CatagoryId = data.CatagoryId,CatagoryName = data.CatagoryName});
        
        }
        [HttpPost]

        public async Task<IActionResult> AddCatagory(CatagoryViewModel viewModel )
        {
            var catagory = new Catagory
            {
                CatagoryName = viewModel.CatagoryName

            };
            var data = _dbContext.Catagory.FirstOrDefault(x=>x.CatagoryId == viewModel.CatagoryId);
            if (data != null)
            {
                data.CatagoryName = viewModel.CatagoryName;
                _dbContext.Catagory.Update(data);
            }
            else
            {
                await _dbContext.Catagory.AddAsync(catagory);
            }
            int result = await _dbContext.SaveChangesAsync();
            ViewData["Catagories"] = _dbContext.Catagory.ToList();
            return View(new CatagoryViewModel());

        }
        [HttpGet]
        public async Task<IActionResult> DeleteCatagory(string CatagoryId)
        {
            var data = await _dbContext.Catagory.FindAsync(CatagoryId);
            if (data != null)
            {
                _dbContext.Catagory.Remove(data);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("AddCatagory","Admin");
        }
       
        [HttpGet]
        public IActionResult AddProduct(string ProductId="")
        {

            ViewData["Catagories"] = _dbContext.Catagory.ToList();
            ViewData["Products"]=_dbContext.Product.ToList();
            var data = _dbContext.Product.FirstOrDefault(x => x.ProductId == ProductId) ?? new();
            return View(new Product { ProductId = data.ProductId, ProductName = data.ProductName,ProductDescription=data.ProductDescription,Quantity=data.Quantity,Price=data.Price,CategoryId=data.CategoryId,ProductImg=data.ProductImg });
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(IFormFile file,Product product)

        {
            //string fileName=Path.GetFileNameWithoutExtension(file.FileName);
            //string extension=Path.GetExtension(file.FileName);
            //product.ProductImg = "~/Image/" + fileName;
            //fileName = Path.Combine(Server.MapPath("~/Image"), fileName);
            //product.ProductImg.SaveAs(file);

            
            if (file!=null && file.Length > 0)
            {

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    product.ProductImg = Convert.ToBase64String(fileBytes);
                }
            }
            
           
            var data = _dbContext.Product.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (data != null)
            {
                data.ProductName = product.ProductName;
                data.ProductDescription = product.ProductDescription;
                data.Price = product.Price;
                data.Quantity = product.Quantity;
                data.CategoryId = product.CategoryId;
                data.ProductImg = string.IsNullOrEmpty(product.ProductImg)?data.ProductImg:product.ProductImg;
            }
            else
            {
                await _dbContext.Product.AddAsync(product);
            }
            await _dbContext.SaveChangesAsync();
            ViewData["Catagories"] = _dbContext.Catagory.ToList();
            ViewData["Products"] = _dbContext.Product.ToList();
            return View(new Product());

        }
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(string ProductId)
        {
            var data = await _dbContext.Product.FindAsync(ProductId);
            if (data != null)
            {
                _dbContext.Product.Remove(data);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("AddProduct", "Admin");
        }
    }
}
