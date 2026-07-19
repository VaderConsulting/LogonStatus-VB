Imports System.Threading

Public Class clsUser

#Region " Structures "

    Public Structure LogonInfo
        Dim LastLogon As String
        Dim LastLogoff As String
        Dim LastLogonServer As String
        Dim LastLogoffServer As String
    End Structure

#End Region

#Region " Locals "

    Private _LastLogon As String
    Private _LastLogoff As String
    Private _LastLogonServer As String
    Private _LastLogoffServer As String
    Private _Server As String
    Private _Username As String
    Private _LogonInfo As New LogonInfo
    Private _NodeIndex As Integer
    Private _Complete As Boolean
    Private _InfoExtracted As Boolean
    Private _ErrorReturned As Boolean

#End Region

#Region " Properties "

    Public Property LastLogon() As String
        Get
            Return _LastLogon
        End Get
        Set(ByVal Value As String)
            _LastLogon = Value
        End Set
    End Property

    Public Property LastLogoff() As String
        Get
            Return _LastLogoff
        End Get
        Set(ByVal Value As String)
            _LastLogoff = Value
        End Set
    End Property

    Public Property LastLogonServer() As String
        Get
            Return _LastLogonServer
        End Get
        Set(ByVal Value As String)
            _LastLogonServer = Value
        End Set
    End Property

    Public Property LastLogoffServer() As String
        Get
            Return _LastLogoffServer
        End Get
        Set(ByVal Value As String)
            _LastLogoffServer = Value
        End Set
    End Property

    Public Property Servername() As String
        Get
            Return _Server
        End Get
        Set(ByVal Value As String)
            _Server = Value
        End Set
    End Property

    Public Property Username() As String
        Get
            Return _Username
        End Get
        Set(ByVal Value As String)
            _Username = Value
        End Set
    End Property

    Public ReadOnly Property UserLogonInfo() As LogonInfo
        Get
            Return _LogonInfo
        End Get
    End Property

    Public Property NodeIndex() As Integer
        Get
            Return _NodeIndex
        End Get
        Set(ByVal Value As Integer)
            _NodeIndex = Value
        End Set
    End Property

    Public ReadOnly Property Complete() As Boolean
        Get
            Return _Complete
        End Get
    End Property

    Public ReadOnly Property ErrorReturned() As Boolean
        Get
            Return _ErrorReturned
        End Get
    End Property

    Public Property InfoExtracted() As Boolean
        Get
            Return _InfoExtracted
        End Get
        Set(ByVal Value As Boolean)
            _InfoExtracted = Value
        End Set
    End Property

#End Region

#Region " Functions "

    Public Function Start() As Thread
        Dim Worker As New Thread(New ThreadStart(AddressOf GetLogonInfo))

        Worker.Name = _Server
        Worker.Start()

        Return Worker
    End Function

#End Region

#Region " Subroutines "

    Private Sub GetLogonInfo()
        Dim oUser As ActiveDs.IADsUser
        Dim oChildNode As New TreeNode

        ' Determine values
        Console.WriteLine("Determining data for " & _Server)

        Try
            oUser = GetObject("WinNT://" & _Server & "/" & _Username & ",User")
            Try
                _LastLogon = oUser.Get("lastLogin")
            Catch
                _LastLogon = "01/01/1970 12:00:00 AM"
            End Try

            Console.WriteLine(_Server & " Last logon: " & _LastLogon)

            Try
                _LastLogoff = oUser.Get("lastLogoff")
                Console.WriteLine()
            Catch
                _LastLogoff = "01/01/1970 12:00:00 AM"
            End Try

            Console.WriteLine(_Server & " Last logoff: " & _LastLogoff)
        Catch
            Console.WriteLine(_Server & " Could not connect to " & _Username)
            _ErrorReturned = True
        End Try

        Application.DoEvents()

        ' Done
        _Complete = True
    End Sub

#End Region

End Class
