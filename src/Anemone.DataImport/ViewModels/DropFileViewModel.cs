using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Anemone.Core;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;

namespace Anemone.DataImport.ViewModels;

public class DropFileViewModel : ViewModelBase
{
    private static readonly string[] SupportedExtensions = { ".xls", ".xlsx", ".csv" };
    private static readonly string Filters = "sheet files|*" + string.Join(";*", SupportedExtensions);
    private string? _uploadedFile;


    public DropFileViewModel(IOpenFileDialog openFileDialog, IToastService toastService)
    {
        OpenFileDialog = openFileDialog;
        ToastService = toastService;
        ChooseFileCommand = new ActionCommand(ExecuteChooseFileCommand);
        DropFileCommand = new DelegateCommand<DragEventArgs>(ExecuteDropFileCommand);
    }

    private IOpenFileDialog OpenFileDialog { get; }
    private IToastService ToastService { get; }

    public string? UploadedFile
    {
        get => _uploadedFile;
        private set => SetProperty(ref _uploadedFile, value);
    }

    public ICommand ChooseFileCommand { get; set; }

    public DelegateCommand<DragEventArgs> DropFileCommand { get; set; }

    private void ExecuteDropFileCommand(DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        var files = e.Data.GetData(DataFormats.FileDrop) as string[];

        if (files is null)
            throw new NullReferenceException(nameof(files));

        var filteredFile = files
            .SingleOrDefault(file => SupportedExtensions.Any(file.ToLower().EndsWith));

        if (filteredFile is null)
        {
            ToastService.Show("Unsupported file extensions");
            return;
        }

        UploadedFile = filteredFile;
    }

    private void ExecuteChooseFileCommand()
    {
        OpenFileDialog.Multiselect = false;
        OpenFileDialog.Title = "Choose file";
        OpenFileDialog.Filter = Filters;

        var opened = OpenFileDialog.ShowDialog();
        if (opened is true) UploadedFile = OpenFileDialog.FileName;

        OpenFileDialog.Reset();
    }
}