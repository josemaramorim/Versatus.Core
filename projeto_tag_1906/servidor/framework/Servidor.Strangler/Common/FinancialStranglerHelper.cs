using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Projeto.Servidor.Strangler.Common
{
    /// <summary>
    /// Helper utilitÃ¡rio para comunicaÃ§Ã£o HTTP sÃ­ncrona com a nova API do .NET 8,
    /// utilizado na estratÃ©gia de estrangulamento do legado Versatus.
    /// </summary>
    public static class FinancialStranglerHelper
    {
        private static string _apiBaseUrl = null;
        private static string _stranglerApiKey = null;

        public static string ApiBaseUrl
        {
            get
            {
                if (_apiBaseUrl == null)
                {
                    try
                    {
                        _apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["Net8ApiBaseUrl"];
                    }
                    catch
                    {
                        // Ignora erro fora de ambiente de aplicaÃ§Ã£o ativa
                    }

                    if (string.IsNullOrEmpty(_apiBaseUrl))
                    {
                        _apiBaseUrl = "http://localhost:5000/api/";
                    }

                    if (!_apiBaseUrl.EndsWith("/"))
                    {
                        _apiBaseUrl += "/";
                    }
                }
                return _apiBaseUrl;
            }
        }

        public static string StranglerApiKey
        {
            get
            {
                if (_stranglerApiKey == null)
                {
                    try
                    {
                        _stranglerApiKey = System.Configuration.ConfigurationManager.AppSettings["Net8StranglerApiKey"];
                    }
                    catch
                    {
                        // Ignora erro fora de ambiente de aplicaÃ§Ã£o ativa
                    }

                    if (string.IsNullOrEmpty(_stranglerApiKey))
                    {
                        _stranglerApiKey = "";
                    }
                }
                return _stranglerApiKey;
            }
        }

        private static void ConfigurarClient(WebClient client)
        {
            client.Encoding = Encoding.UTF8;
            string key = StranglerApiKey;
            if (!string.IsNullOrEmpty(key))
            {
                client.Headers["X-Api-Key"] = key;
            }
        }

        public static T GetJson<T>(string relativeUrl)
        {
            using (var client = new WebClient())
            {
                ConfigurarClient(client);
                string fullUrl = ApiBaseUrl + relativeUrl;
                string json = client.DownloadString(fullUrl);
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public static void PutJson<T>(string relativeUrl, T body)
        {
            using (var client = new WebClient())
            {
                ConfigurarClient(client);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string fullUrl = ApiBaseUrl + relativeUrl;
                string jsonBody = JsonConvert.SerializeObject(body);
                client.UploadString(fullUrl, "PUT", jsonBody);
            }
        }

        public static TResponse PostJson<TRequest, TResponse>(string relativeUrl, TRequest body)
        {
            using (var client = new WebClient())
            {
                ConfigurarClient(client);
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string fullUrl = ApiBaseUrl + relativeUrl;
                string jsonBody = JsonConvert.SerializeObject(body);
                string responseJson = client.UploadString(fullUrl, "POST", jsonBody);
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }
        }
    }
}