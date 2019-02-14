using Services;
using Services.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Caching;
using Web.Helpers;

namespace Web.Controllers
{
    public class OpenReceitasController : Controller
    {
        private OpenAPIService<OpenReceitasItem> _openReceitasService;

        public OpenReceitasController()
        {
            _openReceitasService = new OpenAPIService<OpenReceitasItem>(ConfigurationManager.AppSettings["OpenReceitasHost"]);
        }

        public ActionResult Index()
        {
            ViewBag.Categorias = GetFieldValues("DESCRICAO_CATEGORIA");
            ViewBag.Origens = GetFieldValues("DESCRICAO_ORIGEM");
            ViewBag.Especies = GetFieldValues("DESCRICAO_ESPECIE");
            ViewBag.Tipos = GetFieldValues("TP_RECEITA_ORC");
            ViewBag.Empresas = GetFieldValues("NM_EMPRESA");
            ViewBag.Fontes = GetFieldValues("DS_FONTE");

            return View();
        }

        public ActionResult List(int index, int count, string sortBy = "DT_APROPRIACAO", bool sortDesc = true)
        {
            var findProperties = new Dictionary<string, object>();
            foreach (var key in Request.QueryString.AllKeys)
            {
                if (key.StartsWith("find_"))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[key]))
                    {
                        var propRegex = new Dictionary<string, string>();
                        propRegex.Add("$regex", Request.QueryString[key]);
                        propRegex.Add("$options", "i");
                        findProperties.Add(key.Replace("find_", ""), propRegex);
                    }
                }
                else if (key.StartsWith("range_"))
                {
                    var values = Request.QueryString.GetValues(key);
                    var start = values[0];
                    var end = values[1];
                    var propRange = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(start))
                    {
                        var dt = new DateTime();
                        if(DateTime.TryParse(start, out dt))
                            propRange.Add("$gte", string.Format("{0:yyyy-MM-dd}", dt));

                        var vl = 0;
                        if(int.TryParse(start, out vl))
                            propRange.Add("$gte", vl);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        var dt = new DateTime();
                        if (DateTime.TryParse(end, out dt))
                            propRange.Add("$lt", string.Format("{0:yyyy-MM-dd}", dt));

                        var vl = 0;
                        if (int.TryParse(end, out vl))
                            propRange.Add("$lt", vl);
                    }
                    if (propRange.Count > 0)
                        findProperties.Add(key.Replace("range_", ""), propRange);
                }
            }

            var findJson = string.Empty;
            if (findProperties.Count > 0)
            {
                findJson = JsonConvert.SerializeObject(findProperties);
            }
            var items = _openReceitasService.FindItems(index, count, find: findJson, sortBy: sortBy, sortDesc: sortDesc);

            return PartialView(items);
        }

        public ActionResult ListGroup()
        {
            var findProperties = new Dictionary<string, object>();
            var groupProperties = new Dictionary<string, object>();
            foreach (var key in Request.QueryString.AllKeys)
            {
                if (key.StartsWith("find_"))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[key]))
                    {
                        var propRegex = new Dictionary<string, string>();
                        propRegex.Add("$regex", Request.QueryString[key]);
                        propRegex.Add("$options", "i");
                        findProperties.Add(key.Replace("find_", ""), propRegex);
                    }
                }
                else if (key.StartsWith("range_"))
                {
                    var values = Request.QueryString.GetValues(key);
                    var start = values[0];
                    var end = values[1];
                    var propRange = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(start))
                    {
                        var dt = new DateTime();
                        if (DateTime.TryParse(start, out dt))
                            propRange.Add("$gte", string.Format("{0:yyyy-MM-dd}", dt));

                        var vl = 0;
                        if (int.TryParse(start, out vl))
                            propRange.Add("$gte", vl);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        var dt = new DateTime();
                        if (DateTime.TryParse(end, out dt))
                            propRange.Add("$lt", string.Format("{0:yyyy-MM-dd}", dt));

                        var vl = 0;
                        if (int.TryParse(end, out vl))
                            propRange.Add("$lt", vl);
                    }
                    if (propRange.Count > 0)
                        findProperties.Add(key.Replace("range_", ""), propRange);
                }
                else if (key.StartsWith("group_"))
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[key]))
                    {
                        groupProperties.Add(Request.QueryString[key], key.Replace("group_", "$"));
                    }
                }
            }

            var findJson = string.Empty;
            if (findProperties.Count > 0)
            {
                findJson = JsonConvert.SerializeObject(findProperties);
            }

            var groupJson = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["groupby_MONTH"]))
            {
                var gpDic = new Dictionary<string, object>();
                var year = new Dictionary<string, string>();
                year.Add("$year", "$DT_APROPRIACAO");
                gpDic.Add("Ano", year);

                if (Request.QueryString["groupby_MONTH"] == "1")
                {
                    var month = new Dictionary<string, string>();
                    month.Add("$month", "$DT_APROPRIACAO");
                    gpDic.Add("Mês", month);
                }

                if (groupProperties.Count > 0)
                {
                    foreach (var prop in groupProperties)
                        gpDic.Add(prop.Key, prop.Value);
                }
                groupJson = JsonConvert.SerializeObject(gpDic);
            }

            var items = _openReceitasService.FindGroupItems(find: findJson, group: groupJson, sum: "VL_RECEITA", sortBy: "DT_APROPRIACAO", sortDesc: true);


            if (string.IsNullOrEmpty(Request.QueryString["groupby_MONTH"]) || Request.QueryString["groupby_MONTH"] == "0")
            {
                items = items.OrderBy(x => Convert.ToInt32(x.Group[0]));
            }
            else
            {
                items = items.OrderBy(x => Convert.ToInt32(x.Group[0])).ThenBy(x => Convert.ToInt32(x.Group[1]));
            }
            return PartialView(items);
        }

        public List<SelectListItem> GetFieldValues(string field)
        {
            if (HttpContext.Cache[field] != null)
            {
                return (List<SelectListItem>)HttpContext.Cache[field];
            }
            else
            {
                var result = _openReceitasService.GetFieldValues(field)
                    .Where(x => x != null)
                    .Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x.ToTitleCase()
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
                HttpContext.Cache.Insert(field, result, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);

                return result;
            }
        }
    }
}