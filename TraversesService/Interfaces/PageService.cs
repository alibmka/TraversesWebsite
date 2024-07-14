using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraversesService.Interfaces
{
    public class PageService : IPageService
    {

        #region [ Constructor(s) ]

        public PageService(IFileServices fileServices, IHttpService httpService)
        {
            _fileService = fileServices;
            _httpService = httpService;
        }

        #endregion

        #region [ Public Method(s) ]

        public async Task ManagePageAsync(Uri baseUri, string url, bool processProductList = true)
        {
            if (!string.IsNullOrEmpty(url))
            {
                this.baseUri = baseUri;

                await ManagePageAsync(url, processProductList);
            }

            await Task.WhenAll(tasks);
        }


        #endregion

        #region Private Field(s) 

        private IHttpService _httpService = null;
        private IFileServices _fileService = null;
        private Uri baseUri = null;

        private List<Task> tasks = new List<Task>();

        #endregion

        #region [ Private Method(s) ]

        private async Task ManagePageAsync(string url, bool processProductList = true)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var tempUri = new Uri(url);
                _fileService.SaveFileFromUrlAsync(tempUri);

                string tempContent = await _httpService.DownloadContentAsync(tempUri.ToString());
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(tempContent);

                await ManageImagesInPageAsync(document);
                if (processProductList)
                {
                    await ManagesProductListAsyc(tempUri, document);
                }

                var paginationNextUrl = await ManagePaginationAsync(document);
                if (!string.IsNullOrEmpty(paginationNextUrl))
                {
                    Uri tempDiretoryPath = new Uri(tempUri, ".");
                    var tempUrl = Path.Combine(tempDiretoryPath.OriginalString, paginationNextUrl);
                    if (url.EndsWith("index.html"))
                    {
                        var tempFirstPageUri = new Uri(tempDiretoryPath, Path.Combine(Path.GetDirectoryName(paginationNextUrl), "page-1.html"));
                        _fileService.SaveFileFromUrlAsync(tempFirstPageUri);
                    }

                    await ManagePageAsync(tempUrl, processProductList);
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task<string> ManagePaginationAsync(HtmlDocument document)
        {
            string result = string.Empty;
            // save next button link

            var nextUrl = document.DocumentNode.SelectNodes("//li[@class=\"next\"]")?
                                           .Descendants("a")
                                           .Select(node => node.GetAttributeValue("href", string.Empty))
                                           .Distinct().FirstOrDefault() ?? string.Empty;
            // manage page of next link
            result = nextUrl;

            return result;
        }

        private async Task ManagesProductListAsyc(Uri baseUri, HtmlDocument document)
        {
            var products = document.DocumentNode.SelectNodes("//ol[@class=\"row\"]")?
                                                .Descendants("a")
                                                .Select(node => node.GetAttributeValue("href", string.Empty))
                                                .Select(href => new Uri(baseUri, href).ToString())
                                                .Distinct().ToList() ?? new List<string>();
            foreach (var product in products)
            {
                tasks.Add(ManageProductPageAsync(product));
            }
        }

        private async Task ManageProductPageAsync(string productUrl)
        {
            if (!string.IsNullOrEmpty(productUrl))
            {
                Uri uri = new Uri(productUrl);
                _fileService.SaveFileFromUrlAsync(uri);

                string tempContent = await _httpService.DownloadContentAsync(uri.ToString());
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(tempContent);

                var productImageUrl = document.DocumentNode.SelectNodes("//div[@id=\"product_gallery\"]")?
                                               .Descendants("img")
                                               .Select(node => node.GetAttributeValue("src", string.Empty))
                                               .Select(href => new Uri(baseUri, href).ToString())
                                               .Distinct().FirstOrDefault() ?? string.Empty;
                uri = new Uri(productImageUrl);
                _fileService.SaveFileFromUrlAsync(uri);
            }
        }

        /// <summary>
        /// Downloads all iamges in content
        /// </summary>
        /// <param name="document"></param>
        private async Task ManageImagesInPageAsync(HtmlDocument document)
        {
            var images = document.DocumentNode.Descendants("img")?
                                                .Select(node => node.GetAttributeValue("src", string.Empty))
                                                .Where(href => !string.IsNullOrWhiteSpace(href))
                                                .Select(href => new Uri(baseUri, href).ToString())
                                                .Distinct().ToList() ?? new List<string>();

            foreach (var item in images)
            {
                Uri uri = new Uri(item);
                _fileService.SaveFileFromUrlAsync(uri);
            }
        }

        #endregion

    }
}
