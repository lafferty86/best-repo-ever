module Charts

open System
open Feliz

/// Lightweight, dependency-free SVG charts drawn directly in F#.

/// Donut / ring chart from labelled, coloured segments.
/// Uses the stroke-dasharray technique on concentric circles — crisp at any size.
let donut (size: float) (thickness: float) (centerLabel: string) (centerSub: string) (segments: (string * float * string) list) =
    let total = segments |> List.sumBy (fun (_, v, _) -> v)
    let r = (size / 2.0) - (thickness / 2.0)
    let cx = size / 2.0
    let circ = 2.0 * Math.PI * r
    let mutable acc = 0.0
    let arcs =
        segments
        |> List.filter (fun (_, v, _) -> v > 0.0)
        |> List.map (fun (_, v, color) ->
            let frac = if total <= 0.0 then 0.0 else v / total
            let len = frac * circ
            let gap = circ - len
            let offset = -acc
            acc <- acc + len
            Svg.circle [
                svg.cx cx
                svg.cy cx
                svg.r r
                svg.fill "none"
                svg.stroke color
                svg.strokeWidth thickness
                svg.custom ("stroke-linecap", "round")
                svg.custom ("strokeDasharray", sprintf "%f %f" len gap)
                svg.custom ("strokeDashoffset", string offset)
                svg.custom ("transform", sprintf "rotate(-90 %f %f)" cx cx)
            ])
    Svg.svg [
        svg.viewBox (0, 0, int size, int size)
        svg.width size
        svg.height size
        svg.children [
            Svg.circle [
                svg.cx cx; svg.cy cx; svg.r r; svg.fill "none"
                svg.stroke "var(--track)"; svg.strokeWidth thickness
            ]
            yield! arcs
            Svg.text [
                svg.x cx; svg.y (cx - 4.0)
                svg.custom ("text-anchor", "middle")
                svg.custom ("dominant-baseline", "middle")
                svg.custom ("font-size", "18")
                svg.custom ("font-weight", "800")
                svg.fill "var(--text)"
                svg.text centerLabel
            ]
            Svg.text [
                svg.x cx; svg.y (cx + 16.0)
                svg.custom ("text-anchor", "middle")
                svg.custom ("dominant-baseline", "middle")
                svg.custom ("font-size", "10")
                svg.custom ("font-weight", "600")
                svg.custom ("letter-spacing", ".06em")
                svg.fill "var(--muted)"
                svg.text (centerSub.ToUpper())
            ]
        ]
    ]

/// Smooth-ish area + line chart. `values` maps to evenly-spaced x positions.
let areaLine (w: float) (h: float) (color: string) (gradId: string) (values: float list) =
    if List.isEmpty values then Html.none
    else
        let pad = 8.0
        let minV = List.min values
        let maxV = List.max values
        let range = if maxV - minV = 0.0 then 1.0 else maxV - minV
        let n = List.length values
        let stepX = if n <= 1 then 0.0 else (w - pad * 2.0) / float (n - 1)
        let pt i v =
            let x = pad + stepX * float i
            let y = h - pad - ((v - minV) / range) * (h - pad * 2.0)
            (x, y)
        let pts = values |> List.mapi pt
        let lineD =
            pts
            |> List.mapi (fun i (x, y) -> sprintf "%s%.2f %.2f" (if i = 0 then "M" else "L") x y)
            |> String.concat " "
        let (fx0, _) = pts.[0]
        let (fxN, _) = List.last pts
        let areaD = sprintf "%s L%.2f %.2f L%.2f %.2f Z" lineD fxN (h - pad) fx0 (h - pad)
        Svg.svg [
            svg.viewBox (0, 0, int w, int h)
            svg.custom ("width", "100%")
            svg.height h
            svg.custom ("preserveAspectRatio", "none")
            svg.children [
                Svg.defs [
                    Svg.linearGradient [
                        svg.id gradId
                        svg.x1 0.0; svg.y1 0.0; svg.x2 0.0; svg.y2 1.0
                        svg.children [
                            Svg.stop [ svg.offset 0.0; svg.stopColor color; svg.custom ("stopOpacity", "0.35") ]
                            Svg.stop [ svg.offset 1.0; svg.stopColor color; svg.custom ("stopOpacity", "0.0") ]
                        ]
                    ]
                ]
                Svg.path [ svg.d areaD; svg.fill (sprintf "url(#%s)" gradId) ]
                Svg.path [
                    svg.d lineD; svg.fill "none"; svg.stroke color
                    svg.strokeWidth 2.5; svg.custom ("stroke-linecap", "round"); svg.custom ("stroke-linejoin", "round")
                ]
                yield! pts |> List.map (fun (x, y) ->
                    Svg.circle [ svg.cx x; svg.cy y; svg.r 2.6; svg.fill color ])
            ]
        ]

/// Tiny inline sparkline for cards.
let sparkline (w: float) (h: float) (color: string) (values: float list) =
    if List.length values < 2 then Html.none
    else
        let minV = List.min values
        let maxV = List.max values
        let range = if maxV - minV = 0.0 then 1.0 else maxV - minV
        let n = List.length values
        let stepX = w / float (n - 1)
        let d =
            values
            |> List.mapi (fun i v ->
                let x = stepX * float i
                let y = h - ((v - minV) / range) * h
                sprintf "%s%.2f %.2f" (if i = 0 then "M" else "L") x y)
            |> String.concat " "
        Svg.svg [
            svg.viewBox (0, 0, int w, int h)
            svg.width w; svg.height h
            svg.children [
                Svg.path [
                    svg.d d; svg.fill "none"; svg.stroke color
                    svg.strokeWidth 2.0; svg.custom ("stroke-linecap", "round"); svg.custom ("stroke-linejoin", "round")
                ]
            ]
        ]

/// Grouped vertical bars — used for income vs. expense per month.
let groupedBars (h: float) (rows: (string * float * float) list) =
    let maxV =
        rows
        |> List.collect (fun (_, a, b) -> [ a; b ])
        |> fun xs -> if List.isEmpty xs then 1.0 else List.max xs
    let barH v = if maxV <= 0.0 then 0.0 else (v / maxV) * h
    Html.div [
        prop.className "gbars"
        prop.children [
            for (label, income, expense) in rows do
                Html.div [
                    prop.className "gbar-col"
                    prop.children [
                        Html.div [
                            prop.className "gbar-pair"
                            prop.children [
                                Html.div [
                                    prop.className "gbar income"
                                    prop.style [ style.height (length.px (barH income)) ]
                                    prop.title (Format.currency0 income)
                                ]
                                Html.div [
                                    prop.className "gbar expense"
                                    prop.style [ style.height (length.px (barH expense)) ]
                                    prop.title (Format.currency0 expense)
                                ]
                            ]
                        ]
                        Html.span [ prop.className "gbar-label"; prop.text label ]
                    ]
                ]
        ]
    ]

/// Horizontal ranked bars — used for "top spending categories".
let rankedBars (rows: (string * string * float * string) list) =
    let maxV = rows |> List.map (fun (_, _, v, _) -> v) |> fun xs -> if List.isEmpty xs then 1.0 else List.max xs
    Html.div [
        prop.className "rbars"
        prop.children [
            for (icon, label, value, color) in rows do
                Html.div [
                    prop.className "rbar-row"
                    prop.children [
                        Html.span [ prop.className "rbar-icon"; prop.text icon ]
                        Html.div [
                            prop.className "rbar-body"
                            prop.children [
                                Html.div [
                                    prop.className "rbar-head"
                                    prop.children [
                                        Html.span [ prop.className "rbar-name"; prop.text label ]
                                        Html.span [ prop.className "rbar-val"; prop.text (Format.currency value) ]
                                    ]
                                ]
                                Html.div [
                                    prop.className "rbar-track"
                                    prop.children [
                                        Html.div [
                                            prop.className "rbar-fill"
                                            prop.style [
                                                style.width (length.percent (if maxV <= 0.0 then 0.0 else value / maxV * 100.0))
                                                style.backgroundColor color
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
