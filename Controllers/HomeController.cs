using E_Commerce.Data;
using E_Commerce.Models.Home;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using E_Commerce.Models;
using E_Commerce.Repository;
using System.Web.Security;
using System.Web.UI;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();

        [AllowAnonymous]
        public ActionResult Index(string search, int? page)
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        }

        [Authorize(Roles = "User")]
        public ActionResult AddToCart(int productId)
        {
            if (Session["cart"] == null || ((List<Item>)Session["cart"]).Count == 0)
            {
                List<Item> cart = new List<Item>();
                var product = _unitOfWork.DBEntity.Tbl_Product.Find(productId);
                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var count = cart.Count();
                var product = _unitOfWork.DBEntity.Tbl_Product.Find(productId);
                for (int i = 0; i < count; i++)
                {
                    if (cart[i].Product.ProductId == productId)
                    {
                        int prevQty = cart[i].Quantity;
                        cart.Remove(cart[i]);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty + 1
                        }); ;
                        break;
                    }
                    else
                    {
                        var prd = cart.Where(x => x.Product.ProductId == productId).SingleOrDefault();
                        if (prd == null)
                        {
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = 1
                            });
                        }
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Index");
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your about page.";

            return View();
        }

        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return Redirect("Index");
        }

        public ActionResult Checkout()
        {
            return View();
        }


        public ActionResult CheckoutDetails()
        {
            return View();
        }

        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = _unitOfWork.DBEntity.Tbl_Product.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        cart.Remove(item);
                        int prevQty = item.Quantity;
                        if (prevQty > 1)
                        {
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = prevQty - 1
                            });
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }

        public ActionResult IncreaseQty(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            var product = _unitOfWork.DBEntity.Tbl_Product.Find(productId);
            foreach (var item in cart)
            {
                if (item.Product.ProductId == productId)
                {
                    int prevQty = item.Quantity;
                    if (prevQty > 0)
                    {
                        cart.Remove(item);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty + 1
                        });
                    }
                    break;
                }
            }
            Session["cart"] = cart;
            return Redirect("Checkout");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Tbl_Members member)
        {
            if (ModelState.IsValid)
            {
                var tblMember = _unitOfWork.DBEntity.Tbl_Members.FirstOrDefault(u =>
                                                                    u.Username.ToLower() == member.Username &&
                                                                    u.Password == member.Password);

                if (tblMember != null)
                {
                    FormsAuthentication.SetAuthCookie(tblMember.Username, false);
                    int role = _unitOfWork.DBEntity.Tbl_MemberRole.FirstOrDefault(mr => mr.MemberId == tblMember.MemberId).RoleId ?? -1;
                    if (role == 1)
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (role == 2)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid Username or Password");
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(Tbl_Members member)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.DBEntity.Tbl_Members.Add(member);
                _unitOfWork.DBEntity.Tbl_MemberRole.Add(new Tbl_MemberRole()
                {
                    MemberId = member.MemberId,
                    RoleId = 2
                });
                _unitOfWork.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Category(string category)
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(category, 4, null));
        }

        [AllowAnonymous]
        public ActionResult Product(int? productId)
        {
            if (productId == null)
            {
                return Redirect("Index");
            }

            Tbl_Product product = _unitOfWork.DBEntity.Tbl_Product.FirstOrDefault(p => p.ProductId == productId);

            return View(product);
        }

        public ActionResult OrderPlaced()
        {
            return View();
        }

    }
}
