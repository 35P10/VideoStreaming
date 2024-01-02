using Core.App.Models;
using Core.App.Services;
using Pre.MultiPlatform.Integration;
using System.Collections.ObjectModel;

namespace Pre.MultiPlatform
{
    public partial class MainPage : ContentPage
    {
        private VideoStreamingApiHandler _videoStreamingApiHandler;
        private ObservableCollection<VideoMetadata> _videoList;

        public MainPage()
        {
            _videoStreamingApiHandler = Application.Current.MainPage
                        .Handler
                        .MauiContext
                        .Services  // IServiceProvider
                        .GetService<VideoStreamingApiHandler>();
            InitializeComponent();
            _videoList = new ObservableCollection<VideoMetadata>();
            VideoListView.ItemsSource = _videoList;
            LoadVideoList();
        }

        private async void OnSelectFileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Videos,
                    PickerTitle = "Seleccionar Video"
                });

                if (fileResult != null)
                {
                    _videoList.Clear();
                    LoadVideoList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar o subir el archivo: {ex.Message}");
            }
        }

        private void LoadVideoList()
        {
            List<VideoMetadata> res = _videoStreamingApiHandler.GetAllVideos().Result;
            foreach (VideoMetadata resItem in res)
            {
                _videoList.Add(resItem);
            }
        }

        private async void OnVideoSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is string selectedVideo)
            {
                await Navigation.PushAsync(new VideoPlaybackPage("cloud-videos", selectedVideo));
            }
        }
    }
}