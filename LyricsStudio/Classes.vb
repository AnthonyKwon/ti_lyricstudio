Public Class LyricsData 'Data of Location List
    Public Sub New(ByVal Time As String, ByVal Lyric As String)
        _Time = Time
        _Lyric = Lyric
    End Sub

    Private _Time As String
    Public Property Time As String
        Get
            Return _Time
        End Get
        Set(value As String)
            _Time = value
        End Set
    End Property

    Private _Lyric As String
    Public Property Lyric As String
        Get
            Return _Lyric
        End Get
        Set(value As String)
            _Lyric = value
        End Set
    End Property
End Class

Public Class FileInfo 'Data of Working Files (Working Directory, Extension File, Location File)
    Public Sub New(ByVal AudioLoaded As Boolean, ByVal LyricsLoaded As Boolean, ByVal Location As String, ByVal FileName As String, ByVal Extension As String)
        _AudioLoaded = AudioLoaded
        _LyricsLoaded = LyricsLoaded
        _Location = Location
        _FileName = FileName
        _Extension = Extension
    End Sub

    Private _AudioLoaded As String
    Public Property AudioLoaded As String
        Get
            Return _AudioLoaded
        End Get
        Set(value As String)
            _AudioLoaded = value
        End Set
    End Property

    Private _LyricsLoaded As String
    Public Property LyricsLoaded As String
        Get
            Return _LyricsLoaded
        End Get
        Set(value As String)
            _LyricsLoaded = value
        End Set
    End Property

    Private _Location As String
    Public Property Location As String
        Get
            Return _Location
        End Get
        Set(value As String)
            _Location = value
        End Set
    End Property

    Private _FileName As String
    Public Property FileName As String
        Get
            Return _FileName
        End Get
        Set(value As String)
            _FileName = value
        End Set
    End Property

    Private _Extension As String
    Public Property Extension As String
        Get
            Return _Extension
        End Get
        Set(value As String)
            _Extension = value
        End Set
    End Property
End Class