using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication5.Data;
using WebApplication5.Models;
using System.Web;
using WebApplication5.Models.Entities;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;

namespace AspnetIdentityRoleBasedTutorial.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private const string CartSessionKey = "Cart";
        
        public HomeController(ILogger<HomeController> logger, ApplicationDBContext dBContext, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _dbContext = dBContext;
            _userManager = userManager;
        }
        public IActionResult Index(string searchString = "")
        {
            var temp = HttpContext.Request.Host;

            var cat = _dbContext.Catagory.ToList();
            ViewData["Catagories"] = cat;
          
            Dictionary<Catagory, List<Product>> products = new Dictionary<Catagory, List<Product>>();
            var data = _dbContext.Product.ToList();
            if (!string.IsNullOrEmpty(searchString)) {
                data = data.Where(p => p.ProductName.StartsWith(searchString)).ToList();
            }
            products.Add(new Catagory { CatagoryName = "All Product", CatagoryId = "Tab1" }, data);
            foreach (var item in cat)
            {
                products.Add(item, data.Where(x => x.CategoryId == item.CatagoryId).ToList());
            }
            return View(products);

        }
        [HttpGet]
        public List<Product> GetProductByCatagory(string catagoryId)
        {
            var data = _dbContext.Product.Where(x => x.CategoryId == catagoryId).ToList();
            return data;
        }
        public IActionResult Shop()
        {
            ViewData["Catagories"] = _dbContext.Catagory.ToList();
            ViewData["Products"] = _dbContext.Product.ToList();
            return View();
        }
        [Authorize]
        public IActionResult Contact()
        {
            return View();
        }
        public List<ShoppingCart> GetCartFromSession()
        {
            var session = HttpContext.Session;
            var sessionValue = session.GetString(CartSessionKey);
            if (sessionValue == null)
            {
                session.SetString(CartSessionKey, JsonConvert.SerializeObject(new List<ShoppingCart>()));
                return new List<ShoppingCart>();
            }
            return JsonConvert.DeserializeObject<List<ShoppingCart>>(sessionValue)??new();
        }
        [HttpPost]
        public void SetCartInSession(string? jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData) && jsonData!="[]" && jsonData!="null")
            {
                HttpContext.Session.SetString(CartSessionKey, jsonData);
            }
        }
        [HttpGet]
        public IActionResult AddToCart(string productId)
        {
            var data = new ShoppingCart();
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid).Result;
            if (user != null)
            {
               data = _dbContext.ShoppingCart.Include(x => x.product).FirstOrDefault(x => x.product.ProductId == productId && x.userId==userid);
             
                if (data != null)
                {
                    data.Quantity++;
                    data.Price += data.product.Price * data.Quantity;
                    _dbContext.ShoppingCart.Update(data);
                    _dbContext.SaveChanges();
                }
                else
                {
                    var temp = _dbContext.Product.FirstOrDefault(x => x.ProductId == productId);
                    data = new ShoppingCart
                    {
                        product = temp,
                        Quantity = 1,
                        Price = temp.Price,
                        userId= userid
                    };
                    _dbContext.ShoppingCart.Add(data);
                    _dbContext.SaveChanges();
                }
            }
            else { 
                var cart = GetCartFromSession();
                data=cart.FirstOrDefault(x=>x.product.ProductId==productId);
                if (data == null)
                {
                    var temp = _dbContext.Product.FirstOrDefault(x => x.ProductId == productId);
                    cart.Add(new ShoppingCart
                    {
                        product = temp,
                        Quantity = 1,
                        Price = temp.Price

                    });
                }
                else
                {
                    cart[cart.IndexOf(data)].Quantity++;
                    cart[cart.IndexOf(data)].Price = cart[cart.IndexOf(data)].product.Price * cart[cart.IndexOf(data)].Quantity;
                }
                HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));

            }
             
                return RedirectToAction("Cart");
        }
        [HttpGet]
        public IActionResult Cart()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid).Result;
            var model = new CartViewModel();
               
            if (user == null)
            {
                var cart = GetCartFromSession();
                model.Items = cart;
            }
            else
            {
                var data = _dbContext.ShoppingCart.Include(x=>x.product).Where(x=>x.userId==userid).ToList();
                 model.Items = data;
            }

            ViewData["ShoppingCart"] = _dbContext.ShoppingCart.ToList();
            return View(model);
        }
        [HttpPost]
        public IActionResult AddCart(CartViewModel cartViewModel) {


            if (cartViewModel == null)
            {
                return NoContent();
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            var user  = _userManager.FindByIdAsync(userid).Result;
            if (user != null) 
            { 
                var data =cartViewModel;
            }
            var cart = GetCartFromSession();
            foreach (var item in cartViewModel.Items) {
                var cartItem=cart.FirstOrDefault(x => x.Id == item.Id);
                if (cartItem != null)
                {
                    var index=cart.IndexOf(cartItem);
                    cart[index].Quantity += item.Quantity;
                }
                else
                {
                    cart.Add(item);
                }

            }
            if (user == null)
            {
               HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cartViewModel.Items));
            }
                return RedirectToAction("Cart");
          

        }
        //[HttpPost]
        //public IActionResult UpdateCart(string productId, int quantity)
        //{
        //    var cart = GetCartFromSession();
        //    var item = cart.FirstOrDefault(i => i.product.ProductId == productId);

        //    if (item != null)
        //    {
        //        if (quantity <= 0)
        //        {
        //            cart.Remove(item); 
        //        }
        //        else
        //        {
        //            item.Quantity = quantity;
        //        }
        //    }

        //    return RedirectToAction("Cart");
        //}
        [HttpGet]
        public IActionResult RemoveFromCart(string productId)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid).Result;
            var model = new ShoppingCart();
            if (user != null)
            {
                model = _dbContext.ShoppingCart.Include(x => x.product).FirstOrDefault(x => x.product.ProductId == productId && x.userId == userid);
                _dbContext.Remove(model);
                _dbContext.SaveChanges();
            }
            else
            {
                var cart = GetCartFromSession();
                var item = cart.FirstOrDefault(i => i.product.ProductId == productId);

                if (item != null)
                {
                    cart.Remove(item);
                }
              HttpContext.Session.SetString(CartSessionKey ,JsonConvert.SerializeObject(cart));
            }
            return RedirectToAction("Cart");
        }
        public object IncreaseQ(string Id)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid).Result;
            int quantity = 0;
            decimal? price = 0;
            decimal? total = 0;
            if (user != null)
            {
                var data = _dbContext.ShoppingCart.Include(x => x.product).FirstOrDefault(i => i.Id == Id);
                if (data != null)
                {
                    if (data.Quantity < data.product.Quantity)
                    {
                        ++data.Quantity;

                    }
                    quantity = data.Quantity;
                    price = data.Price = data.product.Price * data.Quantity;
                    
                    _dbContext.ShoppingCart.Update(data);
                    _dbContext.SaveChanges();
                    _dbContext.ShoppingCart.Where(x=>x.userId==userid).ToList().ForEach(item=> { total += item.Price; });
                }

            }
            else
            {
                var cart = GetCartFromSession();
                var item = cart.FirstOrDefault(x => x.Id == Id);
                if (item != null)
                {
                    var index = cart.IndexOf(item);


                    if (item.Quantity < item.product.Quantity)
                    {
                        ++cart[index].Quantity;
                    }
                    quantity = cart[index].Quantity;
                    price= cart[index].Price = cart[index].product.Price * cart[index].Quantity;

                }
                HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
                cart.ForEach(item=> { total += item.Price; });
            }

            return new {quantity,price,total};

        }
        public object DecreseQ(string Id)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.FindByIdAsync(userid).Result;
            int quantity = 0;
            decimal? price = 0;
            decimal? total = 0;
            if (user != null)
            {
                var data = _dbContext.ShoppingCart.Include(x => x.product).FirstOrDefault(i => i.Id == Id);
                if (data != null)
                {
                    if (data.Quantity > 1)
                    {

                        --data.Quantity;
                    }
                    quantity = data.Quantity;
                    price =data.Price= data.product.Price * data.Quantity;
                    
                    _dbContext.ShoppingCart.Update(data);
                    _dbContext.SaveChanges();
                    _dbContext.ShoppingCart.Where(x => x.userId == userid).ToList().ForEach(item => { total += item.Price; });

                }

            }
            else
            {
                var cart = GetCartFromSession();
                var item = cart.FirstOrDefault(x => x.Id == Id);
                if (item != null)
                {
                    var index = cart.IndexOf(item);


                    if (item.Quantity > 1)
                    {
                        --cart[index].Quantity;
                    }
                    quantity = cart[index].Quantity;
                    price = cart[index].Price = cart[index].product.Price * cart[index].Quantity;
                }
                 HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
                cart.ForEach(item => { total += item.Price; });

            }
            return new {quantity,price,total};

        }

        //public IActionResult CheckOut()
        //{
        //    var userid = _userManager.GetUserId(HttpContext.User);
        //    var user = _userManager.FindByIdAsync(userid).Result;
        //    var model = new CartViewModel();

        //    if (user == null)
        //    {
        //    }
        //    else
        //    {
        //        var data = _dbContext.ShoppingCart.Include(x => x.product).Where(x => x.userId == userid).ToList();
        //        model.Items = data;
        //    }

        //    ViewData["ShoppingCart"] = _dbContext.ShoppingCart.ToList();
        //    return View(model);
        //}


        //[HttpPost]
        //public IActionResult CheckOrder(string Id)
        //{9
        //    ViewData["ShoppingCart"] = _dbContext.ShoppingCart.ToList();
        //    return View(CheckOut);
        //}

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            return View();

        }

    }
}