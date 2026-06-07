using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.Sign;

namespace MetanetA_MobileApp.ViewModels
{
    [QueryProperty(nameof(OperationType), "OperationType")]
    public partial class ConfrimSMSViewModel : ObservableObject
    {
        IUserSession userSession;
        private IDispatcherTimer? _timer;
        private int _remaining;
        [ObservableProperty]
        string code;
        [ObservableProperty] private string timerText = "00:59";
        [ObservableProperty]
        public bool isVisibleErrorLabel;

        [ObservableProperty]
        private OperationType operationType;
        public ConfrimSMSViewModel(IUserSession userInfo) 
        {
            this.userSession = userInfo;
            StartCountdown(59);
        }
        [RelayCommand]
        private async Task CompletedEditing(string? textFromEntry)
        {
            userSession.OtpCode = userSession.OtpCode?.Replace("code:", "");
            if (userSession.OtpCode == textFromEntry)
                await Shell.Current.GoToAsync($"//{nameof(SetPasswordPage)}", new Dictionary<string, object>
                {
                    ["OperationType"] = OperationType
                });

            else
                IsVisibleErrorLabel = true;
        }

        [RelayCommand]
        private void StartCountdown(int seconds = 59)
        {
            _timer?.Stop();
            _remaining = seconds;
            TimerText = Format(_remaining);

            var dispatcher = Application.Current?.Dispatcher;
            _timer = dispatcher?.CreateTimer();
            if (_timer is null) return;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                _remaining--;
                if (_remaining <= 0)
                {
                    _remaining = 0;
                    _timer.Stop();
                }
                TimerText = Format(_remaining);
            };
            _timer.Start();
        }
        [RelayCommand]
        private async Task ReturnPreviousPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
        }
        [RelayCommand]
        private async Task AprroveTheSMS()
        {
            userSession.OtpCode = userSession.OtpCode?.Replace("code:", "");
            if (userSession.OtpCode == Code)
                await Shell.Current.GoToAsync($"//{nameof(SetPasswordPage)}", new Dictionary<string, object>
              {
                    ["OperationType"] = OperationType
                });
            else
                IsVisibleErrorLabel = true;
        }

        private static string Format(int totalSeconds)
        {
            var ts = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
            return $"{ts.Minutes:00}:{ts.Seconds:00}";
        }

        [RelayCommand]
        public void StopTimer() => _timer?.Stop();

    }
}
