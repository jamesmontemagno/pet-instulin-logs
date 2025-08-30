using PetInsulinLogs.Services;
using PetInsulinLogs.ViewModels;

namespace PetInsulinLogs.Views;

public partial class DashboardPage : ContentPage
{
    private DashboardViewModel? viewModel;

    public DashboardPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        viewModel = ServiceHelper.Get<DashboardViewModel>();
        BindingContext = viewModel;
        await viewModel.LoadAsync();
    }
}