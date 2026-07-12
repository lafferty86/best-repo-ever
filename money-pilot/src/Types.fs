module Types

/// Core domain model for Money Pilot.

type Page =
    | Dashboard
    | Accounts
    | Transactions
    | Budget
    | CashFlow
    | Investments
    | Recurring
    | Goals

type Theme =
    | Dark
    | Light

type AccountKind =
    | Checking
    | Savings
    | CreditCard
    | Investment
    | Loan
    | Cash

    member this.Label =
        match this with
        | Checking -> "Checking"
        | Savings -> "Savings"
        | CreditCard -> "Credit Card"
        | Investment -> "Investment"
        | Loan -> "Loan"
        | Cash -> "Cash"

    /// Liabilities count against net worth.
    member this.IsLiability =
        match this with
        | CreditCard | Loan -> true
        | _ -> false

type Account =
    { Id: int
      Name: string
      Institution: string
      Kind: AccountKind
      Balance: float
      /// Change since the previous statement, for the little trend chip.
      Change: float
      Color: string
      /// Last-4 or masked identifier.
      Mask: string }

    /// Signed contribution to net worth.
    member this.NetContribution =
        if this.Kind.IsLiability then -(abs this.Balance) else this.Balance

/// A spending / income category with an emoji glyph and accent color.
type Category =
    { Id: string
      Name: string
      Icon: string
      Color: string
      IsIncome: bool }

type Transaction =
    { Id: int
      Date: string // ISO yyyy-MM-dd
      Merchant: string
      CategoryId: string
      AccountId: int
      /// Negative = money out, positive = money in.
      Amount: float
      Note: string
      Reviewed: bool
      Pending: bool }

type Budget =
    { CategoryId: string
      Limit: float
      Spent: float }

    member this.Remaining = this.Limit - this.Spent
    member this.Ratio = if this.Limit <= 0.0 then 0.0 else this.Spent / this.Limit

type Holding =
    { Symbol: string
      Name: string
      Shares: float
      Price: float
      CostBasis: float
      Color: string }

    member this.Value = this.Shares * this.Price
    member this.Cost = this.Shares * this.CostBasis
    member this.Gain = this.Value - this.Cost
    member this.GainPct =
        if this.Cost <= 0.0 then 0.0 else (this.Gain / this.Cost) * 100.0

type Cadence =
    | Weekly
    | Monthly
    | Yearly

    member this.Label =
        match this with
        | Weekly -> "Weekly"
        | Monthly -> "Monthly"
        | Yearly -> "Yearly"

type Recurring =
    { Id: int
      Merchant: string
      CategoryId: string
      Amount: float
      Cadence: Cadence
      NextDate: string
      Icon: string
      Color: string
      /// True for income (paycheck), false for a bill/subscription.
      IsIncome: bool }

type Goal =
    { Id: int
      Name: string
      Icon: string
      Color: string
      Target: float
      Saved: float
      Monthly: float
      TargetDate: string }

    member this.Ratio = if this.Target <= 0.0 then 0.0 else this.Saved / this.Target

/// One month of cash-flow history for the trend charts.
type CashPoint =
    { Month: string
      Income: float
      Expense: float }

    member this.Net = this.Income - this.Expense

/// One data point of net-worth history.
type NetWorthPoint = { Month: string; Value: float }
