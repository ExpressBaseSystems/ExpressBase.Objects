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

using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using ServiceStack.Redis;
using System.Data.Common;
using ExpressBase.Common;
using Newtonsoft.Json;

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

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Decryption")]
        public bool EnableDecryption { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Decryption")]
        public EncryptionAlgorithm EncryptionAlgorithm { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Decryption")]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataReader { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Decryption")]
        public string KeyName { get; set; }

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
                    p.Value = ReplacePlaceholders(param.Value, globalParams);  
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
            Uri uri = new Uri(ReplacePlaceholders(thirdPartyResource.Url, Api.GlobalParams));

            object result;

            try
            {
                RestClient client = new RestClient(uri.GetLeftPart(UriPartial.Authority));

                RestRequest request = thirdPartyResource.CreateRequest(uri.PathAndQuery, Api.GlobalParams);

                List<Param> _params = thirdPartyResource.GetParameters(Api.GlobalParams) ?? new List<Param>();

                if (thirdPartyResource.Method == ApiMethods.POST && thirdPartyResource.RequestFormat == ApiRequestFormat.Raw)
                {
                    if (_params.Count > 0)
                    {
                        RequestParam requestParam = this.Parameters[0];
                        if (!string.IsNullOrEmpty(requestParam.Value) && requestParam.EnableEncryption)
                        {
                            string ciphertext = EbEncryption.ExecuteEncrypt(_params[0].Value, requestParam.KeyName, requestParam.DataReader, Api, requestParam.EncryptionAlgorithm);
                            request.AddJsonBody(ciphertext);
                        }
                        else
                        {
                            request.AddJsonBody(_params[0].Value);
                        }
                    }
                }
                else
                {
                    foreach (Param param in _params)
                    {
                        request.AddParameter(param.Name, param.ValueTo);
                    }
                }

                IRestResponse resp = client.Execute(request);

                if (resp.IsSuccessful)
                {
                    result = resp.Content;
                }
                else
                {
                    throw new Exception($"Failed to execute api [{thirdPartyResource.Url}], {resp.ErrorMessage}, {resp.Content}");
                }

                if (this.EnableDecryption)
                {
                    result = EbEncryption.ExecuteDecrypt(resp.Content, this.KeyName, this.DataReader, Api, this.EncryptionAlgorithm);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[ExecuteThirdPartyApi], " + ex.Message);
            }
            return result;
        }

        public string ReplacePlaceholders(string text, Dictionary<string, object> globalParams)
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
                        if (globalParams.ContainsKey(parameter_name))
                        {
                            string value = globalParams[parameter_name].ToString();
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
    public static class EbEncryption
    {
        public static string ExecuteEncrypt(string plainText, string KeyName, string Refid, EbApi Api, EncryptionAlgorithm EncryptionAlgorithm)
        {
            string pub_key = "";
            string ciphertext = "";
            try
            {
                if (!String.IsNullOrEmpty(KeyName))
                    pub_key = Api.Redis.Get<string>("publickey_" + Api.SolutionId + "_" + KeyName);

                if (String.IsNullOrEmpty(pub_key) && !String.IsNullOrEmpty(Refid))
                {
                    EbDataReader dataReader = Api.GetEbObject<EbDataReader>(Refid, Api.Redis, Api.ObjectsDB);

                    List<DbParameter> dbParameters = new List<DbParameter> {
                        Api.DataDB.GetNewParameter("keyname", EbDbTypes.String, KeyName)
                    };

                    EbDataSet dataSet = Api.DataDB.DoQueries(dataReader.Sql, dbParameters.ToArray());
                    pub_key = dataSet.Tables?[0].Rows[0]["key"].ToString();

                    Api.Redis.Set<string>("publickey_" + Api.SolutionId + "_" + KeyName, pub_key);
                }

                if (EncryptionAlgorithm == EncryptionAlgorithm.RSA)
                    ciphertext = RSAEncrypt(plainText, pub_key);
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteEncrypt], " + ex.Message);
            }
            return ciphertext;
        }

        public static string RSAEncrypt(string plainText, string pub)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            StringReader t = new StringReader(pub);
            PemReader pr = new PemReader(t);
            RsaKeyParameters keys = (RsaKeyParameters)pr.ReadObject();

            OaepEncoding eng = new OaepEncoding(new RsaEngine());
            eng.Init(true, keys);

            int length = plainTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> cipherTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                cipherTextBytes.AddRange(eng.ProcessBlock(
                    plainTextBytes, chunkPosition, chunkSize
                ));
            }
            return Convert.ToBase64String(cipherTextBytes.ToArray());
        }

        public static string ExecuteDecrypt(String ciphertext, string KeyName, string Refid, EbApi Api, EncryptionAlgorithm EncryptionAlgorithm)
        {
            string pri_key = "";
            string plaintext = "";
            try
            {
                if (!String.IsNullOrEmpty(KeyName))
                    pri_key = Api.Redis.Get<string>("privatekey_" + Api.SolutionId + "_" + KeyName);

                if (String.IsNullOrEmpty(pri_key) && !String.IsNullOrEmpty(Refid))
                {
                    EbDataReader dataReader = Api.GetEbObject<EbDataReader>(Refid, Api.Redis, Api.ObjectsDB);

                    List<DbParameter> dbParameters = new List<DbParameter> {
                        Api.DataDB.GetNewParameter("keyname", EbDbTypes.String, KeyName)
                    };

                    EbDataSet dataSet = Api.DataDB.DoQueries(dataReader.Sql, dbParameters.ToArray());
                    pri_key = dataSet.Tables?[0].Rows[0]["key"].ToString();

                    Api.Redis.Set<string>("privatekey_" + Api.SolutionId + "_" + KeyName, pri_key);
                }

                if (EncryptionAlgorithm == EncryptionAlgorithm.RSA)
                    plaintext = RSADecrypt(ciphertext, pri_key);
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteEncrypt], " + ex.Message);
            }
            return plaintext;
        }

        public static string RSADecrypt(string cipherText, string pri)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            StringReader t = new StringReader(pri);
            PemReader pr = new PemReader(t);

            AsymmetricCipherKeyPair keys = (AsymmetricCipherKeyPair)pr.ReadObject();

            OaepEncoding eng = new OaepEncoding(new RsaEngine());
            eng.Init(false, keys.Private);

            int length = cipherTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> plainTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                plainTextBytes.AddRange(eng.ProcessBlock(
                    cipherTextBytes, chunkPosition, chunkSize
                ));
            }
            string st = Encoding.UTF8.GetString(plainTextBytes.ToArray());
            return st;
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

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Encryption")]
        public bool EnableEncryption { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Encryption")]
        public EncryptionAlgorithm EncryptionAlgorithm { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Encryption")]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataReader { get; set; }


        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Encryption")]
        public string KeyName { get; set; }
    }
}
