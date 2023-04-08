using Newtonsoft.Json;
using Parky.lib;
using System.Collections.ObjectModel;
using System.Formats.Tar;
using System.Net;
using System.Text.RegularExpressions;

namespace Parky.Views;

public partial class ParkListPage : ContentPage
{

    ObservableCollection<Park> parkList = new ObservableCollection<Park>();

    public ParkListPage()
    {
        InitializeComponent();

        List<Company> companyList = new List<Company>();
        companyList = GetParkList();

        parkList = createParkList(companyList);

        listParks.ItemsSource = parkList;
    }
    private ObservableCollection<Park> createParkList(List<Company> companyList)
    {
        List<Park> parkList2 = new List<Park>();

        for (int i = 0; i < companyList.Count; i++)
        {
            for (int y = 0; y < companyList[i].parks.Count; y++)
            {
                Park park = new Park();


                park.name = companyList[i].parks[y].name;
                park.id = companyList[i].parks[y].id;
                park.country = companyList[i].parks[y].country;
                park.continent = companyList[i].parks[y].continent;
                park.longitude = companyList[i].parks[y].longitude;
                park.latitude = companyList[i].parks[y].latitude;
                park.location = new Point(Convert.ToDouble(park.latitude), Convert.ToDouble(park.longitude));
                park.timezone = companyList[i].parks[y].timezone;
                park.company = companyList[i].name;

                //park.schedule = getParkSchedule(park.name);

                parkList2.Add(park);

            }
        }
        parkList2 = parkList2.OrderBy(o => o.name).ToList();
        ObservableCollection<Park> result = new ObservableCollection<Park>(parkList2);
        return result;

    }

    private Day getParkSchedule(string parkName)
    {
        Day outDay = new Day();

        try
        {
            var url = "https://api.themeparks.wiki/v1/entity/" + parkName + "/schedule";
            url = Regex.Replace(url, @" ", "");
            url = Regex.Replace(url, @"'", "");
            url = url.ToLower();

            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());

            string responseText = reader.ReadToEnd();

            string today = DateTime.Now.ToString("yyyy-MM-dd");

            if (responseText.Contains(today))
            {
                string data = getBetween(responseText, today, "openingTime");
                data = "{\"date\": " + today + data + responseText.Substring(responseText.IndexOf("openingTime"), 42) + "}";


                Day outDay2 = JsonConvert.DeserializeObject<Day>(data);
            }
            
        }
        catch (Exception)
        {

        }

        return outDay;
    }

    public static string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }

    private void OnAZClicked(object sender, EventArgs e) 
    {
        var newList = new ObservableCollection<Park>(parkList.OrderBy(o => o.name).ToList());

        listParks.ItemsSource = newList;
        
    }
    private async void OnProxClicked(object sender, EventArgs e)
    {
        var newList = new ObservableCollection<Park>(parkList);
        Point currentLocation = await GetLocation();

        for (int i = 0; i < parkList.Count; i++)
        {


            parkList[i].distanceFromCurrentLocation = (Math.Pow((parkList[i].location.X - currentLocation.X), 2) +
                Math.Pow((parkList[i].location.Y - currentLocation.Y), 2));
        }
        newList = new ObservableCollection<Park>(parkList.OrderBy(o => o.distanceFromCurrentLocation).ToList());
        listParks.ItemsSource = newList;
    }

    private void OnCompClicked(object sender, EventArgs e)
    {
        var newList = new ObservableCollection<Park>(parkList.OrderBy(o => o.company).ToList());

        listParks.ItemsSource = newList;
    }

    private async void listParks_ItemTapped(object sender, EventArgs e)
    {
        if(listParks.SelectedItem != null)
        {
            Park temp = (Park)listParks.SelectedItem;

            var parkDetailsPage = new ParkRidesPage(temp);
            SearchBar.Text = string.Empty;

            await Navigation.PushAsync(parkDetailsPage);
        }
        listParks.SelectedItem = null;
        //await Shell.Current.GoToAsync($"temp", parkDetailsPage);
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        ObservableCollection<Park> parkList2 = new ObservableCollection<Park>();
        parkList2 = searchPark(parkList, ((SearchBar)sender).Text);

        var searchList = new ObservableCollection<Park>(parkList2);
        listParks.ItemsSource = parkList2;
    }

    private ObservableCollection<Park> searchPark(ObservableCollection<Park> inParkList, string inString)
    {
        List<Park> parkList2 = new List<Park>();
        for (int i = 0; i < inParkList.Count; i++)
        {
            if (inParkList[i].name.ToLower().Contains(inString.ToLower()) ||
                inParkList[i].company.ToLower().Contains(inString.ToLower()))
            {
                parkList2.Add(inParkList[i]);
            }
        }
        ObservableCollection<Park> result = new ObservableCollection<Park>(parkList2);
        return result;
    }

    private List<Company> GetParkList()
    {

        var url = "https://raw.githubusercontent.com/polyvox-softworks/Parky/main/Parky/json/parks.json";

        WebRequest request = WebRequest.Create(url);

        WebResponse response = request.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());

        string responseText = reader.ReadToEnd();
        List<Company> result = new List<Company>();

        //result = JsonConvert.DeserializeObject<Company>(responseText, new JsonSerializerSettings { TraceWriter = new ConsoleTraceWriter() });
        result = JsonConvert.DeserializeObject<List<Company>>(responseText);

        return result;
    }

    public async Task<Point> GetLocation()
    {
        var location = await Geolocation.GetLocationAsync();
        Point currentLocation = new Point(location.Latitude, location.Longitude);
        return currentLocation;
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {

        var addParksPage = new AddParkPage();
        SearchBar.Text = string.Empty;
        _ = Navigation.PushAsync(addParksPage);

        listParks.SelectedItem = null;
    }
}