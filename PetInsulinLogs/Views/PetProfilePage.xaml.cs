using PetInsulinLogs.ViewModels;
using PetInsulinLogs.Services;

namespace PetInsulinLogs.Views;

public partial class PetProfilePage : ContentPage
{
    public PetProfilePage()
    {
        InitializeComponent();
        BindingContext = Services.ServiceHelper.Get<PetProfileViewModel>();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (BindingContext is PetProfileViewModel vm)
        {
            try
            {
                await vm.SaveAsync();
                await DisplayAlert("Success", "Pet information saved successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
            }
        }
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (BindingContext is PetProfileViewModel vm)
        {
            var code = await vm.GenerateShareCodeAsync();
            await DisplayAlert("Share Code", code, "OK");
        }
    }
}
