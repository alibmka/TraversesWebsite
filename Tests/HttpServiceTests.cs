using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraversesService;

namespace Tests
{
    internal class HttpServiceTests
    {
        [SetUp]
        public void Setup()
        {
            httpService = new HttpService();
        }

        [TestCase("http://")]
        [TestCase("")]
        public async Task Download_InputInvalidUrl_ShouldReturnEmpty(string url)
        {
            var actualResult = await httpService.DownloadContentAsync(url);

            Assert.IsEmpty(actualResult, "Result expted be empty");
        }

        [Test]
        public async Task Download_InputValidUrl_ShouldReturnContent()
        {
            var actualResult = await httpService.DownloadContentAsync("https://books.toscrape.com/index.html");

            Assert.IsNotEmpty(actualResult, "Result expted not be empty");
        }

        #region [ Private Field(s) ]

        private TraversesService.Interfaces.IHttpService httpService = null;

        #endregion
    }
}
