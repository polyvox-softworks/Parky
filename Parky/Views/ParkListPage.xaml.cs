using Mopups.Services;
using Newtonsoft.Json;
using Parky.lib;
using System.Collections.ObjectModel;
using System.Formats.Tar;
using System.Net;
using System.Text.RegularExpressions;

namespace Parky.Views;

public partial class ParkListPage : ContentPage
{

    ObservableCollection<Park> parkList;
    bool deleteMode = false;

    public ParkListPage()
    {
        InitializeComponent();

        var loadedParkList = OpenParkList();
        parkList = new ObservableCollection<Park>(loadedParkList);

        if (parkList.Count == 0)
        {
            DeleteModeLabel.IsVisible = false;
            DeleteSwitch.IsVisible = false;
            getStartedLabel.IsVisible = true;
        }
        else
        {
            DeleteModeLabel.IsVisible = true;
            DeleteSwitch.IsVisible = true;
            getStartedLabel.IsVisible = false;
        }
        listParks.ItemsSource = parkList;
    }

    public ParkListPage(ObservableCollection<Park> inList)
    {
        InitializeComponent();
        
 
        parkList = inList;
        SaveParkList();

        if (parkList.Count == 0)
        {
            DeleteModeLabel.IsVisible = false;
            DeleteSwitch.IsVisible = false;
            getStartedLabel.IsVisible = true;
        }
        else
        {
            DeleteModeLabel.IsVisible = true;
            DeleteSwitch.IsVisible = true;
            getStartedLabel.IsVisible = false;
        }
        listParks.ItemsSource = parkList;
    }


    /*private Day getParkSchedule(string parkName)
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
    }*/

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

    protected override bool OnBackButtonPressed()
    {
        return true;
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
        if (deleteMode == true)
        {
            MauiProgram.parkList.Remove((Park)listParks.SelectedItem);
            parkList.Remove((Park)listParks.SelectedItem);
            listParks.ItemsSource = parkList;
            listParks.SelectedItem = null;
            SaveParkList();
            if (parkList.Count == 0)
            {
                getStartedLabel.IsVisible = true;
            }
        }
        else
        {
            if (listParks.SelectedItem != null)
            {
                Park temp = (Park)listParks.SelectedItem;

                var parkDetailsPage = new ParkRidesPage(temp);
                SearchBar.Text = string.Empty;

                await Navigation.PushAsync(parkDetailsPage);
            }
            listParks.SelectedItem = null;
        }

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

    public async Task<Point> GetLocation()
    {
        var location = await Geolocation.GetLocationAsync();
        Point currentLocation = new Point(location.Latitude, location.Longitude);
        return currentLocation;
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {

        var addParksPage = new AddParkPage(parkList);
        SearchBar.Text = string.Empty;

        _ = Navigation.PushAsync(addParksPage);

 
        listParks.SelectedItem = null;
    }

    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        if (DeleteSwitch.IsToggled)
        {
            listParks.SelectedItem = null;

            deleteMode = true;
        }
        else
        {
            listParks.SelectedItem = null;

            deleteMode = false;
        }
        
    }

    private void SaveParkList()
    {
        var path = FileSystem.Current.AppDataDirectory;
        var fullPath = Path.Combine(path, "userParkList.json");

        TextWriter writer = null;
        try
        {
            var contentsToWriteToFile = JsonConvert.SerializeObject(parkList);
            writer = new StreamWriter(fullPath);
            writer.Write(contentsToWriteToFile);
        }
        finally
        {
            if (writer != null)
                writer.Close();
        }
    }

    private List<Park> OpenParkList()
    {
        var path = FileSystem.Current.AppDataDirectory;
        var fullPath = Path.Combine(path, "userParkList.json");
        TextReader reader = null;

        try
        {
            reader = new StreamReader(fullPath);
            var fileContents = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Park>>(fileContents);
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }
    }

}