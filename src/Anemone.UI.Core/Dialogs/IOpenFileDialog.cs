using System.IO;
using Anemone.Core.Common.ValueObjects;

namespace Anemone.UI.Core.Dialogs;

public interface IOpenFileDialog
{
    public FileExtension? DefaultExt { get; set; }
    public string FileName { get; set; }
    public string[] FileNames { get; }
    public DialogFilterCollection Filter { get; set; }
    public int FilterIndex { get; set; }
    public string InitialDirectory { get; set; }
    public bool Multiselect { get; set; }
    public string Title { get; set; }
    bool? ShowDialog();
    void Reset();
    Stream OpenFile();
    Stream[] OpenFiles();
}