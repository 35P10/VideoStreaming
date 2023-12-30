namespace Pre.MultiPlatform;

using Core.App.Services;
using Microsoft.Maui.Controls;

public partial class VideoPlaybackPage : ContentPage
{
    private ICloudStorageService _cloudStorageService;

    public VideoPlaybackPage(string bucket, string videoFilePath)
    {
        _cloudStorageService = Application.Current.MainPage
                        .Handler
                        .MauiContext
                        .Services  // IServiceProvider
                        .GetService<ICloudStorageService>();

        InitializeComponent();

        mediaElement.Source = new Uri(_cloudStorageService.GetUrlResource(bucket,videoFilePath));
    }
}