﻿namespace Aardvark.UI.Screenshotr

open Aardvark.Base
open Adaptify
open FSharp.Data.Adaptive

type ScreenshotType =
    | WithoutUI
    | WithUI

type ScreenshotrMessage = 
    | SetCredentialsInputUrl of string
    | SetCredentialsInputKey of string
    | SetCredentials
    | ToggleScreenshotUI    of ScreenshotType
    | CloseScreenshotUI // makes UI invisible
    | CancelScreenshotUI // makes UI invisible and clears tags, caption and credentials
    | TakeScreenshotWithoutUI
    | TakeScreenshotWithUI  of byte[]  
    | SetImageWidth         of int
    | SetImageHeight        of int
    | SetTags               of string
    | SetCaption            of string
    | SetCredits            of string
    | ToggleInternalUseOnly

type ClientStatistics =
    {
        session         : System.Guid
        name            : string
        frameCount      : int
        invalidateTime  : float
        renderTime      : float
        compressTime    : float
        frameTime       : float
    }

[<ModelType>]
type ScreenshotrModel = {
    credentialsInputUrl : string
    credentialsInputKey : string
    credentials         : Credentials
    aardvarkUrl         : string
    imageSize           : Screenshotr.ImgSize
    defaultTags         : HashSet<string>
    tags                : HashSet<string>
    withoutUiIsVisible  : bool
    withUiIsVisible     : bool
    caption             : string
    credits             : string
    internalUseOnly     : bool
}

module ScreenshotrModel =

    let c = Credentials.load ()
   
    let Default aardvarkUrl = {
        credentialsInputUrl = match c with | Valid c -> c.url | _ -> ""
        credentialsInputKey = match c with | Valid c -> c.key | _ -> ""
        credentials         = c 
        aardvarkUrl         = aardvarkUrl
        imageSize           = Screenshotr.ImgSize(1024, 768)
        defaultTags         = HashSet.empty
        tags                = HashSet.empty
        withoutUiIsVisible  = false
        withUiIsVisible     = false
        caption             = ""
        credits             = ""
        internalUseOnly     = true
    }

    let Custom aardvarkUrl imageSize (tags : string list) = {
        credentialsInputUrl = match c with | Valid c -> c.url | _ -> ""
        credentialsInputKey = match c with | Valid c -> c.key | _ -> ""
        credentials         = c 
        aardvarkUrl         = aardvarkUrl
        imageSize           = imageSize
        defaultTags         = tags |> HashSet.ofList
        tags                = HashSet.empty
        withoutUiIsVisible  = false
        withUiIsVisible     = false
        caption             = ""
        credits             = ""
        internalUseOnly     = true
    }

