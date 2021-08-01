Imports System.ComponentModel
Imports Geode.Extension
Imports Geode.Network
Imports Geode.Network.Protocol

Class MainWindow
    Public WithEvents Extension As GeodeExtension
    Public WithEvents ConsoleBot As ConsoleBot

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Extension = New GeodeExtension("ConsoleBotVB", "Geode examples.", "Lilith", True, False) 'Instantiate extension
        Extension.Start() 'Start extension
        ConsoleBot = New ConsoleBot(Extension, "VB example") 'Instantiate a new ConsoleBot
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Extension.IsConnected Then
            ConsoleBot.HideBot() 'Hide bot before app closes
        End If
    End Sub

    Sub BotShowAndWelcome()
        ConsoleBot.ShowBot()
        BotWelcome()
    End Sub

    Sub BotWelcome()
        ConsoleBot.BotSendMessage("Welcome |")
        ConsoleBot.BotSendMessage("Use /help to get info.")
    End Sub

    Private Sub ConsoleBot_OnMessageReceived(sender As Object, e As String) Handles ConsoleBot.OnMessageReceived
        Select Case e.ToLower 'Handle received message
            Case "/help"
                ConsoleBot.BotSendMessage("Commands:")
                ConsoleBot.BotSendMessage("/look1 and /look2 to change current look.")
                ConsoleBot.BotSendMessage("/sit to force sit.")
                ConsoleBot.BotSendMessage("/fx to get light sabber fx.")
                ConsoleBot.BotSendMessage("/exit to exit extension.")
            Case "/look1"
                Extension.SendToServerAsync(Extension.Out.UpdateFigureData, "F", "hr-515-45.ch-665-71.lg-3216-73.hd-600-10.fa-3276-72")
            Case "/look2"
                Extension.SendToServerAsync(Extension.Out.UpdateFigureData, "M", "hr-893-45.ch-235-71.lg-3290-82.hd-180-10.fa-3276-72")
            Case "/sit"
                Extension.SendToServerAsync(Extension.Out.ChangePosture, 1)
            Case "/fx"
                Extension.SendToServerAsync(Extension.Out.Chat, ":yyxxabxa", 0, -1)
            Case Else
                BotWelcome()
        End Select
    End Sub

    Private Sub Extension_OnDataInterceptEvent(sender As Object, e As DataInterceptedEventArgs) Handles Extension.OnDataInterceptEvent
        If e.Packet.Id = Extension.In.FriendRequests.Id Then 'Show Bot when the initial console load is complete.
            BotShowAndWelcome()
        End If
    End Sub

    Private Sub Extension_OnDoubleClickEvent(sender As Object, e As HPacket) Handles Extension.OnDoubleClickEvent 'G-Earth extension play button clicked.
        If Extension.IsConnected Then
            BotShowAndWelcome()
        End If
    End Sub

    Private Sub Extension_OnConnectedEvent(sender As Object, e As HPacket) Handles Extension.OnConnectedEvent 'G-Earth is connected.
        BotShowAndWelcome()
    End Sub

    Private Sub Extension_OnCriticalErrorEvent(sender As Object, e As String) Handles Extension.OnCriticalErrorEvent 'Extension critical error.
        MsgBox(e & ".", MsgBoxStyle.Critical, "Critical error")
        Environment.Exit(0)
    End Sub
End Class
