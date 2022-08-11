//using CashTrack.Models.MerchantModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;
//using Shouldly;
//using Newtonsoft.Json;
//using System.Net;

//namespace CashTrack.IntegrationTests
//{

//    public class MerchantsEndpointsShould : IClassFixture<TestServerFixture>
//    {
//        private readonly TestServerFixture _fixture;
//        private ITestOutputHelper _output;
//        const string ENDPOINT = "api/merchants";

//        public MerchantsEndpointsShould(TestServerFixture fixture, ITestOutputHelper output)
//        {
//            _fixture = fixture;
//            _output = output;
//        }
//        [Theory]
//        [InlineData("Costco")]
//        [InlineData("costco")]
//        [InlineData("John")]
//        [InlineData("Home")]
//        public async Task ReturnMerchantsWithMatchingSearchTerm(string query)
//        {
//            var response = await _fixture.Client.GetAsync(ENDPOINT + $"?merchantName={query}");
//            response.EnsureSuccessStatusCode();
//            var responseObject = JsonConvert.DeserializeObject<MerchantResponse>(await response.Content.ReadAsStringAsync());
//            responseObject.ListItems.Count().ShouldBeGreaterThan(1);
//            responseObject.ListItems.First().Name.ShouldContain(query);
//            PrintRequestAndResponse(ENDPOINT + $"?query={query}", await response.Content.ReadAsStringAsync());
//        }

//        private void PrintRequestAndResponse(object request, object response)
//        {
//            _output.WriteLine(request.ToString());
//            _output.WriteLine(response.ToString());
//        }
//    }
//}
