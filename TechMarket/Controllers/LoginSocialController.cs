using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASPSnippets.GoogleAPI;
using System.Net;
using TechMarket.Models;
using System.Web.Script.Serialization;
using TechMarket.Services;
using System.Web.Security;

namespace TechMarket.Controllers
{
    public class LoginSocialController : Controller
    {
        // GET: LoginSocial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void LoginWithGooglePlus()
        {
            GoogleConnect.ClientId = "583617116116-nu2r57uviedlmeik08she2v978evdcbo.apps.googleusercontent.com";
            GoogleConnect.ClientSecret = "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";
            GoogleConnect.RedirectUri = Request.Url.AbsoluteUri.Split('?')[0];
            GoogleConnect.Authorize("profile", "email");
        }
        [ActionName("LoginWithGooglePlus")]
        public ActionResult LoginWithGooglePlusConfirmed()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["code"]))
            {
                string code = Request.QueryString["code"];
                string json = GoogleConnect.Fetch("me", code);
                GoogleProfile profile = new JavaScriptSerializer().Deserialize<GoogleProfile>(json);

                SGDFleaMarketEntities db = new SGDFleaMarketEntities();

                if (db.tbl_Account.FirstOrDefault(x => x.username == profile.Email) != null)
                {
                    LoginData loginData = new LoginData
                    {
                        Username = profile.Email,
                        Password = profile.Id
                    };
                    int userId;
                    if (new AppService().Login(loginData, out userId))
                    {
                        FormsAuthentication.RedirectFromLoginPage(userId.ToString(), loginData.RememberMe);
                        Session["UserID"] = userId.ToString();
                        return Redirect("~/Home/Index");
                    }
                    else
                    {
                        ViewBag.LoginError = "Username Or Password Is Incorrect";
                    }
                }
                else
                {
                    LoginData loginData = new LoginData
                    {
                        Username = profile.Email,
                        Password = profile.Id
                    };
                    tbl_Account acc = new tbl_Account
                    {
                        username = profile.Email,
                        password = CreateMD5.EncodePassword(profile.Id),
                        fullname = profile.Name,
                        email = profile.Email,
                        Role = 1,
                        //gender = profile.Gender;
                        //Type = profile.ObjectType;
                        images = profile.Id + System.IO.Path.GetExtension(profile.Picture.Split('?')[0]) + ".jpg"
                    };

                    using (var client = new WebClient())
                    {
                        client.DownloadFile(profile.Picture, Server.MapPath("~/Images/Avatar/" + acc.images));
                    }
                    acc.images = "../Images/Avatar/" + acc.images;
                    db.tbl_Account.Add(acc);
                    db.SaveChanges();

                    int userId;
                    if (new AppService().Login(loginData, out userId))
                    {
                        FormsAuthentication.RedirectFromLoginPage(userId.ToString(), loginData.RememberMe);
                        Session["UserID"] = userId.ToString();
                        return Redirect("~/Home/Index");
                    }
                    else
                    {
                        ViewBag.LoginError = "Username Or Password Is Incorrect";
                    }
                }
            }
            if (Request.QueryString["error"] == "access_denied")
            {
                return Content("access_denied");
            }
            return RedirectToAction("Index", "Home");
        }
        public class GoogleProfile
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Picture { get; set; }
            public string Email { get; set; }
            public string Verified_Email { get; set; }
        }
    }
}