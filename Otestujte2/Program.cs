namespace Otestujte2;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}

public interface ISaleFetcher
{
    decimal FetchSale(string code);
}

// Jednoduchá reprezentace položky v košíku
public class OrderItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public OrderItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

// Třída, kterou budou žáci testovat
public class OrderService
{
    private const decimal DISOUNT_THRESHOLD = 2000m; // Hranice pro slevu
    private const decimal FREE_SHIPPING_THRESHOLD = 500m; // Hranice pro dopravu zdarma
    private const decimal SHIPPING_COST = 99m; // Cena dopravy
    private const decimal DISCOUNT = 0.10m; // 10% sleva
    
    private readonly ISaleFetcher _saleFetcher;
        
    // Konstruktor pro Dependency Injection
    public OrderService(ISaleFetcher saleFetcher)
    {
        _saleFetcher = saleFetcher;
    }
    
    public decimal CalculateFinalPrice(List<OrderItem> items, string code)
    {
        // 1. Validace vstupů
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("Objednávka musí obsahovat alespoň jednu položku.");
        }

        decimal total = 0m;

        // 2. Základní součet a kontrola záporných cen
        foreach (var item in items)
        {
            if (item.Price < 0)
            {
                throw new ArgumentOutOfRangeException("Cena položky nemůže být záporná.");
            }
            
            // Ošetření záporného množství (volitelné, ale dobré pro testy)
            if (item.Quantity <= 0)
            {
                 throw new ArgumentOutOfRangeException("Množství musí být větší než nula.");
            }

            total += item.Price * item.Quantity;
        }
        
        decimal sale = _saleFetcher.FetchSale(code);

        // 3. Aplikace slevy (pokud je nákup nad 2000)
        if (total > DISOUNT_THRESHOLD)
        {
            sale += DISCOUNT;
        }

        if (sale != 0)
        {
            total -= total * sale;
        }

        // 4. Připočtení dopravy (pokud je nákup pod 500)
        // Pozor: Porovnáváme cenu už po případné slevě!
        if (total < FREE_SHIPPING_THRESHOLD)
        {
            total += SHIPPING_COST;
        }

        return Math.Round(total, 2); // Zaokrouhlení na 2 desetinná místa
    }
}
