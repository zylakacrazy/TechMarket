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
    [Route("api/tbl_Manufacturer/{action}", Name = "tbl_ManufacturerApi")]
    public class tbl_ManufacturerController : ApiController
    {
        private SGDFleaMarketEntities _context = new SGDFleaMarketEntities();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var tbl_manufacturer = _context.tbl_Manufacturer.Select(i => new {
                i.Id,
                i.manufacturer_name,
                i.id_device
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(tbl_manufacturer, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new tbl_Manufacturer();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.tbl_Manufacturer.Add(model);
            await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created, new { result.Id });
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.tbl_Manufacturer.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.tbl_Manufacturer.FirstOrDefaultAsync(item => item.Id == key);

            _context.tbl_Manufacturer.Remove(model);
            await _context.SaveChangesAsync();
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

        private void PopulateModel(tbl_Manufacturer model, IDictionary values) {
            string ID = nameof(tbl_Manufacturer.Id);
            string MANUFACTURER_NAME = nameof(tbl_Manufacturer.manufacturer_name);
            string ID_DEVICE = nameof(tbl_Manufacturer.id_device);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(MANUFACTURER_NAME)) {
                model.manufacturer_name = Convert.ToString(values[MANUFACTURER_NAME]);
            }

            if(values.Contains(ID_DEVICE)) {
                model.id_device = values[ID_DEVICE] != null ? Convert.ToInt32(values[ID_DEVICE]) : (int?)null;
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