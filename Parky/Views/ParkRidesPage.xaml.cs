using Microsoft.Maui.Graphics.Converters;
using Newtonsoft.Json;
using Parky.lib;
using System.Collections.ObjectModel;

namespace Parky.Views;

public partial class ParkRidesPage : ContentPage
{

    public List<Ride> rideList;
    public Park park;

    public ParkRidesPage(Park inPark)
    {
        InitializeComponent();
        park = new Park();
        park = inPark;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadPage();
    }


    public async void LoadPage()
    {

        rideList = new List<Ride>();
        this.Title = park.name;

        park = await GetWaitTimesAsync(park);

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
                        temp.wait_time_true = 1000000000;
                        ColorTypeConverter converter = new ColorTypeConverter();
                        Color color = (Color)(converter.ConvertFromInvariantString("black"));
                        temp.waitCol = color;
                    }
                    else
                    {
                        temp.wait_time_true = Convert.ToInt32(temp.wait_time);
                        if (Convert.ToInt32(temp.wait_time) <= 30)
                        {
                            ColorTypeConverter converter = new ColorTypeConverter();
                            Color color = (Color)(converter.ConvertFromInvariantString("green"));
                            temp.waitCol = color;
                        }
                        else if (Convert.ToInt32(temp.wait_time) < 60)
                        {
                            ColorTypeConverter converter = new ColorTypeConverter();
                            Color color = (Color)(converter.ConvertFromInvariantString("#CCB400"));
                            temp.waitCol = color;
                        }
                        else if (Convert.ToInt32(temp.wait_time) >= 60 && Convert.ToInt32(temp.wait_time) < 90)
                        {
                            ColorTypeConverter converter = new ColorTypeConverter();
                            Color color = (Color)(converter.ConvertFromInvariantString("red"));
                            temp.waitCol = color;
                        }
                        else if (Convert.ToInt32(temp.wait_time) >= 90)
                        {
                            ColorTypeConverter converter = new ColorTypeConverter();
                            Color color = (Color)(converter.ConvertFromInvariantString("#6A2E35"));
                            temp.waitCol = color;
                        }
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
                    ColorTypeConverter converter = new ColorTypeConverter();
                    Color color = (Color)(converter.ConvertFromInvariantString("black"));
                    temp.waitCol = color;
                    temp.wait_time = "Closed";
                    temp.wait_time_true = 1000000000;
                }
                else
                {
                    temp.wait_time_true = Convert.ToInt32(temp.wait_time);

                    if (Convert.ToInt32(temp.wait_time) <= 30)
                    {
                        ColorTypeConverter converter = new ColorTypeConverter();
                        Color color = (Color)(converter.ConvertFromInvariantString("green"));
                        temp.waitCol = color;
                    }
                    else if (Convert.ToInt32(temp.wait_time) < 60)
                    {
                        ColorTypeConverter converter = new ColorTypeConverter();
                        Color color = (Color)(converter.ConvertFromInvariantString("#CCB400"));
                        temp.waitCol = color;
                    }
                    else if (Convert.ToInt32(temp.wait_time) >= 60 && Convert.ToInt32(temp.wait_time) < 90)
                    {
                        ColorTypeConverter converter = new ColorTypeConverter();
                        Color color = (Color)(converter.ConvertFromInvariantString("red"));
                        temp.waitCol = color;
                    }
                    else if (Convert.ToInt32(temp.wait_time) >= 90)
                    {
                        ColorTypeConverter converter = new ColorTypeConverter();
                        Color color = (Color)(converter.ConvertFromInvariantString("#6A2E35"));
                        temp.waitCol = color;
                    }
                }
                rideList.Add(temp);
            }
        }

        rideList = rideList.OrderBy(o => o.name).ToList();

        if (rideList.Count > 0)
        {
            listRides.ItemsSource = rideList;
        }
        else
        {
            await DisplayAlert("Error", park.name + " is not currently reporting wait times.", "OK");

            var parkListPage = new ParkListPage();
            _ = Navigation.PushAsync(parkListPage);
        }

    }

    private void OnAZClicked(object sender, EventArgs e)
    {
        var newList = new ObservableCollection<Ride>(rideList.OrderBy(o => o.name).ToList());

        listRides.ItemsSource = newList;

    }
    private void OnProxClicked(object sender, EventArgs e)
    {
        var newList = new ObservableCollection<Ride>();

        listRides.ItemsSource = newList;
    }
    private void OnCompClicked(object sender, EventArgs e)
    {
        var newList = new ObservableCollection<Ride>(rideList.OrderBy(o => o.inLand).ToList());

        listRides.ItemsSource = newList;
    }
    private void OnWaitClicked(object sender, EventArgs e)
    {
        var newList = new ObservableCollection<Ride>(rideList.OrderBy(o => o.wait_time_true).ToList());

        listRides.ItemsSource = newList;
    }

    private void listRides_ItemTapped(object sender, EventArgs e)
    {

        Ride temp = (Ride)listRides.SelectedItem;

        var rideDetailsPage = new SingleRidePage(temp);
        SearchBar.Text = string.Empty;
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

    async Task<Park> GetWaitTimesAsync(Park park)
    {

        try
        {
            HttpClient client = new HttpClient();
            var url = "https://queue-times.com/en-US/parks/" + park.id + "/queue_times.json";
            using HttpResponseMessage response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();


            //result = JsonConvert.DeserializeObject<Company>(responseText, new JsonSerializerSettings { TraceWriter = new ConsoleTraceWriter() });
            Park park2 = JsonConvert.DeserializeObject<Park>(responseBody);

            park.lands = park2.lands;
            park.ridesNoLands = park2.ridesNoLands;
            
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }

        return park;
    }

    private void RefreshView_Refreshing(object sender, EventArgs e)
    {
         LoadPage();
        refreshParks.IsRefreshing = false;
    }
}