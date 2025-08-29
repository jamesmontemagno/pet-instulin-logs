using PetInsulinLogs.Services;
using PetInsulinLogs.ViewModels;

namespace PetInsulinLogs.Views;

public partial class PetListPage : ContentPage
{
    private PetListViewModel? viewModel;

    public PetListPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        viewModel = ServiceHelper.Get<PetListViewModel>();
        BindingContext = viewModel;
        await viewModel.LoadAsync();
        
        // Navigate to onboarding if no pets
        if (viewModel.IsEmpty)
        {
            await Shell.Current.GoToAsync("//onboarding");
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//pets/profile");
    }

    private async void OnPetTapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border && border.BindingContext is Models.Pet pet)
        {
            // Show action sheet with pet-specific options
            var action = await DisplayActionSheet(
                $"Actions for {pet.Name}", 
                "Cancel", 
                null, 
                "Log Insulin Shot", 
                "View History", 
                "Vacation Mode", 
                "Edit Profile");

            switch (action)
            {
                case "Log Insulin Shot":
                    await Shell.Current.GoToAsync($"//logshot?petId={pet.PetId}");
                    break;
                case "View History":
                    await Shell.Current.GoToAsync($"//history?petId={pet.PetId}");
                    break;
                case "Vacation Mode":
                    await Shell.Current.GoToAsync($"//vacation?petId={pet.PetId}");
                    break;
                case "Edit Profile":
                    await Shell.Current.GoToAsync($"//pets/profile?petId={pet.PetId}");
                    break;
            }
        }
    }
}
