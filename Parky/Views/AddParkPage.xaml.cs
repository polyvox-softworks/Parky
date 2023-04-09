using Newtonsoft.Json;
using Parky.lib;
using System.Collections.ObjectModel;
using System.Net;

namespace Parky.Views;

public partial class AddParkPage : ContentPage
{
    private ObservableCollection<Park> userParkList;
    private ObservableCollection<Park> allParkList;
    public AddParkPage()
	{
        
		InitializeComponent();
        OnLoad();

    }
    public AddParkPage(ObservableCollection<Park> inParkList)
    {
        InitializeComponent();
        userParkList = inParkList;
        OnLoad();
    }

    public void OnLoad()
    {
        List<Company> companyList = new List<Company>();
        companyList = GetParkList();

        allParkList = createParkList(companyList);

        listParks.ItemsSource = allParkList;

    }

    private void OnSaveandClose(object sender, EventArgs e)
    {
        List<Park> newList = new List<Park>();
        var inList = listParks.SelectedItems.ToList();
        foreach ( var item in inList )
        {
            userParkList.Add((Park)item);
        }

        var parkListPage = new ParkListPage(userParkList);
        //SearchBar.Text = string.Empty;
        _ = Navigation.PushAsync(parkListPage);
        
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        ObservableCollection<Park> parkList2 = new ObservableCollection<Park>();
        parkList2 = searchPark(allParkList, ((SearchBar)sender).Text);

        var searchList = new ObservableCollection<Park>(parkList2);
        listParks.ItemsSource = parkList2;
    }
    private ObservableCollection<Park> searchPark(ObservableCollection<Park> inParkList, string inString)
    {
       
        List<Park> parkList2 = new List<Park>();
        try
        {
            for (int i = 0; i < inParkList.Count; i++)
            {
                if (inParkList[i].name.ToLower().Contains(inString.ToLower()) ||
                    inParkList[i].company.ToLower().Contains(inString.ToLower()))
                {
                    parkList2.Add(inParkList[i]);
                }
            }
        }catch (Exception) { }
        ObservableCollection<Park> result = new ObservableCollection<Park>(parkList2);
        return result;
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
                park.img = companyList[i].parks[y].img;
                park.company = companyList[i].name;

                //park.schedule = getParkSchedule(park.name);

                if (userParkList.Any(x => x.name == park.name) == false)
                {
                    parkList2.Add(park);
                }

            }
        }
        parkList2 = parkList2.OrderBy(o => o.name).ToList();
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
}