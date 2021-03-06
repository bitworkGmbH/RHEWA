Public Class uco14PruefungAnsprechvermoegen

    Inherits ucoContent
    Implements IRhewaEditingDialog
    Implements IRhewaPruefungDialog
#Region "Member Variables"
    Private _suspendEvents As Boolean = False 'Variable zum temporären stoppen der Eventlogiken
    'Private AktuellerStatusDirty As Boolean = False 'variable die genutzt wird, um bei öffnen eines existierenden Eichprozesses speichern zu können wenn grundlegende Änderungen vorgenommen wurden. Wie das ändern der Waagenart und der Waegezelle. Dann wird der Vorgang auf Komptabilitätsnachweis zurückgesetzt
    Private _objEichprotokoll As Eichprotokoll

    Private _currentObjPruefungAnsprechvermoegen As PruefungAnsprechvermoegen
    Private _ListPruefungAnsprechvermoegen As New List(Of PruefungAnsprechvermoegen)
#End Region

#Region "Constructors"
    Sub New()
        MyBase.New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
    End Sub
    Sub New(ByRef pParentform As FrmMainContainer, ByRef pObjEichprozess As Eichprozess, Optional ByRef pPreviousUco As ucoContent = Nothing, Optional ByRef pNextUco As ucoContent = Nothing, Optional ByVal pEnuModus As enuDialogModus = enuDialogModus.normal)
        MyBase.New(pParentform, pObjEichprozess, pPreviousUco, pNextUco, pEnuModus)
        InitializeComponent()
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungdesAnsprechvermögens
    End Sub
#End Region

#Region "Events"

    Private Sub ucoBeschaffenheitspruefung_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SetzeUeberschrift()
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungdesAnsprechvermögens

        'daten füllen
        LoadFromDatabase()
    End Sub



    Private Sub RadButtonShowEFGSteigend_Click(sender As Object, e As EventArgs) Handles RadButtonShowEFG.Click
        ShowEichfehlergrenzenDialog()
    End Sub

    'berechnen des D1 Wertes aus MIN Eichwert + Anzeige2
    Private Sub RadTextBoxControlLast1_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlAnzeige1.TextChanged, RadTextBoxControlLast1.TextChanged
        'min
        If _suspendEvents = False Then
            AktuellerStatusDirty = True

        End If
        Try
            RadTextBoxControLastD1.Text = CDec(RadTextBoxControlAnzeige1.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert1
        Catch ex As Exception
            RadTextBoxControLastD1.Text = 0
        End Try
    End Sub
    'berechnen des D2 Wertes aus MAX Eichwert + Anzeige2
    Private Sub RadTextBoxControlLast2_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlAnzeige2.TextChanged, RadTextBoxControlLast1.TextChanged
        Try
            If _suspendEvents = False Then
                AktuellerStatusDirty = True

            End If
            Select Case objEichprozess.Lookup_Waagenart.Art
                Case Is = "Einbereichswaage"
                    RadTextBoxControlLastD2.Text = CDec(RadTextBoxControlAnzeige2.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert1

                Case Is = "Zweibereichswaage", "Zweiteilungswaage"
                    RadTextBoxControlLastD2.Text = CDec(RadTextBoxControlAnzeige2.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert2

                Case Is = "Dreibereichswaage", "Dreiteilungswaage"
                    RadTextBoxControlLastD2.Text = CDec(RadTextBoxControlAnzeige2.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert3

            End Select
        Catch ex As Exception
            RadTextBoxControlLastD2.Text = 0
        End Try
    End Sub

    'berechnen des D3 Wertes aus MAX Eichwert + Anzeige3
    Private Sub RadTextBoxControlLast3_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlAnzeige3.TextChanged, RadTextBoxControlLast1.TextChanged
        If _suspendEvents = False Then
            AktuellerStatusDirty = True

        End If

        Try
            Select Case objEichprozess.Lookup_Waagenart.Art
                Case Is = "Einbereichswaage"
                    RadTextBoxControlLastD3.Text = CDec(RadTextBoxControlAnzeige3.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert1
                Case Is = "Zweibereichswaage", "Zweiteilungswaage"
                    RadTextBoxControlLastD3.Text = CDec(RadTextBoxControlAnzeige3.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert2
                Case Is = "Dreibereichswaage", "Dreiteilungswaage"
                    RadTextBoxControlLastD3.Text = CDec(RadTextBoxControlAnzeige3.Text) + objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert3
            End Select
        Catch ex As Exception
            RadTextBoxControlLastD3.Text = 0
        End Try
    End Sub

    Private Sub RadTextBoxControlAnzeige1_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles RadTextBoxControlAnzeige3.Validating, RadTextBoxControlAnzeige2.Validating, RadTextBoxControlAnzeige1.Validating,
        RadTextBoxControlLast3.Validating, RadTextBoxControlLast2.Validating, RadTextBoxControlLast1.Validating

        BasicTextboxValidation(sender, e)
    End Sub

    Private Sub RadCheckBoxMin_ToggleStateChanged(sender As Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles RadCheckBoxMin.ToggleStateChanged, RadCheckBoxMax.ToggleStateChanged, RadCheckBoxHalb.ToggleStateChanged
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True
    End Sub



    Private Sub RadTextBoxControlLast2_TextChanged_1(sender As System.Object, e As System.EventArgs) Handles RadTextBoxControlLast3.TextChanged, RadTextBoxControlLast2.TextChanged
        If _suspendEvents = False Then
            AktuellerStatusDirty = True
        End If
    End Sub
#End Region

#Region "Methods"


    Protected Friend Sub LadePruefungen() Implements IRhewaPruefungDialog.LadePruefungen
        'Nur laden wenn es sich um eine Bearbeitung handelt (sonst würde das in Memory Objekt überschrieben werden)
        If Not DialogModus = enuDialogModus.lesend And Not DialogModus = enuDialogModus.korrigierend Then
            LadePruefungenBearbeitungsModus()
        Else
            LadePruefungenRHEWAKorrekturModus()
        End If
    End Sub

    Private Sub LadePruefungenRHEWAKorrekturModus() Implements IRhewaPruefungDialog.LadePruefungenRHEWAKorrekturModus
        _objEichprotokoll = objEichprozess.Eichprotokoll
        _ListPruefungAnsprechvermoegen.Clear()
        Try
            'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
            For Each obj In objEichprozess.Eichprotokoll.PruefungAnsprechvermoegen
                obj.Eichprotokoll = objEichprozess.Eichprotokoll

                _ListPruefungAnsprechvermoegen.Add(obj)
            Next
        Catch ex As System.ObjectDisposedException 'fehler im Clientseitigen Lesemodus (bei bereits abegschickter Eichung)
            Using context As New Entities
                'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                Dim query = From a In context.PruefungAnsprechvermoegen Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungAnsprechvermoegen = query.ToList

            End Using
        End Try
    End Sub

    Private Sub LadePruefungenBearbeitungsModus() Implements IRhewaPruefungDialog.LadePruefungenBearbeitungsModus
        Using context As New Entities
            'neu laden des Objekts, diesmal mit den lookup Objekten
            objEichprozess = (From a In context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault
            _objEichprotokoll = objEichprozess.Eichprotokoll

            'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
            Dim query = From a In context.PruefungAnsprechvermoegen Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
            _ListPruefungAnsprechvermoegen = query.ToList
        End Using
    End Sub


    Private Sub UpdatePruefungsObject(ByVal pObjPruefung As PruefungAnsprechvermoegen)
        Select Case pObjPruefung.LastL
            Case Is = "Min"
                pObjPruefung.Last = RadTextBoxControlLast1.Text
                pObjPruefung.Anzeige = RadTextBoxControlAnzeige1.Text
                pObjPruefung.Last1d = RadTextBoxControLastD1.Text
                pObjPruefung.Ziffernsprung = RadCheckBoxMin.Checked
            Case Is = "Halb"
                pObjPruefung.Last = RadTextBoxControlLast2.Text
                pObjPruefung.Anzeige = RadTextBoxControlAnzeige2.Text
                pObjPruefung.Last1d = RadTextBoxControlLastD2.Text
                pObjPruefung.Ziffernsprung = RadCheckBoxHalb.Checked
            Case Is = "Max"
                pObjPruefung.Last = RadTextBoxControlLast3.Text
                pObjPruefung.Anzeige = RadTextBoxControlAnzeige3.Text
                pObjPruefung.Last1d = RadTextBoxControlLastD3.Text
                pObjPruefung.Ziffernsprung = RadCheckBoxMax.Checked
        End Select
    End Sub

    Private Sub UeberschreibePruefungsobjekte()
        objEichprozess.Eichprotokoll.PruefungAnsprechvermoegen.Clear()
        For Each obj In _ListPruefungAnsprechvermoegen
            objEichprozess.Eichprotokoll.PruefungAnsprechvermoegen.Add(obj)
        Next
    End Sub


    Private Sub AddPruefungsObjekt(Context As Entities)
        Dim objPruefung = Context.PruefungAnsprechvermoegen.Create
        'wenn es die eine itereation mehr ist:
        objPruefung.LastL = "Min"
        UpdatePruefungsObject(objPruefung)

        Context.SaveChanges()

        objEichprozess.Eichprotokoll.PruefungAnsprechvermoegen.Add(objPruefung)
        Context.SaveChanges()

        _ListPruefungAnsprechvermoegen.Add(objPruefung)

        'halb
        objPruefung = Nothing
        objPruefung = Context.PruefungAnsprechvermoegen.Create
        'wenn es die eine itereation mehr ist:
        objPruefung.LastL = "Halb"
        UpdatePruefungsObject(objPruefung)

        Context.SaveChanges()

        objEichprozess.Eichprotokoll.PruefungAnsprechvermoegen.Add(objPruefung)
        Context.SaveChanges()

        _ListPruefungAnsprechvermoegen.Add(objPruefung)

        'max
        objPruefung = Nothing
        objPruefung = Context.PruefungAnsprechvermoegen.Create
        'wenn es die eine itereation mehr ist:
        objPruefung.LastL = "Max"
        UpdatePruefungsObject(objPruefung)

        Context.SaveChanges()

        objEichprozess.Eichprotokoll.PruefungAnsprechvermoegen.Add(objPruefung)
        Context.SaveChanges()

        _ListPruefungAnsprechvermoegen.Add(objPruefung)
    End Sub




#End Region

#Region "Interface Methods"

    Protected Friend Overrides Sub LoadFromDatabase() Implements IRhewaEditingDialog.LoadFromDatabase
        objEichprozess = ParentFormular.CurrentEichprozess
        'events abbrechen
        _suspendEvents = True

        LadePruefungen()

        'steuerelemente mit werten aus DB füllen
        FillControls()

        If DialogModus = enuDialogModus.lesend Then
            'falls der Konformitätsbewertungsvorgang nur lesend betrchtet werden soll, wird versucht alle Steuerlemente auf REadonly zu setzen. Wenn das nicht klappt,werden sie disabled
            DisableControls(Me)

        End If
        'events abbrechen
        _suspendEvents = False
    End Sub

    ''' <summary>
    ''' Lädt die Werte aus dem Objekt in die Steuerlemente
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub FillControls() Implements IRhewaEditingDialog.FillControls
        'Steuerlemente füllen
        'min
        _currentObjPruefungAnsprechvermoegen = Nothing
        _currentObjPruefungAnsprechvermoegen = (From o In _ListPruefungAnsprechvermoegen Where o.LastL = "Min").FirstOrDefault

        If Not _currentObjPruefungAnsprechvermoegen Is Nothing Then
            RadTextBoxControlLast1.Text = _currentObjPruefungAnsprechvermoegen.Last
            RadTextBoxControlAnzeige1.Text = _currentObjPruefungAnsprechvermoegen.Anzeige
            RadTextBoxControLastD1.Text = _currentObjPruefungAnsprechvermoegen.Last1d
            RadCheckBoxMin.Checked = _currentObjPruefungAnsprechvermoegen.Ziffernsprung
        Else
            Select Case objEichprozess.Lookup_Waagenart.Art
                Case Is = "Einbereichswaage"
                    RadTextBoxControlLast1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min1
                Case Is = "Zweibereichswaage", "Zweiteilungswaage"
                    RadTextBoxControlLast1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min2
                Case Is = "Dreibereichswaage", "Dreiteilungswaage"
                    RadTextBoxControlLast1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min3
            End Select
            'anzeige + eichwert
        End If

        'halb
        _currentObjPruefungAnsprechvermoegen = Nothing
        _currentObjPruefungAnsprechvermoegen = (From o In _ListPruefungAnsprechvermoegen Where o.LastL = "Halb").FirstOrDefault

        If Not _currentObjPruefungAnsprechvermoegen Is Nothing Then
            RadTextBoxControlLast2.Text = _currentObjPruefungAnsprechvermoegen.Last
            RadTextBoxControlAnzeige2.Text = _currentObjPruefungAnsprechvermoegen.Anzeige
            RadTextBoxControlLastD2.Text = _currentObjPruefungAnsprechvermoegen.Last1d
            RadCheckBoxHalb.Checked = _currentObjPruefungAnsprechvermoegen.Ziffernsprung
        Else
            Select Case objEichprozess.Lookup_Waagenart.Art
                Case Is = "Einbereichswaage"
                    RadTextBoxControlLast2.Text = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1) / 2
                Case Is = "Zweibereichswaage", "Zweiteilungswaage"
                    RadTextBoxControlLast2.Text = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2) / 2
                Case Is = "Dreibereichswaage", "Dreiteilungswaage"
                    RadTextBoxControlLast2.Text = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3) / 2
            End Select
        End If

        'max
        _currentObjPruefungAnsprechvermoegen = Nothing
        _currentObjPruefungAnsprechvermoegen = (From o In _ListPruefungAnsprechvermoegen Where o.LastL = "Max").FirstOrDefault

        If Not _currentObjPruefungAnsprechvermoegen Is Nothing Then
            RadTextBoxControlLast3.Text = _currentObjPruefungAnsprechvermoegen.Last
            RadTextBoxControlAnzeige3.Text = _currentObjPruefungAnsprechvermoegen.Anzeige
            RadTextBoxControlLastD3.Text = _currentObjPruefungAnsprechvermoegen.Last1d
            RadCheckBoxMax.Checked = _currentObjPruefungAnsprechvermoegen.Ziffernsprung
        Else
            Select Case objEichprozess.Lookup_Waagenart.Art
                Case Is = "Einbereichswaage"
                    RadTextBoxControlLast3.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1
                Case Is = "Zweibereichswaage", "Zweiteilungswaage"
                    RadTextBoxControlLast3.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2
                Case Is = "Dreibereichswaage", "Dreiteilungswaage"
                    RadTextBoxControlLast3.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3
            End Select
        End If

    End Sub

    ''' <summary>
    ''' Füllt das Objekt mit den Werten aus den Steuerlementen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub UpdateObjekt() Implements IRhewaEditingDialog.UpdateObjekt
        'neuen Context aufbauen
        Using Context As New Entities
            'jedes objekt initialisieren und aus context laden und updaten
            For Each obj In _ListPruefungAnsprechvermoegen
                Dim objPruefung = Context.PruefungAnsprechvermoegen.FirstOrDefault(Function(value) value.ID = obj.ID)
                If Not objPruefung Is Nothing Then
                    'in lokaler DB gucken
                    UpdatePruefungsObject(objPruefung)
                Else 'es handelt sich um eine Serverobjekt im => Korrekturmodus
                    If DialogModus = enuDialogModus.korrigierend Then
                        UpdatePruefungsObject(obj)
                    End If
                End If
            Next

        End Using
    End Sub
    ''' <summary>
    ''' Gültigkeit der Eingaben überprüfen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Function ValidateControls() As Boolean Implements IRhewaEditingDialog.ValidateControls
        'prüfen ob alle Felder ausgefüllt sind
        Me.AbortSaving = False

        If RadCheckBoxHalb.Checked = False Then
            AbortSaving = True
        End If
        If RadCheckBoxMax.Checked = False Then
            AbortSaving = True
        End If

        If RadCheckBoxMin.Checked = False Then
            AbortSaving = True
        End If

        If RadTextBoxControlAnzeige1.Text.Trim = "" Or
            RadTextBoxControlAnzeige2.Text.Trim = "" Or
            RadTextBoxControlAnzeige3.Text.Trim = "" Or
RadTextBoxControlLast1.Text.Trim = "" Or
RadTextBoxControlLast2.Text.Trim = "" Or
            RadTextBoxControlLast3.Text.Trim = "" Then

            AbortSaving = True
        End If
        'fehlermeldung anzeigen bei falscher validierung
        If AbortSaving Then
            RadTextBoxControlAnzeige1.TextBoxElement.Border.ForeColor = Color.Red
            RadTextBoxControlAnzeige2.TextBoxElement.Border.ForeColor = Color.Red
            RadTextBoxControlAnzeige3.TextBoxElement.Border.ForeColor = Color.Red

        End If
        Dim result = Me.ShowValidationErrorBox(False)
        Return ProcessResult(result)

    End Function

    Protected Friend Overrides Sub OverwriteIstSoll() Implements IRhewaEditingDialog.OverwriteIstSoll
        RadTextBoxControlAnzeige1.Text = RadTextBoxControlLast1.Text
        RadTextBoxControlAnzeige2.Text = RadTextBoxControlLast2.Text
        RadTextBoxControlAnzeige3.Text = RadTextBoxControlLast3.Text
        RadCheckBoxHalb.Checked = True
        RadCheckBoxMax.Checked = True
        RadCheckBoxMin.Checked = True

    End Sub


    Protected Friend Overrides Sub SaveObjekt() Implements IRhewaEditingDialog.SaveObjekt
        'neuen Context aufbauen
        Using Context As New Entities
            'prüfen ob CREATE oder UPDATE durchgeführt werden muss
            If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                'prüfen ob das Objekt anhand der ID gefunden werden kann
                Dim dobjEichprozess As Eichprozess = (From a In Context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault
                If Not dobjEichprozess Is Nothing Then
                    'lokale Variable mit Instanz aus DB überschreiben. Dies ist notwendig, damit das Entity Framework weiß, das ein Update vorgenommen werden muss.
                    objEichprozess = dobjEichprozess
                    'neuen Status zuweisen

                    'wenn es defintiv noch keine pruefungen gibt, neue Anlegen
                    If _ListPruefungAnsprechvermoegen.Count = 0 Then
                        AddPruefungsObjekt(Context)

                    Else ' es gibt bereits welche
                        'jedes objekt initialisieren und aus context laden und updaten
                        For Each objPruefung In _ListPruefungAnsprechvermoegen
                            objPruefung = Context.PruefungAnsprechvermoegen.FirstOrDefault(Function(value) value.ID = objPruefung.ID)
                            UpdatePruefungsObject(objPruefung)
                            Context.SaveChanges()
                        Next
                    End If


                    'Füllt das Objekt mit den Werten aus den Steuerlementen
                    UpdateObjekt()
                    'Speichern in Datenbank
                    Context.SaveChanges()
                End If
            End If
        End Using
    End Sub


    Protected Friend Overrides Sub AktualisiereStatus() Implements IRhewaEditingDialog.AktualisiereStatus
        Using Context As New Entities
            'prüfen ob CREATE oder UPDATE durchgeführt werden muss
            If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                'prüfen ob das Objekt anhand der ID gefunden werden kann
                Dim dobjEichprozess As Eichprozess = (From a In Context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault
                If Not dobjEichprozess Is Nothing Then
                    'lokale Variable mit Instanz aus DB überschreiben. Dies ist notwendig, damit das Entity Framework weiß, das ein Update vorgenommen werden muss.
                    objEichprozess = dobjEichprozess
                    'Wenn kein Drucker gewählt wurde entfällt die PRüfung der Stablität der GLeichgewichtslage
                    If objEichprozess.Eichprotokoll.Verwendungszweck_Drucker = False Then
                        If AktuellerStatusDirty = False Then

                            ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                            End If
                        ElseIf AktuellerStatusDirty = True Then
                            objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                            AktuellerStatusDirty = False
                        End If
                    Else
                        If AktuellerStatusDirty = False Then

                            ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage Then
                                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage
                            End If
                        ElseIf AktuellerStatusDirty = True Then
                            objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage
                            AktuellerStatusDirty = False
                        End If
                    End If

                    'Füllt das Objekt mit den Werten aus den Steuerlementen
                    UpdateObjekt()
                    'Speichern in Datenbank
                    Context.SaveChanges()

                End If
            End If
        End Using
    End Sub

    Protected Friend Overrides Function CheckDialogModus() As Boolean Implements IRhewaEditingDialog.CheckDialogModus
        If DialogModus = enuDialogModus.lesend Then
            'Wenn kein Drucker gewählt wurde entfällt die PRüfung der Stablität der GLeichgewichtslage
            If objEichprozess.Eichprotokoll.Verwendungszweck_Drucker = False Then
                ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                End If
            Else
                ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage Then
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage
                End If
            End If
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If

        If DialogModus = enuDialogModus.korrigierend Then
            UpdateObjekt()
            'Wenn kein Drucker gewählt wurde entfällt die PRüfung der Stablität der GLeichgewichtslage
            If objEichprozess.Eichprotokoll.Verwendungszweck_Drucker = False Then
                ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                End If

            Else
                ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage Then
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage
                End If
            End If
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If
        Return True
    End Function

    Protected Friend Overrides Sub SetzeUeberschrift() Implements IRhewaEditingDialog.SetzeUeberschrift
        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungAnsprechvermoegen)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungAnsprechvermoegen
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Friend Overrides Sub Lokalisiere() Implements IRhewaEditingDialog.Lokalisiere
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uco14PruefungAnsprechvermoegen))
        Lokalisierung(Me, resources)
    End Sub

    Protected Friend Overrides Sub Entsperrung() Implements IRhewaEditingDialog.Entsperrung
        'Hiermit wird ein lesender Vorgang wieder entsperrt.
        EnableControls(Me)

        'ändern des Moduses
        DialogModus = enuDialogModus.korrigierend
        ParentFormular.DialogModus = FrmMainContainer.enuDialogModus.korrigierend
    End Sub

    Protected Friend Overrides Sub Versenden() Implements IRhewaEditingDialog.Versenden
        UpdateObjekt()
        CloneAndSendServerObjekt()
    End Sub


#End Region



End Class