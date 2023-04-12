using System.IO;

namespace Anemone.Core.Dialogs;

public interface IOpenFileDialog
{
    public DialogFilterExtension? DefaultExt { get; set; }
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