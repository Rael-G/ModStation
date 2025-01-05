﻿using CommunityToolkit.Mvvm.ComponentModel;
using ModManager;

namespace ModStation.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
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

    public MainWindowViewModel(Manager manager)
    {
        WindowPush(new ManageGamesViewModel(manager));
    }
}
