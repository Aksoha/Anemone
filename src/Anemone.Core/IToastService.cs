using System;

namespace Anemone.Core;

public interface IToastService
{
    void Show(string message);
    void Show(string message, string actionContent, Action actionHandler);
}