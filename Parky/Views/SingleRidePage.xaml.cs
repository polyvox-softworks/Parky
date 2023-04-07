using Parky.lib;

namespace Parky.Views;

[QueryProperty(nameof(ride), "ride")]
public partial class SingleRidePage : ContentPage
{
    public Ride ride { get; set; }
    public SingleRidePage()
    {
        InitializeComponent();

        this.Title = ride.name;
        ride_Name.Text = ride.name;
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    public SingleRidePage(Ride ride)
    {
        InitializeComponent();

        this.Title = ride.name;
        ride_Name.Text = ride.name;
    }
}