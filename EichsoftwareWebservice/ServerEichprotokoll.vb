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

Partial Public Class ServerEichprotokoll
    Public Property ID As Integer
    Public Property FK_Identifikationsdaten_Konformitaetsbewertungsverfahren As Byte
    Public Property FK_Identifikationsdaten_SuperOfficeBenutzer As String
    Public Property Identifikationsdaten_Benutzer As String
    Public Property Identifikationsdaten_Aufstellungsort As String
    Public Property Identifikationsdaten_Datum As Nullable(Of Date)
    Public Property Identifikationsdaten_Min1 As String
    Public Property Identifikationsdaten_Min2 As String
    Public Property Identifikationsdaten_Min3 As String
    Public Property Identifikationsdaten_NichtSelbsteinspielend As String
    Public Property Identifikationsdaten_HybridMechanisch As String
    Public Property Identifikationsdaten_Selbsteinspielend As String
    Public Property Identifikationsdaten_Pruefer As String
    Public Property Identifikationsdaten_Baujahr As String
    Public Property Pruefverfahren_VolleNormallast As Nullable(Of Boolean)
    Public Property Pruefverfahren_VollstaendigesStaffelverfahren As Nullable(Of Boolean)
    Public Property Pruefverfahren_BetragNormallast As String
    Public Property Komponenten_Softwarestand As String
    Public Property Komponenten_Eichzaehlerstand As String
    Public Property Komponenten_WaegezellenFabriknummer As String
    Public Property Verwendungszweck_HalbAutomatisch As Nullable(Of Boolean)
    Public Property Verwendungszweck_Automatisch As Nullable(Of Boolean)
    Public Property Verwendungszweck_Nullnachfuehrung As Nullable(Of Boolean)
    Public Property Verwendungszweck_AutoTara As Nullable(Of Boolean)
    Public Property Verwendungszweck_HandTara As Nullable(Of Boolean)
    Public Property Verwendungszweck_ZubehoerVerschiedenes As Nullable(Of Boolean)
    Public Property Verwendungszweck_EichfaehigerDatenspeicher As Nullable(Of Boolean)
    Public Property Verwendungszweck_PC As Nullable(Of Boolean)
    Public Property Verwendungszweck_Drucker As Nullable(Of Boolean)
    Public Property Verwendungszweck_Druckertyp As String
    Public Property Verwendungszweck_Fahrzeugwaagen_MxM As String
    Public Property Verwendungszweck_Fahrzeugwaagen_Dimension As String
    Public Property Beschaffenheitspruefung_AufstellungsbedingungenInOrdnung As Nullable(Of Boolean)
    Public Property Beschaffenheitspruefung_ZulassungsunterlagenInLesbarerFassung As Nullable(Of Boolean)
    Public Property Beschaffenheitspruefung_MesstechnischeMerkmaleInOrdnung As Nullable(Of Boolean)
    Public Property Beschaffenheitspruefung_AufschriftenKennzeichnungenInOrdnung As Nullable(Of Boolean)
    Public Property Beschaffenheitspruefung_AnzeigenAbdruckeInOrdnung As Nullable(Of Boolean)
    Public Property Beschaffenheitspruefung_KompatibilitaetsnachweisVorhanden As Nullable(Of Boolean)
    Public Property Beschaffenheitspruefung_Genauigkeitsklasse As String
    Public Property Beschaffenheitspruefung_Pruefintervall As String
    Public Property Beschaffenheitspruefung_LetztePruefung As String
    Public Property Beschaffenheitspruefung_Pruefscheinnummer As String
    Public Property Beschaffenheitspruefung_EichfahrzeugFirma As String
    Public Property GenauigkeitNullstellung_InOrdnung As Nullable(Of Boolean)
    Public Property Ueberlastanzeige_Max As String
    Public Property Ueberlastanzeige_Ueberlast As Nullable(Of Boolean)
    Public Property Taraeinrichtung_TaraausgleichseinrichtungOK As String
    Public Property Taraeinrichtung_GenauigkeitTarierungOK As String
    Public Property Taraeinrichtung_ErweiterteRichtigkeitspruefungOK As String
    Public Property Fallbeschleunigung_g As String
    Public Property Fallbeschleunigung_ms2 As Nullable(Of Boolean)
    Public Property Sicherung_BenannteStelle As Nullable(Of Boolean)
    Public Property Sicherung_BenannteStelleAnzahl As Nullable(Of Short)
    Public Property Sicherung_Eichsiegel13x13 As Nullable(Of Boolean)
    Public Property Sicherung_Eichsiegel13x13Anzahl As Nullable(Of Short)
    Public Property Sicherung_EichsiegelRund As Nullable(Of Boolean)
    Public Property Sicherung_EichsiegelRundAnzahl As Nullable(Of Short)
    Public Property Sicherung_HinweismarkeGelocht As Nullable(Of Boolean)
    Public Property Sicherung_HinweismarkeGelochtAnzahl As Nullable(Of Short)
    Public Property Sicherung_GruenesM As Nullable(Of Boolean)
    Public Property Sicherung_GruenesMAnzahl As Nullable(Of Short)
    Public Property Sicherung_CE As Nullable(Of Boolean)
    Public Property Sicherung_CEAnzahl As Nullable(Of Short)
    Public Property Sicherung_DatenAusgelesen As Nullable(Of Boolean)
    Public Property Sicherung_AlibispeicherEingerichtet As Nullable(Of Boolean)
    Public Property Sicherung_AlibispeicherAufbewahrungsdauerReduziert As Nullable(Of Boolean)
    Public Property Sicherung_AlibispeicherAufbewahrungsdauerReduziertBegruendung As String
    Public Property Sicherung_Bemerkungen As String
    Public Property Wiederholbarkeit_Staffelverfahren_MINNormalien As String
    Public Property EignungAchlastwaegungen_WaagenbrueckeEbene As Nullable(Of Boolean)
    Public Property EignungAchlastwaegungen_WaageNichtGeeignet As Nullable(Of Boolean)
    Public Property EignungAchlastwaegungen_Geprueft As Nullable(Of Boolean)

    Public Overridable Property ServerLookup_Konformitaetsbewertungsverfahren As ServerLookup_Konformitaetsbewertungsverfahren
    Public Overridable Property ServerEichprozess As ICollection(Of ServerEichprozess) = New HashSet(Of ServerEichprozess)
    Public Overridable Property ServerPruefungAussermittigeBelastung As ICollection(Of ServerPruefungAussermittigeBelastung) = New HashSet(Of ServerPruefungAussermittigeBelastung)
    Public Overridable Property ServerPruefungEichfehlergrenzen As ICollection(Of ServerPruefungEichfehlergrenzen) = New HashSet(Of ServerPruefungEichfehlergrenzen)
    Public Overridable Property ServerPruefungLinearitaetFallend As ICollection(Of ServerPruefungLinearitaetFallend) = New HashSet(Of ServerPruefungLinearitaetFallend)
    Public Overridable Property ServerPruefungLinearitaetSteigend As ICollection(Of ServerPruefungLinearitaetSteigend) = New HashSet(Of ServerPruefungLinearitaetSteigend)
    Public Overridable Property ServerPruefungAnsprechvermoegen As ICollection(Of ServerPruefungAnsprechvermoegen) = New HashSet(Of ServerPruefungAnsprechvermoegen)
    Public Overridable Property ServerPruefungRollendeLasten As ICollection(Of ServerPruefungRollendeLasten) = New HashSet(Of ServerPruefungRollendeLasten)
    Public Overridable Property ServerPruefungStabilitaetGleichgewichtslage As ICollection(Of ServerPruefungStabilitaetGleichgewichtslage) = New HashSet(Of ServerPruefungStabilitaetGleichgewichtslage)
    Public Overridable Property ServerPruefungStaffelverfahrenErsatzlast As ICollection(Of ServerPruefungStaffelverfahrenErsatzlast) = New HashSet(Of ServerPruefungStaffelverfahrenErsatzlast)
    Public Overridable Property ServerPruefungStaffelverfahrenNormallast As ICollection(Of ServerPruefungStaffelverfahrenNormallast) = New HashSet(Of ServerPruefungStaffelverfahrenNormallast)
    Public Overridable Property ServerPruefungWiederholbarkeit As ICollection(Of ServerPruefungWiederholbarkeit) = New HashSet(Of ServerPruefungWiederholbarkeit)

End Class
