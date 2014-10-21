﻿''' <summary>
''' Klasse mit allegmeinen aufrufen des Webservices
''' </summary>
''' <remarks></remarks>
''' <author></author>
''' <commentauthor></commentauthor>
Public Class clsWebserviceFunctions

    ''' <summary>
    ''' Prüft ob Webservice aufgerufen werden kann
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function TesteVerbindung() As Boolean
        Try
            'abrufen neuer WZ aus Server, basierend auf dem Wert des letzten erfolgreichen updates
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    Return False
                End Try
                'hole zusätliche Lizenzdaten als Testaufruf
                Return webContext.Test
            End Using
        Catch ex As Exception
           Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Holt der Benutzerlizenz zugeordnete Stammdaten wie Name der Firma vom Webservice
    ''' </summary>
    ''' <param name="bolneuStammdaten"></param>
    ''' <remarks></remarks>
    Public Shared Function GetNeueStammdaten(ByRef bolneuStammdaten As Boolean) As Boolean
        Try
            'abrufen neuer WZ aus Server, basierend auf dem Wert des letzten erfolgreichen updates
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    Return False
                End Try
                Using DBContext As New EichsoftwareClientdatabaseEntities1
                    'lizenzisierung holen
                    Dim objLic = (From db In DBContext.Lizensierung Where db.Lizenzschluessel = AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel And db.HEKennung = AktuellerBenutzer.Instance.Lizenz.HEKennung).FirstOrDefault
                    Dim objLizenzdaten As EichsoftwareWebservice.clsLizenzdaten = webContext.GetLizenzdaten(objLic.HEKennung, objLic.Lizenzschluessel, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                    objLic.Aktiv = objLizenzdaten.Aktiv

                    If objLic.Aktiv = False Then
                        bolneuStammdaten = False
                        Return False
                        DBContext.SaveChanges()
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_UngueltigeLizenz)
                        Application.Exit()
                        Exit Function
                    End If

                    objLic.Name = objLizenzdaten.Name
                    objLic.Vorname = objLizenzdaten.Vorname
                    objLic.Firma = objLizenzdaten.Firma
                    objLic.FirmaOrt = objLizenzdaten.FirmaOrt
                    objLic.FirmaPLZ = objLizenzdaten.FirmaPLZ
                    objLic.FirmaStrasse = objLizenzdaten.FirmaStrasse
                    objLic.FK_BenutzerID = objLizenzdaten.BenutzerID

                    bolneuStammdaten = True
                    DBContext.SaveChanges()
                    Return True

                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' holt neue oder aktualisierte WZ aus DB über Webservice
    ''' </summary>
    ''' <param name="bolNeuWZ">Variable welche für eine Erfolgsmeldung genutzt wird. Wird beim aktualisieren auf True gesetzt um den Nutzer darauf hinzuweisen, dass etwas neues heruntergeladen wurde.</param>
    ''' <remarks></remarks>
    Public Shared Sub GetNeueWZ(ByRef bolNeuWZ As Boolean)
        Try
            'abrufen neuer WZ aus Server, basierend auf dem Wert des letzten erfolgreichen updates
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    Exit Sub
                End Try
                Using DBContext As New EichsoftwareClientdatabaseEntities1

                    'Eingrenzen welche Daten synchronisiert werden müssen, je nach Einstllung des Benutzers 
                    Dim StartDatum As Date = #1/1/2000#
                    Dim EndDatum As Date = #12/31/2999#

                    If AktuellerBenutzer.Instance.Synchronisierungsmodus = "Alles" Then
                    ElseIf AktuellerBenutzer.Instance.Synchronisierungsmodus = "Ab" Then
                        StartDatum = AktuellerBenutzer.Instance.SyncAb
                    ElseIf AktuellerBenutzer.Instance.Synchronisierungsmodus = "Zwischen" Then
                        StartDatum = AktuellerBenutzer.Instance.SyncAb
                        EndDatum = AktuellerBenutzer.Instance.SyncBis
                    Else
                        AktuellerBenutzer.Instance.Synchronisierungsmodus = "Alles"
                        AktuellerBenutzer.SaveSettings()
                    End If


                    Dim objWZResultList = webContext.GetNeueWZ(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, AktuellerBenutzer.Instance.LetztesUpdate, My.User.Name, System.Environment.UserDomainName, My.Computer.Name, StartDatum, EndDatum)

                    If Not objWZResultList Is Nothing Then

                        'alle neuen Artikel aus Server iterieren
                        Dim tObjServerWZ As EichsoftwareWebservice.ServerLookup_Waegezelle 'hilfsvariable für Linq abfrage. Es gibt sonst eine Warnung wenn in Linq mit einer for each variablen gearbeitet wird
                        For Each objServerArtikel As EichsoftwareWebservice.ServerLookup_Waegezelle In objWZResultList
                            tObjServerWZ = objServerArtikel
                            'alle Artikel abrufen in denen die ID mit dem neuem Serverartikel übereinstimmt
                            Try
                                Dim query = From d In DBContext.Lookup_Waegezelle Where d.ID = tObjServerWZ._ID


                                'prüfen ob es bereits einen Artikel in der lokalen DB gibt, mit dem aktuellen ID-Wert
                                If query.Count = 0 Then 'Es gbit den Artikel noch nicht in der lokalen Datebank => insert 
                                    Dim newWZ As New Lookup_Waegezelle


                                    newWZ.ID = objServerArtikel._ID
                                    newWZ.Hoechsteteilungsfaktor = objServerArtikel._Hoechsteteilungsfaktor
                                    newWZ.Kriechteilungsfaktor = objServerArtikel._Kriechteilungsfaktor
                                    newWZ.MaxAnzahlTeilungswerte = objServerArtikel._MaxAnzahlTeilungswerte
                                    newWZ.Mindestvorlast = objServerArtikel._Mindestvorlast
                                    newWZ.MinTeilungswert = objServerArtikel._MinTeilungswert
                                    newWZ.RueckkehrVorlastsignal = objServerArtikel._RueckkehrVorlastsignal
                                    newWZ.Waegezellenkennwert = objServerArtikel._Waegezellenkennwert
                                    newWZ.WiderstandWaegezelle = objServerArtikel._WiderstandWaegezelle
                                    newWZ.Bauartzulassung = objServerArtikel._Bauartzulassung
                                    newWZ.BruchteilEichfehlergrenze = objServerArtikel._BruchteilEichfehlergrenze
                                    newWZ.Genauigkeitsklasse = objServerArtikel._Genauigkeitsklasse
                                    newWZ.GrenzwertTemperaturbereichMAX = objServerArtikel._GrenzwertTemperaturbereichMAX
                                    newWZ.GrenzwertTemperaturbereichMIN = objServerArtikel._GrenzwertTemperaturbereichMIN
                                    newWZ.Hersteller = objServerArtikel._Hersteller
                                    newWZ.Pruefbericht = objServerArtikel._Pruefbericht
                                    newWZ.Typ = objServerArtikel._Typ
                                    newWZ.Deaktiviert = objServerArtikel._Deaktiviert
                                    newWZ.Neu = objServerArtikel._Neu
                                    'hinzufügen des neu erzeugten Artikels in Lokale Datenbank

                                    DBContext.Lookup_Waegezelle.Add(newWZ)
                                    Try
                                        DBContext.SaveChanges()
                                        bolNeuWZ = True
                                    Catch e As Exception
                                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_Speichern, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        MessageBox.Show(e.StackTrace, e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End Try

                                Else 'Es gibt den Artikel bereits, er wird geupdated
                                    For Each objWZ As Lookup_Waegezelle In query 'es sollte nur einen Artikel Geben, da die IDs eindeutig sind.
                                        objWZ.Hoechsteteilungsfaktor = objServerArtikel._Hoechsteteilungsfaktor
                                        objWZ.Kriechteilungsfaktor = objServerArtikel._Kriechteilungsfaktor
                                        objWZ.MaxAnzahlTeilungswerte = objServerArtikel._MaxAnzahlTeilungswerte
                                        objWZ.Mindestvorlast = objServerArtikel._Mindestvorlast
                                        objWZ.MinTeilungswert = objServerArtikel._MinTeilungswert
                                        objWZ.RueckkehrVorlastsignal = objServerArtikel._RueckkehrVorlastsignal
                                        objWZ.Waegezellenkennwert = objServerArtikel._Waegezellenkennwert
                                        objWZ.WiderstandWaegezelle = objServerArtikel._WiderstandWaegezelle
                                        objWZ.Bauartzulassung = objServerArtikel._Bauartzulassung
                                        objWZ.BruchteilEichfehlergrenze = objServerArtikel._BruchteilEichfehlergrenze
                                        objWZ.Genauigkeitsklasse = objServerArtikel._Genauigkeitsklasse
                                        objWZ.GrenzwertTemperaturbereichMAX = objServerArtikel._GrenzwertTemperaturbereichMAX
                                        objWZ.GrenzwertTemperaturbereichMIN = objServerArtikel._GrenzwertTemperaturbereichMIN
                                        objWZ.Hersteller = objServerArtikel._Hersteller
                                        objWZ.Pruefbericht = objServerArtikel._Pruefbericht
                                        objWZ.Typ = objServerArtikel._Typ
                                        objWZ.Deaktiviert = objServerArtikel._Deaktiviert
                                        objWZ.Neu = objServerArtikel._Neu
                                    Next
                                End If
                            Catch ex As Exception
                                MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            End Try
                        Next

                    End If
                    Try
                        DBContext.SaveChanges()
                    Catch ex As Exception
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' holt neue oder aktualisierte AWGs aus DB über Webservice
    ''' </summary>
    ''' <param name="bolNeuAWG">Variable welche für eine Erfolgsmeldung genutzt wird. Wird beim aktualisieren auf True gesetzt um den Nutzer darauf hinzuweisen, dass etwas neues heruntergeladen wurde.</param>
    ''' <remarks></remarks>
    Public Shared Sub GetNeuesAWG(ByRef bolNeuAWG As Boolean)
        Try
            'abrufen neuer WZ aus Server, basierend auf dem Wert des letzten erfolgreichen updates
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    Exit Sub
                End Try
                Using DBContext As New EichsoftwareClientdatabaseEntities1
                    'Eingrenzen welche Daten synchronisiert werden müssen, je nach Einstllung des Benutzers 
                    Dim StartDatum As Date = #1/1/2000#
                    Dim EndDatum As Date = #12/31/2999#

                    If AktuellerBenutzer.Instance.Synchronisierungsmodus = "Alles" Then
                    ElseIf AktuellerBenutzer.Instance.Synchronisierungsmodus = "Ab" Then
                        StartDatum = AktuellerBenutzer.Instance.SyncAb
                    ElseIf AktuellerBenutzer.Instance.Synchronisierungsmodus = "Zwischen" Then
                        StartDatum = AktuellerBenutzer.Instance.SyncAb
                        EndDatum = AktuellerBenutzer.Instance.SyncBis
                    Else
                        AktuellerBenutzer.Instance.Synchronisierungsmodus = "Alles"
                        AktuellerBenutzer.SaveSettings()
                    End If

                    Dim objAWGResultList = webContext.GetNeuesAWG(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, AktuellerBenutzer.Instance.LetztesUpdate, My.User.Name, System.Environment.UserDomainName, My.Computer.Name, StartDatum, EndDatum)
                    If Not objAWGResultList Is Nothing Then

                        'alle neuen Artikel aus Server iterieren
                        Dim tObjServerAWG As EichsoftwareWebservice.ServerLookup_Auswertegeraet 'hilfsvariable für Linq abfrage. Es gibt sonst eine Warnung wenn in Linq mit einer for each variablen gearbeitet wird
                        For Each objServerArtikel As EichsoftwareWebservice.ServerLookup_Auswertegeraet In objAWGResultList
                            tObjServerAWG = objServerArtikel
                            'alle Artikel abrufen in denen die ID mit dem neuem Serverartikel übereinstimmt
                            Dim query = From d In DBContext.Lookup_Auswertegeraet Where d.ID = tObjServerAWG._ID

                            'prüfen ob es bereits einen Artikel in der lokalen DB gibt, mit dem aktuellen ID-Wert
                            If query.Count = 0 Then 'Es gbit den Artikel noch nicht in der lokalen Datebank => insert 
                                Dim newAWG As New Lookup_Auswertegeraet


                                newAWG.ID = objServerArtikel._ID
                                newAWG.Bauartzulassung = objServerArtikel._Bauartzulassung
                                newAWG.BruchteilEichfehlergrenze = objServerArtikel._BruchteilEichfehlergrenze
                                newAWG.Genauigkeitsklasse = objServerArtikel._Genauigkeitsklasse
                                newAWG.GrenzwertLastwiderstandMAX = objServerArtikel._GrenzwertLastwiderstandMAX
                                newAWG.GrenzwertLastwiderstandMIN = objServerArtikel._GrenzwertLastwiderstandMIN
                                newAWG.GrenzwertTemperaturbereichMAX = objServerArtikel._GrenzwertTemperaturbereichMAX
                                newAWG.GrenzwertTemperaturbereichMIN = objServerArtikel._GrenzwertTemperaturbereichMIN
                                newAWG.Hersteller = objServerArtikel._Hersteller
                                newAWG.KabellaengeQuerschnitt = objServerArtikel._KabellaengeQuerschnitt
                                newAWG.MAXAnzahlTeilungswerteEinbereichswaage = objServerArtikel._MAXAnzahlTeilungswerteEinbereichswaage
                                newAWG.MAXAnzahlTeilungswerteMehrbereichswaage = objServerArtikel._MAXAnzahlTeilungswerteMehrbereichswaage
                                newAWG.Mindesteingangsspannung = objServerArtikel._Mindesteingangsspannung
                                newAWG.Mindestmesssignal = objServerArtikel._Mindestmesssignal
                                newAWG.Pruefbericht = objServerArtikel._Pruefbericht
                                newAWG.Speisespannung = objServerArtikel._Speisespannung
                                newAWG.Typ = objServerArtikel._Typ
                                newAWG.Deaktiviert = objServerArtikel._Deaktiviert
                                newAWG.TaraeinrichtungHalbSelbsttaetig = objServerArtikel._TaraeinrichtungHalbSelbsttaetig
                                newAWG.TaraeinrichtungSelbsttaetig = objServerArtikel._TaraeinrichtungSelbsttaetig
                                newAWG.TaraeinrichtungTaraeingabe = objServerArtikel._TaraeinrichtungTaraeingabe
                                newAWG.NullstellungHalbSelbsttaetig = objServerArtikel._NullstellungHalbSelbsttaetig
                                newAWG.NullstellungSelbsttaetig = objServerArtikel._NullstellungSelbsttaetig
                                newAWG.NullstellungNullnachfuehrung = objServerArtikel._NullstellungNullnachfuehrung

                                'hinzufügen des neu erzeugten Artikels in Lokale Datenbank
                                DBContext.Lookup_Auswertegeraet.Add(newAWG)
                                DBContext.SaveChanges()
                                bolNeuAWG = True

                            Else 'Es gibt den Artikel bereits, er wird geupdated
                                For Each objAWG As Lookup_Auswertegeraet In query 'es sollte nur einen Artikel Geben, da die IDs eindeutig sind.
                                    objAWG.Bauartzulassung = objServerArtikel._Bauartzulassung
                                    objAWG.BruchteilEichfehlergrenze = objServerArtikel._BruchteilEichfehlergrenze
                                    objAWG.Genauigkeitsklasse = objServerArtikel._Genauigkeitsklasse
                                    objAWG.GrenzwertLastwiderstandMAX = objServerArtikel._GrenzwertLastwiderstandMAX
                                    objAWG.GrenzwertLastwiderstandMIN = objServerArtikel._GrenzwertLastwiderstandMIN
                                    objAWG.GrenzwertTemperaturbereichMAX = objServerArtikel._GrenzwertTemperaturbereichMAX
                                    objAWG.GrenzwertTemperaturbereichMIN = objServerArtikel._GrenzwertTemperaturbereichMIN
                                    objAWG.Hersteller = objServerArtikel._Hersteller
                                    objAWG.KabellaengeQuerschnitt = objServerArtikel._KabellaengeQuerschnitt
                                    objAWG.MAXAnzahlTeilungswerteEinbereichswaage = objServerArtikel._MAXAnzahlTeilungswerteEinbereichswaage
                                    objAWG.MAXAnzahlTeilungswerteMehrbereichswaage = objServerArtikel._MAXAnzahlTeilungswerteMehrbereichswaage
                                    objAWG.Mindesteingangsspannung = objServerArtikel._Mindesteingangsspannung
                                    objAWG.Mindestmesssignal = objServerArtikel._Mindestmesssignal
                                    objAWG.Pruefbericht = objServerArtikel._Pruefbericht
                                    objAWG.Speisespannung = objServerArtikel._Speisespannung
                                    objAWG.Typ = objServerArtikel._Typ
                                    objAWG.TaraeinrichtungHalbSelbsttaetig = objServerArtikel._TaraeinrichtungHalbSelbsttaetig
                                    objAWG.TaraeinrichtungSelbsttaetig = objServerArtikel._TaraeinrichtungSelbsttaetig
                                    objAWG.TaraeinrichtungTaraeingabe = objServerArtikel._TaraeinrichtungTaraeingabe
                                    objAWG.NullstellungHalbSelbsttaetig = objServerArtikel._NullstellungHalbSelbsttaetig
                                    objAWG.NullstellungSelbsttaetig = objServerArtikel._NullstellungSelbsttaetig
                                    objAWG.NullstellungNullnachfuehrung = objServerArtikel._NullstellungNullnachfuehrung
                                    objAWG.Deaktiviert = objServerArtikel._Deaktiviert
                                    bolNeuAWG = True
                                Next
                            End If
                        Next
                    End If

                    Try
                        DBContext.SaveChanges()
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_Laden, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End Using

            End Using

        Catch ex As Exception
            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ''' <summary>
    ''' holt aktuellen Status eigenerer Eichungenen (z.b. Abgelehnt oder Erfolgreich) aus DB über Webservice
    ''' </summary>
    ''' <param name="bolNeuGenehmigung">Variable welche für eine Erfolgsmeldung genutzt wird. Wird beim aktualisieren auf True gesetzt um den Nutzer darauf hinzuweisen, dass etwas neues heruntergeladen wurde.</param>
    ''' <remarks></remarks>
    Public Shared Sub GetGenehmigungsstatus(ByRef bolNeuGenehmigung As Boolean)
        Try

            'abrufen des Statusts für jeden versendeten Eichprozess
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    Exit Sub
                End Try
                Using DBContext As New EichsoftwareClientdatabaseEntities1


                    'hole die prozesse mit dem status 1 = in bearbeitung bei rhewa
                    Dim query = From db In DBContext.Eichprozess.Include("Eichprotokoll").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select db Where db.FK_Bearbeitungsstatus = 1

                    For Each Eichprozess In query
                        Try
                            Dim NeuerStatus As String = webContext.CheckGueltigkeitEichprozess(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Eichprozess.Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                            If Not NeuerStatus Is Nothing Then
                                If Eichprozess.FK_Bearbeitungsstatus <> NeuerStatus Then

                                    'wenn es eine Änderung gab, wird das geänderte Objekt vom Server abgerufen. Damit können änderungen die von einem RHEWA Mitarbeiter durchgeführt wurden übernommen werden
                                    '###################
                                    'neue Datenbankverbindung
                                    Dim objServerEichprozess = webContext.GetEichProzess(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Eichprozess.Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                                    If objServerEichprozess Is Nothing Then
                                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_KeinServerObjektEichung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    End If

                                    'umwandeln des Serverobjektes in Clientobject
                                    clsClientServerConversionFunctions.CopyClientObjectPropertiesWithOwnIDs(Eichprozess, objServerEichprozess)

                                    Eichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Stammdateneingabe 'überschreiben des Statuses
                                    Eichprozess.FK_Bearbeitungsstatus = NeuerStatus
                                    Try
                                        DBContext.SaveChanges()
                                        bolNeuGenehmigung = True
                                    Catch ex As Entity.Infrastructure.DbUpdateException
                                        MessageBox.Show(ex.InnerException.InnerException.Message)
                                    Catch ex2 As Entity.Validation.DbEntityValidationException
                                        For Each o In ex2.EntityValidationErrors
                                            For Each v In o.ValidationErrors
                                                MessageBox.Show(v.ErrorMessage & " " & v.PropertyName)
                                            Next
                                        Next
                                    End Try
                                    'Methode welche alle N:1 Verbindungen auf einen Eichprozess entfernt und mit neuen Werten neu anlegt. (Es koennen z.b. neue Pruefstaffeln eingetragen worden sein, somit ist es einfacher alles zu löschen und neu anzulegen als zu updaten)
                                    clsClientServerConversionFunctions.UpdateForeignTables(Eichprozess, objServerEichprozess)
                                End If
                            End If
                        Catch ex As Exception
                            MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_Speichern, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    Next
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' holt alle einem zugehörigen Eichprotokolle vom RHEWA Server. z.B. wenn die anwendung auf einem neuem PC installiert wurde
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub GetEichprotokolleVomServer()
        Try
            'abrufen des Statusts für jeden versendeten Eichprozess
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    Exit Sub
                End Try
                Using DBContext As New EichsoftwareClientdatabaseEntities1
                    'Eingrenzen welche Daten synchronisiert werden müssen, je nach Einstllung des Benutzers 
                    Dim StartDatum As Date = #1/1/2000#
                    Dim EndDatum As Date = #12/31/2999#

                    If AktuellerBenutzer.Instance.Synchronisierungsmodus = "Alles" Then
                    ElseIf AktuellerBenutzer.Instance.Synchronisierungsmodus = "Ab" Then
                        StartDatum = AktuellerBenutzer.Instance.SyncAb
                    ElseIf AktuellerBenutzer.Instance.Synchronisierungsmodus = "Zwischen" Then
                        StartDatum = AktuellerBenutzer.Instance.SyncAb
                        EndDatum = AktuellerBenutzer.Instance.SyncBis
                    Else
                        AktuellerBenutzer.Instance.Synchronisierungsmodus = "Alles"
                        AktuellerBenutzer.SaveSettings()
                    End If


                    Try
                        'wenn es eine Änderung gab, wird das geänderte Objekt vom Server abgerufen. Damit können änderungen die von einem RHEWA Mitarbeiter durchgeführt wurden übernommen werden
                        'neue Datenbankverbindung
                        Dim objServerEichprozesse = webContext.GetAlleEichprozesseImZeitraum(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, My.User.Name, System.Environment.UserDomainName, My.Computer.Name, StartDatum, EndDatum)
                        If objServerEichprozesse Is Nothing Then
                            Exit Sub
                            'MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_KeinServerObjektEichung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                        For Each objectServerEichprozess In objServerEichprozesse
                            'umwandeln des Serverobjektes in Clientobject
                            Dim Eichprozess = DBContext.Eichprozess.Create

                            'erzeuge lokale Kopie
                            clsClientServerConversionFunctions.CopyClientObjectPropertiesWithNewIDs(Eichprozess, objectServerEichprozess, True)
                            Try
                                DBContext.Eichprozess.Add(Eichprozess)
                                DBContext.SaveChanges()
                            Catch ex As Entity.Infrastructure.DbUpdateException
                                MessageBox.Show(ex.InnerException.InnerException.Message)
                            Catch ex2 As Entity.Validation.DbEntityValidationException
                                For Each o In ex2.EntityValidationErrors
                                    For Each v In o.ValidationErrors
                                        MessageBox.Show(v.ErrorMessage & " " & v.PropertyName)
                                    Next
                                Next
                            End Try
                        Next
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_Speichern, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    ''' <summary>
    ''' Methode welche Eichprozesse vom Server holt und die Daten in eine für das datagrid Bindbare Auflistung speichert.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetServerEichprotokollListe() As EichsoftwareWebservice.clsEichprozessFuerAuswahlliste()
        Try
            'neuen Context aufbauen
            Using WebContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    WebContext.Open()
                Catch ex As Exception
                    Return Nothing
                End Try

                Try
                    Dim data = WebContext.GetAlleEichprozesse(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                    Return data
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                    If Debugger.IsAttached Then
                        MessageBox.Show(ex.StackTrace)
                        MessageBox.Show(ex.InnerException.Message)
                        MessageBox.Show(ex.InnerException.StackTrace)
                        MessageBox.Show(ex.InnerException.InnerException.Message)
                        MessageBox.Show(ex.InnerException.InnerException.StackTrace)
                    End If
                    Return Nothing
                End Try
            End Using
        Catch ex As ServiceModel.EndpointNotFoundException
            MessageBox.Show("Keine Verbindung zum Stratoserver möglich")
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Methode welche Eichprozesse vom Server holt, die als Standardwaage deklariert sind und die Daten in eine für das datagrid Bindbare Auflistung speichert.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetServerStandardwaagen() As EichsoftwareWebservice.clsEichprozessFuerAuswahlliste()
        Try
            'neuen Context aufbauen
            Using WebContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    WebContext.Open()
                Catch ex As Exception
                    Return Nothing
                End Try

                Try
                    Dim data = WebContext.GetStandardwaagen(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                    Return data
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                    If Debugger.IsAttached Then
                        MessageBox.Show(ex.StackTrace)
                        MessageBox.Show(ex.InnerException.Message)
                        MessageBox.Show(ex.InnerException.StackTrace)
                        MessageBox.Show(ex.InnerException.InnerException.Message)
                        MessageBox.Show(ex.InnerException.InnerException.StackTrace)
                    End If
                    Return Nothing
                End Try
            End Using
        Catch ex As ServiceModel.EndpointNotFoundException
            MessageBox.Show("Keine Verbindung zum Stratoserver möglich")
            Return Nothing
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Lädt ein Serverobjekt anhand von Vorgangsnummer und erzeugt ein Clientobjet zur Verwendung.
    ''' </summary>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="NeueFabriknummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetLokaleKopieVonEichprozess(ByVal Vorgangsnummer As String, ByVal NeueFabriknummer As String) As Eichprozess
        Dim objServerEichprozess As EichsoftwareWebservice.ServerEichprozess = Nothing
        Dim objClientEichprozess As Eichprozess = Nothing
        'neue Datenbankverbindung
        Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
            Try
                webContext.Open()
            Catch ex As Exception
                MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try
            Using dbcontext As New EichsoftwareClientdatabaseEntities1
                Try
                    objClientEichprozess = dbcontext.Eichprozess.Create
                    objServerEichprozess = webContext.GetEichProzess(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)


                    If objServerEichprozess Is Nothing Then
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_KeinServerObjektEichung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    'umwandeln des Serverobjektes in Clientobject
                    clsClientServerConversionFunctions.CopyClientObjectPropertiesWithNewIDs(objClientEichprozess, objServerEichprozess)

                    'vorgangsnummer editieren
                    objClientEichprozess.Vorgangsnummer = Guid.NewGuid.ToString
                    objClientEichprozess.FK_Bearbeitungsstatus = 4 'noch nichts
                    objClientEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Stammdateneingabe

                    'standardwaaeg zur identifizierung des verkürrtzten Prozesses setzen
                    objClientEichprozess.AusStandardwaageErzeugt = True
                    'neue Fabriknummer
                    objClientEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_FabrikNummer = NeueFabriknummer
                    'Prüfscheinnummer leeren
                    objClientEichprozess.Eichprotokoll.Beschaffenheitspruefung_Pruefscheinnummer = ""
                    'Außermittige Belastung Leeren
                    objClientEichprozess.Eichprotokoll.PruefungAussermittigeBelastung.Clear()
                    'Genauigkeit Nullstellung leeren
                    objClientEichprozess.Eichprotokoll.GenauigkeitNullstellung_InOrdnung = False
                    'Prüfung Linearität leeren
                    objClientEichprozess.Eichprotokoll.PruefungLinearitaetFallend.Clear()
                    objClientEichprozess.Eichprotokoll.PruefungLinearitaetSteigend.Clear()
                    'Überlastanzeige leeren
                    objClientEichprozess.Eichprotokoll.Ueberlastanzeige_Ueberlast = False
                    'Fallbeschleunigung wird geleert
                    objClientEichprozess.Eichprotokoll.Fallbeschleunigung_ms2 = False

                    'hinufügen zur Lokalen DB
                    dbcontext.Eichprozess.Add(objClientEichprozess)

                    Try
                        dbcontext.SaveChanges()
                    Catch ex As Entity.Infrastructure.DbUpdateException
                        MessageBox.Show(ex.InnerException.InnerException.Message)
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_SpeicherAnomalie, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK)
                        If AktuellerBenutzer.Instance.Lizenz.RHEWALizenz Then
                            MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_SpeicherAnomalieRhewaZusatztext, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK)
                        End If

                        Return Nothing
                    Catch ex2 As Entity.Validation.DbEntityValidationException
                        For Each o In ex2.EntityValidationErrors
                            For Each v In o.ValidationErrors
                                MessageBox.Show(v.ErrorMessage & " " & v.PropertyName)
                            Next
                        Next
                        Return Nothing
                    End Try

                    'rückgabe 
                    Return objClientEichprozess

                Catch ex As Exception
                    Return Nothing
                End Try
            End Using
        End Using
    End Function


    ''' <summary>
    ''' erzeugt 1:1 kopie von Serverobjekt in lokaler DB. wird nicht mehr benötigt, da der kopiervorgang nun verschärft wurde und keine 1:1 kopie mehr erzeugt werden darf.
    ''' </summary>
    ''' <param name="Vorgangsnummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function old_GetLokaleKopieVonEichprozess(ByVal Vorgangsnummer As String) As Eichprozess
        Dim objServerEichprozess As EichsoftwareWebservice.ServerEichprozess = Nothing
        Dim objClientEichprozess As Eichprozess = Nothing
        'neue Datenbankverbindung
        Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
            Try
                webContext.Open()
            Catch ex As Exception
                MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try
            Using dbcontext As New EichsoftwareClientdatabaseEntities1
                Try
                    objClientEichprozess = dbcontext.Eichprozess.Create
                    objServerEichprozess = webContext.GetEichProzess(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)


                    If objServerEichprozess Is Nothing Then
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_KeinServerObjektEichung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    'umwandeln des Serverobjektes in Clientobject
                    clsClientServerConversionFunctions.CopyClientObjectPropertiesWithNewIDs(objClientEichprozess, objServerEichprozess)

                    'vorgangsnummer editieren
                    objClientEichprozess.Vorgangsnummer = Guid.NewGuid.ToString
                    objClientEichprozess.FK_Bearbeitungsstatus = 4 'noch nichts
                    objClientEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Stammdateneingabe


                    dbcontext.Eichprozess.Add(objClientEichprozess)

                    Try
                        dbcontext.SaveChanges()
                    Catch ex As Entity.Infrastructure.DbUpdateException
                        MessageBox.Show(ex.InnerException.InnerException.Message)
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_SpeicherAnomalie, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK)
                        If AktuellerBenutzer.Instance.Lizenz.RHEWALizenz Then
                            MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_SpeicherAnomalieRhewaZusatztext, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK)
                        End If

                        Return Nothing
                    Catch ex2 As Entity.Validation.DbEntityValidationException
                        For Each o In ex2.EntityValidationErrors
                            For Each v In o.ValidationErrors
                                MessageBox.Show(v.ErrorMessage & " " & v.PropertyName)
                            Next
                        Next
                        Return Nothing
                    End Try

                    Return objClientEichprozess

                Catch ex As Exception
                    Return Nothing
                End Try
            End Using
        End Using

    End Function

    ''' <summary>
    ''' genehmigt Eichprozess anhand von Vorgangsnummer auf dem Server
    ''' </summary>
    ''' <param name="Vorgangsnummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GenehmigeEichprozess(ByVal Vorgangsnummer As String)
        Dim result As String = ""
        Dim Messagetext As String = ""
        Dim bolSetGueltig As Boolean = True 'variable zum abbrechen des Prozesses, falls jemand anderes an dem DS arbeitet

        'neue Datenbankverbindung
        Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
            Try
                webContext.Open()
            Catch ex As Exception
                MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try



            'prüfen ob der datensatz von jemand anderem in Bearbeitung ist

            Messagetext = webContext.CheckSperrung(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
            If Messagetext.Equals("") = False Then
                'rhewa arbeitet in deutsch und hat keine lokalisierung gewünscht
                result = webContext.SetSperrung(True, AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                If result = "" Then
                    bolSetGueltig = True
                Else
                    MessageBox.Show(result)
                    bolSetGueltig = False
                    Return False
                End If
            End If
            If bolSetGueltig Then
                webContext.SetEichprozessGenehmight(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                Return True
            End If
        End Using

        Return False
    End Function

    ''' <summary>
    '''  lehnt Eichprozess ab, anhand von Vorgangsnummer auf dem Server
    ''' </summary>
    ''' <param name="Vorgangsnummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function AblehnenEichprozess(ByVal Vorgangsnummer As String) As Boolean
        Dim result As String = ""
        Dim bolSetUnueltig As Boolean = True 'variable zum abbrechen des Prozesses, falls jemand anderes an dem DS arbeitet
        Dim Messagetext As String = ""
        'neue Datenbankverbindung
        Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
            Try
                webContext.Open()
            Catch ex As Exception
                MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try

            Messagetext = webContext.CheckSperrung(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
            If Messagetext.Equals("") = False Then
                'rhewa arbeitet in deutsch und hat keine lokalisierung gewünscht
                If MessageBox.Show("Dieser Konformitätsbewertungsprozess wird von '" & Messagetext & "' bearbeitet. Möchten Sie seine Arbeit wirklich überschreiben und den Prozess ablehnen?", My.Resources.GlobaleLokalisierung.Frage, MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    result = webContext.SetSperrung(True, AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                    If result = "" Then
                        bolSetUnueltig = True
                    Else
                        MessageBox.Show(result)
                        bolSetUnueltig = False
                        Return False
                    End If
                Else
                    bolSetUnueltig = False
                End If
            End If
            If bolSetUnueltig Then
                webContext.SetEichprozessUngueltig(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                Return True
            End If
        End Using
        Return False
    End Function

    ''' <summary>
    ''' lädt serverobjekt herunter ohne es in der lokalen DB zu speichern. So kann das in Memoryobjekt genutzt werden um es anzuschauen
    ''' </summary>
    ''' <param name="Vorgangsnummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ZeigeServerEichprozess(ByVal Vorgangsnummer As String) As Eichprozess
        Dim objServerEichprozess As EichsoftwareWebservice.ServerEichprozess = Nothing
        Dim objClientEichprozess As Eichprozess = Nothing
        'neue Datenbankverbindung
        Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
            Try
                webContext.Open()
            Catch ex As Exception
                MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End Try

            Using dbcontext As New EichsoftwareClientdatabaseEntities1

                objClientEichprozess = dbcontext.Eichprozess.Create
                objServerEichprozess = webContext.GetEichProzess(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, Vorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                If objServerEichprozess Is Nothing Then
                    MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_KeinServerObjektEichung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                'umwandeln des Serverobjektes in Clientobject
                clsClientServerConversionFunctions.CopyClientObjectPropertiesWithAllLookups(objClientEichprozess, objServerEichprozess)
                clsClientServerConversionFunctions.GetLookupValuesServer(objClientEichprozess)

                Return objClientEichprozess
            End Using
        End Using
        Return Nothing
    End Function


    ''' <summary>
    ''' Sperrt den aktuellen Server Eichprozess zur Bearbeitung. Wenn ein anderer Benutzer diesen DS öffnen will, kriegt er eine hinweismeldung
    ''' </summary>
    ''' <param name="bolSperren"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Public Shared Function SetzeSperrung(ByVal bolSperren As Boolean, ByVal EichprozessVorgangsnummer As String) As Boolean
        Dim result As String = ""
        Dim bolSetSperrung As Boolean = True 'variable zum abbrechen des Prozesses, falls jemand anderes an dem DS arbeitet
        Dim Messagetext As String = "" 'variable bekommt Ergebniss der Sperrprüfung. Ist anschließend leer wenn keine Sperrung vorliegt

        'neue Datenbankverbindung
        Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
            Try
                webContext.Open()
            Catch ex As Exception
                MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try

            'prüfen ob der datensatz von jemand anderem in Bearbeitung ist
           Messagetext = PruefeSperrung(EichprozessVorgangsnummer)

            If Messagetext.Equals("") = False Then
                'rhewa arbeitet in deutsch und hat keine lokalisierung gewünscht
                If MessageBox.Show("Dieser Konformitätsbewertungsprozess wird von '" & Messagetext & "' bearbeitet. Möchten Sie seine Arbeit wirklich überschreiben und den Prozess selbst bearbeiten?", My.Resources.GlobaleLokalisierung.Frage, MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    bolSetSperrung = True
                Else
                    bolSetSperrung = False
                End If
            End If

            If bolSetSperrung Then
                result = webContext.SetSperrung(bolSperren, AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, EichprozessVorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)

                If result = "" Then
                    Return True
                Else
                    MessageBox.Show(result)
                    Return False
                End If
            Else
                Return False
            End If

        End Using
    End Function

    ''' <summary>
    ''' Prüft ob DS gesperrt ist. 
    ''' </summary>
    ''' <param name="EichprozessVorgangsnummer">Die Vorgangsnummer des Eichprozesses den es zu prüfen gilt</param>
    ''' <returns>String.Empty wenn nicht gesperrt. Ansonsten Hinweis auf den Benutzer der gesperrt hat</returns>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Public Shared Function PruefeSperrung(ByVal EichprozessVorgangsnummer As String) As String
        Dim Messagetext As String = ""

        Try
            'neue Datenbankverbindung
            Using webContext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                Try
                    webContext.Open()
                Catch ex As Exception
                    MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End Try

                Messagetext = webContext.CheckSperrung(AktuellerBenutzer.Instance.Lizenz.HEKennung, AktuellerBenutzer.Instance.Lizenz.Lizenzschluessel, EichprozessVorgangsnummer, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)
                Return Messagetext
            End Using
            Return ""
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
