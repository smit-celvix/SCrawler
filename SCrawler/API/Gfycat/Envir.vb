﻿' Copyright (C) 2022  Andy
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY
Imports PersonalUtilities.Functions.RegularExpressions
Imports System.Net
Imports SCrawler.API.Base
Namespace API.Gfycat
    Friend NotInheritable Class Envir
        Private Sub New()
        End Sub
        Friend Shared Function GetVideo(ByVal URL As String) As String
            Try
                Dim r$
                Using w As New WebClient : r = w.DownloadString(URL) : End Using
                If Not r.IsEmptyString Then Return RegexReplace(r, RParams.DMS("contentUrl.:.(http.?://[^""]+?\.mp4)", 1)) Else Return String.Empty
            Catch ex As Exception
                Dim e As EDP = EDP.ReturnValue
                If TypeOf ex Is WebException Then
                    Dim obj As HttpWebResponse = TryCast(DirectCast(ex, WebException).Response, HttpWebResponse)
                    If Not If(obj?.StatusCode, HttpStatusCode.OK) = HttpStatusCode.NotFound Then e += EDP.SendInLog
                End If
                Return ErrorsDescriber.Execute(e, ex, $"[API.Gfycat.Envir.GetVideo({URL})]", String.Empty)
            End Try
        End Function
        Friend Shared Function GetVideoInfo(ByVal URL As String) As IEnumerable(Of UserMedia)
            Dim u$ = GetVideo(URL)
            Return If(u.IsEmptyString, Nothing, {New UserMedia(u, UserMedia.Types.Video)})
        End Function
    End Class
End Namespace