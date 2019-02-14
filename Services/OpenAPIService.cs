using Newtonsoft.Json;
using RestSharp;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class OpenAPIService<T>
    {
        #region [ Private Properties ]

        private string _host;
        private string endpointFindItems { get { return "/items"; } }
        private string endpointGroupItems { get { return "/group"; } }

        #endregion [ Private Methods ]

        public OpenAPIService(string host)
        {
            _host = host;
        }

        #region [ Public Methods ]

        public IEnumerable<T> FindItems(int index = 0, int count = 10, string find = null, string sortBy = null, bool? sortDesc = null)
        {
            var client = new RestClient(_host);

            var querystring = string.Format("?skip={0}&limit={1}", index, count);
            if (!string.IsNullOrEmpty(find))
                querystring += string.Format("&find={0}", find);
            if (!string.IsNullOrEmpty(sortBy))
                querystring += string.Format("&sort={{\"{0}\":{1}}}", sortBy, (sortDesc.HasValue && sortDesc.Value ? -1 : 1));

            var request = new RestRequest(endpointFindItems + querystring, Method.GET);

            request.AddHeader("Accept", "application/json");
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var statusCode = (int)response.StatusCode;
            if (statusCode != 200)
                throw new Exception("Ocorreu um erro inesperado. " + response.Content);

            var result = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Content);
            
            return result;
        }

        public IEnumerable<string> GetFieldValues(string fieldName)
        {
            var client = new RestClient(_host);
            var request = new RestRequest(fieldName, Method.GET);

            request.AddHeader("Accept", "application/json");
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var statusCode = (int)response.StatusCode;
            if (statusCode != 200)
                throw new Exception("Ocorreu um erro inesperado. " + response.Content);

            var result = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);

            return result;
        }
		
		public IEnumerable<GroupItem> FindGroupItems(string find = null, string group = null, string sum = null, string sortBy = null, bool? sortDesc = null)
        {
            var client = new RestClient(_host);

            var querystring = "?";
            if (!string.IsNullOrEmpty(find))
                querystring += string.Format("&find={0}", find);
            if (!string.IsNullOrEmpty(group))
                querystring += string.Format("&group={0}", group);
            if (!string.IsNullOrEmpty(sum))
				querystring += string.Format("&sum={0}", sum);
            if (!string.IsNullOrEmpty(sortBy))
                querystring += string.Format("&sort={{\"{0}\":{1}}}", sortBy, (sortDesc.HasValue && sortDesc.Value ? -1 : 1));

            var request = new RestRequest(endpointGroupItems + querystring, Method.GET);

            request.AddHeader("Accept", "application/json");
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var statusCode = (int)response.StatusCode;
            if (statusCode != 200)
                throw new Exception("Ocorreu um erro inesperado. " + response.Content);

            var result = JsonConvert.DeserializeObject<dynamic>(response.Content);

            var items = new List<GroupItem>();
            foreach (var r in result)
            {
                var gi = new GroupItem();
                gi.Sum = r.sum;
                gi.Count = r.count;

                Dictionary<string, string> propertyValues = r._id.ToObject<Dictionary<string, string>>();
                var groupKeys = new List<string>();
                foreach (var prop in propertyValues)
                {
                    groupKeys.Add(prop.Value);
                }
                gi.Group = groupKeys.ToArray();
                items.Add(gi);
            }

            return items;
        }

        #endregion [ Public Methods ]
    }
}
