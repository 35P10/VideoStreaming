using Core.App.Services;

namespace Pre.MultiPlatform
{
    public partial class MainPage : ContentPage
    {
        private ICloudStorageService _cloudStorageService;

        public MainPage()
        {
            _cloudStorageService = Application.Current.MainPage
                        .Handler
                        .MauiContext
                        .Services  // IServiceProvider
                        .GetService<ICloudStorageService>();
            InitializeComponent();
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
    }
}