using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ModStation.Avalonia.ViewModels;

namespace ModStation.Avalonia;

public partial class WindowManager : ObservableObject
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    private Stack<ViewModelBase> WindowStack { get; } = [];

    public void WindowPush(ViewModelBase vm)
    {
        WindowStack.Push(vm);
        CurrentView = WindowStack.Peek();
    }

    public ViewModelBase WindowPeek()
    {
        return WindowStack.Peek();
    }

    public ViewModelBase WindowPop()
    {
        var vm = WindowStack.Pop();
        CurrentView = WindowStack.Peek();
        return vm;
    }
}
