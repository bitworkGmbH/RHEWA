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

Partial Public Class ServerEichprozess
    Public Property ID As Integer
    Public Property Vorgangsnummer As String
    Public Property FK_Vorgangsstatus As Byte
    Public Property FK_Auswertegeraet As String
    Public Property FK_Waegezelle As String
    Public Property FK_Eichprotokoll As Nullable(Of Integer)
    Public Property FK_WaagenTyp As Nullable(Of Byte)
    Public Property FK_WaagenArt As Nullable(Of Byte)
    Public Property FK_Kompatibilitaetsnachweis As Nullable(Of Integer)
    Public Property Ausgeblendet As Boolean
    Public Property FK_Bearbeitungsstatus As Nullable(Of Byte)
    Public Property UploadFilePath As String
    Public Property UploadDatum As Nullable(Of Date)
    Public Property BearbeitungsDatum As Nullable(Of Date)
    Public Property ErzeugerLizenz As String
    Public Property ZurBearbeitungGesperrtDurch As String
    Public Property Standardwaage As Nullable(Of Boolean)

    Public Overridable Property ServerEichprotokoll As ServerEichprotokoll
    Public Overridable Property ServerKompatiblitaetsnachweis As ServerKompatiblitaetsnachweis
    Public Overridable Property ServerLookup_Auswertegeraet As ServerLookup_Auswertegeraet
    Public Overridable Property ServerLookup_Waegezelle As ServerLookup_Waegezelle
    Public Overridable Property ServerMogelstatistik As ICollection(Of ServerMogelstatistik) = New HashSet(Of ServerMogelstatistik)
    Public Overridable Property ServerLookup_Bearbeitungsstatus As ServerLookup_Bearbeitungsstatus
    Public Overridable Property ServerLookup_Vorgangsstatus As ServerLookup_Vorgangsstatus
    Public Overridable Property ServerLookup_Waagenart As ServerLookup_Waagenart
    Public Overridable Property ServerLookup_Waagentyp As ServerLookup_Waagentyp

End Class