using System.IO;
using Anemone.Core.Common.ValueObjects;

namespace Anemone.UI.Core.Dialogs;

internal class OpenFileDialog : IOpenFileDialog
{
    private FileExtension? _defaultExt;
    private DialogFilterCollection _filter = new();
    public FileExtension? DefaultExt
    {
        get => _defaultExt;
        set
        {
            _defaultExt = value;
            _dialog.DefaultExt = value;
        }
    }

    public string FileName
    {
        get => _dialog.FileName;
        set => _dialog.FileName = value;
    }

    public string[] FileNames => _dialog.FileNames;

    public DialogFilterCollection Filter
    {
        get => _filter;
        set
        {
            _filter = value;
            _dialog.Filter = value;
        }
    }

    public int FilterIndex
    {
        get => _dialog.FilterIndex;
        set => _dialog.FilterIndex = value;
    }

    public string InitialDirectory
    {
        get => _dialog.InitialDirectory;
        set => _dialog.InitialDirectory = value;
    }

    public bool Multiselect
    {
        get => _dialog.Multiselect;
        set => _dialog.Multiselect = value;
    }

    public string Title
    {
        get => _dialog.Title;
        set => _dialog.Title = value;
    }

    private readonly Microsoft.Win32.OpenFileDialog _dialog = new();

    public bool? ShowDialog()
    {
        return _dialog.ShowDialog();
    }

    public void Reset()
    {
        _dialog.Reset();
    }

    public Stream OpenFile()
    {
        return _dialog.OpenFile();
    }

    public Stream[] OpenFiles()
    {
        return _dialog.OpenFiles();
    }
}