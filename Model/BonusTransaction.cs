using System;

namespace MetanetA_MobileApp.Model
{
    public enum BonusTransactionType
    {
        Earned = 0,
        Spent = 1
    }

    public class BonusTransaction
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public BonusTransactionType Type { get; set; } = BonusTransactionType.Earned;

        // neçə bonus / neçə manat (sən necə istəyirsən elə saxla)
        public float Amount { get; set; }

        public string Description { get; set; } = "";

        // UI üçün hazır textlər
        public string DateText => Date.ToString("dd.MM.yyyy");
        public string AmountText => $"{(Type == BonusTransactionType.Earned ? "+" : "-")}{Amount:0.##}";
        public string TypeText => Type == BonusTransactionType.Earned ? "Qazanıldı" : "Xərcləndi";
        public string AmountColor => Type == BonusTransactionType.Earned ? "#16A34A" : "#EF4444";
    }
}
