'------------------------------------------------------------------------------
' <auto-generated>
'    This code was generated from a template.
'
'    Manual changes to this file may cause unexpected behavior in your application.
'    Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class ServerLookup_Konformitaetsbewertungsverfahren
    Public Property ID As Byte
    Public Property Verfahren As String
    Public Property Verfahren_EN As String
    Public Property Verfahren_PL As String

    Public Overridable Property ServerEichprotokoll As ICollection(Of ServerEichprotokoll) = New HashSet(Of ServerEichprotokoll)

End Class
