﻿' Copyright (C) 2022  Andy
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY
Imports PersonalUtilities.Forms.Toolbars
Imports PersonalUtilities.Forms
Public Class SettingsForm : Implements IOkCancelToolbar
    Private ReadOnly MyDefs As DefaultFormProps
    Private ReadOnly Property Settings As SiteSettings
    Friend Sub New(ByRef s As SiteSettings)
        InitializeComponent()
        MyDefs = New DefaultFormProps
        Settings = s
    End Sub
    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            With MyDefs
                .MyViewInitialize(Me, Settings.Design, True)
                .AddOkCancelToolbar()
                .DelegateClosingChecker()
                If Settings.Domains.Count > 0 Then Settings.Domains.ForEach(Sub(d) LIST_DOMAINS.Items.Add(d))
                .EndLoaderOperations()
            End With
        Catch ex As Exception
            MyDefs.InvokeLoaderError(ex)
        End Try
    End Sub
    Private Sub ToolbarBttOK() Implements IOkCancelToolbar.ToolbarBttOK
        Settings.Domains.Clear()
        With LIST_DOMAINS
            If .Items.Count > 0 Then
                For Each i In .Items : Settings.Domains.Add(i.ToString) : Next
            End If
        End With
        Settings.UpdateDomains()
        MyDefs.CloseForm()
    End Sub
    Private Sub ToolbarBttCancel() Implements IOkCancelToolbar.ToolbarBttCancel
        MyDefs.CloseForm(Windows.Forms.DialogResult.Cancel)
    End Sub
    Private Sub BTT_ADD_Click(sender As Object, e As EventArgs) Handles BTT_ADD.Click
        Dim nd$ = InputBoxE("Enter a new domain using the pattern [xvideos.com]:", "New domain")
        If Not nd.IsEmptyString Then
            If Not LIST_DOMAINS.Items.Contains(nd) Then
                LIST_DOMAINS.Items.Add(nd)
            Else
                MsgBoxE($"The domain [{nd}] already added")
            End If
        End If
    End Sub
    Private Sub BTT_DELETE_Click(sender As Object, e As EventArgs) Handles BTT_DELETE.Click
        If _LatestSelected.ValueBetween(0, LIST_DOMAINS.Items.Count - 1) Then
            Dim n$ = LIST_DOMAINS.Items(_LatestSelected)
            If MsgBoxE({$"Are you sure you want to delete the [{n}] domain?",
                        "Removing domains"}, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                LIST_DOMAINS.Items.RemoveAt(_LatestSelected)
                MsgBoxE($"Domain [{n}] removed")
            Else
                MsgBoxE("Operation canceled")
            End If
        Else
            MsgBoxE("No domain selected", vbExclamation)
        End If
    End Sub
    Private _LatestSelected As Integer = -1
    Private Sub LIST_DOMENS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LIST_DOMAINS.SelectedIndexChanged
        _LatestSelected = LIST_DOMAINS.SelectedIndex
    End Sub
End Class