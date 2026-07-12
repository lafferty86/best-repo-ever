module State

open Elmish
open Types

/// Draft used by the "Add transaction" modal.
type TxDraft =
    { Merchant: string
      Amount: string
      CategoryId: string
      AccountId: int
      Date: string
      Note: string
      IsIncome: bool }

let emptyDraft =
    { Merchant = ""
      Amount = ""
      CategoryId = "groceries"
      AccountId = 1
      Date = "2026-07-12"
      Note = ""
      IsIncome = false }

type Sort =
    | ByDate
    | ByAmount

type Model =
    { Page: Page
      Theme: Theme
      Accounts: Account list
      Transactions: Transaction list
      BudgetLimits: Map<string, float>
      Goals: Goal list
      Recurrings: Recurring list
      Holdings: Holding list
      Search: string
      CategoryFilter: string option
      AccountFilter: int option
      UnreviewedOnly: bool
      Sort: Sort
      Draft: TxDraft option
      Toast: string option
      NextTxId: int
      SidebarOpen: bool }

type Msg =
    | Navigate of Page
    | ToggleTheme
    | ToggleSidebar
    | SetSearch of string
    | SetCategoryFilter of string option
    | SetAccountFilter of int option
    | ToggleUnreviewedOnly
    | SetSort of Sort
    | ToggleReviewed of int
    | SetTxCategory of int * string
    | DeleteTx of int
    | OpenAddModal
    | CloseModal
    | UpdateDraft of TxDraft
    | SubmitDraft
    | SetBudget of string * float
    | ContributeGoal of int * float
    | AddGoalMonthly of int
    | ClearToast

let init () : Model * Cmd<Msg> =
    { Page = Dashboard
      Theme = Dark
      Accounts = Data.accounts
      Transactions = Data.transactions
      BudgetLimits = Data.budgets |> List.map (fun b -> b.CategoryId, b.Limit) |> Map.ofList
      Goals = Data.goals
      Recurrings = Data.recurrings
      Holdings = Data.holdings
      Search = ""
      CategoryFilter = None
      AccountFilter = None
      UnreviewedOnly = false
      Sort = ByDate
      Draft = None
      Toast = None
      NextTxId = 1000
      SidebarOpen = true }, Cmd.none

let private toast (msg: string) =
    Cmd.ofEffect (fun dispatch ->
        Fable.Core.JS.setTimeout (fun () -> dispatch ClearToast) 2600 |> ignore)
    |> fun clearCmd -> msg, clearCmd

/// Money spent (positive number) in a category this month, expenses only.
let spentInCategory (transactions: Transaction list) (categoryId: string) =
    transactions
    |> List.filter (fun t -> t.CategoryId = categoryId && t.Amount < 0.0)
    |> List.sumBy (fun t -> abs t.Amount)

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | Navigate p -> { model with Page = p }, Cmd.none
    | ToggleTheme ->
        { model with Theme = (if model.Theme = Dark then Light else Dark) }, Cmd.none
    | ToggleSidebar -> { model with SidebarOpen = not model.SidebarOpen }, Cmd.none
    | SetSearch s -> { model with Search = s }, Cmd.none
    | SetCategoryFilter c -> { model with CategoryFilter = c }, Cmd.none
    | SetAccountFilter a -> { model with AccountFilter = a }, Cmd.none
    | ToggleUnreviewedOnly -> { model with UnreviewedOnly = not model.UnreviewedOnly }, Cmd.none
    | SetSort s -> { model with Sort = s }, Cmd.none
    | ToggleReviewed id ->
        let txs =
            model.Transactions
            |> List.map (fun t -> if t.Id = id then { t with Reviewed = not t.Reviewed } else t)
        { model with Transactions = txs }, Cmd.none
    | SetTxCategory (id, cat) ->
        let txs =
            model.Transactions
            |> List.map (fun t -> if t.Id = id then { t with CategoryId = cat } else t)
        let m, c = toast "Category updated"
        { model with Transactions = txs; Toast = Some m }, c
    | DeleteTx id ->
        let m, c = toast "Transaction removed"
        { model with Transactions = model.Transactions |> List.filter (fun t -> t.Id <> id); Toast = Some m }, c
    | OpenAddModal -> { model with Draft = Some emptyDraft }, Cmd.none
    | CloseModal -> { model with Draft = None }, Cmd.none
    | UpdateDraft d -> { model with Draft = Some d }, Cmd.none
    | SubmitDraft ->
        match model.Draft with
        | Some d ->
            let parsed =
                match System.Double.TryParse(d.Amount) with
                | true, v -> abs v
                | _ -> 0.0
            if d.Merchant.Trim() = "" || parsed <= 0.0 then
                let m, c = toast "Enter a merchant and a positive amount"
                { model with Toast = Some m }, c
            else
                let signed = if d.IsIncome then parsed else -parsed
                let tx =
                    { Id = model.NextTxId
                      Date = d.Date
                      Merchant = d.Merchant.Trim()
                      CategoryId = (if d.IsIncome then "income" else d.CategoryId)
                      AccountId = d.AccountId
                      Amount = signed
                      Note = d.Note.Trim()
                      Reviewed = true
                      Pending = false }
                // Reflect the transaction in the affected account's balance.
                let accts =
                    model.Accounts
                    |> List.map (fun a -> if a.Id = d.AccountId then { a with Balance = a.Balance + signed } else a)
                let m, c = toast "Transaction added"
                { model with
                    Transactions = tx :: model.Transactions
                    Accounts = accts
                    NextTxId = model.NextTxId + 1
                    Draft = None
                    Toast = Some m }, c
        | None -> model, Cmd.none
    | SetBudget (cat, limit) ->
        { model with BudgetLimits = Map.add cat (max 0.0 limit) model.BudgetLimits }, Cmd.none
    | ContributeGoal (id, amount) ->
        let goals =
            model.Goals
            |> List.map (fun g ->
                if g.Id = id then { g with Saved = min g.Target (g.Saved + amount) } else g)
        let m, c = toast (sprintf "Added %s to goal" (Format.currency0 amount))
        { model with Goals = goals; Toast = Some m }, c
    | AddGoalMonthly id ->
        let goals =
            model.Goals
            |> List.map (fun g ->
                if g.Id = id then { g with Saved = min g.Target (g.Saved + g.Monthly) } else g)
        let m, c = toast "Monthly contribution added"
        { model with Goals = goals; Toast = Some m }, c
    | ClearToast -> { model with Toast = None }, Cmd.none
