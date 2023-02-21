using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Anemone.Core;
using Anemone.DataImport.Models;
using ExcelDataReader;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;

namespace Anemone.DataImport.ViewModels;

public class DataImportViewModel : ViewModelBase
{
    public DropFileViewModel DropFileViewModel { get; set; }
    public GetDataViewModel GetDataViewModel { get; set; }

    public MapColumnsViewModel MapColumnsViewModel { get; set; }

    public ObservableCollection<Sheet> Sheets => GetDataViewModel.Sheets;
    public string? SelectedFile => DropFileViewModel.UploadedFile;
    public string? FileName => Path.GetFileName(SelectedFile);


    public ICommand NavigateFirstSlideCommand { get; set; }
    public ICommand NavigateNextSlideCommand { get; }
    public ICommand NavigatePreviousSlideCommand { get; }
    public ICommand OpenFolderCommand { get; }


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

    public const int SlideCount = 2;

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

    private int _currentIndex;


    public DataImportViewModel(IOpenFileDialog openFileDialog, IToastService toastService)
    {
        DropFileViewModel = new DropFileViewModel(openFileDialog, toastService);
        GetDataViewModel = new GetDataViewModel();
        MapColumnsViewModel = new MapColumnsViewModel();


        DropFileViewModel.PropertyChanged += OnDropFileViewModelOnPropertyChanged;
        GetDataViewModel.PropertyChanged += GetDataViewModelOnPropertyChanged;

        NavigateFirstSlideCommand = new ActionCommand(ExecuteNavigateFirstSlideCommand);
        NavigateNextSlideCommand = new DelegateCommand(ExecuteNavigateNextSlideCommand).ObservesCanExecute(() => CanNavigateNext);
        NavigatePreviousSlideCommand = new DelegateCommand(ExecuteNavigatePreviousSlideCommand).ObservesCanExecute(() => IsNotFirstSlide);
        OpenFolderCommand = new ActionCommand(ExecuteOpenFolderCommand);
    }

    private void ExecuteNavigateFirstSlideCommand()
    {
        CurrentIndex = 0;
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
        GetDataViewModel.SelectedSheet = null;
        using var stream = File.Open(DropFileViewModel.UploadedFile, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var result = reader.AsDataSet();

        foreach (DataTable table in result.Tables)
        {
            Sheets.Add(new Sheet { Name = table.TableName, Set = table.AsDataView() });
        }

        GetDataViewModel.SelectedSheet = GetDataViewModel.Sheets.FirstOrDefault();

        RaisePropertyChanged(nameof(CanNavigateNext));
        RaisePropertyChanged(nameof(SelectedFile));
        RaisePropertyChanged(nameof(FileName));
        ExecuteNavigateNextSlideCommand();
    }

    private void GetDataViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(GetDataViewModel.SelectedSheet) or nameof(GetDataViewModel.HeaderVisible)))
            return;

        var selectedSheet = GetDataViewModel.SelectedSheet;
        if (selectedSheet is null)
            return;


        var table = selectedSheet.Set.ToTable();
        MapColumnsViewModel.PreviewColumnData.Clear();
        
        foreach (DataColumn dataColumn in table.Columns)
        {
            if (GetDataViewModel.HeaderVisible)
            {
                MapColumnsViewModel.PreviewColumnData.Add(new ImportColumnInfoModel
                    { ColumnName = table.Rows[0][dataColumn.ColumnName].ToString() ?? string.Empty });
                
            }
            else
            {
                MapColumnsViewModel.PreviewColumnData.Add(new ImportColumnInfoModel
                    { ColumnName = string.Empty });
            }
        }
        
        if(GetDataViewModel.HeaderVisible)
            table.Rows.RemoveAt(0);
        
        MapColumnsViewModel.PreviewData = table;
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

        Process.Start("explorer.exe", Path.GetDirectoryName(SelectedFile)!);
    }
    
}