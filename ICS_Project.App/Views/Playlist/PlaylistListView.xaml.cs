using System.Diagnostics;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using ICS_Project.App.Messages;
using ICS_Project.App.ViewModels.Playlist;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Dispatching; // Required for IDispatcherTimer
using Microsoft.Maui.Controls;    // Required for Point

namespace ICS_Project.App.Views.Playlist
{
    public partial class PlaylistListView : ContentView
    {
        private readonly IServiceProvider _serviceProvider;
        private IDispatcherTimer _animationTimer;
        private double _baseAnimationAngle = 0;

        // --- Animation Parameters ---
        // Ensure these match PlaylistDetailView for synchronization
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
            // Initialize and start the timer only if it's not already running
            if (_animationTimer == null)
            {
                _animationTimer = this.Dispatcher.CreateTimer();
                _animationTimer.Interval = TimeSpan.FromMilliseconds(AnimationIntervalMilliseconds);
                _animationTimer.Tick += AnimationTimer_Tick;
            }
            _animationTimer.Start();
            UpdateBrushPosition(); // Set initial position when loaded
        }

        private void PlaylistListView_Unloaded(object sender, EventArgs e)
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                // Optionally, you could remove the Tick handler if you expect this view to be re-loaded
                // and want to avoid multiple subscriptions, though typically MAUI handles this.
                // _animationTimer.Tick -= AnimationTimer_Tick;
                // If you expect the view to be destroyed and never re-used, you could set _animationTimer to null.
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
            // Ensure the brush is available (it should be after InitializeComponent)
            if (AnimatedListSweepBrush == null) return;

            // Same logic as in PlaylistDetailView to calculate 't'
            double t = (Math.Sin(_baseAnimationAngle) + 1) / 2.0; // Varies from 0 to 1

            // Same logic for StartPoint and EndPoint
            Point startPoint = new Point(t - 0.5, 0); // Sweep horizontally a band of 0.5 width from Y=0
            Point endPoint = new Point(t + 0.5, 1);   // to Y=1

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
    }
}