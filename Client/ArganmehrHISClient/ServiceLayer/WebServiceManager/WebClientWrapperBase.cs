using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.WebServiceManager
{
    public abstract class WebClientWrapperBase : IDisposable
    {
        private readonly string _baseUrl;
        private Lazy<HttpClient> _lazyClient;
        private  string _URL;

        protected WebClientWrapperBase(string baseUrl)
        {
            _baseUrl = baseUrl.Trim('/');
            _lazyClient = new Lazy<HttpClient>(() => new HttpClient());
        }

        protected HttpClient Client()
        {
            if (_lazyClient == null)
            {
                throw new ObjectDisposedException("WebClient has been disposed");
            }

            return _lazyClient.Value;
        }

        protected async  Task<T> Execute<T>(string urlSegment)
        {
            _URL=_baseUrl + '/' + urlSegment.TrimStart('/');
            Client().BaseAddress = new Uri(_URL);
            Client().DefaultRequestHeaders.Accept.Clear();
            Client().DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responseMessage = await Client().GetAsync(_URL);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;
                var Result = JsonConvert.DeserializeObject<T>(responseData);
                return Result;
            }

            HttpResponseMessage r = new HttpResponseMessage(HttpStatusCode.NoContent);
            var rData = r.Content.ReadAsStringAsync().Result;
             var rs = JsonConvert.DeserializeObject<T>(rData);
             return rs;
        }

        ~WebClientWrapperBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_lazyClient != null)
            {
                if (disposing)
                {
                    if (_lazyClient.IsValueCreated)
                    {
                        _lazyClient.Value.Dispose();
                        _lazyClient = null;
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
        }
    }
}
