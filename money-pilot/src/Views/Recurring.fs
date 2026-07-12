module Views.Recurring

open Feliz
open Types
open State
open Views.Shared

/// Normalize any cadence to an approximate monthly cost for totals.
let private monthlyEquivalent (r: Recurring) =
    match r.Cadence with
    | Weekly -> r.Amount * 52.0 / 12.0
    | Monthly -> r.Amount
    | Yearly -> r.Amount / 12.0

let view (model: Model) (dispatch: Msg -> unit) =
    let bills = model.Recurrings |> List.filter (fun r -> not r.IsIncome) |> List.sortBy (fun r -> r.NextDate)
    let income = model.Recurrings |> List.filter (fun r -> r.IsIncome)
    let monthlyBills = bills |> List.sumBy monthlyEquivalent
    let monthlyIncome = income |> List.sumBy monthlyEquivalent
    let subs = bills |> List.filter (fun r -> r.CategoryId = "subs")
    let subsTotal = subs |> List.sumBy monthlyEquivalent

    let recurRow (r: Recurring) =
        Html.div [
            prop.className "recur-row"
            prop.children [
                Html.span [ prop.className "recur-icon"; prop.style [ style.custom ("background", r.Color + "22") ]; prop.text r.Icon ]
                Html.div [
                    prop.className "recur-main"
                    prop.children [
                        Html.span [ prop.className "tx-merchant"; prop.text r.Merchant ]
                        Html.span [ prop.className "tx-sub"; prop.text (sprintf "%s · next %s" r.Cadence.Label (Format.shortDate r.NextDate)) ]
                    ]
                ]
                Html.span [ prop.className "recur-cadence"; prop.text r.Cadence.Label ]
                Html.span [
                    prop.className (if r.IsIncome then "tx-amt pos big" else "tx-amt big")
                    prop.text ((if r.IsIncome then "+" else "") + Format.currency r.Amount)
                ]
            ]
        ]

    Html.div [
        prop.className "page"
        prop.children [
            Html.div [
                prop.className "cf-stat-row"
                prop.children [
                    Html.div [ prop.className "cf-stat"; prop.children [ Html.span [ prop.className "cf-stat-label"; prop.text "Recurring income / mo" ]; Html.span [ prop.className "cf-stat-value pos"; prop.text (Format.currency0 monthlyIncome) ] ] ]
                    Html.div [ prop.className "cf-stat"; prop.children [ Html.span [ prop.className "cf-stat-label"; prop.text "Recurring bills / mo" ]; Html.span [ prop.className "cf-stat-value neg"; prop.text (Format.currency0 monthlyBills) ] ] ]
                    Html.div [ prop.className "cf-stat"; prop.children [ Html.span [ prop.className "cf-stat-label"; prop.text "Subscriptions / mo" ]; Html.span [ prop.className "cf-stat-value"; prop.text (Format.currency0 subsTotal) ] ] ]
                    Html.div [ prop.className "cf-stat"; prop.children [ Html.span [ prop.className "cf-stat-label"; prop.text "Net recurring / mo" ]; Html.span [ prop.className "cf-stat-value pos"; prop.text (Format.currency0 (monthlyIncome - monthlyBills)) ] ] ]
                ]
            ]
            Html.div [
                prop.className "grid-2"
                prop.children [
                    card "" [
                        cardHead "Bills & subscriptions" (Html.span [ prop.className "muted-sm"; prop.text (sprintf "%d active" (List.length bills)) ])
                        Html.div [ prop.className "recur-list"; prop.children [ for r in bills -> recurRow r ] ]
                    ]
                    Html.div [
                        prop.className "stack"
                        prop.children [
                            card "" [
                                cardHead "Income" (Html.none)
                                Html.div [ prop.className "recur-list"; prop.children [ for r in income -> recurRow r ] ]
                            ]
                            card "" [
                                cardHead "Subscription watch" (Html.none)
                                Html.div [
                                    prop.className "sub-cloud"
                                    prop.children [
                                        for r in subs do
                                            Html.span [
                                                prop.className "sub-chip"
                                                prop.style [ style.custom ("borderColor", r.Color + "55") ]
                                                prop.children [
                                                    Html.span [ prop.text r.Icon ]
                                                    Html.span [ prop.text r.Merchant ]
                                                    Html.span [ prop.className "sub-price"; prop.text (Format.currency r.Amount) ]
                                                ]
                                            ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
