using System.IO;

namespace Anemone.Core;

public interface ISaveFileDialog
{
    string DefaultExt { get; set; }
    string FileName { get; set; }
    string[] FileNames { get; }
    string Filter { get; set; }
    int FilterIndex { get; set; }
    string InitialDirectory { get; set; }
    string Title { get; set; }
    
    bool? ShowDialog();
    void Reset();
    Stream OpenFile();
}