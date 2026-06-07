using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class FaqPageViewModel : ObservableObject
    {
        public ObservableCollection<FaqItem> Questions { get; } = new();

        public FaqPageViewModel()
        {
            Questions.Add(new FaqItem
            {
                Question = "Tədbirləriniz nə vaxt olur?",
                Answer = "Tədbirlərimizin tarixləri əvvəlcədən elan olunur. Ən son məlumatları tətbiqdən və ya rəsmi səhifələrimizdən izləyə bilərsiniz."
            });

            Questions.Add(new FaqItem
            {
                Question = "Tədbirlərə necə qoşula bilərəm?",
                Answer = "Tədbirlərə qoşulmaq üçün qeydiyyat formasını doldurmalı və təsdiq mesajını gözləməlisiniz. Bəzi tədbirlər üçün öncədən rezervasiya tələb oluna bilər."
            });

            Questions.Add(new FaqItem
            {
                Question = "Məhsulların çatdırılması necə olur?",
                Answer = "Sifarişlər təsdiqləndikdən sonra məhsullar seçilmiş ünvan üzrə çatdırılır. Çatdırılma müddəti bölgəyə və sifarişin növünə görə dəyişə bilər."
            });
        }

        [RelayCommand]
        private void Toggle(FaqItem item)
        {
            if (item == null)
                return;

            item.IsExpanded = !item.IsExpanded;
        }
    }
}
