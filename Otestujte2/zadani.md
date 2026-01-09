# 🛒 Zadání: Unit Testování s Mockováním – E-shop Košík

**Cíl:** Napsat unit testy pro obchodní logiku, která má externí závislost. Je nutné využít **Mocking**.
**Testovaná třída:** `OrderService`
**Metoda:** `CalculateFinalPrice(List<OrderItem> items, string code)`

---

## 1. Kód k otestování

Tento kód si zkopírujte do projektu. Všimněte si, že třída `OrderService` nyní vyžaduje v konstruktoru `ISaleFetcher`.

```csharp
using System;
using System.Collections.Generic;

namespace EshopSystem
{
    // Závislost: Služba pro získání slevy podle kódu
    public interface ISaleFetcher
    {
        decimal FetchSale(string code);
    }

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

    public class OrderService
    {
        private const decimal DISCOUNT_THRESHOLD = 2000m; // Hranice pro automatickou slevu
        private const decimal FREE_SHIPPING_THRESHOLD = 500m; // Hranice pro dopravu zdarma
        private const decimal SHIPPING_COST = 99m; // Cena dopravy
        private const decimal AUTOMATIC_DISCOUNT = 0.10m; // 10% automatická sleva nad 2000
        
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
                if (item.Price < 0) throw new ArgumentOutOfRangeException("Cena položky nemůže být záporná.");
                if (item.Quantity <= 0) throw new ArgumentOutOfRangeException("Množství musí být větší než nula.");

                total += item.Price * item.Quantity;
            }
            
            // Získání slevy z externí služby (např. databáze/API)
            decimal salePercentage = _saleFetcher.FetchSale(code);

            // 3. Aplikace automatické slevy (pokud je nákup nad 2000)
            // Slevy se sčítají (např. 10% kód + 10% za velký nákup = 20% dolů)
            if (total > DISCOUNT_THRESHOLD)
            {
                salePercentage += AUTOMATIC_DISCOUNT;
            }

            // Aplikace finální slevy
            if (salePercentage != 0)
            {
                total -= total * salePercentage;
            }

            // 4. Připočtení dopravy (pokud je nákup pod 500 po slevě)
            if (total < FREE_SHIPPING_THRESHOLD)
            {
                total += SHIPPING_COST;
            }

            return Math.Round(total, 2);
        }
    }
}
```

---

## 2. Specifikace (Pravidla byznys logiky)

1.  **Základní cena:** Součet `Cena * Množství`.
2.  **Slevový kód (Externí závislost):** Metoda se zeptá služby `ISaleFetcher`, jakou slevu (desetinné číslo, např. `0.2` pro 20 %) dává zadaný kód.
3.  **Množstevní sleva:** Pokud je základní cena **nad 2000 Kč**, přičte se k slevě dalších **10 %** (`0.10`).
    * *Příklad:* Mám kód na 10 % a nakoupím za 3000 Kč -> Celková sleva je 20 %.
4.  **Doprava:**
    * Cena < 500 Kč -> Doprava **99 Kč**.
    * Cena >= 500 Kč -> Doprava **ZDARMA**.

---

## 3. Checklist testů

Při psaní testů musíte pomocí Mocku nasimulovat chování `ISaleFetcher`.

### A) Scénáře bez slevového kódu
*Mock nastavte tak, aby pro prázdný kód nebo "INVALID" vracel `0`.*

- [ ] **Malý nákup (s dopravou):**
    - *Setup:* Mock vrací `0`. Nákup za 100 Kč.
    - *Očekávání:* 199 Kč (100 + 99).
- [ ] **Velký nákup (Pouze automatická sleva):**
    - *Setup:* Mock vrací `0`. Nákup za 3000 Kč.
    - *Logika:* 3000 - 10% (automaticky) = 2700.
    - *Očekávání:* 2700 Kč.

### B) Scénáře se slevovým kódem (Mocking)
*Mock nastavte tak, aby pro kód "SLEVA20" vracel `0.20`.*

- [ ] **Malý nákup se slevou:**
    - *Setup:* Mock pro "SLEVA20" vrací `0.2`. Nákup za 100 Kč.
    - *Logika:* 100 - 20% = 80 Kč. (80 < 500 -> platí se doprava).
    - *Očekávání:* 179 Kč (80 + 99).
- [ ] **Kombinace slev (Sčítání):**
    - *Setup:* Mock pro "SLEVA10" vrací `0.1`. Nákup za 3000 Kč.
    - *Logika:* Základ 3000. Slevy: 10% (kód) + 10% (automatická nad 2000) = 20% celkem.
    - *Výpočet:* 3000 * 0.8 = 2400.
    - *Očekávání:* 2400 Kč.

### C) Edge Cases (Hraniční hodnoty)
- [ ] **Sleva srazí cenu pod limit dopravy:**
    - *Setup:* Mock vrací `0`. Nákup za 550 Kč (Doprava by byla zdarma).
    - *Změna:* Použijeme kód na 10% slevu (Mock vrací `0.1`).
    - *Logika:* 550 - 10% = 495 Kč. (Nyní je < 500, platí se doprava).
    - *Očekávání:* 594 Kč (495 + 99).

### 3. Validace a Výjimky (Exceptions)
Ověřte, že systém správně zareaguje na nevalidní vstupy vyhozením výjimky.

- [ ] **Objednávka je `null`**
  - *Očekávání:* `ArgumentException`
- [ ] **Seznam položek je prázdný**
  - *Očekávání:* `ArgumentException`
- [ ] **Položka má zápornou cenu**
  - *Očekávání:* `ArgumentOutOfRangeException`
- [ ] **Položka má záporné/nulové množství**
  - *Očekávání:* `ArgumentOutOfRangeException`

---

## 💡 Tipy pro vypracování

1.  **Pojmenování testů:** Název testu by měl říkat, co se děje.
    * *Špatně:* `Test1()`
    * *Dobře:* `CalculateFinalPrice_SmallOrder_AddsShippingCost()`
    * *Vzor:* `Metoda_Scenar_OcekavanyVysledek`
2.  **Assert:** Vždy kontrolujte konkrétní číselnou hodnotu. U desetinných čísel (`decimal`) pozor na přesnost (ale v tomto zadání pracujeme s přesnými částkami).
3.  **Setup:** Pokud vytváříte složitější data, vytvořte si pomocnou metodu pro generování položek.