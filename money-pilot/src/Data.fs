module Data

open Types

/// Seed data — realistic sample so Money Pilot feels alive on first load.
/// All state is in-memory and fully editable at runtime.

let categories : Category list =
    [ { Id = "income";    Name = "Income";         Icon = "\U0001F4B0"; Color = "#22c55e"; IsIncome = true }
      { Id = "groceries"; Name = "Groceries";      Icon = "\U0001F6D2"; Color = "#f59e0b"; IsIncome = false }
      { Id = "dining";    Name = "Dining & Bars";  Icon = "\U0001F37D"; Color = "#ef4444"; IsIncome = false }
      { Id = "transport"; Name = "Transport";      Icon = "\U0001F697"; Color = "#3b82f6"; IsIncome = false }
      { Id = "shopping";  Name = "Shopping";       Icon = "\U0001F6CD"; Color = "#ec4899"; IsIncome = false }
      { Id = "housing";   Name = "Housing";        Icon = "\U0001F3E0"; Color = "#8b5cf6"; IsIncome = false }
      { Id = "utilities"; Name = "Utilities";      Icon = "\U0001F4A1"; Color = "#06b6d4"; IsIncome = false }
      { Id = "health";    Name = "Health";         Icon = "\U0001FA7A"; Color = "#14b8a6"; IsIncome = false }
      { Id = "fitness";   Name = "Fitness";        Icon = "\U0001F3CB"; Color = "#84cc16"; IsIncome = false }
      { Id = "subs";      Name = "Subscriptions";  Icon = "\U0001F4FA"; Color = "#a855f7"; IsIncome = false }
      { Id = "travel";    Name = "Travel";         Icon = "✈";     Color = "#0ea5e9"; IsIncome = false }
      { Id = "entertain"; Name = "Entertainment";  Icon = "\U0001F3AC"; Color = "#f43f5e"; IsIncome = false }
      { Id = "transfer";  Name = "Transfer";       Icon = "\U0001F501"; Color = "#64748b"; IsIncome = false }
      { Id = "misc";      Name = "Everything Else"; Icon = "✨";    Color = "#94a3b8"; IsIncome = false } ]

let categoryById (id: string) =
    categories |> List.tryFind (fun c -> c.Id = id)
    |> Option.defaultValue
        { Id = id; Name = "Uncategorized"; Icon = "❓"; Color = "#94a3b8"; IsIncome = false }

let accounts : Account list =
    [ { Id = 1; Name = "Everyday Checking"; Institution = "Chase";         Kind = Checking;   Balance = 8452.19;   Change = 320.5;   Color = "#3b82f6"; Mask = "4021" }
      { Id = 2; Name = "High-Yield Savings"; Institution = "Ally";         Kind = Savings;    Balance = 24980.00;  Change = 104.2;   Color = "#22c55e"; Mask = "8830" }
      { Id = 3; Name = "Sapphire Reserve";  Institution = "Chase";        Kind = CreditCard; Balance = -1843.67;  Change = -412.0;  Color = "#8b5cf6"; Mask = "1188" }
      { Id = 4; Name = "Brokerage";         Institution = "Fidelity";     Kind = Investment; Balance = 68230.44;  Change = 1290.8;  Color = "#f59e0b"; Mask = "9902" }
      { Id = 5; Name = "Roth IRA";          Institution = "Vanguard";     Kind = Investment; Balance = 41120.10;  Change = 640.3;   Color = "#14b8a6"; Mask = "3345" }
      { Id = 6; Name = "Auto Loan";         Institution = "Capital One";  Kind = Loan;       Balance = -12430.00; Change = 380.0;   Color = "#ef4444"; Mask = "7761" }
      { Id = 7; Name = "Travel Rewards";    Institution = "Amex";         Kind = CreditCard; Balance = -642.11;   Change = -128.4;  Color = "#ec4899"; Mask = "2204" } ]

let accountById (id: int) = accounts |> List.tryFind (fun a -> a.Id = id)

let accountName (id: int) =
    accountById id |> Option.map (fun a -> a.Name) |> Option.defaultValue "Unknown"

/// A month of transactions — deliberately varied for good-looking charts.
let transactions : Transaction list =
    [ { Id = 1;  Date = "2026-07-11"; Merchant = "Whole Foods Market"; CategoryId = "groceries"; AccountId = 3; Amount = -86.42;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 2;  Date = "2026-07-11"; Merchant = "Blue Bottle Coffee"; CategoryId = "dining";    AccountId = 3; Amount = -6.75;    Note = "";                 Reviewed = false; Pending = true  }
      { Id = 3;  Date = "2026-07-10"; Merchant = "Shell";              CategoryId = "transport"; AccountId = 1; Amount = -52.30;   Note = "Road trip fuel";   Reviewed = true;  Pending = false }
      { Id = 4;  Date = "2026-07-10"; Merchant = "Netflix";            CategoryId = "subs";      AccountId = 3; Amount = -22.99;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 5;  Date = "2026-07-09"; Merchant = "Trader Joe's";       CategoryId = "groceries"; AccountId = 1; Amount = -63.18;   Note = "";                 Reviewed = false; Pending = false }
      { Id = 6;  Date = "2026-07-09"; Merchant = "Uber";               CategoryId = "transport"; AccountId = 7; Amount = -18.40;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 7;  Date = "2026-07-08"; Merchant = "Sweetgreen";         CategoryId = "dining";    AccountId = 3; Amount = -15.85;   Note = "Lunch";            Reviewed = false; Pending = false }
      { Id = 8;  Date = "2026-07-07"; Merchant = "Acme Corp Payroll";  CategoryId = "income";    AccountId = 1; Amount = 3120.00;  Note = "Bi-weekly salary"; Reviewed = true;  Pending = false }
      { Id = 9;  Date = "2026-07-07"; Merchant = "PG&E";               CategoryId = "utilities"; AccountId = 1; Amount = -142.55;  Note = "";                 Reviewed = true;  Pending = false }
      { Id = 10; Date = "2026-07-06"; Merchant = "Amazon";             CategoryId = "shopping";  AccountId = 3; Amount = -74.20;   Note = "Desk lamp";        Reviewed = false; Pending = false }
      { Id = 11; Date = "2026-07-06"; Merchant = "Equinox";            CategoryId = "fitness";   AccountId = 1; Amount = -215.00;  Note = "Monthly membership";Reviewed = true; Pending = false }
      { Id = 12; Date = "2026-07-05"; Merchant = "Delta Air Lines";    CategoryId = "travel";    AccountId = 7; Amount = -388.60;  Note = "SFO -> JFK";       Reviewed = false; Pending = false }
      { Id = 13; Date = "2026-07-05"; Merchant = "Spotify";            CategoryId = "subs";      AccountId = 3; Amount = -11.99;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 14; Date = "2026-07-04"; Merchant = "AMC Theatres";       CategoryId = "entertain"; AccountId = 3; Amount = -34.50;   Note = "";                 Reviewed = false; Pending = false }
      { Id = 15; Date = "2026-07-03"; Merchant = "CVS Pharmacy";       CategoryId = "health";    AccountId = 1; Amount = -28.14;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 16; Date = "2026-07-03"; Merchant = "Chipotle";           CategoryId = "dining";    AccountId = 3; Amount = -13.20;   Note = "";                 Reviewed = false; Pending = false }
      { Id = 17; Date = "2026-07-02"; Merchant = "Costco";             CategoryId = "groceries"; AccountId = 3; Amount = -184.77;  Note = "Bulk run";         Reviewed = true;  Pending = false }
      { Id = 18; Date = "2026-07-01"; Merchant = "Oakwood Apartments"; CategoryId = "housing";   AccountId = 1; Amount = -2450.00; Note = "July rent";        Reviewed = true;  Pending = false }
      { Id = 19; Date = "2026-07-01"; Merchant = "Comcast Xfinity";    CategoryId = "utilities"; AccountId = 1; Amount = -89.99;   Note = "Internet";         Reviewed = true;  Pending = false }
      { Id = 20; Date = "2026-06-30"; Merchant = "Apple";              CategoryId = "subs";      AccountId = 3; Amount = -2.99;    Note = "iCloud";           Reviewed = true;  Pending = false }
      { Id = 21; Date = "2026-06-29"; Merchant = "Lyft";               CategoryId = "transport"; AccountId = 7; Amount = -22.10;   Note = "";                 Reviewed = false; Pending = false }
      { Id = 22; Date = "2026-06-28"; Merchant = "Nike";               CategoryId = "shopping";  AccountId = 3; Amount = -129.00;  Note = "Running shoes";    Reviewed = true;  Pending = false }
      { Id = 23; Date = "2026-06-27"; Merchant = "Tartine Bakery";     CategoryId = "dining";    AccountId = 3; Amount = -24.60;   Note = "";                 Reviewed = false; Pending = false }
      { Id = 24; Date = "2026-06-26"; Merchant = "Interest Payment";   CategoryId = "income";    AccountId = 2; Amount = 104.22;   Note = "Savings APY";      Reviewed = true;  Pending = false }
      { Id = 25; Date = "2026-06-25"; Merchant = "Safeway";            CategoryId = "groceries"; AccountId = 1; Amount = -71.35;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 26; Date = "2026-06-24"; Merchant = "Acme Corp Payroll";  CategoryId = "income";    AccountId = 1; Amount = 3120.00;  Note = "Bi-weekly salary"; Reviewed = true;  Pending = false }
      { Id = 27; Date = "2026-06-23"; Merchant = "Airbnb";             CategoryId = "travel";    AccountId = 7; Amount = -512.00;  Note = "Tahoe weekend";    Reviewed = false; Pending = false }
      { Id = 28; Date = "2026-06-22"; Merchant = "The Standard Bar";   CategoryId = "dining";    AccountId = 3; Amount = -58.00;   Note = "Birthday drinks";  Reviewed = true;  Pending = false }
      { Id = 29; Date = "2026-06-21"; Merchant = "Best Buy";           CategoryId = "shopping";  AccountId = 3; Amount = -249.99;  Note = "Headphones";       Reviewed = false; Pending = false }
      { Id = 30; Date = "2026-06-20"; Merchant = "OpenAI";             CategoryId = "subs";      AccountId = 3; Amount = -20.00;   Note = "";                 Reviewed = true;  Pending = false }
      { Id = 31; Date = "2026-06-19"; Merchant = "Kaiser Permanente";  CategoryId = "health";    AccountId = 1; Amount = -45.00;   Note = "Copay";            Reviewed = true;  Pending = false }
      { Id = 32; Date = "2026-06-18"; Merchant = "BART";               CategoryId = "transport"; AccountId = 1; Amount = -9.20;    Note = "";                 Reviewed = true;  Pending = false }
      { Id = 33; Date = "2026-06-17"; Merchant = "Philz Coffee";       CategoryId = "dining";    AccountId = 3; Amount = -7.50;    Note = "";                 Reviewed = false; Pending = false }
      { Id = 34; Date = "2026-06-16"; Merchant = "Steam";              CategoryId = "entertain"; AccountId = 3; Amount = -59.99;   Note = "Summer sale";      Reviewed = false; Pending = false }
      { Id = 35; Date = "2026-06-15"; Merchant = "Whole Foods Market"; CategoryId = "groceries"; AccountId = 3; Amount = -92.11;   Note = "";                 Reviewed = true;  Pending = false } ]

let budgets : Budget list =
    [ { CategoryId = "groceries"; Limit = 700.0;  Spent = 497.83 }
      { CategoryId = "dining";    Limit = 300.0;  Spent = 234.90 }
      { CategoryId = "transport"; Limit = 250.0;  Spent = 132.00 }
      { CategoryId = "shopping";  Limit = 400.0;  Spent = 453.19 }
      { CategoryId = "housing";   Limit = 2450.0; Spent = 2450.00 }
      { CategoryId = "utilities"; Limit = 350.0;  Spent = 232.54 }
      { CategoryId = "subs";      Limit = 120.0;  Spent = 80.96 }
      { CategoryId = "fitness";   Limit = 215.0;  Spent = 215.00 }
      { CategoryId = "entertain"; Limit = 150.0;  Spent = 94.49 }
      { CategoryId = "travel";    Limit = 800.0;  Spent = 900.60 } ]

let holdings : Holding list =
    [ { Symbol = "VTI";  Name = "Vanguard Total Market";  Shares = 142.0; Price = 268.40; CostBasis = 210.15; Color = "#f59e0b" }
      { Symbol = "VXUS"; Name = "Vanguard Intl";          Shares = 210.0; Price = 62.10;  CostBasis = 55.80;  Color = "#3b82f6" }
      { Symbol = "AAPL"; Name = "Apple Inc.";             Shares = 60.0;  Price = 224.90; CostBasis = 148.20; Color = "#22c55e" }
      { Symbol = "NVDA"; Name = "NVIDIA Corp.";           Shares = 25.0;  Price = 178.50; CostBasis = 96.40;  Color = "#84cc16" }
      { Symbol = "MSFT"; Name = "Microsoft Corp.";        Shares = 30.0;  Price = 465.20; CostBasis = 320.10; Color = "#14b8a6" }
      { Symbol = "BND";  Name = "Vanguard Total Bond";    Shares = 180.0; Price = 72.30;  CostBasis = 74.90;  Color = "#a855f7" } ]

let recurrings : Recurring list =
    [ { Id = 1; Merchant = "Acme Corp Payroll";  CategoryId = "income";    Amount = 3120.00; Cadence = Weekly;  NextDate = "2026-07-21"; Icon = "\U0001F4B0"; Color = "#22c55e"; IsIncome = true  }
      { Id = 2; Merchant = "Oakwood Apartments"; CategoryId = "housing";   Amount = 2450.00; Cadence = Monthly; NextDate = "2026-08-01"; Icon = "\U0001F3E0"; Color = "#8b5cf6"; IsIncome = false }
      { Id = 3; Merchant = "Equinox";            CategoryId = "fitness";   Amount = 215.00;  Cadence = Monthly; NextDate = "2026-08-06"; Icon = "\U0001F3CB"; Color = "#84cc16"; IsIncome = false }
      { Id = 4; Merchant = "PG&E";               CategoryId = "utilities"; Amount = 142.55;  Cadence = Monthly; NextDate = "2026-08-07"; Icon = "\U0001F4A1"; Color = "#06b6d4"; IsIncome = false }
      { Id = 5; Merchant = "Comcast Xfinity";    CategoryId = "utilities"; Amount = 89.99;   Cadence = Monthly; NextDate = "2026-08-01"; Icon = "\U0001F4F6"; Color = "#0ea5e9"; IsIncome = false }
      { Id = 6; Merchant = "Netflix";            CategoryId = "subs";      Amount = 22.99;   Cadence = Monthly; NextDate = "2026-08-10"; Icon = "\U0001F3AC"; Color = "#ef4444"; IsIncome = false }
      { Id = 7; Merchant = "Spotify";            CategoryId = "subs";      Amount = 11.99;   Cadence = Monthly; NextDate = "2026-08-05"; Icon = "\U0001F3B5"; Color = "#22c55e"; IsIncome = false }
      { Id = 8; Merchant = "iCloud+";            CategoryId = "subs";      Amount = 2.99;    Cadence = Monthly; NextDate = "2026-07-30"; Icon = "☁";     Color = "#64748b"; IsIncome = false }
      { Id = 9; Merchant = "Auto Loan";          CategoryId = "transport"; Amount = 380.00;  Cadence = Monthly; NextDate = "2026-08-03"; Icon = "\U0001F697"; Color = "#3b82f6"; IsIncome = false } ]

let goals : Goal list =
    [ { Id = 1; Name = "Emergency Fund";  Icon = "\U0001F6E1"; Color = "#22c55e"; Target = 30000.0; Saved = 24980.0; Monthly = 800.0;  TargetDate = "2026-12" }
      { Id = 2; Name = "Japan Trip";      Icon = "\U0001F5FC"; Color = "#ef4444"; Target = 6000.0;  Saved = 2150.0;  Monthly = 500.0;  TargetDate = "2027-03" }
      { Id = 3; Name = "New MacBook";     Icon = "\U0001F4BB"; Color = "#64748b"; Target = 2500.0;  Saved = 1800.0;  Monthly = 250.0;  TargetDate = "2026-10" }
      { Id = 4; Name = "House Down Pmt";  Icon = "\U0001F3E1"; Color = "#8b5cf6"; Target = 80000.0; Saved = 31200.0; Monthly = 1500.0; TargetDate = "2028-06" } ]

let cashFlowHistory : CashPoint list =
    [ { Month = "Feb"; Income = 6240.0; Expense = 4820.0 }
      { Month = "Mar"; Income = 6240.0; Expense = 5310.0 }
      { Month = "Apr"; Income = 6580.0; Expense = 4640.0 }
      { Month = "May"; Income = 6240.0; Expense = 5120.0 }
      { Month = "Jun"; Income = 6690.0; Expense = 5480.0 }
      { Month = "Jul"; Income = 6344.0; Expense = 3980.0 } ]

let netWorthHistory : NetWorthPoint list =
    [ { Month = "Jan"; Value = 108420.0 }
      { Month = "Feb"; Value = 111200.0 }
      { Month = "Mar"; Value = 115640.0 }
      { Month = "Apr"; Value = 119880.0 }
      { Month = "May"; Value = 122310.0 }
      { Month = "Jun"; Value = 124960.0 }
      { Month = "Jul"; Value = 127886.0 } ]
