using TPC.Views;

namespace TPC;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new ParkListPage());
	}
}
