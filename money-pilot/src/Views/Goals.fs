module Views.Goals

open Feliz
open Types
open State
open Views.Shared

let view (model: Model) (dispatch: Msg -> unit) =
    let totalTarget = model.Goals |> List.sumBy (fun g -> g.Target)
    let totalSaved = model.Goals |> List.sumBy (fun g -> g.Saved)

    let goalCard (g: Goal) =
        let ratio = g.Ratio
        let pct = ratio * 100.0
        let complete = g.Saved >= g.Target
        Html.div [
            prop.className "goal-card"
            prop.children [
                Html.div [
                    prop.className "goal-top"
                    prop.children [
                        Html.span [ prop.className "goal-icon"; prop.style [ style.custom ("background", g.Color + "22"); style.color g.Color ]; prop.text g.Icon ]
                        Html.div [
                            prop.className "goal-titles"
                            prop.children [
                                Html.span [ prop.className "goal-name"; prop.text g.Name ]
                                Html.span [ prop.className "tx-sub"; prop.text (sprintf "Target %s · by %s" (Format.currency0 g.Target) (Format.longDate (g.TargetDate + "-01") |> fun s -> s.Replace(" 1,", ""))) ]
                            ]
                        ]
                        if complete then Html.span [ prop.className "badge ok"; prop.text "Reached 🎉" ]
                    ]
                ]
                Html.div [
                    prop.className "goal-amounts"
                    prop.children [
                        Html.span [ prop.className "goal-saved"; prop.text (Format.currency0 g.Saved) ]
                        Html.span [ prop.className "goal-of"; prop.text (sprintf "of %s" (Format.currency0 g.Target)) ]
                        Html.span [ prop.className "goal-pct"; prop.style [ style.color g.Color ]; prop.text (Format.percent pct) ]
                    ]
                ]
                Html.div [
                    prop.className "goal-track"
                    prop.children [ Html.div [ prop.className "goal-fill"; prop.style [ style.width (length.percent (min 100.0 pct)); style.backgroundColor g.Color ] ] ]
                ]
                Html.div [
                    prop.className "goal-foot"
                    prop.children [
                        Html.span [ prop.className "tx-sub"; prop.text (
                            if complete then "Fully funded"
                            else sprintf "%s to go · %s/mo" (Format.currency0 (g.Target - g.Saved)) (Format.currency0 g.Monthly)) ]
                        Html.div [
                            prop.className "goal-btns"
                            prop.children [
                                Html.button [ prop.className "mini-btn"; prop.disabled complete; prop.onClick (fun _ -> dispatch (ContributeGoal (g.Id, 100.0))); prop.text "+ $100" ]
                                Html.button [ prop.className "mini-btn primary"; prop.disabled complete; prop.onClick (fun _ -> dispatch (AddGoalMonthly g.Id)); prop.text (sprintf "+ %s" (Format.currency0 g.Monthly)) ]
                            ]
                        ]
                    ]
                ]
            ]
        ]

    Html.div [
        prop.className "page"
        prop.children [
            card "goals-summary" [
                Html.div [
                    prop.className "goals-summary-inner"
                    prop.children [
                        Charts.donut 160.0 20.0 (Format.percent (if totalTarget <= 0.0 then 0.0 else totalSaved / totalTarget * 100.0)) "funded"
                            [ "Saved", totalSaved, "#6366f1"; "Left", max 0.0 (totalTarget - totalSaved), "var(--track)" ]
                        Html.div [
                            prop.className "goals-summary-text"
                            prop.children [
                                Html.span [ prop.className "muted-sm"; prop.text "Saved across all goals" ]
                                Html.div [ prop.className "goals-big"; prop.text (Format.currency0 totalSaved) ]
                                Html.span [ prop.className "tx-sub"; prop.text (sprintf "of %s total · %s remaining" (Format.currency0 totalTarget) (Format.currency0 (totalTarget - totalSaved))) ]
                            ]
                        ]
                    ]
                ]
            ]
            Html.div [ prop.className "goal-grid"; prop.children [ for g in model.Goals -> goalCard g ] ]
        ]
    ]
