using PetInsulinLogs.Services;

namespace PetInsulinLogs.Views;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage()
    {
        InitializeComponent();
    }

    private async void OnAddFirstPetClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("pets/profile");
    }

    private async void OnJoinWithCodeClicked(object sender, EventArgs e)
    {
        // TODO: Navigate to join by share code page
        await DisplayAlert("Join with Code", "Share code functionality coming soon!", "OK");
    }
}