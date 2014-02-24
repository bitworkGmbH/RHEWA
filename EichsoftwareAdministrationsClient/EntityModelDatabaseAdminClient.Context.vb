﻿'------------------------------------------------------------------------------
' <auto-generated>
'    Dieser Code wurde aus einer Vorlage generiert.
'
'    Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten Ihrer Anwendung.
'    Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Public Class EichenEntities
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=EichenEntities")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Throw New UnintentionalCodeFirstException()
    End Sub

    Public Property ServerBeschaffenheitspruefung() As DbSet(Of ServerBeschaffenheitspruefung)
    Public Property ServerEichmarkenverwaltung() As DbSet(Of ServerEichmarkenverwaltung)
    Public Property ServerEichprotokoll() As DbSet(Of ServerEichprotokoll)
    Public Property ServerEichprozess() As DbSet(Of ServerEichprozess)
    Public Property ServerKompatiblitaetsnachweis() As DbSet(Of ServerKompatiblitaetsnachweis)
    Public Property ServerLizensierung() As DbSet(Of ServerLizensierung)
    Public Property ServerLookup_Auswertegeraet() As DbSet(Of ServerLookup_Auswertegeraet)
    Public Property ServerLookup_Bearbeitungsstatus() As DbSet(Of ServerLookup_Bearbeitungsstatus)
    Public Property ServerLookup_Konformitaetsbewertungsverfahren() As DbSet(Of ServerLookup_Konformitaetsbewertungsverfahren)
    Public Property ServerLookup_Vorgangsstatus() As DbSet(Of ServerLookup_Vorgangsstatus)
    Public Property ServerLookup_Waagenart() As DbSet(Of ServerLookup_Waagenart)
    Public Property ServerLookup_Waagentyp() As DbSet(Of ServerLookup_Waagentyp)
    Public Property ServerLookup_Waegezelle() As DbSet(Of ServerLookup_Waegezelle)
    Public Property ServerMogelstatistik() As DbSet(Of ServerMogelstatistik)
    Public Property ServerPruefungAnsprechvermoegen() As DbSet(Of ServerPruefungAnsprechvermoegen)
    Public Property ServerPruefungAussermittigeBelastung() As DbSet(Of ServerPruefungAussermittigeBelastung)
    Public Property ServerPruefungEichfehlergrenzen() As DbSet(Of ServerPruefungEichfehlergrenzen)
    Public Property ServerPruefungLinearitaetFallend() As DbSet(Of ServerPruefungLinearitaetFallend)
    Public Property ServerPruefungLinearitaetSteigend() As DbSet(Of ServerPruefungLinearitaetSteigend)
    Public Property ServerPruefungRollendeLasten() As DbSet(Of ServerPruefungRollendeLasten)
    Public Property ServerPruefungStabilitaetGleichgewichtslage() As DbSet(Of ServerPruefungStabilitaetGleichgewichtslage)
    Public Property ServerPruefungStaffelverfahrenErsatzlast() As DbSet(Of ServerPruefungStaffelverfahrenErsatzlast)
    Public Property ServerPruefungStaffelverfahrenNormallast() As DbSet(Of ServerPruefungStaffelverfahrenNormallast)
    Public Property ServerPruefungWiederholbarkeit() As DbSet(Of ServerPruefungWiederholbarkeit)
    Public Property ServerVerbindungsprotokoll() As DbSet(Of ServerVerbindungsprotokoll)
    Public Property ServerKonfiguration() As DbSet(Of ServerKonfiguration)
    Public Property Benutzer() As DbSet(Of Benutzer)
    Public Property Firmen() As DbSet(Of Firmen)
    Public Property ServerFirmenZusatzdaten() As DbSet(Of ServerFirmenZusatzdaten)
    Public Property ServerLookupVertragspartnerFirma() As DbSet(Of ServerLookupVertragspartnerFirma)

End Class
