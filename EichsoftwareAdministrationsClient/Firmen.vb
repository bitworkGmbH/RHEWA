'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class Firmen
    Public Property ID As String
    Public Property KundenNr As String
    Public Property Name As String
    Public Property Strasse As String
    Public Property PLZ As String
    Public Property Telefon As String
    Public Property Ort As String
    Public Property Land As String

    Public Overridable Property Benutzer As ICollection(Of Benutzer) = New HashSet(Of Benutzer)
    Public Overridable Property ServerFirmenZusatzdaten As ICollection(Of ServerFirmenZusatzdaten) = New HashSet(Of ServerFirmenZusatzdaten)

End Class
