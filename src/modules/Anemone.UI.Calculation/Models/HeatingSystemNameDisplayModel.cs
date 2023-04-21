using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Anemone.Core.Common.Entities;

namespace Anemone.UI.Calculation.Models;

public class HeatingSystemNameDisplayModel : INotifyPropertyChanged
{
    private int _id;
    private string _name = string.Empty;

    public HeatingSystemNameDisplayModel()
    {
    }

    public HeatingSystemNameDisplayModel(HeatingSystem heatingSystem)
    {
        ArgumentNullException.ThrowIfNull(heatingSystem.Id);
        Id = (int)heatingSystem.Id;
        Name = heatingSystem.Name;
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