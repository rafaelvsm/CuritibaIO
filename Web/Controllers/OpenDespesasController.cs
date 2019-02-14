using Newtonsoft.Json;
using Services;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;
using Web.Helpers;

namespace Web.Controllers
{
    public class OpenDespesasController : Controller
    {
        private OpenAPIService<OpenDespesasItem> _openDespesasService;

        public OpenDespesasController()
        {
            _openDespesasService = new OpenAPIService<OpenDespesasItem>(ConfigurationManager.AppSettings["OpenDespesasHost"]);
        }

        public ActionResult Index()
        {
            ViewBag.Fontes = GetFieldValues("DS_FONTE");
            ViewBag.Funcoes = GetFieldValues("DS_FUNCAO");
            ViewBag.Programas = GetFieldValues("DS_PROGRAMA");
            ViewBag.Acoes = GetFieldValues("DS_ACAO");
            ViewBag.Subelementos = GetFieldValues("DS_SUBELEMENTO");
            ViewBag.Orgaos = GetFieldValues("DS_ORGAO");
            ViewBag.Despesas = GetFieldValues("DS_DESPESA");
            ViewBag.Modalidades = GetFieldValues("DS_MODALIDADE");

            return View();
        }

        public ActionResult List(int index, int count, string sortBy = "DT_EMPENHO", bool sortDesc = true)
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
            }

            var findJson = string.Empty;
            if (findProperties.Count > 0)
            {
                findJson = JsonConvert.SerializeObject(findProperties);
            }
            var items = _openDespesasService.FindItems(index, count, find: findJson, sortBy: sortBy, sortDesc: sortDesc);

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
                year.Add("$year", "$DT_EMPENHO");
                gpDic.Add("Ano", year);

                if (Request.QueryString["groupby_MONTH"] == "1")
                {
                    var month = new Dictionary<string, string>();
                    month.Add("$month", "$DT_EMPENHO");
                    gpDic.Add("Mês", month);
                }

                if (groupProperties.Count > 0)
                {
                    foreach (var prop in groupProperties)
                        gpDic.Add(prop.Key, prop.Value);
                }
                groupJson = JsonConvert.SerializeObject(gpDic);
            }

            var items = _openDespesasService.FindGroupItems(find: findJson, group: groupJson, sum: "VL_EMPENHADO", sortBy: "DT_EMPENHO", sortDesc: true);


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
                var result = _openDespesasService.GetFieldValues(field)
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