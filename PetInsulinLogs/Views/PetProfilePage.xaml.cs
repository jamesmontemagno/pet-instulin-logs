using PetInsulinLogs.ViewModels;
using PetInsulinLogs.Services;

namespace PetInsulinLogs.Views;

[QueryProperty(nameof(PetId), "petId")]
public partial class PetProfilePage : ContentPage, IQueryAttributable
{
    private PetProfileViewModel? viewModel;
    
    public string? PetId { get; set; }

    public PetProfilePage()
    {
        InitializeComponent();
        viewModel = Services.ServiceHelper.Get<PetProfileViewModel>();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // If a specific pet ID was provided, load that pet
        if (!string.IsNullOrEmpty(PetId) && viewModel != null)
        {
            await viewModel.LoadPetByIdAsync(PetId);
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("petId"))
        {
            PetId = query["petId"].ToString();
        }
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
