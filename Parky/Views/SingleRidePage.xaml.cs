using Microsoft.Maui.Graphics.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parky.lib;
using System.Text.RegularExpressions;

namespace Parky.Views;

[QueryProperty(nameof(ride), "ride")]
public partial class SingleRidePage : ContentPage
{
    public int rideCountNum;
    public Ride ride { get; set; }
    public Park inPark { get; set; }
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

    public SingleRidePage(Ride ride, Park park)
    {
        InitializeComponent();
        this.ride = ride;
        inPark = park;
        OnLoad();
    }

    public void OnLoad()
    {
        this.Title = ride.name;
        try
        {
            rideCountNum = OpenRideCount();
            rideCount.Text = rideCountNum.ToString();
        }
        catch (Exception)
        {

        }
        
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

    private void Button_Clicked_Add(object sender, EventArgs e)
    {
        int count = Convert.ToInt32(rideCount.Text);
        count++;
        rideCount.Text = count.ToString();
        rideCountNum = count;
        SaveCount();
    }

    private void Button_Clicked_Sub(object sender, EventArgs e)
    {
        int count = Convert.ToInt32(rideCount.Text);
        if(count > 0)
        {
            count--;
        }
        rideCountNum = count;
        rideCount.Text = count.ToString();
        SaveCount();
    }
    private void SaveCount()
    {
        var path = FileSystem.Current.AppDataDirectory;
        string fullPath = Path.Combine(path, inPark.name + "rideCount" + ride.name + ".txt");
        fullPath = Regex.Replace(fullPath, "[']", "", RegexOptions.Compiled);
        fullPath = fullPath.Replace(" ", String.Empty);

        TextWriter writer = null;
        try
        {
            FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            var contentsToWriteToFile = rideCountNum;
            writer = new StreamWriter(fullPath);
            writer.Write(contentsToWriteToFile);
        }
        catch (Exception)
        {

        }
        finally
        {
            if (writer != null)
                writer.Close();
        }
    }
    private int OpenRideCount()
    {
        var path = FileSystem.Current.AppDataDirectory;
        var fullPath = Path.Combine(path, inPark.name + "rideCount" + ride.name + ".txt");
        fullPath = Regex.Replace(fullPath, "[']", "", RegexOptions.Compiled);
        fullPath = fullPath.Replace(" ", String.Empty);
        TextReader reader = null;

        try
        {
            reader = new StreamReader(fullPath);
            var fileContents = reader.ReadToEnd();
            return Convert.ToInt32(fileContents);
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }
    }
}