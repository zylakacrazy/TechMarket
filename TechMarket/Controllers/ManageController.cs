using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechMarket.Models;

namespace TechMarket.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Account()
        {
            return View();
        }
        public ActionResult BannerSlide()
        {
            return View();
        }
        public ActionResult Coin()
        {
            return View();
        }
        public ActionResult Color()
        {
            return View();
        }
        public ActionResult CPU()
        {
            return View();
        }
        public ActionResult Device()
        {
            return View();
        }
        public ActionResult Manufacturer()
        {
            return View();
        }
        public ActionResult Report()
        {
            return View();
        }
        public ActionResult Screensize()
        {
            return View();
        }
        public ActionResult TypeDevice()
        {
            return View();
        }
        public ActionResult browse()
        {
            return View();
        }
        public ActionResult DuyetTin(int id)
        {
            try
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                tbl_Product _product = db.tbl_Product.Find(id);
                _product.status = 1;
                db.SaveChanges();
                return Content("<script language='javascript' type='text/javascript'>alert('Duyệt tin thành công!');window.location.href=('../../manage/browse');</script>");
            }
            catch (Exception)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Sửa không thành công!');window.location.href=('../../manage/browse');</script>");
                throw;
            }
        }
        public ActionResult BoQua(int id)
        {
            try
            {
                SGDFleaMarketEntities db = new SGDFleaMarketEntities();
                tbl_Report re = db.tbl_Report.Find(id);
                db.tbl_Report.Remove(re);
                db.SaveChanges();
                return Content("<script language='javascript' type='text/javascript'>alert('Bỏ qua thành công!');window.location.href=('../../manage/report');</script>");
            }
            catch (Exception)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Bỏ qua không thành công!');window.location.href=('../../manage/report');</script>");
                throw;
            }

        }
        public ActionResult DeleteProduct(int idsp)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
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
                        "window.location.href=('../manage/browse');</script>");
        }
    }
}