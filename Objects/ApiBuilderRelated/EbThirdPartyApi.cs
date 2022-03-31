using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System.Collections.Generic;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Data;
using RestSharp;
using ExpressBase.Common.Structures;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using ExpressBase.Objects.ServiceStack_Artifacts;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbThirdPartyApi : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.String)]
        public string Url { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [OnChangeExec(@"
                if (this.Method === 1){ 
                        pg.ShowProperty('RequestFormat');
                }
                else {
                        pg.HideProperty('RequestFormat');
                }
            ")]
        public ApiMethods Method { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public ApiRequestFormat RequestFormat { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<RequestHeader> Headers { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<RequestParam> Parameters { get; set; }

        public Method RestMethod => this.Method == ApiMethods.POST ? RestSharp.Method.POST : RestSharp.Method.GET;

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='ThirdPartyApi' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public RestRequest CreateRequest(string query, Dictionary<string, object> globalParams)
        {
            RestRequest request = new RestRequest(query, RestMethod);

            if (Headers != null && Headers.Count > 0)
            {
                foreach (RequestHeader header in Headers)
                {
                    string value = header.Value;

                    if (!string.IsNullOrEmpty(header.ValueParameter) && globalParams.ContainsKey(header.ValueParameter))
                    {
                        value = globalParams[header.ValueParameter]?.ToString();
                    }

                    if (header.Type == HttpHeaderType.None)
                    {
                        request.AddHeader(header.HeaderName, value);
                    }
                    else if (header.Type == HttpHeaderType.Bearer)
                    {
                        request.AddHeader(header.HeaderName, $"Bearer {value}");
                    }
                }
            }

            return request;
        }

        public override List<Param> GetParameters(Dictionary<string, object> globalParams)
        {
            if (Parameters == null || Parameters.Count <= 0) return null;

            List<Param> parameters = new List<Param>();

            foreach (RequestParam param in Parameters)
            {
                Param p = new Param
                {
                    Name = param.ParameterName,
                    Type = param.Type.ToString(),
                };

                parameters.Add(p);

                if (param.UseThisVal)
                {
                    p.Value = param.Value;
                }
                else
                {
                    if (globalParams.TryGetValue(param.Name, out var value))
                        p.Value = value?.ToString();
                    else
                        throw new System.Exception($"parameter {param.Name} not found");
                }
            }
            return parameters;
        }

        public object ExecuteThirdPartyApi(EbThirdPartyApi thirdPartyResource, EbApi Api)
        {
            Uri uri = new Uri(ReplacePlaceholders(thirdPartyResource.Url, Api));

            object result;

            try
            {
                RestClient client = new RestClient(uri.GetLeftPart(UriPartial.Authority));

                RestRequest request = thirdPartyResource.CreateRequest(uri.PathAndQuery, Api.GlobalParams);

                List<Param> parameters = thirdPartyResource.GetParameters(Api.GlobalParams) ?? new List<Param>();

                if (thirdPartyResource.Method == ApiMethods.POST && thirdPartyResource.RequestFormat == ApiRequestFormat.Raw)
                {
                    if (parameters.Count > 0)
                    {
                        request.AddJsonBody(parameters[0].Value);
                    }
                }
                else
                {
                    foreach (Param param in parameters)
                    {
                        request.AddParameter(param.Name, param.ValueTo);
                    }
                }

                IRestResponse resp = client.Execute(request);

                if (resp.IsSuccessful)
                    result = resp.Content;
                else
                    throw new Exception($"Failed to execute api [{thirdPartyResource.Url}], {resp.ErrorMessage}, {resp.Content}");
            }
            catch (Exception ex)
            {
                throw new Exception("[ExecuteThirdPartyApi], " + ex.Message);
            }
            return result;
        }

        public string ReplacePlaceholders(string text, EbApi Api)
        {
            if (!String.IsNullOrEmpty(text))
            {
                string pattern = @"\{{(.*?)\}}";
                IEnumerable<string> matches = Regex.Matches(text, pattern).OfType<Match>().Select(m => m.Groups[0].Value).Distinct();
                foreach (string _col in matches)
                {
                    try
                    {
                        string parameter_name = _col.Replace("{{", "").Replace("}}", "");
                        if (Api.GlobalParams.ContainsKey(parameter_name))
                        {
                            string value = Api.GlobalParams[parameter_name].ToString();
                            text = text.Replace(_col, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApiException("[Replace Placeholders in Url], parameter - " + _col + ". " + ex.Message);
                    }
                }
            }
            return text;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class RequestHeader : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string HeaderName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Value { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string ValueParameter { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public HttpHeaderType Type { set; get; }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class RequestParam : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string ParameterName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.String)]
        public string Value { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public EbDbTypes Type { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public bool UseThisVal { set; get; }
    }
}
