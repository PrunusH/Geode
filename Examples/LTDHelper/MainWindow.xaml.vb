Imports System.ComponentModel
Imports Geode.Extension
Imports Geode.Network
Imports Geode.Network.Protocol

Class MainWindow
    Public WithEvents Extension As GeodeExtension
    Public WithEvents ConsoleBot As ConsoleBot
    Public TaskStarted As Boolean = False
    Public TaskBlocked As Boolean = False

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Extension = New GeodeExtension("LTDHelper", "Geode examples.", "Lilith", True, False) 'Instantiate extension
        Extension.Start() 'Start extension
        ConsoleBot = New ConsoleBot(Extension, "LTDHelper") 'Instantiate a new ConsoleBot
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
        ConsoleBot.BotSendMessage("Use /start or /stop")
    End Sub

    Async Function TryToBuyLTD() As Task
        If TaskStarted Then
            Try
                Extension.SendToServerAsync(Extension.Out.GetCatalogIndex, "NORMAL")
                Dim CatalogIndexData = Await Extension.WaitForPacketAsync(Extension.In.CatalogIndex, 1000)
                Dim CatalogRoot As New Geode.Habbo.Packages.HCatalogNode(CatalogIndexData.Packet)
                Dim LTDCategory = FindCatalogCategory(CatalogRoot.Children, "ler") 'You can test with other category like: set_mode
                Extension.SendToServerAsync(Extension.Out.PurchaseFromCatalog, LTDCategory.PageId, LTDCategory.OfferIds(0), "", 1)
                If Await Extension.WaitForPacketAsync(Extension.In.PurchaseOK, 500) IsNot Nothing Then
                    ConsoleBot.BotSendMessage("Successfully purchased an LTD.")
                    TaskBlocked = True
                    TaskStarted = False
                Else
                    Throw New Exception("LTD not purchased!")
                End If
            Catch
                TryToBuyLTD()
            End Try
        End If
    End Function

    Private Function FindCatalogCategory(NodeChildrens As Geode.Habbo.Packages.HCatalogNode(), CategoryName As String) As Geode.Habbo.Packages.HCatalogNode
        For Each NodeChild In NodeChildrens
            If NodeChild.PageName = CategoryName Then
                Return NodeChild
            Else
                Dim RecursiveSearchResult = FindCatalogCategory(NodeChild.Children, CategoryName)
                If RecursiveSearchResult IsNot Nothing Then
                    Return RecursiveSearchResult
                End If
            End If
        Next
        Return Nothing
    End Function

    Private Sub ConsoleBot_OnMessageReceived(sender As Object, e As String) Handles ConsoleBot.OnMessageReceived
        If TaskBlocked = False Then
            Select Case e.ToLower 'Handle received message
                Case "/start"
                    If TaskStarted = False Then
                        ConsoleBot.BotSendMessage("Ok, i will try to buy the LTD, you can use /stop to finish.")
                        TaskStarted = True
                        TryToBuyLTD()
                    End If
                Case "/stop"
                    If TaskStarted Then
                        ConsoleBot.BotSendMessage("Stopped, you can use /start to try again.")
                        TaskStarted = False
                    End If
                Case Else
                    BotWelcome()
            End Select
        Else
            ConsoleBot.BotSendMessage("Use /exit to finish.")
        End If
    End Sub

    Private Sub Extension_OnDataInterceptEvent(sender As Object, e As DataInterceptedEventArgs) Handles Extension.OnDataInterceptEvent
        If e.Packet.Id = Extension.In.FriendRequests.Id Then 'Show Bot when the initial console load is complete.
            BotShowAndWelcome()
        End If
        If e.Packet.Id = Extension.In.ErrorReport.Id Or e.Packet.Id = Extension.In.PurchaseError.Id Or e.Packet.Id = Extension.In.PurchaseNotAllowed.Id Or e.Packet.Id = Extension.In.NotEnoughBalance.Id Then 'Ignore common purchase errors
            If TaskStarted = True Then
                e.IsBlocked = True
            End If
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
