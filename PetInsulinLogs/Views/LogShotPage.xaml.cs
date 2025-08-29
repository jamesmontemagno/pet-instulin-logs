using PetInsulinLogs.ViewModels;

namespace PetInsulinLogs.Views;

public partial class LogShotPage : ContentPage
{
    public LogShotPage(LogShotViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is LogShotViewModel viewModel)
        {
            await viewModel.LoadPetsCommand.ExecuteAsync(null);
        }
    }
}