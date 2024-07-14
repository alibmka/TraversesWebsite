using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraversesService.Interfaces;
using TraversesService.Utilities.Helpers;

namespace TraversesService
{
    public class HttpService : IHttpService
    {

        #region [ Public Method(s) ]

        public async Task<string> DownloadContentAsync(string url)
        {
            string result = string.Empty;
            try
            {
                if (URLHelper.ValidateUrl(url))
                {
                    HttpResponseMessage response = await Client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception exp)
            {
                throw;
            }

            return result;
        }

        #endregion

        #region [ Private Property(s)  ]

        private HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new HttpClient() { Timeout = new TimeSpan(0, 3, 0) };
                }
                return client;
            }
        }

        #endregion

        #region [ Private Fields ]

        HttpClient client = null;

        #endregion
    }
}
