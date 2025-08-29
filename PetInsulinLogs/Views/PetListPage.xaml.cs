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
            await Shell.Current.GoToAsync($"//pets/profile?petId={pet.PetId}");
        }
    }
}
