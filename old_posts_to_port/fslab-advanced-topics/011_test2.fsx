(**
---
title: yes
category: Advanced Topics
categoryindex: 2
index: 5
---
*)


(***hide***)

(***condition:prepare***)
#r "nuget: FSharpAux, 1.1.0"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: FSharp.Stats, 0.4.3"
#r "nuget: FSharp.Data, 4.2.7"


(***condition:ipynb***)
#if IPYNB
#r "nuget: FSharpAux, 1.1.0"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.16"
#r "nuget: FSharp.Stats, 0.4.3"
#r "nuget: FSharp.Data, 4.2.7"
#endif // IPYNB

open Plotly.NET

let myChart3 = Chart.Point([1,2])

(*** condition: ipynb ***)
#if IPYNB
myChart3
#endif // IPYNB

(***hide***)
myChart3 |> GenericChart.toChartHTML
(***include-it-raw***)