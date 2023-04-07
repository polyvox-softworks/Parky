using Parky.Views;

namespace Parky;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new NavigationPage(new ParkListPage());
    }
}
