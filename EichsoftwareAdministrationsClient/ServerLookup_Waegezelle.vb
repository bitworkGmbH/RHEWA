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

Partial Public Class ServerLookup_Waegezelle
    Public Property ID As String
    Public Property Hersteller As String
    Public Property Typ As String
    Public Property Pruefbericht As String
    Public Property Bauartzulassung As String
    Public Property Revisionsnummer As String
    Public Property Genauigkeitsklasse As String
    Public Property Mindestvorlast As String
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
    Public Property ErstellDatum As Nullable(Of Date)
    Public Property Deaktiviert As Boolean
    Public Property Neu As Boolean

    Public Overridable Property ServerMogelstatistik As ICollection(Of ServerMogelstatistik) = New HashSet(Of ServerMogelstatistik)
    Public Overridable Property ServerEichprozess As ICollection(Of ServerEichprozess) = New HashSet(Of ServerEichprozess)

End Class
