using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TechMarket.Hubs;
using TechMarket.Models;
using TechMarket.Services;
using PagedList;
using System.Configuration;

namespace TechMarket.Controllers {
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(int page = 0)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            int PageSize = int.Parse(ConfigurationManager.AppSettings["soluongsanphamhienthi"]); // you can always do something more elegant to set this

            var result = db.tbl_Product.Where(x => x.status == 1).ToList();
            var count = result.Count();

            var data = result.Skip(page * PageSize).Take(PageSize).ToList();

            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);

            ViewBag.Page = page;

            return View(data);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            return View(new LoginData());
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginData loginData)
        {
            if (ModelState.IsValid)
            {
                int userId;
                if (new AppService().Login(loginData, out userId))
                {
                    FormsAuthentication.RedirectFromLoginPage(userId.ToString(), loginData.RememberMe);
                    int id = int.Parse(userId.ToString());
                    SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                    var result = db.tbl_Account.Find(id);
                    if(result.Role == 1)
                    {
                        
                        return Redirect("~/Home/Index");
                    }
                    else
                    {
                        Session["role"] = 0;
                        return Redirect("~/Manage/Index");
                    }
                    
                }
                else
                {
                    ViewBag.LoginError = "Sai tài khoản hoặc mật khẩu";
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }
            else
            {
                int userId = int.Parse(User.Identity.Name);
                new AppService().RemoveAllUserConnections(userId);
                ChatHub.OfflineUser(userId);
                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login");
            }
            
        }
        [HttpPost]
        public ActionResult GetChatbox(int toUserId)
        {
            ChatBoxModel chatBoxModel = new AppService().GetChatbox(toUserId);
            return PartialView("~/Views/Home/_ChatBox.cshtml", chatBoxModel);
        }

        [HttpPost]
        public ActionResult SendMessage(int toUserId, string message)
        {
            return Json(new AppService().SendMessage(toUserId, message));
        }

        [HttpPost]
        public ActionResult LazyLoadMssages(int toUserId, int skip)
        {
            return Json(new AppService().LazyLoadMssages(toUserId, skip));
        }
        public ActionResult Chat()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                return View();
            }
        }

        public ActionResult profile(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Session.Abandon();
                Models.SGDFleaMarketEntities db = new Models.SGDFleaMarketEntities();
                //int a = int.Parse(User.Identity.Name);
                var model = db.tbl_Account.SingleOrDefault(x => x.Id == id);
                return View(model);
            }
        }
        public ActionResult RegisterAccount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterAccount(tbl_Account acc, string passwordnl)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            if (string.IsNullOrEmpty(acc.username) == true || string.IsNullOrEmpty(acc.password) == true)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không được để trống!");
                return View(acc);
            }
            if (string.IsNullOrEmpty(acc.phone) == true)
            {
                ModelState.AddModelError("", "Số điện thoại phải có 10 chữ số!");
                return View(acc);
            }
            if (string.IsNullOrEmpty(passwordnl) == true)
            {
                ModelState.AddModelError("", "Vui lòng nhập lại mật khẩu!");
                return View(acc);
            }
            if(acc.username.Length < 6)
            {
                ModelState.AddModelError("", "Tên đăng nhập phải ít nhất phải có 6 ký tự!");
                return View(acc);
            }
            if(acc.username.Length > 20)
            {
                ModelState.AddModelError("", "Tên đăng nhập không được quá 20 ký tự!");
                return View(acc);
            }
            if(acc.fullname.Length > 20)
            {
                ModelState.AddModelError("", "Tên người dùng không được quá 20 ký tự!");
                return View(acc);
            }
            if (acc.password.Length < 6)
            {
                ModelState.AddModelError("", "Mật khẩu phải ít nhất phải có 6 ký tự!");
                return View(acc);
            }
            var result = db.tbl_Account.Where(x => x.username == acc.username);
            if (result.Count() == 0)
            {
                string pass = acc.password;
                string mahoa = CreateMD5.EncodePassword(pass);
                acc.password = mahoa;
                db.tbl_Account.Add(acc);
                db.SaveChanges();
                if (acc.Id > 0)
                {
                    return Content("<script>" +
                            "alert('Đăng ký tài khoản thành công!');" +
                            "window.location.href=('login');</script>");
                }
                else
                {
                    ModelState.AddModelError("", "Tạo tài khoản thất bại!");
                    return View(acc);
                }
            }
            ModelState.AddModelError("", "Tên tài khoản đã tồn tại!");
            return View(acc);
        }
        public ActionResult editProfile(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                var profile = db.tbl_Account.Find(id);
                return View(profile);
            }
            return RedirectToAction("Login");
        }
        public ActionResult editP(tbl_Account acc, HttpPostedFileBase images, string diachi)
        {
            int idacc = int.Parse(User.Identity.Name);
            if(acc.fullname.Length > 20)
            {
                return Content("<script>" +
                        "alert('Tên người dùng không được quá 20 ký tự!');" +
                        "history.back();</script>");
            }
            if(acc.phone.Length != 10)
            {
                return Content("<script>" +
                        "alert('Số điện thoại phải bao gồm 10 số!');" +
                        "history.back();</script>");
            }
            if (acc.fullname.Length < 6)
            {
                return Content("<script>" +
                        "alert('Tên người dùng không được nhỏ hơn 6 ký tự!');" +
                        "history.back();</script>");
            }
            if (acc.brithday > DateTime.Now.AddYears(-14))
            {
                return Content("<script>" +
                        "alert('Ngày sinh người dùng không hợp lệ!');" +
                        "history.back();</script>");
            }
            if (ModelState.IsValid)
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                var model = db.tbl_Account.Find(acc.Id);
                model.fullname = acc.fullname;
                model.phone = acc.phone;
                model.address = "Tỉnh "+acc.address+ ", " + diachi;
                model.brithday = acc.brithday;
                model.cmnd = acc.cmnd;
                model.email = acc.email;
                model.facebook = acc.facebook;
                model.gender = acc.gender;
                if (images != null)
                {
                    string fileName = Path.GetFileName(images.FileName);
                    string path = Server.MapPath("../Images/Avatar/" + fileName);
                    images.SaveAs(path);

                    model.images = "../Images/Avatar/"+ fileName;
                }
                db.SaveChanges();
                return Content("<script>" +
                        "alert('Cập nhật thông tin thành công!');" +
                        "history.back();</script>");
            }
            return View();
        }
        public ActionResult Push()
        {
            Session["gia1"] = ConfigurationManager.AppSettings["giadaytin1ngay"].ToString();
            Session["gia2"] = ConfigurationManager.AppSettings["giadaytin3ngay"].ToString();
            Session["gia3"] = ConfigurationManager.AppSettings["giadaytin7ngay"].ToString();     
            return View();
        }
        public ActionResult PushProduct(int id, string day)
        {
            //int id = int.Parse(User.Identity.Name);
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            
            int ngay = 0;
            double gia = 0;
            if(day == "daytin1")
            {
                ngay = 1;
                gia = -double.Parse(ConfigurationManager.AppSettings["giadaytin1ngay"].ToString());
            }else if (day == "daytin2")
            {
                ngay = 3;
                gia = -double.Parse(ConfigurationManager.AppSettings["giadaytin3ngay"].ToString());
            }else if (day == "daytin3")
            {
                ngay = 7;
                gia = -double.Parse(ConfigurationManager.AppSettings["giadaytin7ngay"].ToString());
            }

            tbl_Coin coin = new tbl_Coin
            {
                id_account = int.Parse(User.Identity.Name),
                coin_date = DateTime.Now,
                coin_total = gia,
                coin_details = "Đẩy tin bán hàng"
            };
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
            else
            {
            if (ModelState.IsValid)
                {
                    var sp = db.tbl_Product.Find(id);
                    if (sp != null)
                    {
                        sp.EndPush = DateTime.Now.AddDays(ngay);
                        sp.TypeDisplayProduct = 1; // tin ưu tiên
                        sp.PushDate = DateTime.Now;
                        db.tbl_Coin.Add(coin);
                        db.SaveChanges();
                        return Content("<script>alert('Đẩy tin thành công!');" +
                            "window.location.href=('Index');</script>");
                    }
                    else
                    {
                        return Content("<script>alert('Đã xảy ra lỗi trong quá trình xử lý')</script>");
                    }
                }
                return View();
            }            
        }
        public ActionResult DanhGia(int rating, int idcus, int idshop, int loai)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            int idkh = int.Parse(User.Identity.Name);
            int kt = db.tbl_Evaluete.Where(x=>x.id_customer == idkh&& x.id_account == idcus||x.id_shop == idshop).Count();
            if(kt >= 1)
            {
                return Content("<script>alert('Mỗi tài khoản chỉ có thể đánh giá người bán hoặc cửa hàng 1 lần!');history.back();</script>");
            }
            if(loai == 0)
            {
                int idacc = int.Parse(User.Identity.Name);
                
                tbl_Evaluete evaluete = new tbl_Evaluete
                {
                    id_customer = idacc,
                    id_account = idcus,
                    star = rating
                };
                db.tbl_Evaluete.Add(evaluete);
                db.SaveChanges();
                return Content("<script>alert('Đánh giá thành công!');history.back();</script>");
            }
            else
            {
                int idacc = int.Parse(User.Identity.Name);
                tbl_Evaluete evaluete = new tbl_Evaluete
                {
                    id_customer = idacc,
                    star = rating,
                    id_shop = idshop
                };
                db.tbl_Evaluete.Add(evaluete);
                db.SaveChanges();
                return Content("<script>alert('Đánh giá thành công!');history.back();</script>");
            }
        }
        public ActionResult pass()
        {
            return View();
        }
        public ActionResult DoiMatKhau(string matkhauht, string matkhaum, string matkhaunl)
        {
            if (matkhaum.Length < 6)
            {
                return Content("<script>" +
                        "alert('Đổi mật khẩu không thành công! Mật khẩu phải nhiều hơn 6 ký tự');" +
                        "window.location.href=('pass?mk=1');</script>");
            }
            if(string.IsNullOrEmpty(matkhauht) == true || string.IsNullOrEmpty(matkhaum) == true || string.IsNullOrEmpty(matkhaunl) == true)
            {
                return Content("<script>" +
                        "alert('Vui lòng nhập đầy đủ thông tin');" +
                        "window.location.href=('pass?mk=1');</script>");
            }
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            int idacc = int.Parse(User.Identity.Name);
            var acc = db.tbl_Account.Find(idacc);
            string mh_mkht = CreateMD5.EncodePassword(matkhauht);
            if (acc.password.Equals(mh_mkht))
            {
                if (matkhaum.Equals(matkhaunl))
                {
                    string mh_mkm = CreateMD5.EncodePassword(matkhaum);
                    acc.password = mh_mkm;
                    db.SaveChanges();
                    return Content("<script>" +
                        "alert('Đổi mật khẩu thành công!');" +
                        "window.location.href=('pass?mk=1');</script>");
                }
                else
                {
                    return Content("<script>" +
                        "alert('Nhập lại mật khẩu sai, vui lòng thử lại!');" +
                        "window.location.href=('pass?mk=1');</script>");
                }
            }
            else
            {
                return Content("<script>" +
                        "alert('Sai mật khẩu hiện tại, vui lòng thử lại!');" +
                        "window.location.href=('pass?mk=1');</script>");
            }
        }
        public ActionResult MatKhauHai(string matkhauht, string matkhau2, string matkhaunl2)
        {
            if (string.IsNullOrEmpty(matkhauht) == true || string.IsNullOrEmpty(matkhau2) == true || string.IsNullOrEmpty(matkhaunl2) == true)
            {
                return Content("<script>" +
                        "alert('Vui lòng nhập đầy đủ thông tin');" +
                        "window.location.href=('pass?mk=2');</script>");
            }
            if (matkhau2.Length < 6)
            {
                return Content("<script>" +
                        "alert('Đổi mật khẩu không thành công! Mật khẩu phải nhiều hơn 6 ký tự');" +
                        "window.location.href=('pass?mk=2');</script>");
            }
            string mhmkht = CreateMD5.EncodePassword(matkhauht);
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                    int idcs = int.Parse(User.Identity.Name);
                    var mk = db.tbl_Account.Find(idcs);
            if (mhmkht.Equals(mk.password))
            {
                if (matkhau2.Equals(matkhaunl2))
                {
                    
                    string mk2 = CreateMD5.EncodePassword(matkhau2);
                    mk.password_v2 = mk2;
                    db.SaveChanges();
                    return Content("<script>" +
                            "alert('Tạo mật khẩu cấp 2 thành công!');" +
                            "window.location.href=('pass?mk=2');</script>");
                }
                else
                {
                    return Content("<script>" +
                            "alert('Sai mật khẩu nhập lại, vui lòng thử lại!');" +
                            "window.location.href=('pass?mk=2');</script>");
                }
            }
            else
            {
                return Content("<script>" +
                            "alert('Sai mật khẩu cấp 1 hiện tại, vui lòng thử lại!');" +
                            "window.location.href=('pass?mk=2');</script>");
            }
        }
        public ActionResult QuenMatKhau(string tdn, string matkhau2, string matkhaumoi, string matkhaumoinl)
        {
            if (string.IsNullOrEmpty(tdn) == true || string.IsNullOrEmpty(matkhau2) == true || string.IsNullOrEmpty(matkhaumoi) == true || string.IsNullOrEmpty(matkhaumoinl) == true)
            {
                return Content("<script>" +
                        "alert('Vui lòng nhập đầy đủ thông tin');" +
                        "window.location.href=('pass?mk=quenmatkhau');</script>");
            }
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            var acc = db.tbl_Account.Where(x => x.username == tdn).FirstOrDefault();

            string mk2 = CreateMD5.EncodePassword(matkhau2);
            if (matkhaumoi.Equals(matkhaumoinl))
            {
                if (acc.password_v2.Equals(mk2))
                {
                    string mkm = CreateMD5.EncodePassword(matkhaumoi);
                    acc.password = mkm;
                    db.SaveChanges();
                    return Content("<script>" +
                        "alert('Đổi lại mật khẩu thành công!');" +
                        "window.location.href=('login');</script>");
                }
                else
                {
                    return Content("<script>" +
                        "alert('Sai mật khẩu cấp 2!');" +
                        "window.location.href=('pass?mk=quenmatkhau');</script>");
                }
            }
            else
            {
                return Content("<script>" +
                        "alert('Sai mật khẩu nhập lại, vui lòng thử lại!');" +
                        "window.location.href=('pass?mk=quenmatkhau');</script>");
            }
        }
    }
}