module Views.CashFlow

open Feliz
open Types
open State
open Views.Shared

let view (model: Model) (dispatch: Msg -> unit) =
    let hist = Data.cashFlowHistory
    let rows = hist |> List.map (fun p -> p.Month, p.Income, p.Expense)
    let avgIncome = hist |> List.averageBy (fun p -> p.Income)
    let avgExpense = hist |> List.averageBy (fun p -> p.Expense)
    let avgSaved = avgIncome - avgExpense
    let savingsRate = if avgIncome <= 0.0 then 0.0 else avgSaved / avgIncome * 100.0
    let current = List.last hist

    let statTile (label: string) (value: string) (cls: string) =
        Html.div [
            prop.className "cf-stat"
            prop.children [
                Html.span [ prop.className "cf-stat-label"; prop.text label ]
                Html.span [ prop.className (sprintf "cf-stat-value %s" cls); prop.text value ]
            ]
        ]

    Html.div [
        prop.className "page"
        prop.children [
            Html.div [
                prop.className "cf-stat-row"
                prop.children [
                    statTile "Avg income / mo" (Format.currency0 avgIncome) "pos"
                    statTile "Avg spending / mo" (Format.currency0 avgExpense) "neg"
                    statTile "Avg saved / mo" (Format.currency0 avgSaved) "pos"
                    statTile "Savings rate" (Format.percent savingsRate) ""
                ]
            ]
            card "" [
                cardHead "Income vs. spending" (Html.div [
                    prop.className "legend-inline"
                    prop.children [
                        Html.span [ prop.className "li"; prop.children [ Html.span [ prop.className "dot"; prop.style [ style.backgroundColor "#22c55e" ] ]; Html.span [ prop.text "Income" ] ] ]
                        Html.span [ prop.className "li"; prop.children [ Html.span [ prop.className "dot"; prop.style [ style.backgroundColor "#f59e0b" ] ]; Html.span [ prop.text "Spending" ] ] ]
                    ] ])
                Charts.groupedBars 220.0 rows
            ]
            Html.div [
                prop.className "grid-2"
                prop.children [
                    card "" [
                        cardHead "Net savings trend" (Html.span [ prop.className "muted-sm"; prop.text "6 months" ])
                        Charts.areaLine 560.0 180.0 "#22c55e" "netGrad" (hist |> List.map (fun p -> p.Net))
                        Html.div [ prop.className "axis-row"; prop.children [ for p in hist -> Html.span [ prop.text p.Month ] ] ]
                    ]
                    card "" [
                        cardHead (sprintf "%s breakdown" current.Month) (Html.none)
                        Html.div [
                            prop.className "waterfall"
                            prop.children [
                                Html.div [ prop.className "wf-row"; prop.children [
                                    Html.span [ prop.className "wf-label"; prop.text "Income" ]
                                    Html.div [ prop.className "wf-bar"; prop.children [ Html.div [ prop.className "wf-fill pos"; prop.style [ style.width (length.percent 100) ] ] ] ]
                                    Html.span [ prop.className "wf-val pos"; prop.text (Format.currency0 current.Income) ] ] ]
                                Html.div [ prop.className "wf-row"; prop.children [
                                    Html.span [ prop.className "wf-label"; prop.text "Spending" ]
                                    Html.div [ prop.className "wf-bar"; prop.children [ Html.div [ prop.className "wf-fill neg"; prop.style [ style.width (length.percent (current.Expense / current.Income * 100.0)) ] ] ] ]
                                    Html.span [ prop.className "wf-val neg"; prop.text (Format.currency0 current.Expense) ] ] ]
                                Html.div [ prop.className "wf-divider" ]
                                Html.div [ prop.className "wf-row"; prop.children [
                                    Html.span [ prop.className "wf-label strong"; prop.text "Net saved" ]
                                    Html.div [ prop.className "wf-bar"; prop.children [ Html.div [ prop.className "wf-fill accent"; prop.style [ style.width (length.percent (current.Net / current.Income * 100.0)) ] ] ] ]
                                    Html.span [ prop.className "wf-val strong"; prop.text (Format.currency0 current.Net) ] ] ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
