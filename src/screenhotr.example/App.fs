﻿namespace screenhotr.example

open FSharp.Data.Adaptive

open Aardvark.Application
open Aardvark.Base
open Aardvark.Rendering
open Aardvark.UI
open Aardvark.UI.Primitives
open Aardvark.UI.Screenshotr

open screenhotr.example.Model

module App =
    
    let initial (myUrl : string) = 
        { 
            cameraState = FreeFlyController.initial
            imageSize   = V2i(1024, 768)
            tags        = Array.empty
            screenshotr = ScreenshotrModel.Default myUrl
        }

    let update (m : Model) (msg : Message) =
        match msg with
            | CameraMessage msg -> { m with cameraState = FreeFlyController.update m.cameraState msg }
            
            | ScreenshoterMessage msg -> 
                { m with screenshotr = Update.update msg m.screenshotr }
            
            | Message.KeyDown k -> 
                match k with
                | Keys.F8 -> 
                    { m with screenshotr = { m.screenshotr with uiIsVisible = not m.screenshotr.uiIsVisible } } 
                    // todo: gscheid machen! (message an sub-app schicken)
                | _ -> m


    let view (m : AdaptiveModel) =

        let frustum = 
            Frustum.perspective 60.0 0.1 100.0 1.0 
                |> AVal.constant

        let scene =
            [| 
                Sg.sphere' 8 C4b.ForestGreen 1.0
                Sg.box' C4b.CornflowerBlue Box3d.Unit |> Sg.translate 2.0 1.0 0.0
                Sg.cone' 125 C4b.DarkYellow 1.0 2.0 |> Sg.translate (-2.0) -3.0 0.0
                Sg.cylinder' 30 C4b.Tomato 1.0 2.0 |> Sg.translate 2.0 -3.0 0.0
            |]
            |> Sg.ofArray
            |> Sg.shader {
                do! DefaultSurfaces.trafo
                do! DefaultSurfaces.simpleLighting
            }

        let att =
            [
                style "position: fixed; left: 0; top: 0; width: 100%; height: 100%"
                onKeyDown (KeyDown)
            ]

        let dependencies = 
            [
                { kind = Script; name = "screenshot"; url = "screenshot.js" }
            ] @ Html.semui

        body [] [
        
            FreeFlyController.controlledControl m.cameraState CameraMessage frustum (AttributeMap.ofList att) scene

            ScreenshotrView.screenshotrUI m.screenshotr |> UI.map ScreenshoterMessage
              
        ]
        |>  require dependencies

    let app (myUrl : string) =
        {
            initial = initial myUrl
            update = update
            view = view
            threads = fun m -> m.cameraState |> FreeFlyController.threads |> ThreadPool.map CameraMessage
            unpersist = Unpersist.instance
        }
