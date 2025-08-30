using PetInsulinLogs.ViewModels;

namespace PetInsulinLogs.Views;

[QueryProperty(nameof(PetId), "petId")]
public partial class HistoryPage : ContentPage, IQueryAttributable
{
    private HistoryViewModel? viewModel;
    
    public string? PetId { get; set; }

    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (viewModel != null)
        {
            await viewModel.LoadPetsCommand.ExecuteAsync(null);
            
            // If a specific pet ID was provided, select that pet
            if (!string.IsNullOrEmpty(PetId))
            {
                await viewModel.SelectPetByIdAsync(PetId);
            }
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("petId"))
        {
            PetId = query["petId"].ToString();
        }
    }
}