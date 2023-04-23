using Microsoft.AspNetCore.Components.WebView.Maui;
using DLMSmaui.Data;
using DLMSmaui.Services;

namespace DLMSmaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		builder.Services.AddSingleton<IAppService, AppService>();
		builder.Services.AddSingleton<WeatherForecastService>();

		return builder.Build();
	}
}
