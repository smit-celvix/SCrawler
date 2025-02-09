﻿' Copyright (C) 2022  Andy
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY
Imports PersonalUtilities.Forms.Toolbars
Imports Download = SCrawler.Plugin.ISiteSettings.Download
Imports TDJob = SCrawler.DownloadObjects.TDownloader.Job
Namespace DownloadObjects
    Friend Class DownloadProgress : Implements IDisposable
        Friend Event OnDownloadDone(ByVal Message As String)
        Friend Event OnTotalCountChange()
        Private ReadOnly TP_MAIN As TableLayoutPanel
        Private ReadOnly TP_CONTROLS As TableLayoutPanel
        Private WithEvents BTT_START As Button
        Private WithEvents BTT_STOP As Button
        Private WithEvents BTT_OPEN As Button
        Private ReadOnly PR_MAIN As ProgressBar
        Private ReadOnly LBL_INFO As Label
        Private ReadOnly Property Instance As API.Base.ProfileSaved
        Friend ReadOnly Property Job As TDJob
#Region "Initializer"
        Friend Sub New(ByVal _Job As TDJob)
            Job = _Job

            TP_MAIN = New TableLayoutPanel With {.Margin = New Padding(0), .Dock = DockStyle.Fill}
            TP_MAIN.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
            TP_MAIN.ColumnCount = 1
            TP_CONTROLS = New TableLayoutPanel With {.Margin = New Padding(0), .Dock = DockStyle.Fill}
            PR_MAIN = New ProgressBar With {.Dock = DockStyle.Fill}
            LBL_INFO = New Label With {.Text = String.Empty, .Dock = DockStyle.Fill}
            CreateButton(BTT_STOP, My.Resources.Delete)

            If Job.Type = Download.Main Then
                LBL_INFO.Margin = New Padding(3)
                LBL_INFO.TextAlign = ContentAlignment.MiddleLeft
                With TP_MAIN
                    .RowStyles.Add(New RowStyle(SizeType.Percent, 100))
                    .RowCount = 1
                End With
                With TP_CONTROLS
                    .ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 30))
                    .ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 150))
                    .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
                    .ColumnCount = .ColumnStyles.Count
                    .RowStyles.Add(New RowStyle(SizeType.Percent, 100))
                    .RowCount = 1
                    With .Controls
                        .Add(BTT_STOP, 0, 0)
                        .Add(PR_MAIN, 1, 0)
                        .Add(LBL_INFO, 2, 0)
                    End With
                End With
                TP_MAIN.Controls.Add(TP_CONTROLS, 0, 0)
            Else
                LBL_INFO.Padding = New Padding(3, 0, 3, 0)
                LBL_INFO.TextAlign = ContentAlignment.TopCenter
                CreateButton(BTT_START, My.Resources.StartPic_01_Green_16)
                CreateButton(BTT_OPEN, PersonalUtilities.My.Resources.OpenFolderPic)
                With TP_CONTROLS
                    With .ColumnStyles
                        .Add(New ColumnStyle(SizeType.Absolute, 30))
                        .Add(New ColumnStyle(SizeType.Absolute, 30))
                        .Add(New ColumnStyle(SizeType.Absolute, 30))
                        .Add(New ColumnStyle(SizeType.Percent, 100))
                    End With
                    .ColumnCount = 4
                    .RowStyles.Add(New RowStyle(SizeType.Percent, 50))
                    .RowCount = 1
                    With .Controls
                        .Add(BTT_START, 0, 0)
                        .Add(BTT_STOP, 1, 0)
                        .Add(BTT_OPEN, 2, 0)
                        .Add(PR_MAIN, 3, 0)
                    End With
                End With
                With TP_MAIN
                    With .RowStyles
                        .Add(New RowStyle(SizeType.Absolute, 30))
                        .Add(New RowStyle(SizeType.Percent, 100))
                    End With
                    .RowCount = 2
                End With
                TP_MAIN.Controls.Add(TP_CONTROLS, 0, 0)
                TP_MAIN.Controls.Add(LBL_INFO, 0, 1)
            End If

            With Job
                .Progress = New MyProgress(PR_MAIN, LBL_INFO) With {.DropCurrentProgressOnTotalChange = False}
                With .Progress
                    AddHandler .OnProgressChange, AddressOf JobProgress_OnProgressChange
                    AddHandler .OnTotalCountChange, AddressOf JobProgress_OnTotalCountChange
                End With
            End With

            If Job.Type = Download.SavedPosts And Not Job.Progress Is Nothing Then Job.Progress.InformationTemporary = Job.Host.Name
            Instance = New API.Base.ProfileSaved(Job.Host, Job.Progress)
        End Sub
        Private Sub CreateButton(ByRef BTT As Button, ByVal Img As Image)
            BTT = New Button With {
                .BackgroundImage = Img,
                .BackgroundImageLayout = ImageLayout.Zoom,
                .Text = String.Empty,
                .Dock = DockStyle.Fill
            }
        End Sub
#End Region
        Friend Function [Get]() As TableLayoutPanel
            Return TP_MAIN
        End Function
#Region "Buttons"
        Private Sub BTT_START_Click(sender As Object, e As EventArgs) Handles BTT_START.Click
            Start()
        End Sub
        Private Sub BTT_STOP_Click(sender As Object, e As EventArgs) Handles BTT_STOP.Click
            [Stop]()
        End Sub
        Private Sub BTT_OPEN_Click(sender As Object, e As EventArgs) Handles BTT_OPEN.Click
            GlobalOpenPath(Job.Host.SavedPostsPath)
        End Sub
#End Region
#Region "Start, Stop"
        Friend Sub Start()
            Job.Start(AddressOf DownloadData)
        End Sub
        Friend Sub [Stop]()
            Job.Stop()
        End Sub
#End Region
#Region "SavedPosts downloading"
        Private Sub DownloadData()
            Dim btte As Action(Of Button, Boolean) = Sub(b, e) If b.InvokeRequired Then b.Invoke(Sub() b.Enabled = e) Else b.Enabled = e
            Try
                btte.Invoke(BTT_START, False)
                btte.Invoke(BTT_STOP, True)
                Job.Progress.InformationTemporary = $"{Job.Host.Name} downloading started"
                Job.Start()
                Instance.Download(Job.Token)
                RaiseEvent OnDownloadDone($"Downloading saved {Job.Host.Name} posts is completed")
            Catch ex As Exception
                Job.Progress.InformationTemporary = $"{Job.Host.Name} downloading error"
                ErrorsDescriber.Execute(EDP.LogMessageValue, ex, {$"{Job.Host.Name} saved posts downloading error", "Saved posts"})
            Finally
                btte.Invoke(BTT_START, True)
                btte.Invoke(BTT_STOP, False)
                Job.Stopped()
                If Job.Type = Download.SavedPosts Then Job.Progress.TotalCount = 0 : Job.Progress.CurrentCounter = 0
            End Try
        End Sub
#End Region
#Region "Progress, Jobs count"
        Private Sub JobProgress_OnTotalCountChange(ByVal Source As IMyProgress, ByVal Index As Integer)
            RaiseEvent OnTotalCountChange()
        End Sub
        Private Sub JobProgress_OnProgressChange(ByVal Source As IMyProgress, ByVal Index As Integer)
            If Not Job.Type = Download.SavedPosts Then MainProgress.Perform()
        End Sub
#End Region
#Region "IDisposable Support"
        Private disposedValue As Boolean = False
        Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    If Not BTT_START Is Nothing Then BTT_START.Dispose()
                    If Not BTT_STOP Is Nothing Then BTT_STOP.Dispose()
                    If Not BTT_OPEN Is Nothing Then BTT_OPEN.Dispose()
                    PR_MAIN.Dispose()
                    LBL_INFO.Dispose()
                    TP_CONTROLS.Controls.Clear()
                    TP_CONTROLS.Dispose()
                    TP_MAIN.Controls.Clear()
                    TP_MAIN.Dispose()
                End If
                disposedValue = True
            End If
        End Sub
        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub
        Friend Overloads Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace