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

Partial Public Class Lookup_Waegezelle
    Public Property ID As String
    Public Property Hersteller As String
    Public Property Typ As String
    Public Property Pruefbericht As String
    Public Property Bauartzulassung As String
    Public Property Revisionsnummer As String
    Public Property Genauigkeitsklasse As String
    Public Property Mindestvorlast As String
    Public Property MindestvorlastProzent As String
    Public Property Waegezellenkennwert As String
    Public Property MaxAnzahlTeilungswerte As String
    Public Property MinTeilungswert As String
    Public Property Hoechsteteilungsfaktor As String
    Public Property Kriechteilungsfaktor As String
    Public Property RueckkehrVorlastsignal As String
    Public Property WiderstandWaegezelle As String
    Public Property GrenzwertTemperaturbereichMIN As String
    Public Property GrenzwertTemperaturbereichMAX As String
    Public Property BruchteilEichfehlergrenze As String
    Public Property Neu As Boolean
    Public Property Deaktiviert As Boolean

    Public Overridable Property Eichprozess As ICollection(Of Eichprozess) = New HashSet(Of Eichprozess)
    Public Overridable Property Mogelstatistik As ICollection(Of Mogelstatistik) = New HashSet(Of Mogelstatistik)

End Class
