using Newtonsoft.Json;
using System.Net;
using TPC.lib;

namespace TPC.Views;

public partial class ParkListPage : ContentPage
{
    List<Company> companyList;
    List<Park> parkList = new List<Park>();

    public ParkListPage()
    {
        InitializeComponent();

        companyList = new List<Company>();
        companyList = GetParkList();



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
                park.timezone = companyList[i].parks[y].timezone;
                //park.lands = GetWaitTimesFromID(park.id);
                parkList.Add(park);
            }
        }


        parkList = parkList.OrderBy(o => o.name).ToList();


        listParks.ItemsSource = parkList;

    }

    private async void listParks_ItemTapped(object sender, EventArgs e)
	{

        Park temp = (Park)listParks.SelectedItem;

        var parkDetailsPage = new ParkRidesPage(temp);

        await Navigation.PushAsync(parkDetailsPage);
    }

    private List<Company> GetParkList()
    {

        var url = "https://queue-times.com/en-US/parks.json";

        WebRequest request = WebRequest.Create(url);

        WebResponse response = request.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());

        string responseText = reader.ReadToEnd();
        List<Company> result = new List<Company>();

        //result = JsonConvert.DeserializeObject<Company>(responseText, new JsonSerializerSettings { TraceWriter = new ConsoleTraceWriter() });
        result = JsonConvert.DeserializeObject<List<Company>>(responseText);

        return result;
    }

    private void listParks_ItemTapped_1(object sender, ItemTappedEventArgs e)
    {

    }
}