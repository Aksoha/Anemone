using System.IO;

namespace Anemone.Core.Dialogs;

public interface ISaveFileDialog
{
    DialogFilterExtension? DefaultExt { get; set; }
    string FileName { get; set; }
    string[] FileNames { get; }
    DialogFilterCollection Filter { get; set; }
    int FilterIndex { get; set; }
    string InitialDirectory { get; set; }
    string Title { get; set; }

    bool? ShowDialog();
    void Reset();
    Stream OpenFile();
}