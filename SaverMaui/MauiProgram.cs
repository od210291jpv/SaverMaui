using CarouselView;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using FFImageLoading.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace SaverMaui
{

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiCommunityToolkitMarkup()
                .UseMauiCarouselView()
                .UseFFImageLoading()
                .ConfigureFonts(fonts =>
                {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if __ANDROID__
            ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => PrependToMappingImageSource(handler, view));
#endif

#if ANDROID
            // Перехоплюємо абсолютно всі незловлені помилки в Android
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                var ex = args.Exception;
                var realMessage = ex.InnerException?.Message ?? ex.Message;
                Android.Util.Log.Error("CRITICAL_CRASH", $"ГЛОБАЛЬНА ПОМИЛКА: {realMessage} | StackTrace: {ex.StackTrace}");

                // Щоб система не вбила додаток миттєво (дає час записати лог)
                args.Handled = true;
            };
#endif
            return builder.Build();
        }

#if __ANDROID__
        public static void PrependToMappingImageSource(IImageHandler handler, Microsoft.Maui.IImage image)
        {
            handler.PlatformView?.Clear();
        }
#endif
    }
}
