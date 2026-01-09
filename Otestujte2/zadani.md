# 🛒 Zadání: Unit Testování – E-shop Košík

**Cíl:** Napsat unit testy pro obchodní logiku aplikace.  
**Testovaná třída:** `OrderService`  
**Metoda:** `CalculateFinalPrice(List<OrderItem> items)`

---

## 📋 Pravidla byznys logiky (Specifikace)

1.  **Základ:** Cena je součtem všech položek (`Cena * Množství`).
2.  **Sleva 10 %:** Aplikuje se, pokud celková cena položek **přesáhne 2000 Kč**.
3.  **Doprava:**
    * Pokud je cena (po případné slevě) **nižší než 500 Kč**, připočte se doprava **99 Kč**.
    * Pokud je cena **500 Kč a více**, doprava je **ZDARMA**.
4.  **Validace:** Nesmí projít prázdná objednávka ani záporné ceny.

---

## 🛠 Úkoly k otestování

Vytvořte testovací projekt podle toho, co vyšlo nejlépe z vaší analízy a pokryjte následující scénáře. Používejte pattern **AAA (Arrange, Act, Assert)**.

### 1. Happy Path (Standardní scénáře)
Ověřte, že systém funguje pro běžné zákazníky.

- [ ] **Malý nákup (s dopravou)**
    - *Vstup:* 1 položka za 100 Kč.
    - *Očekávání:* Výsledek 199 Kč (100 cena + 99 doprava).
- [ ] **Střední nákup (Doprava zdarma, bez slevy)**
    - *Vstup:* 1 položka za 600 Kč.
    - *Očekávání:* Výsledek 600 Kč.
- [ ] **Velký nákup (Sleva 10 % + Doprava zdarma)**
    - *Vstup:* 1 položka za 3000 Kč.
    - *Očekávání:* Výsledek 2700 Kč (3000 - 10 %).

### 2. Edge Cases (Hraniční hodnoty)
Zde se nejčastěji dělají chyby v podmínkách (`<` vs `<=`).

- [ ] **Hranice dopravy (Těsně pod)**
    - *Vstup:* Nákup za 499 Kč.
    - *Očekávání:* Platí se doprava (+99 Kč).
- [ ] **Hranice dopravy (Přesně na hranici)**
    - *Vstup:* Nákup za 500 Kč.
    - *Očekávání:* Doprava je zdarma.
- [ ] **Hranice slevy (Těsně pod/Přesně)**
    - *Vstup:* Nákup za 2000 Kč.
    - *Očekávání:* Žádná sleva.
- [ ] **Hranice slevy (Těsně nad)**
    - *Vstup:* Nákup za 2001 Kč.
    - *Očekávání:* Aplikována sleva 10 %.

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