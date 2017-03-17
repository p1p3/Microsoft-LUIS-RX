using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Observable.Aliases;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Luis.Reactive.Structures;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Extensions.MonoHttp;

namespace Luis.Reactive
{
    public class LuisClient : IDisposable
    {

        private const string DEFAULT_BASE_URI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/{appId}?subscription-key={subscriptionKey}&verbose=true&q=";

        /// <summary>
        /// flag to indidicate whether to return full result of all intents not just the top scoring intent (for preview features only)
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// flag to inidicate that preview features will be used
        /// </summary>
        public bool Preview { get; set; }

        protected string BASE_API_URL { get; set; }
        private readonly HttpClient _http;
        private readonly string _appId;
        private readonly string _appKey;

        /// <summary>
        /// Generates an API URI using the provided id and key for a registered LUIS application.
        /// </summary>
        /// <param name="id">Application id</param>
        /// <param name="subscriptionKey">Application key</param>
        /// <returns>Application URL for <see cref="LuisClient"/></returns>
        private string CreateApplicationUri(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));

            return $"{BASE_API_URL}?id={id}&q=";
        }

        /// <summary>
        /// Generates an API URI (for preview features) using the provided id and key for a registered LUIS application.
        /// </summary>
        /// <param name="id">Application id</param>
        /// <param name="subscriptionKey">Application key</param>
        /// /// <param name="verbose">if true, return all intents not just the top scoring intent</param>
        /// <returns>Application Preview URL for <see cref="LuisClient"/></returns>
        private string CreateApplicationPreviewUri(string id)
        {
            if (String.IsNullOrWhiteSpace(id)) throw new ArgumentException(nameof(id));

            string verboseQuery = (Verbose) ? "&verbose=true" : "";

            return $"{BASE_API_URL}/preview?id={id}{verboseQuery}&q=";
        }

        /// <summary>
        /// Construct a new Luis client with a shared <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="appId">The application ID of the LUIS application</param>
        /// <param name="appKey">The application subscription key of the LUIS application</param>
        /// <param name="preview">A flag indicating whether to use preview features or not (Dialogue)</param>
        /// top scoring in case of using the dialogue</param>
        public LuisClient(string appId, string appKey, bool preview = false) : this(appId, appKey, DEFAULT_BASE_URI, preview) { }

        /// <summary>
        /// Construct a new Luis client with a shared <see cref="HttpClient"/> instance.
        /// </summary>
        /// <param name="appId">The application ID of the LUIS application</param>
        /// <param name="appKey">The application subscription key of the LUIS application</param>
        /// <param name="baseApiUrl">Root URI for the service endpoint.</param>
        /// <param name="preview">A flag indicating whether to use preview features or not (Dialogue)</param>
        /// top scoring in case of using the dialogue</param>
        public LuisClient(string appId, string appKey, string baseApiUrl, bool preview = false)
        {
            if (String.IsNullOrWhiteSpace(appId)) throw new ArgumentException(nameof(appId));
            if (String.IsNullOrWhiteSpace(appKey)) throw new ArgumentException(nameof(appKey));
            if (String.IsNullOrWhiteSpace(baseApiUrl)) throw new ArgumentException(nameof(baseApiUrl));

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("OCP-APIM-Subscription-Key", appKey);
            BASE_API_URL = baseApiUrl;

            Preview = preview;
            _appId = appId;
            _http = httpClient;
            _appKey = appKey;
        }

        /// <summary>
        /// Encodes text to be suitable for http requests
        /// </summary>
        /// <param name="text"></param>
        /// <param name="forceSetParameterName">The name of a parameter to reset in the current dialog</param>
        /// <returns></returns>
        private string EncodeRequest(string text, string forceSetParameterName = null)
        {
            var queryString = HttpUtility.ParseQueryString(text);
            var url = BASE_API_URL.Replace("{appId}", _appId).Replace("{subscriptionKey}", _appKey) + queryString;
            return url;
        }

        /// <summary>
        /// Sends a request to the LUIS service to parse <paramref name="text"/> for intents and entities.
        /// </summary>
        /// <param name="text">The text to Predict the intent for</param>
        /// <returns></returns>
        public IObservable<LuisResult> Predict(string text)
        {
            if (String.IsNullOrWhiteSpace(text)) throw new ArgumentException(nameof(text));

            var uri = EncodeRequest(text);
            var client = new RestClient(uri);

            var request = new RestRequest(Method.GET);
            return client.ExecuteTaskAsync(request).ToObservable()
                         .Map(httpResponse => JToken.Parse(httpResponse.Content))
                         .Map(result => new LuisResult(result));
        }

        public IObservable<string> PredictAndAct(string text)
        {
            return this.Predict(text)
                  .Map(luisResult =>
                  {
                      var handler = HandlersContainer.ResolveHandler(luisResult);
                      return handler.Exec(luisResult);
                  });
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _http.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes this <see cref="LuisClient"/> and associated managed objects.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
