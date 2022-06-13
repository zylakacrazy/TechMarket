using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TechMarket.Models;

namespace TechMarket.Controllers
{
    [Route("api/tbl_Product/{action}", Name = "tbl_ProductApi")]
    public class tbl_ProductController : ApiController
    {
        private SGDFleaMarketEntities _context = new SGDFleaMarketEntities();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var tbl_product = _context.tbl_Product.Select(i => new {
                i.Id,
                i.id_manufacturer,
                i.id_color,
                i.product_capacity,
                i.product_status,
                i.product_price,
                i.product_title,
                i.product_details,
                i.id_cpu,
                i.product_gpu,
                i.product_ram,
                i.id_screensize,
                i.product_version,
                i.product_disk,
                i.product_insurance,
                i.id_device,
                i.id_account,
                i.id_shop,
                i.product_images
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(tbl_product, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new tbl_Product();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.tbl_Product.Add(model);
            await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created, new { result.Id });
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.tbl_Product.FirstOrDefaultAsync(item => item.Id == key);
            if(model == null)
                return Request.CreateResponse(HttpStatusCode.Conflict, "Object not found");

            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public async Task Delete(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.tbl_Product.FirstOrDefaultAsync(item => item.Id == key);

            _context.tbl_Product.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<HttpResponseMessage> tbl_ColorLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_Color
                         orderby i.color_name
                         select new {
                             Value = i.Id,
                             Text = i.color_name
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> tbl_CPULookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_CPU
                         orderby i.cpu_name
                         select new {
                             Value = i.Id,
                             Text = i.cpu_name
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> tbl_DeviceLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_Device
                         orderby i.device_name
                         select new {
                             Value = i.Id,
                             Text = i.device_name
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> tbl_ManufacturerLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_Manufacturer
                         orderby i.manufacturer_name
                         select new {
                             Value = i.Id,
                             Text = i.manufacturer_name
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> tbl_ScreensizeLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_Screensize
                         orderby i.screensize_name
                         select new {
                             Value = i.Id,
                             Text = i.screensize_name
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> tbl_ShopLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_Shop
                         orderby i.shop_name
                         select new {
                             Value = i.Id,
                             Text = i.shop_name
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> tbl_AccountLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.tbl_Account
                         orderby i.username
                         select new {
                             Value = i.Id,
                             Text = i.username
                         };
            return Request.CreateResponse(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(tbl_Product model, IDictionary values) {
            string ID = nameof(tbl_Product.Id);
            string ID_MANUFACTURER = nameof(tbl_Product.id_manufacturer);
            string ID_COLOR = nameof(tbl_Product.id_color);
            string PRODUCT_CAPACITY = nameof(tbl_Product.product_capacity);
            string PRODUCT_STATUS = nameof(tbl_Product.product_status);
            string PRODUCT_PRICE = nameof(tbl_Product.product_price);
            string PRODUCT_TITLE = nameof(tbl_Product.product_title);
            string PRODUCT_DETAILS = nameof(tbl_Product.product_details);
            string ID_CPU = nameof(tbl_Product.id_cpu);
            string PRODUCT_GPU = nameof(tbl_Product.product_gpu);
            string PRODUCT_RAM = nameof(tbl_Product.product_ram);
            string ID_SCREENSIZE = nameof(tbl_Product.id_screensize);
            string PRODUCT_VERSION = nameof(tbl_Product.product_version);
            string PRODUCT_DISK = nameof(tbl_Product.product_disk);
            string PRODUCT_INSURANCE = nameof(tbl_Product.product_insurance);
            string ID_DEVICE = nameof(tbl_Product.id_device);
            string ID_ACCOUNT = nameof(tbl_Product.id_account);
            string ID_SHOP = nameof(tbl_Product.id_shop);
            string PRODUCT_IMAGES = nameof(tbl_Product.product_images);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(ID_MANUFACTURER)) {
                model.id_manufacturer = Convert.ToInt32(values[ID_MANUFACTURER]);
            }

            if(values.Contains(ID_COLOR)) {
                model.id_color = Convert.ToInt32(values[ID_COLOR]);
            }

            if(values.Contains(PRODUCT_CAPACITY)) {
                model.product_capacity = values[PRODUCT_CAPACITY] != null ? Convert.ToDouble(values[PRODUCT_CAPACITY], CultureInfo.InvariantCulture) : (double?)null;
            }

            if(values.Contains(PRODUCT_STATUS)) {
                model.product_status = values[PRODUCT_STATUS] != null ? Convert.ToInt32(values[PRODUCT_STATUS]) : (int?)null;
            }

            if(values.Contains(PRODUCT_PRICE)) {
                model.product_price = values[PRODUCT_PRICE] != null ? Convert.ToDouble(values[PRODUCT_PRICE], CultureInfo.InvariantCulture) : (double?)null;
            }

            if(values.Contains(PRODUCT_TITLE)) {
                model.product_title = Convert.ToString(values[PRODUCT_TITLE]);
            }

            if(values.Contains(PRODUCT_DETAILS)) {
                model.product_details = Convert.ToString(values[PRODUCT_DETAILS]);
            }

            if(values.Contains(ID_CPU)) {
                model.id_cpu = values[ID_CPU] != null ? Convert.ToInt32(values[ID_CPU]) : (int?)null;
            }

            if(values.Contains(PRODUCT_GPU)) {
                model.product_gpu = Convert.ToString(values[PRODUCT_GPU]);
            }

            if(values.Contains(PRODUCT_RAM)) {
                model.product_ram = values[PRODUCT_RAM] != null ? Convert.ToInt32(values[PRODUCT_RAM]) : (int?)null;
            }

            if(values.Contains(ID_SCREENSIZE)) {
                model.id_screensize = values[ID_SCREENSIZE] != null ? Convert.ToInt32(values[ID_SCREENSIZE]) : (int?)null;
            }

            if(values.Contains(PRODUCT_VERSION)) {
                model.product_version = Convert.ToString(values[PRODUCT_VERSION]);
            }

            if(values.Contains(PRODUCT_DISK)) {
                model.product_disk = Convert.ToString(values[PRODUCT_DISK]);
            }

            if(values.Contains(PRODUCT_INSURANCE)) {
                model.product_insurance = Convert.ToString(values[PRODUCT_INSURANCE]);
            }

            if(values.Contains(ID_DEVICE)) {
                model.id_device = values[ID_DEVICE] != null ? Convert.ToInt32(values[ID_DEVICE]) : (int?)null;
            }

            if(values.Contains(ID_ACCOUNT)) {
                model.id_account = values[ID_ACCOUNT] != null ? Convert.ToInt32(values[ID_ACCOUNT]) : (int?)null;
            }

            if(values.Contains(ID_SHOP)) {
                model.id_shop = values[ID_SHOP] != null ? Convert.ToInt32(values[ID_SHOP]) : (int?)null;
            }

            if(values.Contains(PRODUCT_IMAGES)) {
                model.product_images = Convert.ToString(values[PRODUCT_IMAGES]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}