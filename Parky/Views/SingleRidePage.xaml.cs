using Microsoft.Maui.Graphics.Converters;
using Newtonsoft.Json.Linq;
using Parky.lib;

namespace Parky.Views;

[QueryProperty(nameof(ride), "ride")]
public partial class SingleRidePage : ContentPage
{
    public Ride ride { get; set; }
    public SingleRidePage()
    {
        InitializeComponent();
        ride = new Ride();
        OnLoad();
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    public SingleRidePage(Ride ride)
    {
        InitializeComponent();
        this.ride = ride;
        OnLoad();
    }

    public void OnLoad()
    {
        this.Title = ride.name;

        haveYouLabel.Text = "test";

        if (!ride.is_open)
        {
            ColorTypeConverter converter = new ColorTypeConverter();
            Color color = (Color)(converter.ConvertFromInvariantString("black"));
            ride.waitCol = color;
            ride.wait_time = "Closed";
            ride.wait_time_true = 1000000000;

            rideWait.Text = "\n" + ride.wait_time;
            
        }
        else
        {
            ride.wait_time_true = Convert.ToInt32(ride.wait_time);

            if (Convert.ToInt32(ride.wait_time) <= 30)
            {
                ColorTypeConverter converter = new ColorTypeConverter();
                Color color = (Color)(converter.ConvertFromInvariantString("green"));
                ride.waitCol = color;
            }
            else if (Convert.ToInt32(ride.wait_time) < 60)
            {
                ColorTypeConverter converter = new ColorTypeConverter();
                Color color = (Color)(converter.ConvertFromInvariantString("#CCB400"));
                ride.waitCol = color;
            }
            else if (Convert.ToInt32(ride.wait_time) >= 60 && Convert.ToInt32(ride.wait_time) < 90)
            {
                ColorTypeConverter converter = new ColorTypeConverter();
                Color color = (Color)(converter.ConvertFromInvariantString("red"));
                ride.waitCol = color;
            }
            else if (Convert.ToInt32(ride.wait_time) >= 90)
            {
                ColorTypeConverter converter = new ColorTypeConverter();
                Color color = (Color)(converter.ConvertFromInvariantString("#6A2E35"));
                ride.waitCol = color;
            }

            rideWait.Text = "\n" + ride.wait_time + " minutes.";
        }
        rideWait.TextColor = ride.waitCol;
    }
}