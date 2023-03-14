using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Anemone.Algorithms.Models;
using Anemone.Core;
using Anemone.DataImport.Views;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Anemone.Algorithms.ViewModels;

public class LlcAlgorithmViewModel : ViewModelBase
{
    private IValidator<LlcAlgorithmParameters> Validator { get; }
    private IToastService ToastService { get; }
    private ILogger<LlcAlgorithmViewModel> Logger { get; }
    public LlcAlgorithmParameters Parameters { get; } = new();
    public ICommand CalculateCommand { get; }
    public HeatingRepositoryListViewModel HeatingRepositoryListViewModel { get; set; }
    
    private CancellationTokenSource? _cancellationToken;
    private bool _calculationInProgress;

    public bool CalculationInProgress
    {
        get => _calculationInProgress;
        set
        {
            if(SetProperty(ref _calculationInProgress, value))
                RaisePropertyChanged(nameof(ButtonText));
        }
    }

    public string ButtonText => CalculationInProgress ? "Cancel" : "Calculate";
    
    // private bool CanCancel => _cancellationToken is not null;
    
    private bool CanCancel { get; set; } = true;
    public LlcAlgorithmViewModel(HeatingRepositoryListViewModel heatingRepositoryListViewModel,IValidator<LlcAlgorithmParameters> validator, IToastService toastService, ILogger<LlcAlgorithmViewModel> logger)
    {
        HeatingRepositoryListViewModel = heatingRepositoryListViewModel;
        Validator = validator;
        ToastService = toastService;
        Logger = logger;
        CalculateCommand = new ActionCommandAsync(ExecuteCalculateCommand);
    }
    

    private bool CanExecuteCalculateCommand()
    {
        var validationResult = Validator.Validate(Parameters);
        if (validationResult.IsValid)
            return true;

        var errorBuilder = new StringBuilder();
        foreach (var validationError in validationResult.Errors)
        {
            errorBuilder.Append(validationError.ErrorMessage);
        }
        
        ToastService.Show(errorBuilder.ToString());
        
        return false;
    }

    private async Task Calculate()
    {
        
    }

    private async Task ExecuteCalculateCommand()
    {
        if (CalculationInProgress)
        {
            _cancellationToken?.Cancel();
            CalculationInProgress = !CalculationInProgress;
            return;
        }
        
        if (CanExecuteCalculateCommand() is false)
        {
            Logger.LogDebug("can't start calculation");
            return;
        }

        Logger.LogDebug("starting calculation");
        CalculationInProgress = true;
        await Calculate();
    }
}