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

Partial Public Class Eichprozess
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
    Public Property UploadFilePath As String
    Public Property FK_Bearbeitungsstatus As Nullable(Of Integer)
    Public Property ErzeugerLizenz As String
    Public Property AusStandardwaageErzeugt As Boolean
    Public Property Bearbeitungsdatum As Nullable(Of Date)

    Public Overridable Property Eichprotokoll As Eichprotokoll
    Public Overridable Property Kompatiblitaetsnachweis As Kompatiblitaetsnachweis
    Public Overridable Property Lookup_Auswertegeraet As Lookup_Auswertegeraet
    Public Overridable Property Lookup_Bearbeitungsstatus As Lookup_Bearbeitungsstatus
    Public Overridable Property Lookup_Vorgangsstatus As Lookup_Vorgangsstatus
    Public Overridable Property Lookup_Waagenart As Lookup_Waagenart
    Public Overridable Property Lookup_Waagentyp As Lookup_Waagentyp
    Public Overridable Property Lookup_Waegezelle As Lookup_Waegezelle
    Public Overridable Property Mogelstatistik As ICollection(Of Mogelstatistik) = New HashSet(Of Mogelstatistik)

End Class
