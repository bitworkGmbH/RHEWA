' HINWEIS: Mit dem Befehl "Umbenennen" im Kontextmenü können Sie den Klassennamen "Service1" sowohl im Code als auch in der SVC-Datei und der Konfigurationsdatei ändern.
Public Class EichsoftwareWebservice
    Implements IEichsoftwareWebservice

#Region "Konstruktur"
    Public Sub New()
    End Sub
#End Region
#Region "obsolete"
    Public Function Test() As Boolean Implements IEichsoftwareWebservice.Test
        'TODO entfernen und im Client Servicecontract aktualisieren
        Return True
    End Function
#End Region
#Region "Logging"

    ''' <summary>
    ''' Funktion welche einen Eintrag in dem SQL Verbindungsprotokoll vornimmt. Dieses dient zur Nachkontrolle über die Aktivitäten von Benutzern / Lizenzen
    ''' </summary>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <remarks></remarks>
    Public Sub SchreibeVerbindungsprotokoll(ByVal Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String, ByVal Aktivitaet As String) Implements IEichsoftwareWebservice.SchreibeVerbindungsprotokoll

        Dim thread As New Threading.Thread(Sub()
                                               Try
                                                   Using dbcontext As New HerstellerersteichungEntities

                                                       Dim objProtokoll = New ServerVerbindungsprotokoll
                                                       objProtokoll.Lizenzschluessel_FK = Lizenzschluessel
                                                       objProtokoll.Computername = Computername
                                                       objProtokoll.Domain = Domainname
                                                       objProtokoll.Windowsbenutzer = WindowsUsername
                                                       objProtokoll.Aktion = Aktivitaet
                                                       objProtokoll.Zeitstempel = Date.Now

                                                       dbcontext.ServerVerbindungsprotokoll.Add(objProtokoll)
                                                       dbcontext.SaveChanges()
                                                   End Using
                                               Catch ex As Exception
                                                   Try
                                                       SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
                                                   Catch ex2 As Exception
                                                       'scheinbar fehler bei der Verbindung zur Datenbank. Dies kann logischerweise nicht gelogt werden
                                                   End Try
                                               End Try
                                           End Sub)
        thread.Start()
    End Sub

#End Region
#Region "Lizenzen"
    ''' <summary>
    ''' Prüft ob es sich um eine gültige noch aktive Lizenz handelt
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLizenz(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As Boolean Implements IEichsoftwareWebservice.PruefeLizenz
        'SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Prüfe Lizenz")

        Try
            Using dbcontext As New HerstellerersteichungEntities
                Dim ObjLizenz = (From lic In dbcontext.ServerLizensierung Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel And lic.Aktiv = True).FirstOrDefault
                If Not ObjLizenz Is Nothing Then
                    Return True
                Else
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Lizenz ungültig")
                    Return False
                End If
            End Using
        Catch ex As Exception
            Return False

        End Try

    End Function





    ''' <summary>
    ''' Aggregiert aus der Server DB Kunden und Benutzerinformationen, für Client Datenbank (diese hat keine Tabellen wie Firma, Benutzer etc.) Dort sind alle Informationen der Lizenz zugeordnet
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetLizenzdaten(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As clsLizenzdaten Implements IEichsoftwareWebservice.GetLizenzdaten
        Dim objLizenzdaten As New clsLizenzdaten
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole Lizenzdaten")
        Try
            Using dbcontext As New HerstellerersteichungEntities
                'lizenz holen für Benutzer_FK
                Dim objLic = (From lic In dbcontext.ServerLizensierung Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel And lic.Aktiv = True).FirstOrDefault
                'Benutzer anhand von Benutzer_FK holen für Firma
                Dim objBenutzer = (From Benutzer In dbcontext.Benutzer Where Benutzer.ID = objLic.FK_BenutzerID).FirstOrDefault
                'firma holen
                Dim objfirma = (From Firma In dbcontext.Firmen Where Firma.ID = objBenutzer.Firma_FK).FirstOrDefault

                objLizenzdaten.Vorname = objBenutzer.Vorname
                objLizenzdaten.Name = objBenutzer.Nachname
                objLizenzdaten.BenutzerID = objBenutzer.ID
                objLizenzdaten.Aktiv = True 'wird oben bereits gefiltert
                objLizenzdaten.Firma = objfirma.Name
                objLizenzdaten.FirmaPLZ = objfirma.PLZ
                objLizenzdaten.FirmaOrt = objfirma.Ort
                objLizenzdaten.FirmaStrasse = objfirma.Strasse

                Return objLizenzdaten
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return objLizenzdaten
        End Try
    End Function

    ''' <summary>
    ''' Aktiviert die Lizenz
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetValidLizenz(ByVal HEKennung As String, ByVal Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As Boolean Implements IEichsoftwareWebservice.AktiviereLizenz
        Try
            Using dbcontext As New HerstellerersteichungEntities
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Aktiviere Lizenz")

                Dim ObjLizenz = (From lic In dbcontext.ServerLizensierung Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel And lic.Aktiv = True).FirstOrDefault
                If Not ObjLizenz Is Nothing Then
                    ObjLizenz.LetzteAktivierung = Now
                    dbcontext.SaveChanges()
                    Return True
                Else
                    Return False
                End If
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
        End Try

    End Function

    ''' <summary>
    ''' gibt zurück ob es sich um einen RHEWA Mitarbeiter handelt
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValidRHEWALizenz(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As Boolean Implements IEichsoftwareWebservice.PruefeObRHEWALizenz
        Try
            Using dbcontext As New HerstellerersteichungEntities
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Prüfe ob RHEWA Mitarbeiter")

                Dim ObjLizenz = (From lic In dbcontext.ServerLizensierung Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel).FirstOrDefault
                If Not ObjLizenz Is Nothing Then
                    Return ObjLizenz.RHEWALizenz
                Else
                    Return False
                End If
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
        End Try

    End Function
#End Region
#Region "Anlegen von neuen Wägezellen seitens des Clients in Server DB"
    ''' <summary>
    ''' Clients können WZ anlegen und veröffentlichen. Diese werden hiermit dem Server zugeführt. Über den Adminclient müssen die WZ bearbeitet und freigegeben werden. Ab dann stehen sie allen Clients zur Verfügung
    ''' </summary>
    ''' <param name="HEKennung">The HE kennung.</param>
    ''' <param name="Lizenzschluessel">The lizenzschluessel.</param>
    ''' <param name="pObjWZ">The p obj WZ.</param>
    ''' <param name="WindowsUsername">The windows username.</param>
    ''' <param name="Domainname">The domainname.</param>
    ''' <param name="Computername">The computername.</param>
    ''' <returns></returns>
    Public Function AddWaegezelle(HEKennung As String, Lizenzschluessel As String, ByVal pObjWZ As ServerLookup_Waegezelle, WindowsUsername As String, Domainname As String, Computername As String) As String Implements IEichsoftwareWebservice.AddWaegezelle
        Try
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Füge neue WZ hinzu")

            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return ""
            'neuen Context aufbauen
            'prüfen ob der eichprozess schoneinmal eingegangen ist anhand von Vorgangsnummer
            If pObjWZ.Neu Then
                Using DbContext As New HerstellerersteichungEntities
                    DbContext.Configuration.LazyLoadingEnabled = True

                    Dim Serverob = (From db In DbContext.ServerLookup_Waegezelle Select db Where db.Hersteller = pObjWZ.Hersteller And db.Typ = pObjWZ.Typ).FirstOrDefault
                    If Serverob Is Nothing Then
                        'wenn neue nicht WZ vorhanden ist
                        pObjWZ.ErstellDatum = Date.Now
                        pObjWZ.Deaktiviert = True

                        Try
                            DbContext.ServerLookup_Waegezelle.Add(pObjWZ)
                            DbContext.SaveChanges()
                        Catch ex As Exception
                            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
                            Return Nothing
                        End Try
                    Else 'ein anderer Benutzer hat diese WZ bereits angelegt
                        pObjWZ = Serverob

                    End If

                End Using
            End If
            HEKennung = Nothing
            Lizenzschluessel = Nothing

            'rückgabe der ID
            Return pObjWZ.ID
        Catch ex As Entity.Infrastructure.DbUpdateException
            Debug.WriteLine(ex.InnerException.InnerException.Message)
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return Nothing
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each e In ex.EntityValidationErrors
                For Each v In e.ValidationErrors
                    Debug.WriteLine(v.ErrorMessage & " " & v.PropertyName)
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & v.ErrorMessage & v.PropertyName)

                Next

            Next
            Return Nothing
        End Try
    End Function

#End Region

#Region "Eichprozess"

    ''' <summary>
    ''' fügt einen Lokalen Eichprozess der Serverdb hinzu
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="NewServerObj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddEichprozess(ByVal HEKennung As String, Lizenzschluessel As String, ByRef NewServerObj As ServerEichprozess, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String, Programmversionsnummer As String) As Boolean Implements IEichsoftwareWebservice.AddEichprozess
        Try
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Füge Konformitätsbewertungsprozess hinzu bzw. Aktualisiere")

            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return False
            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = True
                Dim Vorgangsnummer As String = NewServerObj.Vorgangsnummer
                Dim CurrentServerobj = (From db In DbContext.ServerEichprozess.Include("ServerEichprotokoll").Include("ServerKompatiblitaetsnachweis") Select db Where db.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                NewServerObj.UploadDatum = Date.Now
                NewServerObj.BearbeitungsDatum = Date.Now

                'anpassen des Bemerkungstextes Timestamp / Benutzer / Kommentar
                SchreibeKommentarMitVersionsinformation(HEKennung, Lizenzschluessel, NewServerObj, WindowsUsername, Domainname, Computername, Programmversionsnummer, DbContext, Vorgangsnummer, CurrentServerobj)

                'Statt einem Update wird ein Hochgeladener Eichprozess immer gelöscht und neu hinzugefügt, da er vollständig im Client zusammengebaut wird
                DeleteOldServerEichprozess(DbContext, Vorgangsnummer, CurrentServerobj)


                If NewServerObj.ErzeugerLizenz Is Nothing Then
                    NewServerObj.ErzeugerLizenz = Lizenzschluessel 'lizenzschlüssel zur identifizierung des datensatztes hinzufügen
                ElseIf NewServerObj.ErzeugerLizenz.Equals("") Then
                    NewServerObj.ErzeugerLizenz = Lizenzschluessel 'lizenzschlüssel zur identifizierung des datensatztes hinzufügen

                End If

                DbContext.ServerEichprozess.Add(NewServerObj)
                DbContext.SaveChanges()
            End Using

            HEKennung = Nothing
            Lizenzschluessel = Nothing
            NewServerObj = Nothing
            Return True
        Catch ex As Entity.Infrastructure.DbUpdateException
            Debug.WriteLine(ex.InnerException.InnerException.Message)
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Try
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Ergänzung: " & ex.InnerException.Message)
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Ergänzung: " & ex.InnerException.InnerException.Message)
            Catch ex2 As Exception
            End Try
            Return False
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each e In ex.EntityValidationErrors
                For Each v In e.ValidationErrors
                    Debug.WriteLine(v.ErrorMessage & " " & v.PropertyName)
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & v.ErrorMessage & v.PropertyName)

                Next

            Next
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Statt einem Update wird ein Hochgeladener Eichprozess immer gelöscht und neu hinzugefügt
    ''' </summary>
    ''' <param name="DbContext"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="CurrentServerobj"></param>
    ''' <returns></returns>
    Private Shared Function DeleteOldServerEichprozess(DbContext As HerstellerersteichungEntities, Vorgangsnummer As String, CurrentServerobj As ServerEichprozess) As ServerEichprozess
        'Methode welche alle N:1 Verbindungen auf einen Eichprozess entfernt
        clsServerHelper.DeleteForeignTables(CurrentServerobj)

        Try
            DbContext.SaveChanges()

            CurrentServerobj = (From db In DbContext.ServerEichprozess Select db Where db.Vorgangsnummer = Vorgangsnummer).FirstOrDefault
            DbContext.ServerEichprozess.Remove(CurrentServerobj)
            DbContext.SaveChanges()
        Catch ex As Exception

        End Try

        Return CurrentServerobj
    End Function

    ''' <summary>
    ''' anpassen des Bemerkungstextes Timestamp / Benutzer / Kommentar
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="NewServerObj"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <param name="Programmversionsnummer"></param>
    ''' <param name="DbContext"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="CurrentServerobj"></param>
    Private Sub SchreibeKommentarMitVersionsinformation(HEKennung As String, Lizenzschluessel As String, ByRef NewServerObj As ServerEichprozess, WindowsUsername As String, Domainname As String, Computername As String, Programmversionsnummer As String, DbContext As HerstellerersteichungEntities, Vorgangsnummer As String, ByRef CurrentServerobj As ServerEichprozess)
        If CurrentServerobj Is Nothing Then
            'anpassen des Bemerkungstextes Timestamp / Benutzer / Kommentar
            NewServerObj.ServerEichprotokoll.Sicherung_Bemerkungen = "#(" & Date.Now & " " & WindowsUsername & "(" & HEKennung & ") @ v." & Programmversionsnummer & ")# " & vbNewLine & NewServerObj.ServerEichprotokoll.Sicherung_Bemerkungen.ToString
        Else 'update, neuen Text an alten Bemerkungstext anhängen
            'aufräumen und löschen der alten Einträge in der Datenbank
            Dim tmpoldMessage As String = ""
            Try
                tmpoldMessage = CurrentServerobj.ServerEichprotokoll.Sicherung_Bemerkungen.ToString()
            Catch ex As Exception
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            End Try

            Try
                'anhängen des neuen Bemerkungstextes 'anpassen des Bemerkungstextes alte nachricht/  Timestamp / Benutzer / Kommentar
                If Not NewServerObj.ServerEichprotokoll.Sicherung_Bemerkungen.ToString.Equals(tmpoldMessage) Then
                    Dim tmpNewMessage As String = NewServerObj.ServerEichprotokoll.Sicherung_Bemerkungen.ToString 'neue nachricht von alter extrahieren

                    If tmpNewMessage.Contains(tmpoldMessage) Then
                        Try
                            tmpNewMessage = tmpNewMessage.Replace(tmpoldMessage, "").Trim
                        Catch ex As Exception
                        End Try
                    End If

                    NewServerObj.ServerEichprotokoll.Sicherung_Bemerkungen = tmpoldMessage & vbNewLine & vbNewLine & "#(" & Date.Now & " " & WindowsUsername & "(" & HEKennung & ") @ v." & Programmversionsnummer & ")# " & vbNewLine & tmpNewMessage
                End If
            Catch ex As Exception
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' holt einen bestimmten serverseitigen Eichprozess
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEichProzess(ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As ServerEichprozess Implements IEichsoftwareWebservice.GetEichProzess
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing

            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole Konformitätsbewertungsprozess")

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim ObjLizenz = (From lic In DbContext.ServerLizensierung.Include("Benutzer") Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel And lic.Aktiv = True).FirstOrDefault

                    Dim Obj = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerEichprotokoll") _
                               .Include("ServerLookup_Auswertegeraet").Include("ServerKompatiblitaetsnachweis") _
                               .Include("ServerLookup_Waegezelle").Include("ServerLookup_Waagenart") _
                               .Include("ServerLookup_Waagentyp")
                               Where Eichprozess.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                    ''abruch
                    If Obj Is Nothing Then Return Nothing
                    'lade prüfungen zum Eichprozess
                    Obj = GetEichprozessPruefungen(DbContext, Obj, Lizenzschluessel, WindowsUsername, Domainname, Computername)

                    'anpassungen an stammdaten bei Standardwaagen
                    If Obj.Standardwaage = True Then
                        Obj = UeberschreibeStammdatenFuerStandardWaage(DbContext, ObjLizenz, Obj)
                    End If

                    HEKennung = Nothing
                    Lizenzschluessel = Nothing
                    Vorgangsnummer = Nothing

                    Return Obj

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return Nothing
                End Try
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Anpassungen an stammdaten bei Standardwaagen
    ''' </summary>
    ''' <param name="DbContext"></param>
    ''' <param name="ObjLizenz"></param>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    Private Shared Function UeberschreibeStammdatenFuerStandardWaage(DbContext As HerstellerersteichungEntities, ObjLizenz As ServerLizensierung, Obj As ServerEichprozess) As ServerEichprozess
        If Not Obj.ServerEichprotokoll Is Nothing Then
            Dim objBenutzer = (From Benutzer In DbContext.Benutzer Where Benutzer.ID = ObjLizenz.FK_BenutzerID).FirstOrDefault

            Obj.ServerEichprotokoll.Identifikationsdaten_Benutzer = "RHEWA-KUNDE"
            Obj.ServerEichprotokoll.Identifikationsdaten_Aufstellungsort = "Deutschland"
            Obj.ServerEichprotokoll.Identifikationsdaten_Baujahr = Date.Now.Year
            Obj.ServerEichprotokoll.Identifikationsdaten_Pruefer = objBenutzer.Nachname & ", " & objBenutzer.Vorname & " (" + ObjLizenz.HEKennung & ")"

            Obj.ServerEichprotokoll.Komponenten_Softwarestand = "siehe Konfig-Progr."
            Obj.ServerEichprotokoll.Komponenten_Eichzaehlerstand = "siehe Konfig-Progr."
            Obj.ServerEichprotokoll.Komponenten_WaegezellenFabriknummer = "siehe Auftrag"
        End If

        Return Obj
    End Function

    ''' <summary>
    ''' lade Prüfungen zum Eichprozess ins Objekt
    ''' </summary>
    ''' <param name="DbContext"></param>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    Private Function GetEichprozessPruefungen(DbContext As HerstellerersteichungEntities, Obj As ServerEichprozess, Lizenzschluessel As String, WindowsUsername As String, Domainname As String, Computername As String) As ServerEichprozess

        'prüfungen laden
        Dim EichID As String = Obj.ServerEichprotokoll.ID

        Try
            Dim query = From db In DbContext.ServerPruefungAnsprechvermoegen Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungAnsprechvermoegen.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))
        End Try

        Try
            Dim query = From db In DbContext.ServerLookup_Vorgangsstatus Where db.ID = Obj.FK_Vorgangsstatus
            For Each sourceo In query
                Obj.ServerLookup_Vorgangsstatus = sourceo
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungAussermittigeBelastung Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungAussermittigeBelastung.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungLinearitaetFallend Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungLinearitaetFallend.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungLinearitaetSteigend Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungLinearitaetSteigend.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungRollendeLasten Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungRollendeLasten.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungStabilitaetGleichgewichtslage Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungStabilitaetGleichgewichtslage.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungStaffelverfahrenErsatzlast Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungStaffelverfahrenErsatzlast.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungStaffelverfahrenNormallast Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungStaffelverfahrenNormallast.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Try
            Dim query = From db In DbContext.ServerPruefungWiederholbarkeit Where db.FK_Eichprotokoll = EichID
            For Each sourceo In query
                Obj.ServerEichprotokoll.ServerPruefungWiederholbarkeit.Add(sourceo)
            Next
        Catch e As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", e.Message, e.StackTrace))

        End Try

        Return Obj
    End Function

    ''' <summary>
    ''' holt alle Eichprozesse des Nutzers (Lizenzschlüssel) im angebebenen Zeitraum. diese funktion ist z.b. für eine neuinstallation gedacht, damit der client wieder seine Eichungen bekommt
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <param name="SyncAllesSeit"></param>
    ''' <param name="SyncAllesBis"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAlleEichprozesseImZeitraum(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String, Optional ByVal SyncAllesSeit As Date = #1/1/2000#, Optional ByVal SyncAllesBis As Date = #12/31/2999#) As ServerEichprozess() Implements IEichsoftwareWebservice.GetAlleEichprozesseImZeitraum
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole alle Eichprozesse im Zeitraum")

            SyncAllesSeit = New Date(SyncAllesSeit.Year, SyncAllesSeit.Month, SyncAllesSeit.Day, 0, 0, 0)
            SyncAllesBis = New Date(SyncAllesBis.Year, SyncAllesBis.Month, SyncAllesBis.Day, 23, 59, 59)
            'neuen Context aufbauen
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Baue DB Verbindung auf ")

            Using DbContext As New HerstellerersteichungEntities
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "DB Verbindung aufgebaut ")

                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim Eichprozesse = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerEichprotokoll") _
                               .Include("ServerLookup_Auswertegeraet").Include("ServerKompatiblitaetsnachweis") _
                               .Include("ServerLookup_Waegezelle").Include("ServerLookup_Waagenart") _
                               .Include("ServerLookup_Waagentyp")
                                        Where Eichprozess.UploadDatum > SyncAllesSeit And Eichprozess.UploadDatum < SyncAllesBis And Eichprozess.ErzeugerLizenz = Lizenzschluessel)

                    ''abruch
                    If Eichprozesse Is Nothing Then
                        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Keine Konformitätsbewertungsprozess im Zeitraum")
                        Return Nothing
                    End If

                    'liste aufbereiten
                    Dim returnlist As New List(Of ServerEichprozess)
                    For Each Obj In Eichprozesse
                        Try

                            Obj = GetEichprozessPruefungen(DbContext, Obj, Lizenzschluessel, WindowsUsername, Domainname, Computername)
                            returnlist.Add(Obj)

                        Catch ex As Exception
                            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, String.Format("Fehler: {0} Stacktrace: {1}", ex.Message, ex.StackTrace))
                        End Try
                    Next
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Gebe zurück")

                    Return returnlist.ToArray

                Catch ex As Exception
                    'hat nicht funktioniert
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    Return Nothing
                End Try
            End Using

        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Holt alle Eichprozesse als Datatable für RHEWA ansicht aller Eichprozesse
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAlleEichprozesse(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As clsEichprozessFuerAuswahlliste() Implements IEichsoftwareWebservice.GetAlleEichprozesse
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole alle Eichprozesse")

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                Dim lokalerPfadFuerAnhaenge As String = "" 'variable die genutzt wird um den Client den Pfad an dem die Anhänge zu finden sind mitzuteilen
                lokalerPfadFuerAnhaenge = GetLokalerPfadAnhaenge(Lizenzschluessel, WindowsUsername, Domainname, Computername, DbContext)

                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try

                    'clsEichprozessFuerAuswahlliste als Wrapper für die KLasse. Problematischer Weise kann man keine anoynmen Typen zurückgeben. Deswegen gibt es die behilfsklasse clsEichprozessFuerAuswahlliste.
                    'Diese hat exakt die Eigenschaften die benötigt werden und zusammengesetzt aus der Status Tabelle und dem Eichprozess zusammengebaut wird
                    Dim ReturnList = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerLookup_Waegezelle")
                                      Join Lookup2 In DbContext.ServerLookup_Bearbeitungsstatus On Eichprozess.FK_Bearbeitungsstatus Equals Lookup2.ID
                                      Select New clsEichprozessFuerAuswahlliste With
                                {
                                .ID = Eichprozess.ID,
                                .Vorgangsnummer = Eichprozess.Vorgangsnummer,
                                .Fabriknummer = Eichprozess.ServerKompatiblitaetsnachweis.Kompatiblitaet_Waage_FabrikNummer,
                                .Pruefscheinnummer = Eichprozess.ServerEichprotokoll.Beschaffenheitspruefung_Pruefscheinnummer,
                                .WZ = Eichprozess.ServerLookup_Waegezelle.Typ,
                                .Waagentyp = Eichprozess.ServerLookup_Waagentyp.Typ,
                                .Waagenart = Eichprozess.ServerLookup_Waagenart.Art,
                                .AWG = Eichprozess.ServerLookup_Auswertegeraet.Typ,
                                .Eichbevollmaechtigter = Eichprozess.ServerEichprotokoll.Identifikationsdaten_Pruefer,
                                .GesperrtDurch = Eichprozess.ZurBearbeitungGesperrtDurch,
                                .AnhangPfad = Eichprozess.UploadFilePath,
                                .Bearbeitungsstatus = Lookup2.Status,
                                .Uploaddatum = Eichprozess.UploadDatum,
                                .Bemerkung = Eichprozess.ServerEichprotokoll.Sicherung_Bemerkungen,
                                .NeueWZ = Eichprozess.ServerLookup_Waegezelle.Neu,
                                .HKBDatum = Eichprozess.ServerEichprotokoll.Identifikationsdaten_Datum
                                }
                                     ).ToList


                    'Wrapper für die KLasse. Problematischer Weise kann man keine anoynmen Typen zurückgeben. Deswegen gibt es die behilfsklasse clsEichprozessFuerAuswahlliste.
                    'Diese hat exakt die Eigenschaften die benötigt werden und zusammengesetzt aus der Status Tabelle und dem Eichprozess zusammengebaut wird

                    BuildAnhangPfad(lokalerPfadFuerAnhaenge, ReturnList)


                    'ergebnismenge zurückgeben
                    If Not ReturnList.Count = 0 Then
                        Return ReturnList.ToArray
                    Else
                        'es gibt keine neuerungen
                        Return Nothing
                    End If

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return Nothing
                End Try
            End Using
        Catch ex As Exception

            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Holt alle Eichprozesse als Datatable für RHEWA ansicht aller Eichprozesse, gefiltert nach Uploadmonat
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAlleEichprozesseNachUploadMonat(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String, ByVal UploadjahrVon As Integer, ByVal UploadmonatVon As Integer, ByVal UploadjahrBis As Integer, ByVal UploadmonatBis As Integer) As clsEichprozessFuerAuswahlliste() Implements IEichsoftwareWebservice.GetAlleEichprozesseNachUploadMonat
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole alle Eichprozesse")

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                Dim lokalerPfadFuerAnhaenge As String = "" 'variable die genutzt wird um den Client den Pfad an dem die Anhänge zu finden sind mitzuteilen
                lokalerPfadFuerAnhaenge = GetLokalerPfadAnhaenge(Lizenzschluessel, WindowsUsername, Domainname, Computername, DbContext)


                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False

                Dim dateVon As New Date(UploadjahrVon, UploadmonatVon, 1)
                Dim dateBis As Date
                Try
                    dateBis = New Date(UploadjahrBis, UploadmonatBis, 31)

                Catch ex As ArgumentOutOfRangeException
                    Try
                        dateBis = New Date(UploadjahrBis, UploadmonatBis, 30)
                    Catch exw As Exception
                        dateBis = New Date(UploadjahrBis, UploadmonatBis, 27)
                    End Try
                End Try

                'aufgrund einer Limitierung im Entity Framework 5 in Verbindung mit LINQ und den Vergleich zweier Datumswerte, wird hier leider nur auf das MinDate geprüft und im Anschluss nach dem Query auf das Max Date.
                'syntaktisch funktioniert der Vergleich Mindate >= Value AND Max Date <= Value, aber es kommen falsche ergebnisse zurück
                Try
                    Dim Query = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerLookup_Waegezelle")
                                 Join Lookup In DbContext.ServerLookup_Vorgangsstatus On Eichprozess.FK_Vorgangsstatus Equals Lookup.ID
                                 Join Lookup2 In DbContext.ServerLookup_Bearbeitungsstatus On Eichprozess.FK_Bearbeitungsstatus Equals Lookup2.ID
                                 Where Eichprozess.UploadDatum.Value >= dateVon
                                 Select New clsEichprozessFuerAuswahlliste With
                                {
                                  .ID = Eichprozess.ID,
                                  .Vorgangsnummer = Eichprozess.Vorgangsnummer,
                                  .Fabriknummer = Eichprozess.ServerKompatiblitaetsnachweis.Kompatiblitaet_Waage_FabrikNummer,
                                  .Pruefscheinnummer = Eichprozess.ServerEichprotokoll.Beschaffenheitspruefung_Pruefscheinnummer,
                                  .WZ = Eichprozess.ServerLookup_Waegezelle.Typ,
                                  .Waagentyp = Eichprozess.ServerLookup_Waagentyp.Typ,
                                  .Waagenart = Eichprozess.ServerLookup_Waagenart.Art,
                                  .AWG = Eichprozess.ServerLookup_Auswertegeraet.Typ,
                                  .Eichbevollmaechtigter = Eichprozess.ServerEichprotokoll.Identifikationsdaten_Pruefer,
                                  .GesperrtDurch = Eichprozess.ZurBearbeitungGesperrtDurch,
                                  .AnhangPfad = Eichprozess.UploadFilePath,
                                  .Bearbeitungsstatus = Lookup2.Status,
                                  .Uploaddatum = Eichprozess.UploadDatum,
                                  .Bemerkung = Eichprozess.ServerEichprotokoll.Sicherung_Bemerkungen,
                                  .NeueWZ = Eichprozess.ServerLookup_Waegezelle.Neu,
                                .HKBDatum = Eichprozess.ServerEichprotokoll.Identifikationsdaten_Datum
                                }
                                     ).ToList


                    'Wrapper für die KLasse. Problematischer Weise kann man keine anoynmen Typen zurückgeben. Deswegen gibt es die behilfsklasse clsEichprozessFuerAuswahlliste.
                    'Diese hat exakt die Eigenschaften die benötigt werden und zusammengesetzt aus der Status Tabelle und dem Eichprozess zusammengebaut wird
                    Dim ReturnList As New List(Of clsEichprozessFuerAuswahlliste)


                    For Each objeichprozess In Query
                        'max date prüfen, weil es oben nicht klappt
                        Try
                            If objeichprozess.Uploaddatum >= dateBis Then
                                Continue For 'abbruch
                            Else
                                ReturnList.Add(objeichprozess)
                            End If
                        Catch ex As Exception
                            Continue For
                        End Try
                    Next

                    BuildAnhangPfad(lokalerPfadFuerAnhaenge, ReturnList)

                    'ergebnismenge zurückgeben
                    If Not ReturnList.Count = 0 Then
                        Return ReturnList.ToArray
                    Else
                        'es gibt keine neuerungen
                        Return Nothing
                    End If

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return Nothing
                End Try
            End Using
        Catch ex As Exception

            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Lädt eichprozesse die als Standardwaagen definiert wurden, für das das schnelle Duplizieren seitens RHEWA
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    Public Function GetStandardwaagen(ByVal HEKennung As String, Lizenzschluessel As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As clsEichprozessFuerAuswahlliste() Implements IEichsoftwareWebservice.GetStandardwaagen
        Try
            Dim boldebug As Boolean = False ' debug logs schreiben
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities

                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim ReturnList = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerLookup_Waegezelle") Where Eichprozess.Standardwaage = True
                                      Join Lookup In DbContext.ServerLookup_Vorgangsstatus On Eichprozess.FK_Vorgangsstatus Equals Lookup.ID
                                      Join Lookup2 In DbContext.ServerLookup_Bearbeitungsstatus On Eichprozess.FK_Bearbeitungsstatus Equals Lookup2.ID
                                      Select New clsEichprozessFuerAuswahlliste With
                                            {
                                            .ID = Eichprozess.ID,
                                            .Vorgangsnummer = Eichprozess.Vorgangsnummer,
                                            .Fabriknummer = Eichprozess.ServerKompatiblitaetsnachweis.Kompatiblitaet_Waage_FabrikNummer,
                                            .Pruefscheinnummer = Eichprozess.ServerEichprotokoll.Beschaffenheitspruefung_Pruefscheinnummer,
                                            .WZ = Eichprozess.ServerLookup_Waegezelle.Typ,
                                            .Waagentyp = Eichprozess.ServerLookup_Waagentyp.Typ,
                                            .Waagenart = Eichprozess.ServerLookup_Waagenart.Art,
                                            .AWG = Eichprozess.ServerLookup_Auswertegeraet.Typ,
                                            .Eichbevollmaechtigter = Eichprozess.ServerEichprotokoll.Identifikationsdaten_Pruefer,
                                            .GesperrtDurch = Eichprozess.ZurBearbeitungGesperrtDurch,
                                            .AnhangPfad = Eichprozess.UploadFilePath,
                                            .Bearbeitungsstatus = Lookup2.Status,
                                            .Uploaddatum = Eichprozess.UploadDatum,
                                            .Bemerkung = Eichprozess.ServerEichprotokoll.Sicherung_Bemerkungen,
                                            .NeueWZ = Eichprozess.ServerLookup_Waegezelle.Neu,
                                            .HKBDatum = Eichprozess.ServerEichprotokoll.Identifikationsdaten_Datum
                                            }
                                     ).ToList



                    'ergebnismenge zurückgeben
                    If Not ReturnList.Count = 0 Then
                        Return ReturnList.ToArray
                    Else
                        'es gibt keine neuerungen
                        Return Nothing
                    End If

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return Nothing
                End Try
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return Nothing
        End Try
    End Function
#End Region

#Region "Anhänge"
    Private Sub BuildAnhangPfad(lokalerPfadFuerAnhaenge As String, ReturnList As List(Of clsEichprozessFuerAuswahlliste))
        For Each objeichprozess In ReturnList 'TODO  Pfad über .io.path kontrollieren
            'dateipfad zusammenbauen
            If Not objeichprozess.AnhangPfad Is Nothing Then
                If Not objeichprozess.AnhangPfad.Trim.Equals("") Then

                    objeichprozess.AnhangPfad = objeichprozess.AnhangPfad.Split("\").LastOrDefault 'ungültige Zeichen entfernen
                    If objeichprozess.AnhangPfad.Equals("") Then objeichprozess.AnhangPfad = "tmp.zip" 'wenn nach entfernen der Zeichen leer, dann tmpnamen
                    If lokalerPfadFuerAnhaenge.EndsWith("\") Then
                        objeichprozess.AnhangPfad = lokalerPfadFuerAnhaenge & objeichprozess.AnhangPfad
                    Else
                        objeichprozess.AnhangPfad = lokalerPfadFuerAnhaenge & "\" & objeichprozess.AnhangPfad
                    End If

                End If
            End If
        Next
    End Sub

    ''' <summary>
    ''' Netzwerkpfad für Attachments aus DB auslesen
    ''' </summary>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <param name="DbContext"></param>
    ''' <returns></returns>
    Private Function GetLokalerPfadAnhaenge(Lizenzschluessel As String, WindowsUsername As String, Domainname As String, Computername As String, DbContext As HerstellerersteichungEntities) As String
        Dim lokalerPfadFuerAnhaenge As String

        Try
            lokalerPfadFuerAnhaenge = (From Config In DbContext.ServerKonfiguration Select Config.NetzwerkpfadFuerDateianhaenge).FirstOrDefault
            If lokalerPfadFuerAnhaenge Is Nothing Then
                lokalerPfadFuerAnhaenge = ""
            End If
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            lokalerPfadFuerAnhaenge = ""
        End Try

        Return lokalerPfadFuerAnhaenge
    End Function
#End Region




    ''' <summary>
    ''' Holt alle WZ im Zeitraum
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="LetztesUpdate"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <param name="SyncAllesSeit"></param>
    ''' <param name="SyncAllesBis"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNeueWZ(ByVal HEKennung As String, Lizenzschluessel As String, ByVal LetztesUpdate As Date, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String, Optional ByVal SyncAllesSeit As Date = #1/1/2000#, Optional ByVal SyncAllesBis As Date = #12/31/2999#) As ServerLookup_Waegezelle() Implements IEichsoftwareWebservice.GetNeueWZ
        ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
        If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole WZ")

        Using DBContext As New HerstellerersteichungEntities
            DBContext.Configuration.LazyLoadingEnabled = False
            DBContext.Configuration.ProxyCreationEnabled = False
            Try
                'fallunterscheidung für die zu holende Datenmenge:
                'fall A= Start und Enddate sind auf Default wert => Hole alles seit letztem Update
                'fall B = StartWert ist ungleich default werd, aber Enddate ist default => Holle alles ab dem Startdate und dann seit dem letzten Update
                'fall c = StartWert ist ungleich default werd, und Enddate ist ungleich default => Holle alles ab dem Startdate und Endwert, dann seit dem letzten Update wenn dieses nicht über dem Endwert liegt
                Dim query As System.Linq.IOrderedQueryable(Of ServerLookup_Waegezelle)

                If SyncAllesSeit = #1/1/2000# And SyncAllesBis = #12/31/2999# Then 'fall A
                    query = From d In DBContext.ServerLookup_Waegezelle Where d.ErstellDatum >= LetztesUpdate Order By d.ID
                ElseIf SyncAllesSeit <> #1/1/2000# And SyncAllesBis = #12/31/2999# Then 'fall b
                    query = From d In DBContext.ServerLookup_Waegezelle
                            Where d.ErstellDatum >= SyncAllesSeit And d.ErstellDatum >= LetztesUpdate
                            Order By d.ID
                ElseIf SyncAllesSeit <> #1/1/2000# And SyncAllesBis <> #12/31/2999# Then 'fall c
                    '#ndern des Letztes Update wertes
                    If LetztesUpdate > SyncAllesBis Then
                        LetztesUpdate = SyncAllesBis
                    End If
                    If LetztesUpdate < SyncAllesSeit Then
                        LetztesUpdate = SyncAllesSeit
                    End If
                    query = From d In DBContext.ServerLookup_Waegezelle
                            Where d.ErstellDatum >= SyncAllesSeit And d.ErstellDatum <= SyncAllesBis And d.ErstellDatum >= LetztesUpdate
                            Order By d.ID
                Else 'fall das Startwert gleich default aber endwert nicht, ist nicht vorgesehen. Fallback auf Sync Alles seit
                    query = From d In DBContext.ServerLookup_Waegezelle Where d.ErstellDatum >= LetztesUpdate Order By d.ID
                End If

                Dim ReturnList As New List(Of ServerLookup_Waegezelle)

                For Each objWZ In query
                    If objWZ.Deaktiviert = True Then Continue For
                    '   Dim ModelArtikel As New Model.clsArtikel(objArtikel.Id, objArtikel.HEKennung, objArtikel.Beschreibung, objArtikel.Preis, objArtikel.ErstellDatum)
                    ReturnList.Add(objWZ)
                Next

                'ergebnismenge zurückgeben
                If Not ReturnList.Count = 0 Then
                    Return ReturnList.ToArray
                Else
                    'es gibt keine neuerungen
                    Return Nothing
                End If

            Catch ex As Exception
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                'hat nicht funktioniert
                Return Nothing
            End Try
        End Using

    End Function

    ''' <summary>
    ''' Holt alle AWGs im Zeitraum
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="LetztesUpdate"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <param name="SyncAllesSeit"></param>
    ''' <param name="SyncAllesBis"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNeuesAWG(ByVal HEKennung As String, Lizenzschluessel As String, ByVal LetztesUpdate As Date, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String, Optional ByVal SyncAllesSeit As Date = #1/1/2000#, Optional ByVal SyncAllesBis As Date = #12/31/2999#) As ServerLookup_Auswertegeraet() Implements IEichsoftwareWebservice.GetNeuesAWG
        ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
        If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole AWG")

        Using DBContext As New HerstellerersteichungEntities
            DBContext.Configuration.LazyLoadingEnabled = False
            DBContext.Configuration.ProxyCreationEnabled = False
            Try
                'fallunterscheidung für die zu holende Datenmenge:
                'fall A= Start und Enddate sind auf Default wert => Hole alles seit letztem Update
                'fall B = StartWert ist ungleich default werd, aber Enddate ist default => Holle alles ab dem Startdate und dann seit dem letzten Update
                'fall c = StartWert ist ungleich default werd, und Enddate ist ungleich default => Holle alles ab dem Startdate und Endwert, dann seit dem letzten Update wenn dieses nicht über dem Endwert liegt
                Dim query As System.Linq.IOrderedQueryable(Of ServerLookup_Auswertegeraet)

                If SyncAllesSeit = #1/1/2000# And SyncAllesBis = #12/31/2999# Then 'fall A
                    query = From d In DBContext.ServerLookup_Auswertegeraet Where d.ErstellDatum >= LetztesUpdate Order By d.ID
                ElseIf SyncAllesSeit <> #1/1/2000# And SyncAllesBis = #12/31/2999# Then 'fall b
                    query = From d In DBContext.ServerLookup_Auswertegeraet
                            Where d.ErstellDatum >= SyncAllesSeit And d.ErstellDatum >= LetztesUpdate
                            Order By d.ID
                ElseIf SyncAllesSeit <> #1/1/2000# And SyncAllesBis <> #12/31/2999# Then 'fall c
                    '#ndern des Letztes Update wertes
                    If LetztesUpdate > SyncAllesBis Then
                        LetztesUpdate = SyncAllesBis
                    End If
                    If LetztesUpdate < SyncAllesSeit Then
                        LetztesUpdate = SyncAllesSeit
                    End If
                    query = From d In DBContext.ServerLookup_Auswertegeraet
                            Where d.ErstellDatum >= SyncAllesSeit And d.ErstellDatum <= SyncAllesBis And d.ErstellDatum >= LetztesUpdate
                            Order By d.ID
                Else 'fall das Startwert gleich default aber endwert nicht, ist nicht vorgesehen. Fallback auf Sync Alles seit
                    query = From d In DBContext.ServerLookup_Auswertegeraet Where d.ErstellDatum >= LetztesUpdate Order By d.ID
                End If

                Dim ReturnList As New List(Of ServerLookup_Auswertegeraet)

                For Each objWZ In query
                    '   Dim ModelArtikel As New Model.clsArtikel(objArtikel.Id, objArtikel.HEKennung, objArtikel.Beschreibung, objArtikel.Preis, objArtikel.ErstellDatum)
                    ReturnList.Add(objWZ)
                Next

                'ergebnismenge zurückgeben
                If Not ReturnList.Count = 0 Then
                    Return ReturnList.ToArray
                Else
                    'es gibt keine neuerungen+
                    Return Nothing
                End If

            Catch ex As Exception
                SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                'hat nicht funktioniert
                Return Nothing
            End Try
        End Using

    End Function

    ''' <summary>
    ''' Funktion für die Statistikfunktion der Eichmarkenverwaltung. Dort können Marken eingetragen werden, die ausgeteilt wurden. Wenn ein Benutzer dann Marken im Eichprozess verwendet, werden sie dort abgezogen um einen Ist-Bestand zu erhalten
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="AnzahlBenannteStelle"></param>
    ''' <param name="AnzahlSicherungsmarkeKlein"></param>
    ''' <param name="AnzahlSicherungsmarkeGross"></param>
    ''' <param name="AnzahlHinweismarke"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddEichmarkenverwaltung(ByVal HEKennung As String, Lizenzschluessel As String, ByVal BenutzerIDFK As String, ByVal AnzahlBenannteStelle As Integer, ByVal AnzahlSicherungsmarkeKlein As Integer, ByVal AnzahlSicherungsmarkeGross As Integer, ByVal AnzahlHinweismarke As Integer, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As Boolean Implements IEichsoftwareWebservice.AddEichmarkenverwaltung
        ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
        If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Aktualisiere Eichmarkenverwaltung")
        Try
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False

                Dim Element = (From d In DbContext.ServerEichmarkenverwaltung Where d.FK_BenutzerID = BenutzerIDFK Order By d.ID).FirstOrDefault

                If Element Is Nothing Then
                    Element = New ServerEichmarkenverwaltung
                    Element.Bemerkung = ""
                    Element.FK_BenutzerID = BenutzerIDFK
                    Element.HEKennung = HEKennung

                    Element.BenannteStelleAnzahl = AnzahlBenannteStelle
                    Element.HinweismarkeAnzahl = AnzahlHinweismarke
                    Element.SicherungsmarkeGrossAnzahl = AnzahlSicherungsmarkeGross
                    Element.SicherungsmarkeKleinAnzahl = AnzahlSicherungsmarkeKlein

                    DbContext.ServerEichmarkenverwaltung.Add(Element)

                Else

                    Try
                        If Element.BenannteStelleAnzahl Is Nothing Then
                            Element.BenannteStelleAnzahl = AnzahlBenannteStelle
                        Else
                            Element.BenannteStelleAnzahl += AnzahlBenannteStelle
                        End If
                    Catch e As Exception
                    End Try
                    Try
                        If Element.SicherungsmarkeKleinAnzahl Is Nothing Then
                            Element.SicherungsmarkeKleinAnzahl = AnzahlSicherungsmarkeKlein

                        Else
                            Element.SicherungsmarkeKleinAnzahl += AnzahlSicherungsmarkeKlein

                        End If
                    Catch e As Exception
                    End Try
                    Try
                        If Element.SicherungsmarkeGrossAnzahl Is Nothing Then
                            Element.SicherungsmarkeGrossAnzahl = AnzahlSicherungsmarkeGross

                        Else
                            Element.SicherungsmarkeGrossAnzahl += AnzahlSicherungsmarkeGross

                        End If
                    Catch e As Exception
                    End Try
                    Try
                        If Element.HinweismarkeAnzahl Is Nothing Then
                            Element.HinweismarkeAnzahl = AnzahlHinweismarke
                        Else
                            Element.HinweismarkeAnzahl += AnzahlHinweismarke
                        End If
                    Catch e As Exception
                    End Try

                End If


                Try
                    DbContext.SaveChanges()
                Catch e As Exception
                End Try

                Element = Nothing

                Return True
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return False
        End Try

    End Function

#Region "Eichprozess genehmigungsprozess"

    ''' <summary>
    '''  Der Client ruft diese Methode auf, mit allen seinen lokalen Prüfungen die im Status auf "wird geprüft stehen" auf
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Public Function GetGueltigkeitEichprozess(ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As String Implements IEichsoftwareWebservice.CheckGueltigkeitEichprozess
        'da die ID im Server von der im Client abweichen kann wird hier mit der Vorgangsnummer gearbeitet die pro Prozess Eindeutig generiert wrid
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Prüfe gültigkeit des Eichprozesses")

            'neuen Context aufbauen
            'prüfen ob der eichprozess schoneinmal eingegangen ist anhand von Vorgangsnummer
            Using DbContext As New HerstellerersteichungEntities
                Dim Serverob = (From db In DbContext.ServerEichprozess Select db Where db.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                If Not Serverob Is Nothing Then
                    HEKennung = Nothing
                    Lizenzschluessel = Nothing
                    Vorgangsnummer = Nothing
                    Return Serverob.FK_Bearbeitungsstatus
                Else
                    'Vorgang konnte auf dem Server nicht gefunden werden. Muss erneut gesendet werden
                    Return 4
                End If

            End Using

            HEKennung = Nothing
            Lizenzschluessel = Nothing
            Vorgangsnummer = Nothing
            Return Nothing
        Catch ex As Entity.Infrastructure.DbUpdateException
            Debug.WriteLine(ex.InnerException.InnerException.Message)
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return Nothing
        Catch ex As Entity.Validation.DbEntityValidationException
            For Each e In ex.EntityValidationErrors
                For Each v In e.ValidationErrors
                    Debug.WriteLine(v.ErrorMessage & " " & v.PropertyName)

                Next

            Next
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Genehmigt übermittelten Eichprozess, so das Client ihn herunterladen kann
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetEichprozessUngueltig(ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As Boolean Implements IEichsoftwareWebservice.SetEichprozessUngueltig
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Setze Konformitätsbewertungsprozess auf ungültig")

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim Obj = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerEichprotokoll")
                               Where Eichprozess.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                    ''abruch
                    If Obj Is Nothing Then Return Nothing

                    'Fehlerhafter Status aus Lookup_Bearbeitungsstatus = 2
                    Obj.FK_Bearbeitungsstatus = 2
                    Obj.FK_Vorgangsstatus = 2
                    DbContext.SaveChanges()

                    HEKennung = Nothing
                    Lizenzschluessel = Nothing
                    Vorgangsnummer = Nothing

                    Return True

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return False
                End Try
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return False
        End Try
    End Function

    ''' <summary>
    ''' Lehnt Eichprozess ab. Client muss erst korrigieren und neu einsenden
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetEichprozessgenehmigt(ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As Boolean Implements IEichsoftwareWebservice.SetEichprozessGenehmight
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Setze Konformitätsbewertungsprozess auf gültig")

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim Obj = (From Eichprozess In DbContext.ServerEichprozess.Include("ServerEichprotokoll")
                               Where Eichprozess.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                    ''abruch
                    If Obj Is Nothing Then Return Nothing

                    'Genehmighter Status aus Lookup_Bearbeitungsstatus = 3
                    Obj.FK_Bearbeitungsstatus = 3
                    Obj.FK_Vorgangsstatus = 2
                    DbContext.SaveChanges()

                    HEKennung = Nothing
                    Lizenzschluessel = Nothing
                    Vorgangsnummer = Nothing

                    Return True

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return False
                End Try
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return False
        End Try
    End Function
#End Region
#Region "Sperrung des Vorgangs damit kein konkurierender Zugriff stattfindet"

    ''' <summary>
    ''' Prüft ob die aktuelle Eichung von jemand anderem Bearbeitet wird
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns>WindowsUsername des Sperrers wenn gesperrt</returns>
    ''' <remarks></remarks>
    Function GetSperrung(ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As String Implements IEichsoftwareWebservice.CheckSperrung
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Prüfe Sperrung")

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim Obj = (From Eichprozess In DbContext.ServerEichprozess
                               Where Eichprozess.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                    ''abruch
                    If Obj Is Nothing Then Return "Objekt nicht gefunden"

                    'Keine sperrung vorliegend
                    Try
                        If Obj.ZurBearbeitungGesperrtDurch Is Nothing Then
                            Return ""
                        End If
                    Catch ex As Exception

                    End Try

                    Try
                        If IsDBNull(Obj.ZurBearbeitungGesperrtDurch) Then
                            Return ""
                        End If
                    Catch ex As Exception

                    End Try
                    Try
                        If Obj.ZurBearbeitungGesperrtDurch.Equals("") Then
                            Return ""
                        End If
                    Catch ex As Exception

                    End Try

                    'prüfen ob der Sperrer nicht einem selbst entspricht
                    If Obj.ZurBearbeitungGesperrtDurch = WindowsUsername Then
                        Return ""
                    Else
                        Return Obj.ZurBearbeitungGesperrtDurch
                    End If

                    'ansonsten
                    Return ""

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return "Fehler beim Prüfen"
                End Try
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

            Return False
        End Try
    End Function

    ''' <summary>
    ''' Sperrt oder entsperrt aktuelle Eichung, so das ein anderer Mitarbeiter der sie öffnet einen Warnhinweis bekommt
    ''' </summary>
    ''' <param name="bolSperren"></param>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function SetSperrung(ByVal bolSperren As Boolean, ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As String Implements IEichsoftwareWebservice.SetSperrung
        Try
            ''abruch falls irgend jemand den Service ohne gültige Lizenz aufruft
            If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Setze Sperrung auf: " & bolSperren)

            'neuen Context aufbauen
            Using DbContext As New HerstellerersteichungEntities
                DbContext.Configuration.LazyLoadingEnabled = False
                DbContext.Configuration.ProxyCreationEnabled = False
                Try
                    Dim Obj = (From Eichprozess In DbContext.ServerEichprozess
                               Where Eichprozess.Vorgangsnummer = Vorgangsnummer).FirstOrDefault

                    ''abruch
                    If Obj Is Nothing Then Return "Objekt nicht gefunden"

                    'entsperren wen bolsperren = false
                    If bolSperren Then
                        'Keine sperrung vorliegend, wir setzten sie
                        Try
                            If Obj.ZurBearbeitungGesperrtDurch Is Nothing Then
                                Obj.ZurBearbeitungGesperrtDurch = WindowsUsername
                                DbContext.SaveChanges()
                                Return ""
                            End If
                        Catch ex As Exception

                        End Try
                        Try
                            If IsDBNull(Obj.ZurBearbeitungGesperrtDurch) Then
                                Obj.ZurBearbeitungGesperrtDurch = WindowsUsername
                                DbContext.SaveChanges()
                                Return ""
                            End If
                        Catch ex As Exception

                        End Try
                        Try
                            If (Obj.ZurBearbeitungGesperrtDurch).Equals("") Then
                                Obj.ZurBearbeitungGesperrtDurch = WindowsUsername
                                DbContext.SaveChanges()
                                Return ""
                            End If
                        Catch ex As Exception

                        End Try

                        'prüfen ob man selbst gesperrt hat
                        If Obj.ZurBearbeitungGesperrtDurch = WindowsUsername Then
                            Return ""
                        End If

                        'ansonsten
                        Return "Benutzer ist nicht der Sperrer"

                    Else
                        Obj.ZurBearbeitungGesperrtDurch = ""
                        DbContext.SaveChanges()
                        Return ""
                    End If

                Catch ex As Exception
                    SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)

                    'hat nicht funktioniert
                    Return ex.Message
                End Try
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return ex.Message
        End Try
    End Function

#End Region

#Region "FTP"
    ''' <summary>
    ''' funktion welche die konfigurationsdatenbank von RHEWA ausliest und die benötigten Werte für eine FTP verschlüsselung zurückgibt
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetFTPCredentials(ByVal HEKennung As String, Lizenzschluessel As String, ByVal Vorgangsnummer As String, ByVal WindowsUsername As String, ByVal Domainname As String, ByVal Computername As String) As clsServerFTPDaten Implements IEichsoftwareWebservice.GetFTPCredentials
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Hole FTP Zugangsdaten")
        Try
            Using dbcontext As New HerstellerersteichungEntities
                Dim ObjLizenz = (From lic In dbcontext.ServerLizensierung Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel And lic.Aktiv = True).FirstOrDefault
                Dim interal As Boolean = False
                'debug
                If HEKennung = "internal" And Lizenzschluessel = "999999" And Vorgangsnummer = "999999" Then
                    interal = True
                End If

                If Not ObjLizenz Is Nothing Or interal = True Then

                    Dim objServerKonfiguration = (From FTPDaten In dbcontext.ServerKonfiguration).FirstOrDefault
                    If Not objServerKonfiguration Is Nothing Then

                        Dim objServerFTPDaten As New clsServerFTPDaten
                        objServerFTPDaten.FTPServername = objServerKonfiguration.FTPServerAdresse
                        objServerFTPDaten.FTPUserName = objServerKonfiguration.FTPServerBenutzername
                        objServerFTPDaten.FTPEncryptedPassword = objServerKonfiguration.FTPServerEnkodiertesPasswort
                        objServerFTPDaten.FTPSaltKey = objServerKonfiguration.FTPServerCryptoSaltKey

                        'versuchen ob es zur Vorgangsnummer ein Konformitätsbewertungsvorgang gibt. Wenn ja gucke ob es einen Uploadpfad gibt
                        Try
                            Dim FilePath As String = (From eichprozess In dbcontext.ServerEichprozess Where eichprozess.Vorgangsnummer = Vorgangsnummer Select eichprozess.UploadFilePath).FirstOrDefault

                            If Not FilePath Is Nothing Then
                                If Not FilePath.Trim.Equals("") Then
                                    If objServerKonfiguration.NetzwerkpfadFuerDateianhaenge.EndsWith("\") Then
                                        objServerFTPDaten.FTPFilePath = objServerKonfiguration.NetzwerkpfadFuerDateianhaenge & FilePath.ToString

                                    Else
                                        objServerFTPDaten.FTPFilePath = objServerKonfiguration.NetzwerkpfadFuerDateianhaenge & "\" & FilePath.ToString

                                    End If
                                End If
                            End If

                        Catch ex As Exception
                        End Try

                        Return objServerFTPDaten
                    End If
                End If
            End Using
            Return Nothing
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return Nothing
        End Try
    End Function
#End Region

    ''' <summary>
    ''' In der Liste werden momentan per Hand Prüfscheinnummern gepflegt. Diese können entweder ablaufen oder gesperrt sein. Gibt diese Liste zurück
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    Public Function GetGesperrtePrüfscheinnummern(HEKennung As String, Lizenzschluessel As String, Vorgangsnummer As String, WindowsUsername As String, Domainname As String, Computername As String) As List(Of StatusPrüfscheinnummer) Implements IEichsoftwareWebservice.GetGesperrtePrüfscheinnummern

        Using dbcontext As New HerstellerersteichungEntities
            Dim ObjLizenz = (From lic In dbcontext.ServerLizensierung Where lic.HEKennung = HEKennung And lic.Lizenzschluessel = Lizenzschluessel And lic.Aktiv = True).FirstOrDefault
            If Not ObjLizenz Is Nothing Then

                Dim Pruefscheinnummern = (From o In dbcontext.StatusPrüfscheinnummer).ToList
                Return Pruefscheinnummern

            End If
        End Using

        Return New List(Of StatusPrüfscheinnummer)
    End Function

#Region "Ablage des unfertigen Eichprozesses im JSON Format"

    ''' <summary>
    ''' Parsed einen unfertigen Eichprozess in JSON und legt diesen ab. Funktion wird genutzt um z.b. von einem PC einen Vorgang anzulegen und diesen an einem anderem PC wieder abzurufen
    ''' </summary>
    ''' <param name="JSONEichprozess"></param>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="Vorgangsnummer"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    Public Function SetAblageEichprozess(JSONEichprozess As String, HEKennung As String, Lizenzschluessel As String, Vorgangsnummer As String, WindowsUsername As String, Domainname As String, Computername As String) As Boolean Implements IEichsoftwareWebservice.SetAblageEichprozess
        If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Lege Eichprozess ab")
        Try
            Using dbcontext As New HerstellerersteichungEntities
                Dim Ablage As New ServerAblageEichprozesse()
                Ablage.HEKennung = HEKennung
                Ablage.Lizenz = Lizenzschluessel
                Ablage.Vorgangsnummer = Vorgangsnummer
                Ablage.JSON = JSONEichprozess

                dbcontext.ServerAblageEichprozesse.Add(Ablage)
                dbcontext.SaveChanges()
                Return True

            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return False
        End Try
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Eichprozess erfolgreich abgelegt")
    End Function

    ''' <summary>
    ''' Lädt als JSON abgelegte Eichprozesse aus der DB
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    Public Function getAblageEichprozesse(HEKennung As String, Lizenzschluessel As String, WindowsUsername As String, Domainname As String, Computername As String) As List(Of String) Implements IEichsoftwareWebservice.getAblageEichprozesse
        If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Rufe abgelegte Eichprozesse ab von Lizenz " + Lizenzschluessel)
        Try
            Using dbcontext As New HerstellerersteichungEntities
                Dim ablagen = From Ablage In dbcontext.ServerAblageEichprozesse Where Ablage.HEKennung = HEKennung And Ablage.Lizenz = Lizenzschluessel
                Dim returnlist As New List(Of String)

                For Each ablage In ablagen
                    returnlist.Add(ablage.JSON)
                Next

                Return returnlist
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return Nothing
        End Try
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Eichprozesse erfolgreich aus Ablage geladen")

    End Function

    ''' <summary>
    ''' Löscht einen als JSON abgelegten Eichprozess. Wenn ein Eichprozess erfolgreich abgerufen wurde, muss er gelöscht werden
    ''' </summary>
    ''' <param name="HEKennung"></param>
    ''' <param name="Lizenzschluessel"></param>
    ''' <param name="WindowsUsername"></param>
    ''' <param name="Domainname"></param>
    ''' <param name="Computername"></param>
    ''' <returns></returns>
    Public Function deleteAblageEichprozesse(HEKennung As String, Lizenzschluessel As String, WindowsUsername As String, Domainname As String, Computername As String) As Boolean Implements IEichsoftwareWebservice.deleteAblageEichprozesse
        If GetLizenz(HEKennung, Lizenzschluessel, WindowsUsername, Domainname, Computername) = False Then Return Nothing
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Lösche abgelegte Eichprozesse von Lizenz " + Lizenzschluessel)
        Try
            Using dbcontext As New HerstellerersteichungEntities
                Dim ablagen = From Ablage In dbcontext.ServerAblageEichprozesse Where Ablage.HEKennung = HEKennung And Ablage.Lizenz = Lizenzschluessel

                For Each ablage In ablagen
                    dbcontext.ServerAblageEichprozesse.Remove(ablage)
                Next
                dbcontext.SaveChanges()

                Return True
            End Using
        Catch ex As Exception
            SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Fehler aufgetreten: " & ex.Message & ex.StackTrace)
            Return False
        End Try
        SchreibeVerbindungsprotokoll(Lizenzschluessel, WindowsUsername, Domainname, Computername, "Eichprozesse erfolgreich aus Ablage entfernt")

    End Function
#End Region

End Class