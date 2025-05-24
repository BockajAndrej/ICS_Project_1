using ICS_Project.App.ViewModels.Playlist;
using Microsoft.Maui.Dispatching;
using System;
using Microsoft.Maui.Controls;

namespace ICS_Project.App.Views.Playlist
{
    public partial class PlaylistDetailView : ContentView
    {
        private IDispatcherTimer _animationTimer;
        private double _baseAnimationAngle = 0;

        // --- Animation Parameters ---
        // How far the center of the gradient will move from the center of its Border (0.0 to 0.5 means inside, >0.5 means center can go outside)
        // Increased from 0.4 to make the "circles" orbit further away. Adjust as needed.
        private const double AnimationSpeed = 0.02; // Slower speed for a more ambient effect

        public PlaylistDetailView(PlaylistDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;

            this.Loaded += PlaylistDetailView_Loaded;
            this.Unloaded += PlaylistDetailView_Unloaded;
        }

        private void PlaylistDetailView_Loaded(object sender, EventArgs e)
        {
            _animationTimer = this.Dispatcher.CreateTimer();
            _animationTimer.Interval = TimeSpan.FromMilliseconds(50); // ~20 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
            _animationTimer.Start();
            UpdateBrushPositions(); // Call once to set initial positions
        }

        private void PlaylistDetailView_Unloaded(object sender, EventArgs e)
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                _animationTimer.Tick -= AnimationTimer_Tick;
                _animationTimer = null;
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _baseAnimationAngle += AnimationSpeed;
            if (_baseAnimationAngle > Math.PI * 2)
            {
                _baseAnimationAngle -= Math.PI * 2;
            }
            UpdateBrushPositions();
        }

        private void UpdateBrushPositions() // Rename to UpdateGradientPoints or similar
        {
            // Animate 'x' for StartPoint and EndPoint to make it sweep
            // 't' goes from 0 to 1 and back, or cycles
            double t = (Math.Sin(_baseAnimationAngle) + 1) / 2.0; // Varies from 0 to 1

            // Example: Diagonal sweep from top-left to bottom-right then back
            // Or more complex: make it look like a light bar scanning across
            Point startPoint = new Point(t - 0.5, 0); // Sweep horizontally a band of 0.5 width
            Point endPoint = new Point(t + 0.5, 1);

            if (AnimatedDetailBrush1 is LinearGradientBrush lgb1)
            {
                lgb1.StartPoint = startPoint;
                lgb1.EndPoint = endPoint;
            }
            if (AnimatedDetailBrush2 is LinearGradientBrush lgb2)
            {
                lgb2.StartPoint = startPoint; // Or slightly offset for a wave
                lgb2.EndPoint = endPoint;
            }
            if (AnimatedDetailBrush3 is LinearGradientBrush lgb3)
            {
                lgb3.StartPoint = startPoint; // Or slightly offset
                lgb3.EndPoint = endPoint;
            }
        }
    }
}