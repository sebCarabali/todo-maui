using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LoginApplication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApplication.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userName;
        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool _isBussy = false;
        
        public bool IsNotBussy => !IsBussy;
        partial void OnIsBussyChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotBussy));
        }

        [RelayCommand]        
        private async void Login()
        {
            if (IsBussy) return;

            IsBussy = true;
            await Task.Delay(3000);
            IsBussy = false;

            WeakReferenceMessenger.Default.Send(new LoginMessage(true, "Login successful"));
        }
    }
}
