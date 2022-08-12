using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AutoMapper.Configuration.Conventions;
using Bogus.DataSets;
using CashTrack.IntegrationTests.Common;
using CashTrack.IntegrationTests.Pages.Common;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CashTrack.IntegrationTests.Pages
{
    public class SubCategoryPageTests : IClassFixture<AuthenticatedWebApplicationFactory<CashTrack.Program>>
    {
        private readonly AuthenticatedWebApplicationFactory<CashTrack.Program> _factory;
        private readonly HttpClient _client;
        private ITestOutputHelper _output;
        private string _endpoint;
        public SubCategoryPageTests(AuthenticatedWebApplicationFactory<CashTrack.Program> factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
            _client = factory._client;
            _endpoint = "/Expenses/Categories";
        }
        [Fact]
        public async Task Can_View_SubCategory_Table()
        {
            var subCategoryPage = await _client.GetAsync(_endpoint);
            var subCategoryPageresult = await subCategoryPage.Content.ReadAsStringAsync();
            var subCategoryPageContent = await HtmlHelpers.GetDocumentAsync(subCategoryPage);
            subCategoryPageContent.QuerySelector<IHtmlSpanElement>("#totalCount")!.TextContent.ShouldContain("20 of 73");
            PrintRequestAndResponse(_endpoint, subCategoryPageresult);
            subCategoryPage.EnsureSuccessStatusCode();
        }
        [Fact]
        public async Task Can_Search_Categories_and_Redirect_To_Detail()
        {
            var subCategoryPage = await _client.GetAsync($"{_endpoint}?searchTerm=Groceries");
            var subCategoryPageResult = await subCategoryPage.Content.ReadAsStringAsync();
            PrintRequestAndResponse($"{_endpoint}?searchTerm=Groceries", subCategoryPageResult);
            subCategoryPage!.RequestMessage!.RequestUri!.AbsolutePath.ShouldBe("/Expenses/Categories/Detail/31");
            subCategoryPage.EnsureSuccessStatusCode();
        }


        private void PrintRequestAndResponse(object request, object response)
        {
            _output.WriteLine(request.ToString());
            _output.WriteLine(response.ToString());
        }
    }
}
