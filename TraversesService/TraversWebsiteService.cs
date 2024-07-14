using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TraversesService.Interfaces;
using TraversesService.Utilities.Helpers;

namespace TraversesService
{
    public class TraversWebsiteService : ITraversWebsiteService
    {
        #region [ Constructor(s) ]

        public TraversWebsiteService(IFileServices fileServices,
                                     IHttpService httpService,
                                     IPageService pageService)
        {
            _fileService = fileServices;
            _httpService = httpService;
            _pageService = pageService;
        }

        #endregion

        #region [ Public Method(s) ]

        public async Task<bool> StartAsync(string url)
        {
            bool result = false;
            try
            {
                if (URLHelper.ValidateUrl(url))
                {
                    Uri uri = new Uri(url);
                    baseUri = new Uri(uri.GetLeftPart(UriPartial.Authority));

                    string tempContent = await _httpService.DownloadContentAsync(baseUri.ToString());

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(tempContent);

                    tasks.Add(ManageResourcesAsync(document));
                    var tempHome = FindHomeLink(document);

                    tasks.Add(_pageService.ManagePageAsync(baseUri, tempHome, false));

                    tasks.Add(ManageNavigateLinksAsync(document));

                    await Task.WhenAll(tasks);
                    Thread.Sleep(2000);

                    result = true;
                }
            }
            catch (Exception exp)
            {
                throw;
            }

            return result;
        }

        #endregion

        #region [ Private Field(s) ]

        private IHttpService _httpService = null;
        private IFileServices _fileService = null;
        private IPageService _pageService = null;

        private Uri baseUri = null;
        private List<Task> tasks = new List<Task>();

        #endregion

        #region [ Private Method(s) ]

        /// <summary>
        /// Downloads all script and css resource files
        /// </summary>
        /// <param name="document"></param>
        private async Task ManageResourcesAsync(HtmlDocument document)
        {
            var links = document.DocumentNode.
                     Descendants("link")?
                    .Select(node => node.GetAttributeValue("href", string.Empty))
                    .Where(href => !string.IsNullOrWhiteSpace(href))
                    .Select(href => new Uri(baseUri, href).ToString())
                    .Distinct().ToList() ?? new List<string>();

            foreach (var item in links)
            {
                Uri uri = new Uri(item);
                _fileService.SaveFileFromUrlAsync(uri);
            }

            var scripts = document.DocumentNode.
                              Descendants("script")
                             .Select(node => node.GetAttributeValue("src", string.Empty))
                             .Where(href => !string.IsNullOrWhiteSpace(href))
                             .Select(href => new Uri(baseUri, href).ToString())
                             .Distinct().ToList();

            foreach (var item in scripts)
            {
                Uri uri = new Uri(item);
                _fileService.SaveFileFromUrlAsync(uri);
            }
        }

        private async Task ManageNavigateLinksAsync(HtmlDocument document)
        {
            var navLinks = document.DocumentNode.SelectNodes("//div[@class=\"side_categories\"]")?
                                                .Descendants("a")
                                                .Select(node => node.GetAttributeValue("href", string.Empty))
                                                .Select(href => new Uri(baseUri, href).ToString())
                                                .Distinct().ToList() ?? new List<string>();

            foreach (var item in navLinks)
            {
                tasks.Add(_pageService.ManagePageAsync(baseUri, item));
            }
        }

        private List<string> FindLinks(HtmlDocument document)
        {
            List<string> result = new List<string>();

            result = document.DocumentNode.SelectNodes("//[@href]")?
                            .Select(node => node.GetAttributeValue("href", string.Empty))
                            .Where(href => !string.IsNullOrWhiteSpace(href))
                            .Select(href => new Uri(baseUri, href).ToString())
                            .Distinct().ToList() ?? new List<string>();

            return result;
        }

        private string FindHomeLink(HtmlDocument document)
        {
            string result = string.Empty;

            result = document.DocumentNode.SelectNodes("//div[@class=\"page_inner\"]")?
                                           .Descendants("a")
                                           .Where(href => href.GetDirectInnerText() == "Home")
                                           .Select(node => node.GetAttributeValue("href", string.Empty))
                                           .Select(href => new Uri(baseUri, href).ToString())
                                           .Distinct().FirstOrDefault() ?? string.Empty;
            return result;
        }

        //private async Task<string> ManagePaginationAsync(HtmlDocument document)
        //{
        //    string result = string.Empty;
        //    // save next button link

        //    var nextUrl = document.DocumentNode.SelectNodes("//li[@class=\"next\"]")?
        //                                   .Descendants("a")
        //                                   .Select(node => node.GetAttributeValue("href", string.Empty))
        //                                   .Distinct().FirstOrDefault() ?? string.Empty;
        //    // manage page of next link
        //    result = nextUrl;

        //    // if should save previous button
        //    // save previous button link page

        //    return result;
        //}

        //private async Task ManagesProductListAsyc(Uri baseUri, HtmlDocument document)
        //{
        //    var products = document.DocumentNode.SelectNodes("//ol[@class=\"row\"]")?
        //                                        .Descendants("a")
        //                                        .Select(node => node.GetAttributeValue("href", string.Empty))
        //                                        .Select(href => new Uri(baseUri, href).ToString())
        //                                        .Distinct().ToList() ?? new List<string>();
        //    foreach (var product in products)
        //    {
        //        tasks.Add(ManageProductPageAsync(product));
        //    }
        //}

        //private async Task ManageProductPageAsync(string productUrl)
        //{
        //    if (!string.IsNullOrEmpty(productUrl))
        //    {
        //        Uri uri = new Uri(productUrl);
        //        _fileService.SaveFileFromUrlAsync(uri);

        //        string tempContent = await _httpService.DownloadContentAsync(uri.ToString());
        //        HtmlDocument document = new HtmlDocument();
        //        document.LoadHtml(tempContent);

        //        var productImageUrl = document.DocumentNode.SelectNodes("//div[@id=\"product_gallery\"]")?
        //                                       .Descendants("img")
        //                                       .Select(node => node.GetAttributeValue("src", string.Empty))
        //                                       .Select(href => new Uri(baseUri, href).ToString())
        //                                       .Distinct().FirstOrDefault() ?? string.Empty;
        //        uri = new Uri(productImageUrl);
        //        _fileService.SaveFileFromUrlAsync(uri);
        //    }
        //}

        ///// <summary>
        ///// Downloads all iamges in content
        ///// </summary>
        ///// <param name="document"></param>
        //private async Task ManageImagesInPageAsync(HtmlDocument document)
        //{
        //    var images = document.DocumentNode.Descendants("img")?
        //                                        .Select(node => node.GetAttributeValue("src", string.Empty))
        //                                        .Where(href => !string.IsNullOrWhiteSpace(href))
        //                                        .Select(href => new Uri(baseUri, href).ToString())
        //                                        .Distinct().ToList() ?? new List<string>();

        //    foreach (var item in images)
        //    {
        //        Uri uri = new Uri(item);
        //        _fileService.SaveFileFromUrlAsync(uri);
        //    }
        //}

        //private async Task ManagePageAsync(string url, bool processProductList = true)
        //{
        //    if (!string.IsNullOrEmpty(url))
        //    {
        //        var tempUri = new Uri(url);
        //        _fileService.SaveFileFromUrlAsync(tempUri);

        //        string tempContent = await _httpService.DownloadContentAsync(tempUri.ToString());
        //        HtmlDocument document = new HtmlDocument();
        //        document.LoadHtml(tempContent);

        //        await ManageImagesInPageAsync(document);
        //        if (processProductList)
        //        {
        //            await ManagesProductListAsyc(tempUri, document);
        //        }

        //        var paginationNextUrl = await ManagePaginationAsync(document);
        //        if (!string.IsNullOrEmpty(paginationNextUrl))
        //        {
        //            Uri tempDiretoryPath = new Uri(tempUri, ".");
        //            var tempUrl = Path.Combine(tempDiretoryPath.OriginalString, paginationNextUrl);
        //            if (url.EndsWith("index.html"))
        //            {
        //                var tempFirstPageUri = new Uri(tempDiretoryPath, Path.Combine(Path.GetDirectoryName(paginationNextUrl), "page-1.html"));
        //                _fileService.SaveFileFromUrlAsync(tempFirstPageUri);
        //            }

        //            await ManagePageAsync(tempUrl, processProductList);
        //        }
        //    }
        //    // await Task.WhenAll(tasks);
        //}

        #endregion
    }
}
