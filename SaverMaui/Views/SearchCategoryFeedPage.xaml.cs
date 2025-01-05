using Newtonsoft.Json;
using Realms;
using RestSharp;
using SaverMaui.Models;
using SaverMaui.Services;
using SaverMaui.Services.Contracts;
using SaverMaui.Services.Contracts.Category;
using SaverMaui.Services.Contracts.Content;
using SaverMaui.Services.Helpers;
using SaverMaui.Services.ServiceExtensions;
using SaverMaui.ViewModels;
using System.Web;

namespace SaverMaui.Views;

public partial class SearchCategoryFeedPage : ContentPage
{
    private Category currentCat;

    public Category CurrentCat { get => currentCat; set => currentCat = value; }


    public SearchCategoryFeedPage()
    {
        InitializeComponent();

        this.Appearing += OnAppear;
    }

    private void OnAppear(object sender, EventArgs e)
    {
        foreach (var content in Environment.CurrectSearchResultCategory.Value)
        {
            SearchCategoryFeedViewModel.instance?.SearchResults.Add(content);
        }
        this.Title = Environment.CurrectSearchResultCategory.Key;
    }

    private async void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        Realm _realm = Realm.GetInstance();

        var catss = await BackendServiceClient.GetInstance().GetAllCategoriesAsync();

        var ImageUri = SearchCategoryFeedViewModel.instance.CurrentResult.Url;

        var ur = $"{UriHelper.ImageRecognitionApi}{HttpUtility.UrlEncode(ImageUri)}";
        var resp = await new RestClient().ExecuteGetAsync<string>(new RestRequest(ur, Method.Get));

        var reqCat = catss.FirstOrDefault(c => c.Name == JsonConvert.DeserializeObject<string>(resp.Content)) ?? catss.Single(c => c.Name.ToLower() == "rest");

        if (reqCat != null)
        {
            Content content = new Content()
            {
                CategoryId = reqCat.CategoryId,
                ImageUri = Environment.CurrectSearchResultItem,
                Title = "Sexy"
            };

            _realm.Write(() => _realm.Add<Content>(content));

            PostContentDataRequest request = new PostContentDataRequest()
            {
                Categories = Array.Empty<CategoryDto>(),
                Content = new ContentDto[] { new ContentDto
                {
                    CategoryId = reqCat.CategoryId,
                    DateCreated = DateTime.Now,
                    Title = "Sexy",
                    ImageUri = SearchCategoryFeedViewModel.instance.CurrentResult.Url
                }}
            };

            _ = await BackendServiceClient.GetInstance().PostAllContentDataAsync(request);

            await Application.Current.MainPage.DisplayAlert("Ok", $"Content added into the category: {reqCat.Name}", "Ok");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Required category  does not exist!", "Ok");
        }
    }
}