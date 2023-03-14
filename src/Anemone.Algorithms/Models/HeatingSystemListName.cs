using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Anemone.Repository.HeatingSystemData;

namespace Anemone.Algorithms.Models;

public class HeatingSystemListName : INotifyPropertyChanged
{
    private int _id;
    private string _name = string.Empty;

    public HeatingSystemListName()
    {
    }

    public HeatingSystemListName(HeatingSystemName heatingSystemName)
    {
        Id = (int)heatingSystemName.Id!;
        Name = heatingSystemName.Name;
    }

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}