using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechMarket.Models;

namespace TechMarket.Controllers
{
    public class ProductController : Controller
    {
        SGDFleaMarketEntities db = new SGDFleaMarketEntities();
        // GET: Product
        public ActionResult Index()
        {
            List<tbl_Product> result = db.tbl_Product.ToList();
            return View(result);
        }
        public ActionResult showDetails(int id)
        {
            var result1 = db.tbl_Product.SingleOrDefault(n => n.Id == id);
            if (result1 == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(result1);
        }

        public ActionResult addProduct()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddP(tbl_Product product,string product_price1, string address, string diachi, HttpPostedFileBase product_images, HttpPostedFileBase[] images_name)
        {
            if (string.IsNullOrEmpty(product.product_title) == true)
            {
                return Content("<script>" +
                        "alert('Tiêu đề không được để trống!');" +
                        "history.back();</script>");
            }
            if (string.IsNullOrEmpty(product.product_images) == true)
            {
                return Content("<script>" +
                        "alert('Phải có hình ảnh đại diện!');" +
                        "history.back();</script>");
            }
            if (string.IsNullOrEmpty(product_price1) == true)
            {
                return Content("<script>" +
                        "alert('Giá tiền không được để trống!');" +
                        "history.back();</script>");
            }
            if (ModelState.IsValid)
            {
                if (product_images != null && product_images.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(product_images.FileName);
                    string path = Server.MapPath("~/Images/Product/" + fileName);
                    product_images.SaveAs(path);
                    product.product_images = fileName;
                    product.Address = "Tỉnh " + address + ", " + diachi;
                    product.date = DateTime.Now;
                    var charsToRemove = new string[] { "VNĐ", ",", ".", ";", "'" };
                    foreach (var c in charsToRemove)
                    {
                        product_price1 = product_price1.Replace(c, string.Empty);
                    }
                    product.product_price = double.Parse(product_price1);
                    db.tbl_Product.Add(product);
                    db.SaveChanges();
                }
                var idsp = db.tbl_Product.OrderByDescending(x => x.Id).FirstOrDefault();
                int idproduct = idsp.Id; 
                var hinhanh = new tbl_Images();
                foreach (HttpPostedFileBase file in images_name)
                {
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        string path1 = Server.MapPath("~/Images/Product/" + fileName);
                        file.SaveAs(path1);
                        
                        hinhanh.id_product = idproduct;
                        hinhanh.images_name = fileName;
                        db.tbl_Images.Add(hinhanh);
                        db.SaveChanges();
                    }
                }
                return Content("<script>" +
                            "alert('Đăng tin thành công, vui lòng chờ xác nhận!');" +
                            "window.location.href=('../home/index');</script>");
            }
            else
            {
                ModelState.AddModelError("", "Thêm thất bại!");
                return View(product);
            }
        }

        public ActionResult UpdateProduct(int idsp)
        {
            var productModel = db.tbl_Product.Find(idsp);
            return View(productModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateP(tbl_Product model, string address, string diachi, HttpPostedFileBase product_images, HttpPostedFileBase[] images_name)
        {
            using (var db = new SGDFleaMarketEntities())
            {
                if (string.IsNullOrEmpty(model.product_title) == true)
                {
                    return Content("<script>" +
                            "alert('Tiêu đề không được để trống!');" +
                            "history.back();</script>");
                }
                if (string.IsNullOrEmpty(model.product_images) == true)
                {
                    return Content("<script>" +
                            "alert('Phải có hình ảnh đại diện!');" +
                            "history.back();</script>");
                }
                if (model != null)
                {
                    tbl_Product product = db.tbl_Product.Find(model.Id);

                    product.id_color = model.id_color;
                    product.id_cpu = model.id_cpu;
                    product.id_device = model.id_device;
                    product.id_manufacturer = model.id_manufacturer;
                    product.id_screensize = model.id_screensize;
                    product.id_shop = model.id_shop;
                    product.product_capacity = model.product_capacity;
                    product.product_details = model.product_details;
                    product.product_disk = model.product_disk;
                    product.product_gpu = model.product_gpu;
                    product.product_insurance = model.product_insurance;
                    product.product_price = model.product_price;
                    product.product_ram = model.product_ram;
                    product.product_status = model.product_status;
                    product.product_title = model.product_title;
                    product.product_version = model.product_version;
                    product.Address = "Tỉnh " + address + ", " + diachi;

                   
                    var hinh = db.tbl_Images.Where(x => x.id_product == model.Id);
                    foreach (var item in hinh)
                    {
                        string fullPath = Request.MapPath("~/Images/Product/" + item.images_name);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        var hin = db.tbl_Images.Find(item.Id);
                        db.tbl_Images.Remove(hin);
                        
                    }db.SaveChanges();
                    var hinhanh = new tbl_Images();
                    
                    foreach (HttpPostedFileBase file in images_name)
                    {
                        if (file != null)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            string path1 = Server.MapPath("~/Images/Product/" + fileName);
                            file.SaveAs(path1);

                            hinhanh.id_product = model.Id;
                            hinhanh.images_name = fileName;
                            db.tbl_Images.Add(hinhanh);
                            db.SaveChanges();
                        }
                    }
                    if (product_images != null)
                    {
                        string fileName = Path.GetFileName(product_images.FileName);
                        string path = Server.MapPath("~/Images/Product/" + fileName);
                        product_images.SaveAs(path);

                        product.product_images = fileName;
                    }
                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm thất bại!");
                        return View(product);
                    }
                    
                }
            }
            return View();
        }

        public ActionResult DeleteProduct(int idsp)
        {
            var hinh = db.tbl_Images.Where(x => x.id_product == idsp).ToList();
            foreach (var item in hinh)
            {
                string fullPath = Request.MapPath("~/Images/Product/" + item.images_name);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                var xoa = db.tbl_Images.Find(item.Id);
                db.tbl_Images.Remove(xoa);
                db.SaveChanges();
            }
            var model = db.tbl_Product.Find(idsp);
            db.tbl_Product.Remove(model);
            db.SaveChanges();
            return Content("<script>" +
                        "alert('Xóa sản phẩm thành công!');" +
                        "window.location.href=('../Home/index');</script>");
        }
        // Trang danh mục
        public ActionResult Category()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        public ActionResult Report(int idsp, int idcustomer, string details)
        {
            if (ModelState.IsValid)
            {
                var test = db.tbl_Report.Where(x => x.id_product == idsp);
                if (test.Count() > 0)
                {
                    return Content("<script language='javascript' type='text/javascript'>" +
                        "alert('Tin đã được báo cáo thành công vui lòng đợi xử lý!');" +
                        "window.location.href=('../../Home/Index');</script>");
                }
                tbl_Report report = new tbl_Report
                {
                    id_customer = idcustomer,
                    id_product = idsp,
                    typeReport = details,
                    status = 0
                };
                db.tbl_Report.Add(report);
                db.SaveChanges();
                return Content("<script language='javascript' type='text/javascript'>" +
                        "alert('Báo tin thành công vui lòng đợi xử lý!');" +
                        "window.location.href=('../../Home/Index');</script>");
            }
            return Content("<script language='javascript' type='text/javascript'>" +
                        "alert('Báo tin không thành công vui lòng thử lại sau!');" +
                        "window.location.href=('../../Home/Index');</script>");
        }
    }
}