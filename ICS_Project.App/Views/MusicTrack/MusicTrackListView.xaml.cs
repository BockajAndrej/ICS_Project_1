using ICS_Project.App.ViewModels.MusicTrack;
using ICS_Project.BL.Models;

namespace ICS_Project.App.Views.MusicTrack;

public partial class MusicTrackListView : ContentView
{
    private IDispatcherTimer _animationTimer;
    private double _baseAnimationAngle = 0;
    private const double AnimationSpeed = 0.03;
    public MusicTrackListView(MusicTrackListViewModel playlistViewModel)
    {
        InitializeComponent();
        BindingContext = playlistViewModel;

        this.Loaded += PlaylistDetailView_Loaded;
        this.Unloaded += PlaylistDetailView_Unloaded;
    }

    private void PlaylistDetailView_Loaded(object sender, EventArgs e)
    {
        _animationTimer = this.Dispatcher.CreateTimer();
        _animationTimer.Interval = TimeSpan.FromMilliseconds(50);
        _animationTimer.Tick += AnimationTimer_Tick;
        _animationTimer.Start();
        UpdateBrushPositions(); 
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

    private void UpdateBrushPositions()
    {
        double t = (Math.Sin(_baseAnimationAngle) + 1) / 2.0;
        Point startPoint = new Point(t - 0.5, 0);
        Point endPoint = new Point(t + 0.5, 1);

        if (AnimatedDetailBrush1 is LinearGradientBrush lgb1)
        {
            lgb1.StartPoint = startPoint;
            lgb1.EndPoint = endPoint;
        }
        if (AnimatedDetailBrush2 is LinearGradientBrush lgb2)
        {
            lgb2.StartPoint = startPoint;
            lgb2.EndPoint = endPoint;
        }
    }
}