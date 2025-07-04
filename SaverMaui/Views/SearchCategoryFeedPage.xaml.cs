using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
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
        ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => handler.PlatformView?.Clear());

        foreach (var content in Environment.CurrectSearchResultCategory.Value)
        {
            SearchCategoryFeedViewModel.instance?.SearchResults.Add(content);
        }
        this.Title = Environment.CurrectSearchResultCategory.Key;
    }

    private async void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => handler.PlatformView?.Clear());
        
        Realm _realm = Realm.GetInstance();

        var catss = await BackendServiceClient.GetInstance().GetAllCategoriesAsync();

        var ImageUri = SearchCategoryFeedViewModel.instance.CurrentResult.Url;

        var ur = $"{UriHelper.ImageRecognitionApi}{HttpUtility.UrlEncode(ImageUri)}";
        var resp = await new RestClient().ExecuteGetAsync<string>(new RestRequest(ur, Method.Get));

        var reqCat = catss.FirstOrDefault(c => c.Name == JsonConvert.DeserializeObject<string>(resp.Content)) ?? catss.Single(c => c.Name.ToLower() == "rest");

        var lastIdResponse = await BackendServiceClient.GetInstance().GetAllContentAsync();
        var lastId = lastIdResponse.Select(c => c.Id).Max();

        if (reqCat != null)
        {
            Content content = new Content()
            {
                CategoryId = reqCat.CategoryId,
                ImageUri = SearchCategoryFeedViewModel.instance.CurrentResult.Url,
                Title = SearchCategoryFeedViewModel.instance.CurrentResult.Url.Split("/").Last().Split("_").First(),
                Id = lastId + 1
            };

            _realm.Write(() => _realm.Add<Content>(content));

            PostContentDataRequest request = new PostContentDataRequest()
            {
                Categories = Array.Empty<CategoryDto>(),
                Content = new ContentDto[] { new ContentDto
                {
                    CategoryId = reqCat.CategoryId,
                    DateCreated = DateTime.Now,
                    Title = SearchCategoryFeedViewModel.instance.CurrentResult.Url.Split("/").Last().Split("_").First(),
                    ImageUri = SearchCategoryFeedViewModel.instance.CurrentResult.Url,
                    Id = content.Id
                }}
            };

            var response = await BackendServiceClient.GetInstance().PostAllContentDataAsync(request);

            await Application.Current.MainPage.DisplayAlert("Ok", $"Content added into the category: {reqCat.Name}", "Ok");
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Required category  does not exist!", "Ok");
        }
    }

    private async void Feed_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => handler.PlatformView?.Clear());
        await Task.Delay(1000);
    }

    private async void Feed_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => handler.PlatformView?.Clear());
        await Task.Delay(1000);
    }
}