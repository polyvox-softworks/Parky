using Newtonsoft.Json;
using System.Net;

using TPC.lib;

namespace TPC.Views;

public partial class ParkRidesPage : ContentPage
{

    public List<Ride> rideList;
    //public List<Grouped_List> new_list { get; set; } = new List<Grouped_List>();

    public ParkRidesPage(Park park)
	{
		InitializeComponent();
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
                    temp.wait_time=  "Closed";
                }
                else
                {
                    temp.wait_time = temp.wait_time + " minutes";
                }
                rideList.Add(temp);
            }
        }

        /*var dict = rideList.GroupBy(o => o.inLand)
                   .ToDictionary(g => g.Key, g => g.ToList());



        foreach (KeyValuePair<string, List<Ride>> item in dict)
        {
            //new_list.Add(new Grouped_List(item.Key, new List<Ride>(item.Value)));
        }*/

        //listRides.ItemsSource = new_list;
        listRides.ItemsSource = rideList;
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