using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechMarket.Models;
using TechMarket.Services;

namespace TechMarket.Controllers
{
    public class PaymentController : Controller
    {
        
        // GET: Payment
        public ActionResult Index()
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

        public ActionResult ThanhToan(string gia)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMO7FSP20211026";
            string accessKey = "47URKlsuFyYjKJD3";
            string serectKey = "1qASxz2fBcmRBQT8Fcx0suey4QikockQ";
            string orderInfo = "DH" + DateTime.Now.ToLongTimeString();
            //string returnUrl = "http://localhost:50698/Payment/ReturnUrl";
            //string notifyurl = "http://localhost:50698/Payment/NotifyUrl";
            string returnUrl = "http://techmarket.kltn.com/Payment/ReturnUrl";
            string notifyurl = "http://techmarket.kltn.com/Payment/NotifyUrl";

            string amount = gia;
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            string signature = crypto.signSHA256(rawHash, serectKey);
            JObject message = new JObject
            {
                {"partnerCode",partnerCode },
                {"accessKey",accessKey },
                {"requestId", requestId},
                {"amount",amount },
                {"orderId", orderid },
                {"orderInfo",orderInfo },
                {"returnUrl",returnUrl },
                {"notifyUrl",notifyurl },
                {"requestType","captureMoMoWallet" },
                {"signature",signature }
            };
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult ReturnUrl()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = "1qASxz2fBcmRBQT8Fcx0suey4QikockQ";
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thông tin Request không hộp lệ";
                return View();
            }
            if (Request.QueryString["errorCode"] == "0")
            {
                float price = float.Parse(Request.QueryString["amount"].ToString());
                int acc_id = int.Parse(User.Identity.Name);
                ViewBag.message = "Thanh toán thành công!";
                return RedirectToAction("addCoin", new { id_acc = acc_id, date = DateTime.Now, total = price, details = "Nạp xu qua momo" });
            }
            else
            {
                ViewBag.message = Request.QueryString["errorCode"].ToString();
            }
            return View();
        }
        /// <summary>

        public ActionResult addCoin(int id_acc, DateTime date, float total, string details)
        {
            SGDFleaMarketEntities db = new SGDFleaMarketEntities();
            tbl_Coin updateModel = new tbl_Coin
            {
                id_account = id_acc,
                coin_date = date,
                coin_total = total,
                coin_details = details
            };

            try
            {
                db.tbl_Coin.Add(updateModel);
                db.SaveChanges();
                return RedirectToAction("Coin");
            }
            catch (Exception)
            {
                ViewBag.message = "failed";
                return RedirectToAction("Index");
                throw;
            }
        }
        /// </summary>
        /// <returns></returns>
        public ActionResult NotifyUrl()
        {
            string param = "";
            param = "partner_code=" + Request["partner_code"] +
                "&access_key=" + Request["access_key"] +
                "&amount=" + Request["amount"] +
                "&order_id=" + Request["order_id"] +
                "&order_info=" + Request["order_info"] +
                "&order_type=" + Request["order_type"] +
                "&transaction_id=" + Request["transaction_id"] +
                "&message=" + Request["message"] +
                "&response_time=" + Request["response_time"] +
                "&status_code=" + Request["status_code"];

            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = "1qASxz2fBcmRBQT8Fcx0suey4QikockQ";
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                //cập nhật trạng thái đơn hàng là thất bại
            }
            string status_code = Request["status_code"].ToString();
            if (status_code != "0")
            {
                // thất bại
            }
            else
            {
                // thành công
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Coin()
        {
            return View();
        }
        public ActionResult History()
        {
            return View();
        }
        public ActionResult PaymentMethod()
        {
            return View(new Price());
        }
        public ActionResult PayMethod(Price gia)
        {
            string menhgia = gia.price.ToString();
            string Paymethod = gia.Paymethod.ToString();
            ViewBag.msg = Paymethod;
            if(Paymethod == "momo")
            {
                return ThanhToan(menhgia);
            }
            if(Paymethod == "vnpay")
            {
                return ThanhToanVNP(menhgia);
            }
            return View();
        }
        //////////////
        ///
        private static readonly ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult ThanhToanVNP(string gia)
        {
            //Get Config Info
            //string vnp_Returnurl = "http://localhost:50698/Payment/ReturnUrlVNP"; //ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Returnurl = "http://techmarket.com/Payment/ReturnUrlVNP"; //ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; //ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = "OPBMRGA2";//ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = "STCVVVZEVUPTHCTNVJXMRXEPSSWJFFQK";//ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat

            
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (long.Parse(gia)*100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_BankCode", "NCB");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
            vnpay.AddRequestData("vnp_OrderType", "Chi tiết thanh toán"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            log.InfoFormat("VNPAY URL: {0}", paymentUrl);

            return Redirect(paymentUrl);
        }
        public ActionResult ReturnUrlVNP()
        {
            log.InfoFormat("Begin VNPAY Return, URL={0}", Request.RawUrl);
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = "STCVVVZEVUPTHCTNVJXMRXEPSSWJFFQK"; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        //ViewBag.message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        //log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                        float price = float.Parse(Request.QueryString["vnp_Amount"].ToString())/100;
                        int acc_id = int.Parse(User.Identity.Name);
                        ViewBag.message = "Thanh toán thành công!";
                        return RedirectToAction("addCoin", new { id_acc = acc_id, date = DateTime.Now, total = price, details = "Nạp xu qua VNPAY" });
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        ViewBag.message = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                    //displayTmnCode.InnerText = "Mã Website (Terminal ID):" + TerminalID;
                    //displayTxnRef.InnerText = "Mã giao dịch thanh toán:" + orderId.ToString();
                    //displayVnpayTranNo.InnerText = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    //displayAmount.InnerText = "Số tiền thanh toán (VND):" + vnp_Amount.ToString();
                    //displayBankCode.InnerText = "Ngân hàng thanh toán:" + bankCode;
                }
                else
                {
                    log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
                    ViewBag.message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
            return View();
        }
    }
}