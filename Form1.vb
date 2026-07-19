#Region " Imports "

Imports System.DirectoryServices
Imports System.Threading
Imports LogonStatus.clsUser

#End Region

Public Class frmMain

#Region " Inherits "

    Inherits System.Windows.Forms.Form

#End Region

#Region " Public variables "

    Public mstrServers() As String

#End Region

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents ilMain As System.Windows.Forms.ImageList
    Friend WithEvents txtUsername As System.Windows.Forms.TextBox
    Friend WithEvents btnInfo As System.Windows.Forms.Button
    Friend WithEvents tvwMain As System.Windows.Forms.TreeView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMain))
        Me.btnConnect = New System.Windows.Forms.Button
        Me.ilMain = New System.Windows.Forms.ImageList(Me.components)
        Me.txtUsername = New System.Windows.Forms.TextBox
        Me.btnInfo = New System.Windows.Forms.Button
        Me.tvwMain = New System.Windows.Forms.TreeView
        Me.SuspendLayout()
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(8, 8)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(64, 24)
        Me.btnConnect.TabIndex = 0
        Me.btnConnect.Text = "Connect"
        '
        'ilMain
        '
        Me.ilMain.ImageSize = New System.Drawing.Size(16, 16)
        Me.ilMain.ImageStream = CType(resources.GetObject("ilMain.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ilMain.TransparentColor = System.Drawing.Color.Transparent
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(8, 320)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(64, 20)
        Me.txtUsername.TabIndex = 2
        Me.txtUsername.Text = "robinsod"
        '
        'btnInfo
        '
        Me.btnInfo.Location = New System.Drawing.Point(80, 320)
        Me.btnInfo.Name = "btnInfo"
        Me.btnInfo.Size = New System.Drawing.Size(64, 24)
        Me.btnInfo.TabIndex = 3
        Me.btnInfo.Text = "Get Info"
        '
        'tvwMain
        '
        Me.tvwMain.ImageList = Me.ilMain
        Me.tvwMain.Location = New System.Drawing.Point(8, 40)
        Me.tvwMain.Name = "tvwMain"
        Me.tvwMain.Size = New System.Drawing.Size(248, 272)
        Me.tvwMain.Sorted = True
        Me.tvwMain.TabIndex = 4
        '
        'frmMain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(264, 349)
        Me.Controls.Add(Me.tvwMain)
        Me.Controls.Add(Me.btnInfo)
        Me.Controls.Add(Me.txtUsername)
        Me.Controls.Add(Me.btnConnect)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "LogonStatus"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region " Form elements "

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        Dim oDS As New System.DirectoryServices.DirectorySearcher
        Dim oSC As System.DirectoryServices.SearchResultCollection
        Dim oSR As System.DirectoryServices.SearchResult
        Dim oNode As TreeNode
        Dim i As Integer

        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor

        oDS.PropertiesToLoad.Add("name")
        oDS.PropertiesToLoad.Add("aDSPath")
        oDS.PropertiesToLoad.Add("samAccountName")
        oDS.SearchRoot.Path = "LDAP://OU=Domain Controllers,DC=intranet,DC=justice,DC=wa,DC=gov,DC=au"
        oDS.SizeLimit = 1000 '  maximum number of objects to return
        oDS.Filter = "(objectclass=computer)" ' or = |, and = &

        oSC = oDS.FindAll()

        For Each oSR In oSC
            ReDim Preserve mstrServers(i)

            mstrServers(i) = oSR.Properties("name").Item(0)

            Try
                oNode = tvwMain.Nodes.Add(mstrServers(i))
                oNode.ImageIndex = 1 ' Question
                oNode.SelectedImageIndex = 1 ' Question
            Catch
            End Try

            i += 1

            Application.DoEvents()
        Next

        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
    End Sub

    Private Sub btnInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInfo.Click
        Dim oLogonInfo As LogonInfo

        oLogonInfo = GetLogonInfo(Me.txtUsername.Text)

        'Console.WriteLine("Last Logon : " & oLogonInfo.LastLogon & " on " & oLogonInfo.LastLogonServer)
        'Console.WriteLine("Last Logoff: " & oLogonInfo.LastLogoff & " on " & oLogonInfo.LastLogoffServer)
    End Sub

#End Region

#Region " Functions "

    Private Function GetLogonInfo(ByVal strUsername As String) As LogonInfo
        Dim strServer As String
        Dim intNodeCount As Integer
        Dim i As Integer
        Dim oUserInfo() As clsUser = Nothing
        Dim mLogonInfo As New LogonInfo

        Dim oNode As TreeNode
        Dim bAllComplete As Boolean

        intNodeCount = UBound(mstrServers)

        For i = 0 To intNodeCount
            oNode = Me.tvwMain.Nodes(i)

            strServer = Me.tvwMain.Nodes(i).Text

            Console.WriteLine(strServer & "...")

            ReDim Preserve oUserInfo(i)

            oUserInfo(i) = GetUserInfoFromServer(strServer, strUsername, i)

        Next

        Do

            ' Set All Complete flag default
            bAllComplete = True

            ' Now set it according to the server info
            For i = 0 To UBound(oUserInfo)

                Application.DoEvents()

                If oUserInfo(i).Complete = False Then
                    bAllComplete = False
                Else ' Complete
                    If oUserInfo(i).InfoExtracted = False Then
                        If oUserInfo(i).ErrorReturned Then
                            Try ' required because the user may attempt to close the app whilst it is being accessed
                                oNode = Me.tvwMain.Nodes(oUserInfo(i).NodeIndex)
                                oNode.ImageIndex = 3 ' Error
                                oNode.SelectedImageIndex = 3 ' Error
                            Catch
                            End Try
                            oUserInfo(i).InfoExtracted = True
                        Else
                            Select Case Trim(oUserInfo(i).LastLogon)
                                Case "01/01/1970 12:00:00 AM"
                                    oNode = Me.tvwMain.Nodes(oUserInfo(i).NodeIndex)
                                    oNode.ImageIndex = 2 ' Exclamation
                                    oNode.SelectedImageIndex = 2 ' Exclamation
                                Case ""
                                Case Else
                                    oNode = Me.tvwMain.Nodes(oUserInfo(i).NodeIndex)
                                    oNode.ImageIndex = 4 ' Computer
                                    oNode.SelectedImageIndex = 4 ' Computer

                                    oNode = oNode.Nodes.Add("Last Logon " & oUserInfo(i).LastLogon)
                                    oNode.ImageIndex = 4 ' Computer
                                    oNode.SelectedImageIndex = 4 ' Computer
                            End Select

                            Select Case Trim(oUserInfo(i).LastLogoff)
                                Case "01/01/1970 12:00:00 AM"
                                    oNode = Me.tvwMain.Nodes(oUserInfo(i).NodeIndex)
                                    If oNode.ImageIndex <> 4 Then ' Only change icon if it hasn't already been set
                                        oNode.ImageIndex = 2 ' Exclamation
                                        oNode.SelectedImageIndex = 2 ' Exclamation
                                    End If
                                Case ""
                                Case Else
                                    oNode = Me.tvwMain.Nodes(oUserInfo(i).NodeIndex)
                                    oNode.ImageIndex = 4 ' Computer
                                    oNode.SelectedImageIndex = 4 ' Computer

                                    oNode = oNode.Nodes.Add("Last Logoff " & oUserInfo(i).LastLogoff)
                                    oNode.ImageIndex = 4 ' Computer
                                    oNode.SelectedImageIndex = 4 ' Computer
                            End Select

                            oUserInfo(i).InfoExtracted = True
                        End If
                    End If
                End If
            Next

        Loop Until bAllComplete

        Return mLogonInfo

    End Function

    Private Function GetUserInfoFromServer(ByVal strServername As String, ByVal strUsername As String, ByVal ID As Integer) As clsUser
        Dim oUserInfo As New clsUser

        oUserInfo.Username = strUsername
        oUserInfo.Servername = strServername
        oUserInfo.NodeIndex = ID

        oUserInfo.Start()

        Return oUserInfo
    End Function

#End Region

End Class
