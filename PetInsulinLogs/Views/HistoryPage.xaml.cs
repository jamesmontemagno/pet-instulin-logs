using PetInsulinLogs.ViewModels;

namespace PetInsulinLogs.Views;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is HistoryViewModel viewModel)
        {
            await viewModel.LoadPetsCommand.ExecuteAsync(null);
        }
    }
}