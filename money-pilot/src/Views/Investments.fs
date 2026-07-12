module Views.Investments

open Feliz
open Types
open State
open Views.Shared

let view (model: Model) (dispatch: Msg -> unit) =
    let holdings = model.Holdings |> List.sortByDescending (fun h -> h.Value)
    let totalValue = holdings |> List.sumBy (fun h -> h.Value)
    let totalCost = holdings |> List.sumBy (fun h -> h.Cost)
    let totalGain = totalValue - totalCost
    let totalGainPct = if totalCost <= 0.0 then 0.0 else totalGain / totalCost * 100.0
    let segs = holdings |> List.map (fun h -> h.Symbol, h.Value, h.Color)

    Html.div [
        prop.className "page"
        prop.children [
            Html.div [
                prop.className "hero invest-hero"
                prop.children [
                    Html.div [
                        prop.className "hero-left"
                        prop.children [
                            Html.span [ prop.className "hero-label"; prop.text "Portfolio value" ]
                            Html.div [ prop.className "hero-value"; prop.text (Format.currency totalValue) ]
                            Html.div [
                                prop.className "hero-meta"
                                prop.children [
                                    pctChip totalGainPct
                                    Html.span [
                                        prop.className (if totalGain >= 0.0 then "hero-meta-text pos" else "hero-meta-text neg")
                                        prop.text (sprintf "%s all-time" (Format.currency totalGain))
                                    ]
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "hero-donut"
                        prop.children [ Charts.donut 150.0 20.0 (Format.currencyCompact totalValue) "invested" segs ]
                    ]
                ]
            ]
            Html.div [
                prop.className "grid-2"
                prop.children [
                    card "" [
                        cardHead "Holdings" (Html.span [ prop.className "muted-sm"; prop.text (sprintf "%d positions" (List.length holdings)) ])
                        Html.div [
                            prop.className "holding-list"
                            prop.children [
                                for h in holdings do
                                    Html.div [
                                        prop.className "holding-row"
                                        prop.children [
                                            Html.div [ prop.className "holding-sym"; prop.style [ style.custom ("background", h.Color + "22"); style.color h.Color ]; prop.text h.Symbol ]
                                            Html.div [
                                                prop.className "holding-main"
                                                prop.children [
                                                    Html.span [ prop.className "tx-merchant"; prop.text h.Name ]
                                                    Html.span [ prop.className "tx-sub"; prop.text (sprintf "%.0f sh · %s" h.Shares (Format.currency h.Price)) ]
                                                ]
                                            ]
                                            Html.div [
                                                prop.className "holding-right"
                                                prop.children [
                                                    Html.span [ prop.className "holding-val"; prop.text (Format.currency0 h.Value) ]
                                                    pctChip h.GainPct
                                                ]
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
                                cardHead "Allocation" (Html.none)
                                Html.div [
                                    prop.className "alloc-list"
                                    prop.children [
                                        for h in holdings do
                                            let pct = if totalValue <= 0.0 then 0.0 else h.Value / totalValue * 100.0
                                            Html.div [
                                                prop.className "alloc-row"
                                                prop.children [
                                                    Html.span [ prop.className "alloc-sym"; prop.text h.Symbol ]
                                                    Html.div [ prop.className "alloc-track"; prop.children [ Html.div [ prop.className "alloc-fill"; prop.style [ style.width (length.percent pct); style.backgroundColor h.Color ] ] ] ]
                                                    Html.span [ prop.className "alloc-pct"; prop.text (Format.percent pct) ]
                                                ]
                                            ]
                                    ]
                                ]
                            ]
                            card "" [
                                cardHead "Summary" (Html.none)
                                Html.div [ prop.className "kv-list"; prop.children [
                                    Html.div [ prop.className "kv"; prop.children [ Html.span [ prop.text "Cost basis" ]; Html.span [ prop.className "kv-val"; prop.text (Format.currency0 totalCost) ] ] ]
                                    Html.div [ prop.className "kv"; prop.children [ Html.span [ prop.text "Market value" ]; Html.span [ prop.className "kv-val"; prop.text (Format.currency0 totalValue) ] ] ]
                                    Html.div [ prop.className "kv"; prop.children [ Html.span [ prop.text "Total return" ]; Html.span [ prop.className (if totalGain >= 0.0 then "kv-val pos" else "kv-val neg"); prop.text (sprintf "%s (%s)" (Format.currency0 totalGain) (Format.percentSigned totalGainPct)) ] ] ]
                                ] ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
