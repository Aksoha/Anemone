using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Anemone.Core;
using Anemone.DataImport.Models;
using Anemone.DataImport.Services;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;

namespace Anemone.DataImport.ViewModels;

internal class DataImportViewModel : ViewModelBase
{
    private const int SlideCount = 2;

    private int _currentIndex;


    public DataImportViewModel(DropFileViewModel dropFileViewModel,
        MapColumnsViewModel mapColumnsViewModel, SaveDataViewModel saveDataViewModel, ISheetFileReader sheetFileReader,
        IProcess process)
    {
        DropFileViewModel = dropFileViewModel;
        MapColumnsViewModel = mapColumnsViewModel;
        SaveDataViewModel = saveDataViewModel;
        SheetFileReader = sheetFileReader;
        Process = process;


        FrequencyData = SaveDataViewModel.FrequencyData;
        TemperatureData = SaveDataViewModel.TemperatureData;


        DropFileViewModel.PropertyChanged += OnDropFileViewModelOnPropertyChanged;
        MapColumnsViewModel.DataChanged += MapColumnsViewModelOnDataChanged;


        NavigateNextSlideCommand =
            new DelegateCommand(ExecuteNavigateNextSlideCommand).ObservesCanExecute(() => CanNavigateNext);
        NavigatePreviousSlideCommand =
            new DelegateCommand(ExecuteNavigatePreviousSlideCommand).ObservesCanExecute(() => IsNotFirstSlide);
        OpenFolderCommand = new ActionCommand(ExecuteOpenFolderCommand);
        MouseDownCommand = new DelegateCommand<MouseButtonEventArgs>(ExecuteMouseDownCommand);
    }

    public DropFileViewModel DropFileViewModel { get; }
    public MapColumnsViewModel MapColumnsViewModel { get; }
    public SaveDataViewModel SaveDataViewModel { get; }
    private ISheetFileReader SheetFileReader { get; }
    private IProcess Process { get; }

    private ObservableCollection<Sheet> Sheets => MapColumnsViewModel.Sheets;
    public string? SelectedFile => DropFileViewModel.UploadedFile;
    public string? FileName => Path.GetFileName(SelectedFile);


    private ObservableCollection<HeatingSystemData> FrequencyData { get; }
    private ObservableCollection<HeatingSystemData> TemperatureData { get; }

    public ICommand NavigateNextSlideCommand { get; }
    public ICommand NavigatePreviousSlideCommand { get; }
    public ICommand OpenFolderCommand { get; }

    public DelegateCommand<MouseButtonEventArgs> MouseDownCommand { get; }

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (value == _currentIndex) return;
            _currentIndex = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsFirstSlide));
            RaisePropertyChanged(nameof(IsNotFirstSlide));
            RaisePropertyChanged(nameof(IsNotLastSlide));
            RaisePropertyChanged(nameof(CanNavigateNext));
        }
    }

    public bool CanNavigateNext
    {
        get
        {
            if (!IsFirstSlide) return IsNotLastSlide;
            return DropFileViewModel.UploadedFile is not null && IsNotLastSlide;
        }
    }

    public bool IsFirstSlide => CurrentIndex <= 0;
    public bool IsNotFirstSlide => !IsFirstSlide;
    public bool IsNotLastSlide => CurrentIndex < SlideCount;

    private void MapColumnsViewModelOnDataChanged(object? sender, HeatingDataEventArgs e)
    {
        FrequencyData.Clear();
        foreach (var data in e.FrequencyData) FrequencyData.Add(data);

        TemperatureData.Clear();
        foreach (var data in e.TemperatureData) TemperatureData.Add(data);
    }


    private void OnDropFileViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(DropFileViewModel.UploadedFile))
            return;

        if (DropFileViewModel.UploadedFile is null)
            return;

        if (CurrentIndex != 0)
            throw new UnreachableException("uploaded file should not be changed from page other then a starting one");


        Sheets.Clear();
        var result = SheetFileReader.ReadAsDataSet(DropFileViewModel.UploadedFile);

        foreach (DataTable table in result.Tables)
            Sheets.Add(new Sheet { Name = table.TableName, Set = table.AsDataView() });

        MapColumnsViewModel.SelectedSheet = MapColumnsViewModel.Sheets.FirstOrDefault();

        RaisePropertyChanged(nameof(CanNavigateNext));
        RaisePropertyChanged(nameof(SelectedFile));
        RaisePropertyChanged(nameof(FileName));
        ExecuteNavigateNextSlideCommand();
    }

    private void ExecuteNavigateNextSlideCommand()
    {
        CurrentIndex++;
    }

    private void ExecuteNavigatePreviousSlideCommand()
    {
        CurrentIndex--;
    }

    private void ExecuteOpenFolderCommand()
    {
        if (SelectedFile is null)
            throw new NullReferenceException(nameof(SelectedFile));

        var argument = $"/select, \"{SelectedFile}\"";

        Process.Start("explorer.exe", argument);
    }

    private void ExecuteMouseDownCommand(MouseButtonEventArgs e)
    {
        switch (e.ChangedButton)
        {
            case MouseButton.XButton1:
                if (NavigatePreviousSlideCommand.CanExecute(null))
                    NavigatePreviousSlideCommand.Execute(null);
                e.Handled = true;
                break;
            case MouseButton.XButton2:
                if (NavigateNextSlideCommand.CanExecute(null))
                    NavigateNextSlideCommand.Execute(null);
                e.Handled = true;
                break;
            case MouseButton.Left:
                break;
            case MouseButton.Middle:
                break;
            case MouseButton.Right:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}