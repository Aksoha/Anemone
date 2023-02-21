using System.Collections.ObjectModel;
using Anemone.Core;
using Anemone.DataImport.Models;

namespace Anemone.DataImport.ViewModels;

public class GetDataViewModel : ViewModelBase
{
    private Sheet? _selectedSheet;
    private bool _headerVisible;
    public ObservableCollection<Sheet> Sheets { get; set; } = new();

    public Sheet? SelectedSheet
    {
        get => _selectedSheet;
        set => SetProperty(ref _selectedSheet, value);
    }

    public bool HeaderVisible
    {
        get => _headerVisible;
        set => SetProperty(ref _headerVisible, value);
    }
}