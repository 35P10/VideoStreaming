using Core.App.Services;
using System.Collections.ObjectModel;

namespace Pre.MultiPlatform
{
    public partial class MainPage : ContentPage
    {
        private ICloudStorageService _cloudStorageService;
        private ObservableCollection<string> _videoList;

        public MainPage()
        {
            _cloudStorageService = Application.Current.MainPage
                        .Handler
                        .MauiContext
                        .Services  // IServiceProvider
                        .GetService<ICloudStorageService>();
            InitializeComponent();
            _videoList = new ObservableCollection<string>();
            VideoListView.ItemsSource = _videoList;
            LoadVideoList();
        }

        private async void OnSelectFileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Utilizar la API de Essentials para seleccionar un archivo
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Videos,
                    PickerTitle = "Seleccionar Video"
                });

                if (fileResult != null)
                {
                    _cloudStorageService.UploadFile("cloud-videos", fileResult.FullPath, "video/mp4");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al seleccionar o subir el archivo: {ex.Message}");
            }
        }

        private void LoadVideoList()
        {
            var bucketName = "cloud-videos";
            var videos = _cloudStorageService.ListFiles(bucketName);
            foreach (var video in videos)
            {
                _videoList.Add(video);
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