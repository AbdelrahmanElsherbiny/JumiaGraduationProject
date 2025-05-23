﻿using System.Security.Claims;
using JumiaProject.Context;
using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JumiaProject.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly JumiaContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICart _cart;
        private readonly IProduct _product;
        public CartController(JumiaContext context, UserManager<ApplicationUser> userManager, ICart _cart, IProduct product) : base(_cart, userManager)
        {
            this.context = context;
            this.userManager = userManager;
            this._cart = _cart;
            _product = product;
        }
     
        public async Task<IActionResult> Index()
        {
            string id = userManager.GetUserId(User);
            //if (id == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var cart=await _cart.GetCartByUserId(id);
            var cartItems = await _cart.GetAllCartItems(id);
            var price = await _cart.CalculateCartTotalPrice(id);
            ViewBag.Price = price;
            var totalitems = await _cart.GetTotalCartQuantity(cart.CartId);
            ViewBag.Totalitems = totalitems;
            var bestseller = _product.Get6BestSeller();
            ViewBag.Bestseller = bestseller;
            var mostDiscount=_product.GetMostDiscount();
            ViewBag.MostDiscount = mostDiscount;
            return View("Index", cartItems);
        }
        [HttpGet]
        public async Task<IActionResult> GetCartItemQuantity(int productId, int? variantId)
        {
            string userId=userManager.GetUserId(User);
            var cart = await _cart.GetCartByUserId(userId);
            if (!variantId.HasValue) { variantId = null; }
            int quantity = await _cart.GetCartItemQuantity(cart.CartId, productId, variantId);

            return Ok(quantity);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCartItemAjax(int itemId)
        {
            try
            {
                string userId = userManager.GetUserId(User);
                Console.WriteLine("User ID: " + userId);

                var result = await _cart.RemoveItemFromCart(userId, itemId);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCartItem(int productId, int? variantId, int quantity)
        {
           
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
               
            
                if (userId != null)
                {
                    var cart = await _cart.GetCartByUserId(userId);
                    await _cart.AddOrUpdateCartItem(cart.CartId, productId, variantId, quantity);
                    return Json(new { success = true, message = "Item added to cart successfully" });
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
          
        
        public async Task<IActionResult> GetTotalCartQuantity()
        {
              string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Content("0");
            }
            var cart = await _cart.GetCartByUserId(userId);

            int totalQuantity = await _cart.GetTotalCartQuantity(cart.CartId);

            return Json(totalQuantity);
        }
        //public IActionResult GetVariants(int productId)
        //{
        //    var variants = context.ProductVariants
        //        .Where(v => v.ProductId == productId)
        //        .Select(v => new
        //        {
        //            variantId = v.VariantId,
        //            sizeName = v.Size.SizeLabel,
        //            stock = v.Stock,
        //            price=v.Product.Price,
        //            discount=v.Product.Discount,

        //        }).ToList();
        //    return Json(variants);
        //}
        public async Task<IActionResult> GetVariants(int productId)
        {
            string userId = userManager.GetUserId(User);
            var userCart = await _cart.GetCartByUserId(userId);

            var variants = context.ProductVariants
                .Where(v => v.ProductId == productId)
                .Select(v => new
                {
                    variantId = v.VariantId,
                    sizeName = v.Size.SizeLabel,
                    stock = v.Stock,
                    price = v.Product.Price,
                    discount = v.Product.Discount,
                    // Left join with cart items to get quantity
                    quantityInCart = userCart != null ?
                        context.CartItems
                            .Where(ci => ci.CartId == userCart.CartId &&
                                   ci.VariantId == v.VariantId)
                            .Select(ci => ci.Quantity)
                            .FirstOrDefault()
                        : 0
                }).ToList();

            return Json(variants);
        }
        [HttpGet]
        public IActionResult GetProductVariants(int productId)
        {
            var variants = context.ProductVariants
                .Where(v => v.ProductId == productId)
                .Select(v => new
                {
                    variantId = v.VariantId,
                    sizeName = v.Size.SizeLabel,
                    stock = v.Stock
                })
                .ToList();

            return Json(new { success = true, variants = variants });
        }
        public async Task<IActionResult> GetTotalPrice()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Content("0");
            }
            var cart = await _cart.GetCartByUserId(userId);

            decimal totalprice = await _cart.CalculateCartTotalPrice(userId);

            return Json(totalprice.ToString("N2"));
        }
        public async Task<IActionResult> GetCartCount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Content("0");
            }
            var cart = await _cart.GetCartByUserId(userId);
            if (cart == null)
            {
                return Content("0");
            }

            int cartCount = await _cart.GetTotalCartQuantity(cart.CartId);
            
            return Content(cartCount.ToString());
        }

        [HttpGet]
        public JsonResult GetTotalQuantityForProduct(int productId)
        {
            var totalQuantity = 0;

            // Find all cart items for this product (with any variant)
            var cartItems = context.CartItems
                .Where(ci => ci.ProductId == productId)
                .ToList();

            // Sum up all quantities
            foreach (var item in cartItems)
            {
                totalQuantity += item.Quantity;
            }

            return Json(totalQuantity);
        }
        [HttpGet]
        public JsonResult GetTotalProductQuantity(int productId)
        {
            // Find all cart items for this product (any variant)
            var items = context.CartItems
                .Where(c => c.ProductId == productId)
                .ToList();

            int totalQuantity = items.Sum(i => i.Quantity);
           
            return Json(totalQuantity);
        }
    }
}
