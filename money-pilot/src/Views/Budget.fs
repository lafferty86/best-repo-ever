module Views.Budget

open Feliz
open Types
open State
open Views.Shared

let private budgetRow (model: Model) (dispatch: Msg -> unit) (cat: Category) (limit: float) =
    let spent = spentInCategory model.Transactions cat.Id
    let ratio = if limit <= 0.0 then 0.0 else spent / limit
    let over = spent > limit
    let barClass =
        if over then "budget-fill over"
        elif ratio > 0.85 then "budget-fill warn"
        else "budget-fill"
    Html.div [
        prop.className "budget-row"
        prop.children [
            categoryGlyph cat "md"
            Html.div [
                prop.className "budget-main"
                prop.children [
                    Html.div [
                        prop.className "budget-head"
                        prop.children [
                            Html.span [ prop.className "budget-name"; prop.text cat.Name ]
                            Html.span [
                                prop.className (if over then "budget-amt over" else "budget-amt")
                                prop.text (sprintf "%s / %s" (Format.currency0 spent) (Format.currency0 limit))
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "budget-track"
                        prop.children [
                            Html.div [
                                prop.className barClass
                                prop.style [ style.width (length.percent (min 100.0 (ratio * 100.0))); style.backgroundColor (if over then "#ef4444" else cat.Color) ]
                            ]
                        ]
                    ]
                    Html.span [
                        prop.className (if over then "budget-note over" else "budget-note")
                        prop.text (
                            if over then sprintf "%s over budget" (Format.currency0 (spent - limit))
                            else sprintf "%s left" (Format.currency0 (limit - spent)))
                    ]
                ]
            ]
            Html.div [
                prop.className "budget-stepper"
                prop.children [
                    Html.button [ prop.className "step-btn"; prop.title "Decrease budget"; prop.onClick (fun _ -> dispatch (SetBudget (cat.Id, limit - 50.0))); prop.text "−" ]
                    Html.button [ prop.className "step-btn"; prop.title "Increase budget"; prop.onClick (fun _ -> dispatch (SetBudget (cat.Id, limit + 50.0))); prop.text "＋" ]
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    let items =
        model.BudgetLimits
        |> Map.toList
        |> List.map (fun (id, limit) -> Data.categoryById id, limit)
        |> List.sortByDescending (fun (cat, _) -> spentInCategory model.Transactions cat.Id)

    let totalBudget = model.BudgetLimits |> Map.toList |> List.sumBy snd
    let totalSpent = items |> List.sumBy (fun (cat, _) -> spentInCategory model.Transactions cat.Id)
    let remaining = totalBudget - totalSpent
    let ratio = if totalBudget <= 0.0 then 0.0 else totalSpent / totalBudget
    let overCount = items |> List.filter (fun (cat, lim) -> spentInCategory model.Transactions cat.Id > lim) |> List.length

    Html.div [
        prop.className "page"
        prop.children [
            Html.div [
                prop.className "grid-2"
                prop.children [
                    card "budget-summary" [
                        Html.div [
                            prop.className "budget-donut-wrap"
                            prop.children [
                                Charts.donut 200.0 24.0 (Format.percent (ratio * 100.0)) "of budget"
                                    [ "Spent", totalSpent, (if totalSpent > totalBudget then "#ef4444" else "#6366f1")
                                      "Left", max 0.0 remaining, "var(--track)" ]
                                Html.div [
                                    prop.className "budget-summary-meta"
                                    prop.children [
                                        Html.div [ prop.className "bsm-row"; prop.children [
                                            Html.span [ prop.className "muted-sm"; prop.text "Total budget" ]
                                            Html.span [ prop.className "bsm-val"; prop.text (Format.currency0 totalBudget) ] ] ]
                                        Html.div [ prop.className "bsm-row"; prop.children [
                                            Html.span [ prop.className "muted-sm"; prop.text "Spent" ]
                                            Html.span [ prop.className "bsm-val"; prop.text (Format.currency0 totalSpent) ] ] ]
                                        Html.div [ prop.className "bsm-row"; prop.children [
                                            Html.span [ prop.className "muted-sm"; prop.text "Remaining" ]
                                            Html.span [ prop.className (if remaining >= 0.0 then "bsm-val pos" else "bsm-val neg"); prop.text (Format.currency0 remaining) ] ] ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                    card "" [
                        cardHead "This month"
                            (if overCount > 0 then Html.span [ prop.className "badge over"; prop.text (sprintf "%d over" overCount) ]
                             else Html.span [ prop.className "badge ok"; prop.text "On track" ])
                        Html.div [
                            prop.className "budget-tip"
                            prop.children [
                                Html.span [ prop.className "budget-tip-emoji"; prop.text "💡" ]
                                Html.span [
                                    prop.text (
                                        if remaining >= 0.0 then sprintf "You have %s left to spend this month. Use ＋ / − to tune any category." (Format.currency0 remaining)
                                        else sprintf "You're %s over your total plan. Trim a category or raise its limit." (Format.currency0 (abs remaining)))
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            card "" [
                cardHead "Category budgets" (Html.span [ prop.className "muted-sm"; prop.text "Adjustable" ])
                Html.div [ prop.className "budget-list"; prop.children [ for (cat, limit) in items -> budgetRow model dispatch cat limit ] ]
            ]
        ]
    ]
