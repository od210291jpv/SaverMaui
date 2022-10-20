using Realms;

using SaverMaui.Models;

using SaverMaui.Services;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.ServiceExtensions;

using SaverMaui.ViewModels;

using System.Windows.Input;

namespace SaverMaui.Commands
{
    internal class GetMostPopularCategoriesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private MostPopularCategoriesViewModel viewModel;

        private Realm realmInstance;

        public GetMostPopularCategoriesCommand(MostPopularCategoriesViewModel vm)
        {
            this.viewModel = vm;

            this.realmInstance = Realm.GetInstance();
        }

        public bool CanExecute(object parameter)
        {
            return IsOnlineHelper.IsOnline;
        }

        public async void Execute(object parameter)
        {
            this.viewModel.IsDataRefreshing = true;

            CategoryDto[] popularCategories = await BackendServiceClient.GetInstance().GetMostPopularCategories(50);

            this.viewModel.MostPopularCategories.Clear();
            var allRealmCats = this.realmInstance.All<Category>();

            foreach (var category in popularCategories) 
            {
                this.viewModel.MostPopularCategories.Add(allRealmCats.Single(ct => ct.CategoryId == category.CategoryId));
            }

            this.viewModel.IsDataRefreshing = false;
        }
    }
}
