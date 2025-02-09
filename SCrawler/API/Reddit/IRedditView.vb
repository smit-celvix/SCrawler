﻿' Copyright (C) 2022  Andy
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY
Namespace API.Reddit
    Friend Interface IRedditView
        Enum View As Integer
            [New] = 0
            Hot = 1
            Top = 2
        End Enum
        Enum Period As Integer
            All = 0
            Hour = 1
            Day = 2
            Week = 3
            Month = 4
            Year = 5
        End Enum
        Property ViewMode As View
        Property ViewPeriod As Period
        Sub SetView(ByVal Options As IRedditView)
    End Interface
    Friend Class RedditViewExchange : Implements IRedditView
        Friend Const Name_ViewMode As String = "ViewMode"
        Friend Const Name_ViewPeriod As String = "ViewPeriod"
        Friend Property ViewMode As IRedditView.View Implements IRedditView.ViewMode
        Friend Property ViewPeriod As IRedditView.Period Implements IRedditView.ViewPeriod
        Friend Sub SetView(ByVal Options As IRedditView) Implements IRedditView.SetView
            If Not Options Is Nothing Then
                ViewMode = Options.ViewMode
                ViewPeriod = Options.ViewPeriod
            End If
        End Sub
    End Class
End Namespace