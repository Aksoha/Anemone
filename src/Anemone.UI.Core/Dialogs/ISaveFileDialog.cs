using System.IO;
using Anemone.Core.Common.ValueObjects;

namespace Anemone.UI.Core.Dialogs;

public interface ISaveFileDialog
{
    FileExtension? DefaultExt { get; set; }
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