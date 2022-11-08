using NUnit.Framework;

using SpecFlowSaverTests.Services;
using SpecFlowSaverTests.Services.Extensions;
using SpecFlowSaverTests.Support.Models;

namespace SpecFlowSaverTests.StepDefinitions
{
    [Binding]
    [TestFixture]
    public class DeleteExistingCategoryStepDefinitions
    {
        private Guid categoryId;

        [Given(@"Category '([^']*)'")]
        public void GivenCategory(string test)
        {
            CreateCategoryRequestModel category = new CreateCategoryRequestModel()
            {
                AmountOfFavorites = 0,
                AmountOfOpenings = 0,
                CategoryId = Guid.NewGuid(),
                Name = test,
            };

            this.categoryId = category.CategoryId;

            BackendServiceClient.GetInstance().CreateCategoryAsync(category).Wait();
        }

        [When(@"the category is available by GetCategories endpoint")]
        public void WhenTheCategoryIsAvailableByGetCategoriesEndpoint()
        {
            var result = BackendServiceClient.GetInstance().GetAllCategoriesAsync().Result;
            result.Select(result => result.CategoryId).ToArray().Should().Contain(categoryId);
        }

        [Then(@"I can delete the category")]
        public void ThenICanDeleteTheCategory()
        {
            throw new PendingStepException();
        }

        [Then(@"the deleted category should not be available anymore in GetCategories results")]
        public void ThenTheDeletedCategoryShouldNotBeAvailableAnymoreInGetCategoriesResults()
        {
            throw new PendingStepException();
        }

    }
}
