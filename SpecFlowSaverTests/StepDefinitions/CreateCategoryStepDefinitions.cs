using NUnit.Framework;
using NUnit.Framework.Internal;
using SpecFlowSaverTests.Services;
using SpecFlowSaverTests.Services.Extensions;
using SpecFlowSaverTests.Support.Models;

namespace SpecFlowSaverTests.StepDefinitions
{
    [Binding]
    [TestFixture]
    public class CreateCategoryStepDefinitions
    {
        private BackendServiceClient client;
        private Guid categoryId;

        [OneTimeSetUp]
        public void SetUp() 
        {
            this.client = BackendServiceClient.GetInstance();
        }

        [When(@"I call endpoint for create the category with name ""([^""]*)""")]
        public void WhenICallEndpointForCreateTheCategoryWithName(string test)
        {
            var category = new CreateCategoryRequestModel()
            {
                AmountOfFavorites = 0,
                AmountOfOpenings = 0,
                Name = test,
            };

            this.categoryId = category.CategoryId;

            BackendServiceClient.GetInstance().CreateCategory(category).Wait();
        }

        [Then(@"I can find this category in results of GetAllCategory endpoint")]
        public void ThenICanFindThisCategoryInResultsOfGetAllCategoryEndpoint()
        {
            AllCategoriesResponseModel[] allCats = BackendServiceClient.GetInstance().GetAllCategoriesAsync().Result;

            allCats.Select(x => x.CategoryId).ToList().Should().Contain(this.categoryId);
        }
    }
}
