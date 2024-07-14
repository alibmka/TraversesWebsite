using System;
using System.Diagnostics.CodeAnalysis;
using TraversesService;
using TraversesService.Interfaces;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            IHttpService httpService = new HttpService();
            IFileServices fileServices = new FileService();
            IPageService pageService = new PageService(fileServices, httpService);

            traversWebsiteService = new TraversWebsiteService(fileServices, httpService, pageService);
        }

        [TestCase("yahoo.com")]
        [TestCase("htt://a.c")]
        [TestCase("a.cc")]
        public async Task Start_UrlWithNoScheme_ShouldReturnFalse(string url)
        {
            var actualResult = await traversWebsiteService.StartAsync(url);

            Assert.IsFalse(actualResult, "Not Correct");
        }

        [Test]
        public async Task Start_ValidUrl_ShouldFetchAllData()
        {
            var tempUrl = "https://books.toscrape.com/";
            var actualResult = await traversWebsiteService.StartAsync(tempUrl);

            Assert.IsTrue(actualResult);
        }

        [Test]
        public async Task TestHelper()
        {
            var tempUrl = "https://books.toscrape.com/index.html";
            var actualResult = TraversesService.Utilities.Helpers.URLHelper.GetFileNameFromUrl(tempUrl);

            Assert.IsNotEmpty(actualResult);
        }

        #region [ Private Field(s) ]

        private ITraversWebsiteService traversWebsiteService = null;

        #endregion
    }
}