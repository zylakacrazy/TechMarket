using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechMarket.Models;

namespace TechMarket.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                int id = int.Parse(User.Identity.Name);
                var tt = db.tbl_Shop.Where(x => x.id_account == id).ToList();
                if (tt.Count > 0)
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Mỗi tài khoản chỉ có thể tạo 1 cửa hàng!');window.location.href=('../../Home/Index');</script>");

                }
                else
                {
                    return View();
                }
            }
        }
        [HttpPost]
        public ActionResult Index(tbl_Shop shop, HttpPostedFileBase shop_avatar, HttpPostedFileBase shop_background, string diachi)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            if (ModelState.IsValid)
            {
                int a = int.Parse(User.Identity.Name);
                var result = db.tbl_Coin.Where(x => x.id_account == a);
                float coin = 0;
                float giach = -float.Parse(ConfigurationManager.AppSettings["giataocuahang"]);
                foreach (var item in result)
                {
                    coin += (float)item.coin_total;
                }
                if (coin >= giach)
                {
                    var shopne = new tbl_Shop
                    {
                        id_account = shop.id_account,
                        shop_name = shop.shop_name,
                        shop_address ="Tỉnh " + shop.shop_address+ ", "+diachi,
                        //shop_avatar = shop.shop_avatar,
                        //shop_background = shop.shop_background,
                        shop_introduce = shop.shop_introduce,
                        shop_link = shop.shop_link,
                        shop_phone = shop.shop_phone,
                    };
                    if (shop_avatar != null && shop_avatar.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(shop_avatar.FileName);
                        string path = Server.MapPath("~/Images/Shop/Avatar/" + fileName);
                        shop_avatar.SaveAs(path);

                        shopne.shop_avatar = "../Images/Shop/Avatar/"+ fileName;
                    }
                    if (shop_background != null && shop_background.ContentLength > 0)
                    {
                        string fileName1 = Path.GetFileName(shop_background.FileName);
                        string path1 = Server.MapPath("~/Images/Shop/Background/" + fileName1);
                        shop_background.SaveAs(path1);

                        shopne.shop_background = "../Images/Shop/Background/" + fileName1;
                    }
                    if (ModelState.IsValid)
                    {
                        db.tbl_Shop.Add(shopne);
                        db.SaveChanges();
                    }
                    
                    tbl_Coin upcoin = new tbl_Coin
                    {
                        id_account = a,
                        coin_total = giach,
                        coin_details = "Tạo gian hàng: " + shop.shop_name
                    };

                    var idshop = db.tbl_Shop.OrderByDescending(x => x.Id).FirstOrDefault();
                    int songay = int.Parse(ConfigurationManager.AppSettings["songaykhitaocuahang"].ToString());
                    tbl_DateOfHire _DateOfHire = new tbl_DateOfHire
                    {
                        doh_startday = DateTime.Now,
                        doh_expirationdate = DateTime.Now.AddDays(songay),
                        doh_note = "Tạo gian hàng: "+ shop.shop_name,
                        id_shop = idshop.Id
                    };
                    db.tbl_DateOfHire.Add(_DateOfHire); ;
                    db.tbl_Coin.Add(upcoin);
                    db.SaveChanges();

                    var ids = db.tbl_Shop.Where(x => x.id_account == a).FirstOrDefault();
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.msg = "Vui lòng nạp thêm tiền để tiếp tục!";
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Thêm thất bại!");
                return View(shop);
            }
        }
        public ActionResult ShopProfile(int id)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var hsd = db.tbl_DateOfHire.Where(x => x.id_shop == id).OrderByDescending(x => x.Id).FirstOrDefault();

                if (DateTime.Now > hsd.doh_expirationdate)
                {
                    int idh = int.Parse(User.Identity.Name);
                    var idch = db.tbl_Shop.Where(x => x.id_account == idh&& x.Id == id);
                    if (idch.Count() == 1)
                    {
                        return Content("<script>alert('Cửa hàng đã hết hạn sử dụng vui lòng gia hạng để tiếp tục sử dụng!');window.location.href=('../../Shop/extend');</script>");
                    }
                    else
                    {
                        var result = db.tbl_Shop.FirstOrDefault(x => x.Id == id);
                        if (result != null)
                        {
                            //Session["IdShop"] = result.Id;
                            return View(result);
                        }
                        else
                        {
                            return RedirectToAction("Login", "Home");
                        }
                    }
                }
                else
                {
                    //int idcu = int.Parse(User.Identity.Name);
                    var result = db.tbl_Shop.FirstOrDefault(x => x.Id == id);
                    if (result != null)
                    {
                        Session["IdShop"] = result.Id;
                        return View(result);
                    }
                    else
                    {
                        TempData["msg"] = "<script>alert('Tài khoản chưa đăng ký shop kinh doanh!');</script>";
                        return RedirectToAction("Login", "Home");
                    }
                }
            }
        }
        public ActionResult extend()
        {
            Session["giahang10ngay"] = ConfigurationManager.AppSettings["giahang10ngay"].ToString();
            Session["giahang30ngay"] = ConfigurationManager.AppSettings["giahang30ngay"].ToString();
            Session["giahang6thang"] = ConfigurationManager.AppSettings["giahang6thang"].ToString();
            Session["giahang1nam"] = ConfigurationManager.AppSettings["giahang1nam"].ToString();
            Session["giahang5nam"] = ConfigurationManager.AppSettings["giahang5nam"].ToString();
            return View();
        }
        public ActionResult GiaHan(int gia, int ngay, int thang, int nam)
        {
            int a = int.Parse(User.Identity.Name);
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            int idacc = int.Parse(User.Identity.Name);
            var coin1 = db.tbl_Coin.Where(x => x.id_account == idacc);
            float tong = 0;
            foreach (var item in coin1)
            {
                tong += (float)item.coin_total;
            }
            if (tong < Math.Abs(gia))
            {
                return Content("<script>alert('Tài khoản không đủ thực hiện dịch vụ!');" +
                        "window.location.href=('../Payment/index');</script>");
            }
            if (ModelState.IsValid)
            {
                tbl_Coin upcoin = new tbl_Coin
                {
                    id_account = a,
                    coin_total = gia,
                    coin_date = DateTime.Now,
                    coin_details = "Gia hạn cửa hàng"
                };
                var shop = db.tbl_Shop.Where(x => x.id_account == a).FirstOrDefault();
                int idc = shop.Id;
                tbl_DateOfHire _DateOfHire = new tbl_DateOfHire
                {
                    doh_startday = DateTime.Now,
                    doh_expirationdate = DateTime.Now.AddDays(ngay).AddMonths(thang).AddYears(nam),
                    doh_note = "Gia hạn cửa hàng ",
                    id_shop = idc
                };
                db.tbl_DateOfHire.Add(_DateOfHire); ;
                db.tbl_Coin.Add(upcoin);
                db.SaveChanges();
                return RedirectToAction("index","home");
            }
            return View();
        }
        public ActionResult EditShop(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                var profile = db.tbl_Shop.Find(id);
                return View(profile);
            }
            return RedirectToAction("Login","Home");
        }
        public ActionResult editS(tbl_Shop shop, HttpPostedFileBase shop_avatar, HttpPostedFileBase shop_background, string diachi)
        {
            if (ModelState.IsValid)
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                var model = db.tbl_Shop.Find(shop.Id);
                model.id_account = shop.id_account;
                model.shop_address ="Tỉnh " + shop.shop_address + ", " + diachi;
                model.shop_introduce = shop.shop_introduce;
                model.shop_link = shop.shop_link;
                model.shop_name = shop.shop_name;
                model.shop_phone = shop.shop_phone;
                
                if (shop_avatar != null && shop_avatar.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(shop_avatar.FileName);
                    string path = Server.MapPath("~/Images/Shop/Avatar/" + fileName);
                    shop_avatar.SaveAs(path);

                    model.shop_avatar = "../Images/Shop/Avatar/" + fileName;
                }
                if (shop_background != null && shop_background.ContentLength > 0)
                {
                    string fileName1 = Path.GetFileName(shop_background.FileName);
                    string path1 = Server.MapPath("~/Images/Shop/Background/" + fileName1);
                    shop_background.SaveAs(path1);

                    model.shop_background = "../Images/Shop/Background/" + fileName1;
                }
                db.SaveChanges();
                return Content("<script language='javascript' type='text/javascript'>" +
                    "alert('Sửa thông tin thành công!');history.back()</script>");

            }
            return View();
        }
    }
}