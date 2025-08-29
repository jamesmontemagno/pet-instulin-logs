using PetInsulinLogs.ViewModels;

namespace PetInsulinLogs.Views;

public partial class VacationModePage : ContentPage
{
    public VacationModePage(VacationModeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is VacationModeViewModel viewModel)
        {
            await viewModel.LoadPetsCommand.ExecuteAsync(null);
        }
    }
}