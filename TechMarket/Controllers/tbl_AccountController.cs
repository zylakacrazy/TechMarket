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
    [Route("api/tbl_Account/{action}", Name = "tbl_AccountApi")]
    public class tbl_AccountController : ApiController
    {
        private SGDFleaMarketEntities _context = new SGDFleaMarketEntities();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var tbl_account = _context.tbl_Account.Select(i => new {
                i.Id,
                i.username,
                i.password,
                i.password_v2,
                i.phone,
                i.email,
                i.facebook,
                i.fullname,
                i.cmnd,
                i.brithday,
                i.address,
                i.images,
                i.gender
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(tbl_account, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new tbl_Account();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.tbl_Account.Add(model);
            await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created, new { result.Id });
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.tbl_Account.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.tbl_Account.FirstOrDefaultAsync(item => item.Id == key);

            _context.tbl_Account.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(tbl_Account model, IDictionary values) {
            string ID = nameof(tbl_Account.Id);
            string USERNAME = nameof(tbl_Account.username);
            string PASSWORD = nameof(tbl_Account.password);
            string PASSWORD_V2 = nameof(tbl_Account.password_v2);
            string PHONE = nameof(tbl_Account.phone);
            string EMAIL = nameof(tbl_Account.email);
            string FACEBOOK = nameof(tbl_Account.facebook);
            string FULLNAME = nameof(tbl_Account.fullname);
            string CMND = nameof(tbl_Account.cmnd);
            string BRITHDAY = nameof(tbl_Account.brithday);
            string ADDRESS = nameof(tbl_Account.address);
            string IMAGES = nameof(tbl_Account.images);
            string GENDER = nameof(tbl_Account.gender);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(USERNAME)) {
                model.username = Convert.ToString(values[USERNAME]);
            }

            if(values.Contains(PASSWORD)) {
                model.password = Convert.ToString(values[PASSWORD]);
            }

            if(values.Contains(PASSWORD_V2)) {
                model.password_v2 = Convert.ToString(values[PASSWORD_V2]);
            }

            if(values.Contains(PHONE)) {
                model.phone = Convert.ToString(values[PHONE]);
            }

            if(values.Contains(EMAIL)) {
                model.email = Convert.ToString(values[EMAIL]);
            }

            if(values.Contains(FACEBOOK)) {
                model.facebook = Convert.ToString(values[FACEBOOK]);
            }

            if(values.Contains(FULLNAME)) {
                model.fullname = Convert.ToString(values[FULLNAME]);
            }

            if(values.Contains(CMND)) {
                model.cmnd = Convert.ToString(values[CMND]);
            }

            if(values.Contains(BRITHDAY)) {
                model.brithday = values[BRITHDAY] != null ? Convert.ToDateTime(values[BRITHDAY]) : (DateTime?)null;
            }

            if(values.Contains(ADDRESS)) {
                model.address = Convert.ToString(values[ADDRESS]);
            }

            if(values.Contains(IMAGES)) {
                model.images = Convert.ToString(values[IMAGES]);
            }

            if(values.Contains(GENDER)) {
                model.gender = values[GENDER] != null ? Convert.ToInt32(values[GENDER]) : (int?)null;
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