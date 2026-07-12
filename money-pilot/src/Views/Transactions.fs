module Views.Transactions

open Feliz
open Types
open State
open Views.Shared

let private matchesSearch (s: string) (t: Transaction) =
    let q = s.Trim().ToLower()
    q = ""
    || t.Merchant.ToLower().Contains(q)
    || t.Note.ToLower().Contains(q)
    || (Data.categoryById t.CategoryId).Name.ToLower().Contains(q)

let private filtered (model: Model) =
    model.Transactions
    |> List.filter (matchesSearch model.Search)
    |> List.filter (fun t ->
        match model.CategoryFilter with Some c -> t.CategoryId = c | None -> true)
    |> List.filter (fun t ->
        match model.AccountFilter with Some a -> t.AccountId = a | None -> true)
    |> List.filter (fun t -> not model.UnreviewedOnly || not t.Reviewed)
    |> (fun xs ->
        match model.Sort with
        | ByDate -> xs |> List.sortByDescending (fun t -> t.Date)
        | ByAmount -> xs |> List.sortBy (fun t -> t.Amount))

let private txRow (model: Model) (dispatch: Msg -> unit) (t: Transaction) =
    let cat = Data.categoryById t.CategoryId
    Html.div [
        prop.className (if t.Reviewed then "tx-row" else "tx-row unreviewed")
        prop.children [
            categoryGlyph cat "md"
            Html.div [
                prop.className "tx-row-main"
                prop.children [
                    Html.div [
                        prop.className "tx-row-top"
                        prop.children [
                            Html.span [ prop.className "tx-merchant"; prop.text t.Merchant ]
                            if t.Pending then Html.span [ prop.className "badge pending"; prop.text "Pending" ]
                        ]
                    ]
                    Html.span [
                        prop.className "tx-sub"
                        prop.text (sprintf "%s · %s ••%s" (Format.shortDate t.Date) (Data.accountName t.AccountId) (Data.accountById t.AccountId |> Option.map (fun a -> a.Mask) |> Option.defaultValue ""))
                    ]
                    if t.Note <> "" then Html.span [ prop.className "tx-note"; prop.text ("“" + t.Note + "”") ]
                ]
            ]
            Html.select [
                prop.className "cat-select"
                prop.value t.CategoryId
                prop.onChange (fun (v: string) -> dispatch (SetTxCategory (t.Id, v)))
                prop.children [
                    for c in Data.categories do
                        Html.option [ prop.value c.Id; prop.text (sprintf "%s %s" c.Icon c.Name) ]
                ]
            ]
            Html.span [
                prop.className (if t.Amount >= 0.0 then "tx-amt pos big" else "tx-amt big")
                prop.text (Format.currency t.Amount)
            ]
            Html.div [
                prop.className "tx-actions"
                prop.children [
                    Html.button [
                        prop.className (if t.Reviewed then "review-btn done" else "review-btn")
                        prop.title (if t.Reviewed then "Reviewed" else "Mark reviewed")
                        prop.onClick (fun _ -> dispatch (ToggleReviewed t.Id))
                        prop.text (if t.Reviewed then "✓" else "○")
                    ]
                    Html.button [
                        prop.className "del-btn"
                        prop.title "Delete"
                        prop.onClick (fun _ -> dispatch (DeleteTx t.Id))
                        prop.text "🗑"
                    ]
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    let rows = filtered model
    let inflow = rows |> List.filter (fun t -> t.Amount > 0.0) |> List.sumBy (fun t -> t.Amount)
    let outflow = rows |> List.filter (fun t -> t.Amount < 0.0) |> List.sumBy (fun t -> abs t.Amount)
    let unreviewedCount = model.Transactions |> List.filter (fun t -> not t.Reviewed) |> List.length

    Html.div [
        prop.className "page"
        prop.children [
            // Toolbar.
            Html.div [
                prop.className "toolbar"
                prop.children [
                    Html.div [
                        prop.className "search"
                        prop.children [
                            Html.span [ prop.className "search-icon"; prop.text "🔍" ]
                            Html.input [
                                prop.className "search-input"
                                prop.placeholder "Search merchants, notes, categories…"
                                prop.value model.Search
                                prop.onChange (fun (v: string) -> dispatch (SetSearch v))
                            ]
                        ]
                    ]
                    Html.select [
                        prop.className "filter-select"
                        prop.value (model.CategoryFilter |> Option.defaultValue "")
                        prop.onChange (fun (v: string) ->
                            dispatch (SetCategoryFilter (if v = "" then None else Some v)))
                        prop.children [
                            Html.option [ prop.value ""; prop.text "All categories" ]
                            for c in Data.categories do
                                Html.option [ prop.value c.Id; prop.text (sprintf "%s %s" c.Icon c.Name) ]
                        ]
                    ]
                    Html.select [
                        prop.className "filter-select"
                        prop.value (model.AccountFilter |> Option.map string |> Option.defaultValue "")
                        prop.onChange (fun (v: string) ->
                            dispatch (SetAccountFilter (if v = "" then None else Some (int v))))
                        prop.children [
                            Html.option [ prop.value ""; prop.text "All accounts" ]
                            for a in model.Accounts do
                                Html.option [ prop.value (string a.Id); prop.text a.Name ]
                        ]
                    ]
                    Html.select [
                        prop.className "filter-select"
                        prop.value (match model.Sort with ByDate -> "date" | ByAmount -> "amount")
                        prop.onChange (fun (v: string) -> dispatch (SetSort (if v = "amount" then ByAmount else ByDate)))
                        prop.children [
                            Html.option [ prop.value "date"; prop.text "Sort: Date" ]
                            Html.option [ prop.value "amount"; prop.text "Sort: Amount" ]
                        ]
                    ]
                    Html.button [
                        prop.className (if model.UnreviewedOnly then "chip-btn active" else "chip-btn")
                        prop.onClick (fun _ -> dispatch ToggleUnreviewedOnly)
                        prop.text (sprintf "To review (%d)" unreviewedCount)
                    ]
                ]
            ]

            // Summary strip.
            Html.div [
                prop.className "summary-strip"
                prop.children [
                    Html.div [ prop.className "summary-cell"; prop.children [
                        Html.span [ prop.className "summary-label"; prop.text "Showing" ]
                        Html.span [ prop.className "summary-val"; prop.text (sprintf "%d transactions" (List.length rows)) ] ] ]
                    Html.div [ prop.className "summary-cell"; prop.children [
                        Html.span [ prop.className "summary-label"; prop.text "Inflow" ]
                        Html.span [ prop.className "summary-val pos"; prop.text (Format.currency0 inflow) ] ] ]
                    Html.div [ prop.className "summary-cell"; prop.children [
                        Html.span [ prop.className "summary-label"; prop.text "Outflow" ]
                        Html.span [ prop.className "summary-val neg"; prop.text (Format.currency0 outflow) ] ] ]
                    Html.div [ prop.className "summary-cell"; prop.children [
                        Html.span [ prop.className "summary-label"; prop.text "Net" ]
                        Html.span [ prop.className (if inflow - outflow >= 0.0 then "summary-val pos" else "summary-val neg"); prop.text (Format.currency0 (inflow - outflow)) ] ] ]
                ]
            ]

            // The list.
            card "" [
                if List.isEmpty rows then
                    Html.div [ prop.className "empty"; prop.children [
                        Html.span [ prop.className "empty-emoji"; prop.text "🔍" ]
                        Html.span [ prop.text "No transactions match your filters." ] ] ]
                else
                    Html.div [ prop.className "tx-full-list"; prop.children [ for t in rows -> txRow model dispatch t ] ]
            ]
        ]
    ]
