using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.ImageProcessing;
using LoginApplication.Messages;
using LoginApplication.ViewModels;
using SkiaSharp;

namespace LoginApplication.Views;

public partial class HomePage : ContentPage
{
    public HomePage(UserValidationViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }

}
