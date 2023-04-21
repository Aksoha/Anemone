using MaterialDesignThemes.Wpf;

namespace Anemone.UI.Core.Notifications;

internal class ToastService : IToastService
{
    // right now we are just forwarding messages to material design snack
    private ISnackbarMessageQueue SnackbarMessageQueue { get; }

    public ToastService(ISnackbarMessageQueue snackbarMessageQueue)
    {
        SnackbarMessageQueue = snackbarMessageQueue;
    }
    
    public void Show(string message)
    {
        SnackbarMessageQueue.Enqueue(message);
    }

    public void Show(string message, string actionContent, Action actionHandler)
    {
        SnackbarMessageQueue.Enqueue(message, actionContent, actionHandler);
    }
}