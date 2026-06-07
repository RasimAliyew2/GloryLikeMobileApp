using System;

namespace MetanetA_MobileApp.Model;

public class PurchaseHistoryItem
{
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
