'------------------------------------------------------------------------------
' <auto-generated>
'    Dieser Code wurde aus einer Vorlage generiert.
'
'    Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten Ihrer Anwendung.
'    Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class ServerLizensierung
    Public Property ID As Integer
    Public Property Lizenzschluessel As String
    Public Property LetzteAktivierung As Nullable(Of Date)
    Public Property RHEWALizenz As Boolean
    Public Property Aktiv As Boolean
    Public Property HEKennung As String
    Public Property FK_BenutzerID As String

    Public Overridable Property Benutzer As Benutzer

End Class
