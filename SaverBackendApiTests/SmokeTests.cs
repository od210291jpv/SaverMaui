using FluentAssertions;
using Microsoft.Extensions.Options;
using SaverBackendApiClient;
using SaverBackendApiClient.Configuration;
using System.Net;

namespace SaverBackendApiTests
{
    [TestFixture]
    public class Tests
    {
        protected SaverBackendClient client;

        [SetUp]
        public void Setup()
        {
            this.client = new SaverBackendClient(new SaverBackendClientSettings() { BaseUrl = "http://192.168.88.252", Timeout = TimeSpan.FromSeconds(10) });
        }

        [Test]
        public async Task RegisterUserTest()
        {
            var response = await this.client.ProfileActions.RegisterProfileAsync(new SaverBackendApiClient.DTO.Profile.RegisterProfileRequestModel() 
            {
                Password = "test",
                UserName = "test123" + DateTime.Now.ToString("hmsttmm"),
                VerificationCode = ""
            });
           
            response.Should().Be(HttpStatusCode.OK, "The registration should succeed with a valid request.");
        }

        [Test]
        public async Task GetPagedContent() 
        {
            var content = await this.client.ContentActions.GetPagedContent();
            content.Should().NotBeNull("The content should not be null.");
            content.Length.Should().Be(50);
        }
    }
}