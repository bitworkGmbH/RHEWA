﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Public Class Entities
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=Entities")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Throw New UnintentionalCodeFirstException()
    End Sub

    Public Overridable Property Eichprotokoll() As DbSet(Of Eichprotokoll)
    Public Overridable Property Eichprozess() As DbSet(Of Eichprozess)
    Public Overridable Property Kompatiblitaetsnachweis() As DbSet(Of Kompatiblitaetsnachweis)
    Public Overridable Property Konfiguration() As DbSet(Of Konfiguration)
    Public Overridable Property Lizensierung() As DbSet(Of Lizensierung)
    Public Overridable Property Lookup_Auswertegeraet() As DbSet(Of Lookup_Auswertegeraet)
    Public Overridable Property Lookup_Bearbeitungsstatus() As DbSet(Of Lookup_Bearbeitungsstatus)
    Public Overridable Property Lookup_Konformitaetsbewertungsverfahren() As DbSet(Of Lookup_Konformitaetsbewertungsverfahren)
    Public Overridable Property Lookup_Vorgangsstatus() As DbSet(Of Lookup_Vorgangsstatus)
    Public Overridable Property Lookup_Waagenart() As DbSet(Of Lookup_Waagenart)
    Public Overridable Property Lookup_Waagentyp() As DbSet(Of Lookup_Waagentyp)
    Public Overridable Property Lookup_Waegezelle() As DbSet(Of Lookup_Waegezelle)
    Public Overridable Property Mogelstatistik() As DbSet(Of Mogelstatistik)
    Public Overridable Property PruefungAnsprechvermoegen() As DbSet(Of PruefungAnsprechvermoegen)
    Public Overridable Property PruefungAussermittigeBelastung() As DbSet(Of PruefungAussermittigeBelastung)
    Public Overridable Property PruefungLinearitaetFallend() As DbSet(Of PruefungLinearitaetFallend)
    Public Overridable Property PruefungLinearitaetSteigend() As DbSet(Of PruefungLinearitaetSteigend)
    Public Overridable Property PruefungRollendeLasten() As DbSet(Of PruefungRollendeLasten)
    Public Overridable Property PruefungStabilitaetGleichgewichtslage() As DbSet(Of PruefungStabilitaetGleichgewichtslage)
    Public Overridable Property PruefungStaffelverfahrenErsatzlast() As DbSet(Of PruefungStaffelverfahrenErsatzlast)
    Public Overridable Property PruefungStaffelverfahrenNormallast() As DbSet(Of PruefungStaffelverfahrenNormallast)
    Public Overridable Property PruefungWiederholbarkeit() As DbSet(Of PruefungWiederholbarkeit)
    Public Overridable Property Datenbankversion() As DbSet(Of Datenbankversion)

End Class
