namespace PetInsulinLogs;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("pets/profile", typeof(Views.PetProfilePage));
		Routing.RegisterRoute("onboarding", typeof(Views.OnboardingPage));
	}
}
