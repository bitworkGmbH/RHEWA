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

Partial Public Class ServerLookup_Waagenart
    Public Property ID As Byte
    Public Property Art As String
    Public Property Art_EN As String
    Public Property Art_PL As String

    Public Overridable Property ServerEichprozess As ICollection(Of ServerEichprozess) = New HashSet(Of ServerEichprozess)

End Class
