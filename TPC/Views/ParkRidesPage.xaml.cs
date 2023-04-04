using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using TPC.lib;

namespace TPC.Views;

public partial class ParkRidesPage : ContentPage
{

    public List<Ride> rideList;
    public Park park;
    //public List<Grouped_List> new_list { get; set; } = new List<Grouped_List>();

    public ParkRidesPage(Park inPark)
	{
		InitializeComponent();
        park = new Park();
        park = inPark;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        rideList = new List<Ride>();


        park = GetWaitTimes(park);

        if (park.lands.Count != 0)
        {
            for (int i = 0; i < park.lands.Count; i++)
            {
                for (int y = 0; y < park.lands[i].rides.Count; y++)
                {
                    Ride temp = park.lands[i].rides[y];
                    temp.inLand = park.lands[i].name;

                    if (!temp.is_open)
                    {
                        temp.wait_time = "Closed";
                    }
                    else
                    {
                        temp.wait_time = temp.wait_time + " minutes";
                    }
                    rideList.Add(temp);
                }

            }
        }
        else
        {
            for (int y = 0; y < park.ridesNoLands.Count; y++)
            {
                Ride temp = park.ridesNoLands[y];

                temp.inLand = "Rides";

                if (!temp.is_open)
                {
                    temp.wait_time = "Closed";
                }
                else
                {
                    temp.wait_time = temp.wait_time + " minutes";
                }
                rideList.Add(temp);
            }
        }
        listRides.ItemsSource = rideList;
    }


    private void listRides_ItemTapped(object sender, EventArgs e)
    {

        Ride temp = (Ride)listRides.SelectedItem;

        var rideDetailsPage = new SingleRidePage(temp);
        RideSearchBar.Text = string.Empty;
        _ = Navigation.PushAsync(rideDetailsPage);

    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        List<Ride> rideList2 = new List<Ride>();
        rideList2 = searchRide(rideList, ((SearchBar)sender).Text);

        var searchList = new ObservableCollection<Ride>(rideList2);
        listRides.ItemsSource = rideList2;
    }

    private List<Ride> searchRide(List<Ride> inRideList, string inString)
    {
        List<Ride> rideList2 = new List<Ride>();
        for (int i = 0; i < inRideList.Count; i++)
        {
            if (inRideList[i].name.ToLower().Contains(inString.ToLower()))
            {
                rideList2.Add(inRideList[i]);
            }
        }
        return rideList2;
    }

    Park GetWaitTimes(Park park)
    {
        var url = "https://queue-times.com/en-US/parks/" + park.id + "/queue_times.json";

        WebRequest request = WebRequest.Create(url);

        WebResponse response = request.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());

        string responseText = reader.ReadToEnd();


        Park park2 = JsonConvert.DeserializeObject<Park>(responseText);
        park.lands = park2.lands;
        park.ridesNoLands = park2.ridesNoLands;
        return park;
    }
}