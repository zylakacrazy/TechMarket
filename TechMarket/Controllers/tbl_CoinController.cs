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
    [Route("api/tbl_Coin/{action}", Name = "tbl_CoinApi")]
    public class tbl_CoinController : ApiController
    {
        private SGDFleaMarketEntities _context = new SGDFleaMarketEntities();

        [HttpGet]
        public async Task<HttpResponseMessage> Get(DataSourceLoadOptions loadOptions) {
            var tbl_coin = _context.tbl_Coin.Select(i => new {
                i.Id,
                i.coin_total,
                i.coin_date,
                i.coin_details,
                i.id_account
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Request.CreateResponse(await DataSourceLoader.LoadAsync(tbl_coin, loadOptions));
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post(FormDataCollection form) {
            var model = new tbl_Coin();
            var values = JsonConvert.DeserializeObject<IDictionary>(form.Get("values"));
            PopulateModel(model, values);

            Validate(model);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, GetFullErrorMessage(ModelState));

            var result = _context.tbl_Coin.Add(model);
            await _context.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.Created, new { result.Id });
        }

        [HttpPut]
        public async Task<HttpResponseMessage> Put(FormDataCollection form) {
            var key = Convert.ToInt32(form.Get("key"));
            var model = await _context.tbl_Coin.FirstOrDefaultAsync(item => item.Id == key);
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
            var model = await _context.tbl_Coin.FirstOrDefaultAsync(item => item.Id == key);

            _context.tbl_Coin.Remove(model);
            await _context.SaveChangesAsync();
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

        private void PopulateModel(tbl_Coin model, IDictionary values) {
            string ID = nameof(tbl_Coin.Id);
            string COIN_TOTAL = nameof(tbl_Coin.coin_total);
            string COIN_DATE = nameof(tbl_Coin.coin_date);
            string COIN_DETAILS = nameof(tbl_Coin.coin_details);
            string ID_ACCOUNT = nameof(tbl_Coin.id_account);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(COIN_TOTAL)) {
                model.coin_total = values[COIN_TOTAL] != null ? Convert.ToDouble(values[COIN_TOTAL], CultureInfo.InvariantCulture) : (double?)null;
            }

            if(values.Contains(COIN_DATE)) {
                model.coin_date = values[COIN_DATE] != null ? Convert.ToDateTime(values[COIN_DATE]) : (DateTime?)null;
            }

            if(values.Contains(COIN_DETAILS)) {
                model.coin_details = Convert.ToString(values[COIN_DETAILS]);
            }

            if(values.Contains(ID_ACCOUNT)) {
                model.id_account = values[ID_ACCOUNT] != null ? Convert.ToInt32(values[ID_ACCOUNT]) : (int?)null;
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