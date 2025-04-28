using JumiaProject.Interfaces;
using JumiaProject.Models;
using JumiaProject.Repositories;
using JumiaProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace JumiaProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ICart Cart;
        private readonly ICartItem CartItem;
        private readonly IAddress Address;
        private readonly IOrder Order;
        private readonly IOrderItem OrderItem;
        private readonly IPayment Payment;
        private readonly IShippingTracking ShippingTracking;
        private readonly IProduct Product;
        private readonly IProductVariant ProductVariant;
        public OrderController(ICart _cart, UserManager<ApplicationUser> _userManager, IAddress _address, IOrder _order, IOrderItem _orderItem,IPayment _payment, IShippingTracking _ShippingTracking,IProduct _product,IProductVariant _productVariant,ICartItem _cartItem,IConfiguration configuration)
        {
            Cart = _cart;
            UserManager = _userManager;
            Address = _address;
            Order = _order;
            OrderItem = _orderItem;
            Payment = _payment;
            ShippingTracking = _ShippingTracking;
            Product = _product;
            ProductVariant = _productVariant;
            CartItem = _cartItem;
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        [HttpPost]
        public IActionResult ProcessPayment(string stripeToken, int addressId, int shippingMethodId, string userId, string paymentMethod)
        {
            try
            {
                var cartItems = Cart.GetAllCartItems(userId).Result;

                decimal? totalAmount = 0;
                foreach (var cartItem in cartItems)
                {
                    totalAmount += (cartItem.Product.Price * (1 - cartItem.Product.Discount)) * cartItem.Quantity;
                }
                decimal? taxes = totalAmount * 0.14m;
                var shippingMethod = Order.GetShippingMethodById(shippingMethodId);
                decimal? amount = totalAmount + taxes + shippingMethod.Cost;

                var options = new ChargeCreateOptions
                {
                    Amount = (long)(amount * 100), // Convert to cents
                    Currency = "egp",
                    Description = $"Order for user {userId}",
                    Source = stripeToken
                };
                var service = new ChargeService();
                var charge = service.Create(options);

                // Only now: create the order in DB
                int orderId = CreateOrderAndRelatedData(addressId, shippingMethodId, userId, paymentMethod, charge.Id);

                return RedirectToAction("OrderConfirmation", new { orderId });
            }
            catch (StripeException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("PaymentFailed", new { errorMessage = ex.Message });
            }
        }

        public IActionResult Index()
        {
            var userId = UserManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cartItems = Cart.GetAllCartItems(userId).Result;
            ViewBag.addresses = Address.GetUserAddresses(userId);
            ViewBag.userId = userId;
            return View(cartItems);
        }
        [HttpGet]
        public IActionResult EditAddress(int id)
        {
            AddressUserVM addressUser = new AddressUserVM();
            var userId = UserManager.GetUserId(User);
            var user = UserManager.FindByIdAsync(userId).Result;
            var address = Address.GetAddressById(id);
            addressUser.AddressId = address.AddressId;
            addressUser.UserId = userId;
            addressUser.UserName = user.UserName;
            addressUser.PhoneNumber = user.PhoneNumber;
            addressUser.Country = address.Country;
            addressUser.City = address.City;
            addressUser.Street = address.Street;
            addressUser.ZipCode = address.ZipCode;
            return View(addressUser);
        }
        [HttpPost]
        public IActionResult EditAddress(AddressUserVM addressUser)
        { 
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByIdAsync(addressUser.UserId).Result;
                var address = Address.GetAddressById(addressUser.AddressId);
                user.UserName = addressUser.UserName;
                user.PhoneNumber = addressUser.PhoneNumber;
                address.Country = addressUser.Country;
                address.City = addressUser.City;
                address.Street = addressUser.Street;
                address.ZipCode = addressUser.ZipCode;
                Address.UpdateAddress(address);
                var result = UserManager.UpdateAsync(user).Result;
                return RedirectToAction("Index");
            }
            return View(addressUser);
        }
        [HttpGet]
        public IActionResult AddAddress()
        {
            var userId = UserManager.GetUserId(User);
            ViewBag.userId = userId;
            return View();
        }
        [HttpPost]
        public IActionResult AddAddress(AddressUserVM addressUser)
        {
            if (ModelState.IsValid)
            {
                var userId = UserManager.GetUserId(User);
                var user = UserManager.FindByIdAsync(userId).Result;
                var address = new Models.Address();
                user.UserName = addressUser.UserName;
                user.PhoneNumber = addressUser.PhoneNumber;
                address.UserId = userId;
                address.Country = addressUser.Country;
                address.City = addressUser.City;
                address.Street = addressUser.Street;
                address.ZipCode = addressUser.ZipCode;
                Address.AddAddress(address);
                var result = UserManager.UpdateAsync(user).Result;
                return RedirectToAction("Index");
            }
            return View(addressUser);
        }



        [HttpPost]
        public IActionResult Checkout(int addressId, int shippingMethodId, string userId, string paymentMethod)
        {
            var cartItems = Cart.GetAllCartItems(userId).Result;

            decimal? totalAmount = 0;
            foreach (var cartItem in cartItems)
            {
                totalAmount += (cartItem.Quantity * (cartItem.Product.Price * (1 - cartItem.Product.Discount)));
            }

            decimal? taxes = totalAmount * 0.14m;
            decimal? amount = totalAmount + taxes;

            if (paymentMethod != "CashOnDelivery")
            {
                var shippingMethod = Order.GetShippingMethodById(shippingMethodId); // You may need to implement this
                amount += shippingMethod.Cost;
                ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
                ViewBag.AddressId = addressId;
                ViewBag.ShippingMethodId = shippingMethodId;
                ViewBag.UserId = userId;
                ViewBag.PaymentMethod = paymentMethod;
                ViewBag.Amount = amount;
                return View("Payment");
            }
            else
            {
                // For CashOnDelivery, save directly
                int orderId = CreateOrderAndRelatedData(addressId, shippingMethodId, userId, paymentMethod);
                return RedirectToAction("OrderConfirmation", new { orderId });
            }
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = Order.GetOrderById(orderId);
            return View(order);
        }

        public IActionResult PaymentFailed(int orderId, string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            var order = Order.GetOrderById(orderId);
            return View(order);
        }
        // Add this action method to your OrderController
        public IActionResult PaymentA(int orderId, string paymentMethod)
        {
            //var order = Order.GetOrderById(orderId);
            //if (order == null)
            //{
            //    return NotFound();
            //}

            //// Calculate total amount including taxes and shipping
            //decimal? amount = order.TotalAmount + order.Taxes;
            //if (order.ShippingMethod != null)
            //{
            //    amount += order.ShippingMethod.Cost;
            //}

            //ViewBag.StripePublishableKey = _configuration["Stripe:PublishableKey"];
            //ViewBag.OrderId = orderId;
            //ViewBag.PaymentMethod = paymentMethod;
            //ViewBag.Amount = amount;

            return View("Payment");
        }
        private int CreateOrderAndRelatedData(int addressId, int shippingMethodId, string userId, string paymentMethod, string transactionId = null)
        {
            var cartItems = Cart.GetAllCartItems(userId).Result;

            decimal? totalAmount = 0;
            foreach (var cartItem in cartItems)
            {
                totalAmount += (cartItem.Product.Price * (1 - cartItem.Product.Discount)) * cartItem.Quantity;
            }
            decimal? taxes = totalAmount * 0.14m;
            var shippingMethod = Order.GetShippingMethodById(shippingMethodId);
            decimal? amount = totalAmount + taxes + (shippingMethod.Cost);

            Order order = new Order
            {
                UserId = userId,
                AddressId = addressId,
                ShippingMethodId = shippingMethodId,
                OrderDate = DateTime.Now,
                OrderStatus = "Processing",
                TotalAmount = totalAmount.Value,
                Taxes = taxes.Value
            };
            Order.AddOrder(order);

            foreach (var cartItem in cartItems)
            {
                OrderItem orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceAtTime = cartItem.Product.Price,
                    Discount = cartItem.Product.Discount,
                };
                OrderItem.AddOrderItem(orderItem);
            }

            string paymentStatus = paymentMethod == "CashOnDelivery" ? "Pending" : "Completed";

            Payment payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = paymentMethod,
                Amount = amount.Value,
                PaymentDate = DateTime.Now,
                PaymentStatus = paymentStatus,
                TransactionId = transactionId
            };
            Payment.AddPayment(payment);

            string[] availableCarriers = { "FedEx", "Aramex", "DHL" };
            Random random = new Random();
            string selectedCarrier = availableCarriers[random.Next(availableCarriers.Length)];

            ShippingTracking shippingTracking = new ShippingTracking
            {
                OrderId = order.OrderId,
                TrackingNumber = Guid.NewGuid().ToString(),
                ShippingStatus = "Processing",
                LastUpdated = DateTime.Now,
                Carrier = selectedCarrier
            };
            ShippingTracking.AddShippingTracking(shippingTracking);

            foreach (var cartItem in cartItems)
            {
                var product = Product.GetProductById(cartItem.ProductId);
                product.Stock -= cartItem.Quantity;
                product.SoldNumber += cartItem.Quantity;
                Product.UpdateProduct(product);
            }

            foreach (var cartItem in cartItems)
            {
                var productVariant = ProductVariant.GetProductVariantById(cartItem.VariantId);
                if (productVariant != null)
                {
                    productVariant.Stock -= cartItem.Quantity;
                    ProductVariant.UpdateProductVariant(productVariant);
                }
            }

            foreach (var cartItem in cartItems)
            {
                CartItem.RemoveCartItem(cartItem.CartItemId);
            }

            return order.OrderId;
        }

    }
}
