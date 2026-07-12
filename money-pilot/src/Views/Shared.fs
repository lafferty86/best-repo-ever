module Views.Shared

open Feliz
open Types
open State

/// Reusable building blocks and the app chrome (sidebar, top bar, modal, toast).

let netWorth (accounts: Account list) =
    accounts |> List.sumBy (fun a -> a.NetContribution)

let assetsTotal (accounts: Account list) =
    accounts |> List.filter (fun a -> not a.Kind.IsLiability) |> List.sumBy (fun a -> a.Balance)

let liabilitiesTotal (accounts: Account list) =
    accounts |> List.filter (fun a -> a.Kind.IsLiability) |> List.sumBy (fun a -> abs a.Balance)

let navItems =
    [ Dashboard,    "\U0001F9ED", "Dashboard"
      Accounts,     "\U0001F3E6", "Accounts"
      Transactions, "\U0001F4B3", "Transactions"
      Budget,       "\U0001F4CA", "Budget"
      CashFlow,     "\U0001F30A", "Cash Flow"
      Investments,  "\U0001F4C8", "Investments"
      Recurring,    "\U0001F501", "Recurring"
      Goals,        "\U0001F3AF", "Goals" ]

let pageTitle =
    function
    | Dashboard -> "Dashboard"
    | Accounts -> "Accounts"
    | Transactions -> "Transactions"
    | Budget -> "Budget"
    | CashFlow -> "Cash Flow"
    | Investments -> "Investments"
    | Recurring -> "Recurring"
    | Goals -> "Goals"

let pageSubtitle =
    function
    | Dashboard -> "Your money at a glance"
    | Accounts -> "Every account in one cockpit"
    | Transactions -> "Search, categorize and review"
    | Budget -> "Spending against your plan"
    | CashFlow -> "What comes in, what goes out"
    | Investments -> "Portfolio performance & allocation"
    | Recurring -> "Bills, subscriptions & paychecks"
    | Goals -> "Save toward what matters"

let card (extraClass: string) (children: ReactElement list) =
    Html.div [ prop.className (sprintf "card %s" extraClass); prop.children children ]

let cardHead (title: string) (right: ReactElement) =
    Html.div [
        prop.className "card-head"
        prop.children [
            Html.h3 [ prop.className "card-title"; prop.text title ]
            right
        ]
    ]

let trendChip (value: float) =
    let cls = if value >= 0.0 then "chip up" else "chip down"
    let arrow = if value >= 0.0 then "▲" else "▼"
    Html.span [
        prop.className cls
        prop.children [
            Html.span [ prop.className "chip-arrow"; prop.text arrow ]
            Html.span [ prop.text (Format.currency0 (abs value)) ]
        ]
    ]

let pctChip (value: float) =
    let cls = if value >= 0.0 then "chip up" else "chip down"
    let arrow = if value >= 0.0 then "▲" else "▼"
    Html.span [
        prop.className cls
        prop.children [
            Html.span [ prop.className "chip-arrow"; prop.text arrow ]
            Html.span [ prop.text (Format.percentSigned value |> fun s -> s.TrimStart('+')) ]
        ]
    ]

let categoryGlyph (cat: Category) (sizeClass: string) =
    Html.div [
        prop.className (sprintf "glyph %s" sizeClass)
        prop.style [ style.custom ("background", cat.Color + "22"); style.color cat.Color ]
        prop.children [ Html.span [ prop.text cat.Icon ] ]
    ]

let sidebar (model: Model) (dispatch: Msg -> unit) =
    Html.aside [
        prop.className (if model.SidebarOpen then "sidebar" else "sidebar collapsed")
        prop.children [
            Html.div [
                prop.className "brand"
                prop.children [
                    Html.div [
                        prop.className "brand-mark"
                        prop.children [
                            Svg.svg [
                                svg.viewBox (0, 0, 32, 32)
                                svg.width 26; svg.height 26
                                svg.children [
                                    Svg.rect [ svg.x 0; svg.y 0; svg.width 32; svg.height 32; svg.rx 8; svg.fill "#6366f1" ]
                                    Svg.path [
                                        svg.d "M9 21l5-6 4 3 5-8"; svg.stroke "white"; svg.strokeWidth 2.5
                                        svg.fill "none"; svg.custom ("stroke-linecap", "round"); svg.custom ("stroke-linejoin", "round")
                                    ]
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "brand-text"
                        prop.children [
                            Html.span [ prop.className "brand-name"; prop.text "Money Pilot" ]
                            Html.span [ prop.className "brand-tag"; prop.text "Financial cockpit" ]
                        ]
                    ]
                ]
            ]
            Html.nav [
                prop.className "nav"
                prop.children [
                    for (page, icon, label) in navItems do
                        Html.button [
                            prop.className (if model.Page = page then "nav-item active" else "nav-item")
                            prop.onClick (fun _ -> dispatch (Navigate page))
                            prop.children [
                                Html.span [ prop.className "nav-icon"; prop.text icon ]
                                Html.span [ prop.className "nav-label"; prop.text label ]
                            ]
                        ]
                ]
            ]
            Html.div [
                prop.className "sidebar-foot"
                prop.children [
                    Html.button [
                        prop.className "add-btn"
                        prop.onClick (fun _ -> dispatch OpenAddModal)
                        prop.children [
                            Html.span [ prop.text "＋" ]
                            Html.span [ prop.className "nav-label"; prop.text "Add transaction" ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let topbar (model: Model) (dispatch: Msg -> unit) =
    Html.header [
        prop.className "topbar"
        prop.children [
            Html.div [
                prop.className "topbar-left"
                prop.children [
                    Html.button [
                        prop.className "icon-btn"
                        prop.onClick (fun _ -> dispatch ToggleSidebar)
                        prop.text "☰"
                    ]
                    Html.div [
                        prop.children [
                            Html.h1 [ prop.className "page-title"; prop.text (pageTitle model.Page) ]
                            Html.p [ prop.className "page-sub"; prop.text (pageSubtitle model.Page) ]
                        ]
                    ]
                ]
            ]
            Html.div [
                prop.className "topbar-right"
                prop.children [
                    Html.button [
                        prop.className "icon-btn"
                        prop.title "Toggle theme"
                        prop.onClick (fun _ -> dispatch ToggleTheme)
                        prop.text (if model.Theme = Dark then "☀" else "\U0001F319")
                    ]
                    Html.button [
                        prop.className "primary-btn"
                        prop.onClick (fun _ -> dispatch OpenAddModal)
                        prop.children [
                            Html.span [ prop.text "＋" ]
                            Html.span [ prop.text "Add" ]
                        ]
                    ]
                    Html.div [ prop.className "avatar"; prop.text "AL" ]
                ]
            ]
        ]
    ]

let toast (model: Model) =
    match model.Toast with
    | Some msg ->
        Html.div [ prop.className "toast"; prop.children [ Html.span [ prop.text "✓" ]; Html.span [ prop.text msg ] ] ]
    | None -> Html.none

let private field (label: string) (control: ReactElement) =
    Html.label [
        prop.className "field"
        prop.children [ Html.span [ prop.className "field-label"; prop.text label ]; control ]
    ]

let modal (model: Model) (dispatch: Msg -> unit) =
    match model.Draft with
    | None -> Html.none
    | Some d ->
        Html.div [
            prop.className "modal-overlay"
            prop.onClick (fun _ -> dispatch CloseModal)
            prop.children [
                Html.div [
                    prop.className "modal"
                    prop.onClick (fun e -> e.stopPropagation ())
                    prop.children [
                        Html.div [
                            prop.className "modal-head"
                            prop.children [
                                Html.h3 [ prop.text "Add transaction" ]
                                Html.button [ prop.className "icon-btn"; prop.onClick (fun _ -> dispatch CloseModal); prop.text "✕" ]
                            ]
                        ]
                        Html.div [
                            prop.className "seg"
                            prop.children [
                                Html.button [
                                    prop.className (if not d.IsIncome then "seg-item active" else "seg-item")
                                    prop.onClick (fun _ -> dispatch (UpdateDraft { d with IsIncome = false }))
                                    prop.text "Expense"
                                ]
                                Html.button [
                                    prop.className (if d.IsIncome then "seg-item active" else "seg-item")
                                    prop.onClick (fun _ -> dispatch (UpdateDraft { d with IsIncome = true }))
                                    prop.text "Income"
                                ]
                            ]
                        ]
                        Html.div [
                            prop.className "modal-body"
                            prop.children [
                                field "Merchant" (Html.input [
                                    prop.className "input"; prop.placeholder "e.g. Whole Foods"
                                    prop.value d.Merchant
                                    prop.onChange (fun (v: string) -> dispatch (UpdateDraft { d with Merchant = v }))
                                ])
                                Html.div [
                                    prop.className "field-row"
                                    prop.children [
                                        field "Amount" (Html.input [
                                            prop.className "input"; prop.placeholder "0.00"; prop.type' "number"
                                            prop.value d.Amount
                                            prop.onChange (fun (v: string) -> dispatch (UpdateDraft { d with Amount = v }))
                                        ])
                                        field "Date" (Html.input [
                                            prop.className "input"; prop.type' "date"
                                            prop.value d.Date
                                            prop.onChange (fun (v: string) -> dispatch (UpdateDraft { d with Date = v }))
                                        ])
                                    ]
                                ]
                                if not d.IsIncome then
                                    field "Category" (Html.select [
                                        prop.className "input"
                                        prop.value d.CategoryId
                                        prop.onChange (fun (v: string) -> dispatch (UpdateDraft { d with CategoryId = v }))
                                        prop.children [
                                            for c in Data.categories do
                                                if not c.IsIncome && c.Id <> "transfer" then
                                                    Html.option [ prop.value c.Id; prop.text (sprintf "%s  %s" c.Icon c.Name) ]
                                        ]
                                    ])
                                field "Account" (Html.select [
                                    prop.className "input"
                                    prop.value (string d.AccountId)
                                    prop.onChange (fun (v: string) -> dispatch (UpdateDraft { d with AccountId = int v }))
                                    prop.children [
                                        for a in model.Accounts do
                                            Html.option [ prop.value (string a.Id); prop.text (sprintf "%s ••%s" a.Name a.Mask) ]
                                    ]
                                ])
                                field "Note (optional)" (Html.input [
                                    prop.className "input"; prop.placeholder "Add a note"
                                    prop.value d.Note
                                    prop.onChange (fun (v: string) -> dispatch (UpdateDraft { d with Note = v }))
                                ])
                            ]
                        ]
                        Html.div [
                            prop.className "modal-foot"
                            prop.children [
                                Html.button [ prop.className "ghost-btn"; prop.onClick (fun _ -> dispatch CloseModal); prop.text "Cancel" ]
                                Html.button [ prop.className "primary-btn"; prop.onClick (fun _ -> dispatch SubmitDraft); prop.text "Save transaction" ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
