using Parky.Views;

namespace Parky;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(ParkListPage), typeof(ParkListPage));
        Routing.RegisterRoute(nameof(ParkRidesPage), typeof(ParkRidesPage));
        Routing.RegisterRoute(nameof(SingleRidePage), typeof(SingleRidePage));
    }
}
