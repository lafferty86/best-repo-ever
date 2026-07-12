module Views.Dashboard

open Feliz
open Types
open State
open Views.Shared

let private statTile (label: string) (value: string) (sub: ReactElement) (accent: string) =
    Html.div [
        prop.className "stat-tile"
        prop.children [
            Html.div [ prop.className "stat-accent"; prop.style [ style.backgroundColor accent ] ]
            Html.div [
                prop.children [
                    Html.span [ prop.className "stat-label"; prop.text label ]
                    Html.div [ prop.className "stat-value"; prop.text value ]
                    sub
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    let nw = netWorth model.Accounts
    let assets = assetsTotal model.Accounts
    let liabilities = liabilitiesTotal model.Accounts

    // This month's income & spend from the live transaction list.
    let income =
        model.Transactions
        |> List.filter (fun t -> t.Amount > 0.0)
        |> List.sumBy (fun t -> t.Amount)
    let spend =
        model.Transactions
        |> List.filter (fun t -> t.Amount < 0.0)
        |> List.sumBy (fun t -> abs t.Amount)

    // Spending by category → donut + ranked list.
    let byCategory =
        Data.categories
        |> List.filter (fun c -> not c.IsIncome && c.Id <> "transfer")
        |> List.map (fun c -> c, spentInCategory model.Transactions c.Id)
        |> List.filter (fun (_, v) -> v > 0.0)
        |> List.sortByDescending snd

    let donutSegs = byCategory |> List.map (fun (c, v) -> c.Name, v, c.Color)
    let topCats =
        byCategory
        |> List.truncate 5
        |> List.map (fun (c, v) -> c.Icon, c.Name, v, c.Color)

    let recent = model.Transactions |> List.sortByDescending (fun t -> t.Date) |> List.truncate 6

    let upcoming =
        model.Recurrings
        |> List.filter (fun r -> not r.IsIncome)
        |> List.sortBy (fun r -> r.NextDate)
        |> List.truncate 4

    Html.div [
        prop.className "page"
        prop.children [
            // Hero net-worth band.
            Html.div [
                prop.className "hero"
                prop.children [
                    Html.div [
                        prop.className "hero-left"
                        prop.children [
                            Html.span [ prop.className "hero-label"; prop.text "Total net worth" ]
                            Html.div [ prop.className "hero-value"; prop.text (Format.currency nw) ]
                            Html.div [
                                prop.className "hero-meta"
                                prop.children [
                                    pctChip 2.3
                                    Html.span [ prop.className "hero-meta-text"; prop.text "＋$2,926 this month" ]
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "hero-chart"
                        prop.children [
                            Charts.areaLine 520.0 120.0 "#818cf8" "heroGrad"
                                (Data.netWorthHistory |> List.map (fun p -> p.Value))
                        ]
                    ]
                ]
            ]

            // Stat row.
            Html.div [
                prop.className "stat-row"
                prop.children [
                    statTile "Assets" (Format.currency0 assets)
                        (Html.span [ prop.className "stat-sub up"; prop.text "Cash + investments" ]) "#22c55e"
                    statTile "Liabilities" (Format.currency0 liabilities)
                        (Html.span [ prop.className "stat-sub down"; prop.text "Cards + loans" ]) "#ef4444"
                    statTile "Income (mo)" (Format.currency0 income)
                        (Html.span [ prop.className "stat-sub up"; prop.text "This period" ]) "#3b82f6"
                    statTile "Spending (mo)" (Format.currency0 spend)
                        (Html.span [ prop.className "stat-sub down"; prop.text "This period" ]) "#f59e0b"
                ]
            ]

            // Main grid.
            Html.div [
                prop.className "grid-2"
                prop.children [
                    card "" [
                        cardHead "Net worth trend" (Html.span [ prop.className "muted-sm"; prop.text "Last 7 months" ])
                        Charts.areaLine 620.0 200.0 "#22c55e" "nwGrad"
                            (Data.netWorthHistory |> List.map (fun p -> p.Value))
                        Html.div [
                            prop.className "axis-row"
                            prop.children [ for p in Data.netWorthHistory -> Html.span [ prop.text p.Month ] ]
                        ]
                    ]
                    card "" [
                        cardHead "Spending by category" (Html.span [ prop.className "muted-sm"; prop.text (Format.currency0 spend) ])
                        Html.div [
                            prop.className "donut-wrap"
                            prop.children [
                                Charts.donut 180.0 22.0 (Format.currencyCompact spend) "spent" donutSegs
                                Html.div [
                                    prop.className "legend"
                                    prop.children [
                                        for (c, v) in byCategory |> List.truncate 6 do
                                            Html.div [
                                                prop.className "legend-item"
                                                prop.children [
                                                    Html.span [ prop.className "dot"; prop.style [ style.backgroundColor c.Color ] ]
                                                    Html.span [ prop.className "legend-name"; prop.text c.Name ]
                                                    Html.span [ prop.className "legend-val"; prop.text (Format.currency0 v) ]
                                                ]
                                            ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]

            Html.div [
                prop.className "grid-2"
                prop.children [
                    card "" [
                        cardHead "Recent activity"
                            (Html.button [
                                prop.className "link-btn"
                                prop.onClick (fun _ -> dispatch (Navigate Transactions))
                                prop.text "View all →" ])
                        Html.div [
                            prop.className "tx-list"
                            prop.children [
                                for t in recent do
                                    let cat = Data.categoryById t.CategoryId
                                    Html.div [
                                        prop.className "tx-mini"
                                        prop.children [
                                            categoryGlyph cat "sm"
                                            Html.div [
                                                prop.className "tx-mini-main"
                                                prop.children [
                                                    Html.span [ prop.className "tx-merchant"; prop.text t.Merchant ]
                                                    Html.span [ prop.className "tx-sub"; prop.text (Format.shortDate t.Date + " · " + cat.Name) ]
                                                ]
                                            ]
                                            Html.span [
                                                prop.className (if t.Amount >= 0.0 then "tx-amt pos" else "tx-amt")
                                                prop.text (Format.currency t.Amount)
                                            ]
                                        ]
                                    ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "stack"
                        prop.children [
                            card "" [
                                cardHead "Top categories" (Html.none)
                                Charts.rankedBars topCats
                            ]
                            card "" [
                                cardHead "Upcoming bills"
                                    (Html.button [
                                        prop.className "link-btn"
                                        prop.onClick (fun _ -> dispatch (Navigate Recurring))
                                        prop.text "All →" ])
                                Html.div [
                                    prop.className "bill-list"
                                    prop.children [
                                        for r in upcoming do
                                            Html.div [
                                                prop.className "bill-row"
                                                prop.children [
                                                    Html.span [
                                                        prop.className "bill-icon"
                                                        prop.style [ style.custom ("background", r.Color + "22") ]
                                                        prop.text r.Icon
                                                    ]
                                                    Html.div [
                                                        prop.className "bill-main"
                                                        prop.children [
                                                            Html.span [ prop.className "tx-merchant"; prop.text r.Merchant ]
                                                            Html.span [ prop.className "tx-sub"; prop.text (Format.shortDate r.NextDate) ]
                                                        ]
                                                    ]
                                                    Html.span [ prop.className "tx-amt"; prop.text (Format.currency r.Amount) ]
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
