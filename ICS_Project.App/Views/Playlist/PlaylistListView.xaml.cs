using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.ViewModels.Playlist;
using ICS_Project.App.Views.MusicTrack;
using System.Diagnostics;

namespace ICS_Project.App.Views.Playlist
{
    public partial class PlaylistListView : ContentView
    {
        private readonly IServiceProvider _serviceProvider;
        private IDispatcherTimer _animationTimer;
        private double _baseAnimationAngle = 0;

        // --- Animation Parameters ---
        private const double AnimationSpeed = 0.02;
        private const int AnimationIntervalMilliseconds = 50; // ~20 FPS

        public PlaylistListView(PlaylistListViewModel playlistListViewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            this.BindingContext = playlistListViewModel;

            this.Loaded += PlaylistListView_Loaded;
            this.Unloaded += PlaylistListView_Unloaded;
        }

        private void PlaylistListView_Loaded(object sender, EventArgs e)
        {
            if (_animationTimer == null)
            {
                _animationTimer = this.Dispatcher.CreateTimer();
                _animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationIntervalMilliseconds);
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            _animationTimer.Start();
            UpdateBrushPosition();
        }

        private void PlaylistListView_Unloaded(object sender, EventArgs e)
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _baseAnimationAngle += AnimationSpeed;
            if (_baseAnimationAngle > Math.PI * 2)
            {
                _baseAnimationAngle -= Math.PI * 2;
            }

            UpdateBrushPosition();
        }

        private void UpdateBrushPosition()
        {
            if (AnimatedListSweepBrush == null) return;

            double t = (Math.Sin(_baseAnimationAngle) + 1) / 2.0; // Varies from 0 to 1

            Point startPoint = new Point(t - 0.5, 0);
            Point endPoint = new Point(t + 0.5, 1);

            AnimatedListSweepBrush.StartPoint = startPoint;
            AnimatedListSweepBrush.EndPoint = endPoint;
        }

        private async void OnNewPlaylistClicked(object sender, EventArgs e)
        {
            var popup = _serviceProvider.GetRequiredService<PlaylistCreateNewPopup>();
            WeakReferenceMessenger.Default.Send(new PlaylistPopupContext { IsEditMode = false });

            Element parent = this;
            while (parent != null && !(parent is Page))
            {
                parent = parent.Parent;
            }

            Page currentPage = parent as Page;

            if (currentPage != null)
            {
                currentPage.ShowPopup(popup);
            }
            else
            {
                Debug.WriteLine("Error: Could not find the parent Page to display the popup.");
            }
        }


        private async void OnNewMusicTrackClicked(object sender, EventArgs e)
        {
            var popup = _serviceProvider.GetRequiredService<MusicTrackCreateNewPopup>();

            WeakReferenceMessenger.Default.Send(new MusicTrackPopupContext { IsEditMode = false });

            // --- Find the parent Page ---
            Element parent = this;
            while (parent != null && !(parent is Page))
            {
                parent = parent.Parent;
            }

            // Cast the found parent element to a Page
            Page currentPage = parent as Page;


            if (currentPage != null)
            {
                currentPage.ShowPopup(popup);
            }
            else
            {
                Debug.WriteLine("Error: Could not find the parent Page to display the popup.");
            }
        }
    }
}