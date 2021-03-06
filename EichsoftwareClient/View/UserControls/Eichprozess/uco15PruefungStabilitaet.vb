Public Class uco15PruefungStabilitaet

    Inherits ucoContent
    Implements IRhewaEditingDialog
    Implements IRhewaPruefungDialog

#Region "Member Variables"
    Private _suspendEvents As Boolean = False 'Variable zum temporären stoppen der Eventlogiken
    'Private AktuellerStatusDirty As Boolean = False 'variable die genutzt wird, um bei öffnen eines existierenden Eichprozesses speichern zu können wenn grundlegende Änderungen vorgenommen wurden. Wie das ändern der Waagenart und der Waegezelle. Dann wird der Vorgang auf Komptabilitätsnachweis zurückgesetzt
    Private _ListPruefungStabilitaet As New List(Of PruefungStabilitaetGleichgewichtslage)
    Private _currentObjPruefungStabilitaetGleichgewichtslage As PruefungStabilitaetGleichgewichtslage
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
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage
    End Sub
#End Region

#Region "Events"

    Private Sub ucoBeschaffenheitspruefung_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SetzeUeberschrift()
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage

        'daten füllen
        LoadFromDatabase()
    End Sub


    ''' <summary>
    ''' LAst auf alle Lasten ausweiten
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControlLast1_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlLast5.TextChanged, RadTextBoxControlLast4.TextChanged, RadTextBoxControlLast3.TextChanged, RadTextBoxControlLast2.TextChanged, RadTextBoxControlLast1.TextChanged
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True

        UeberschreibeLast(sender)

    End Sub


    Private Sub RadTextBoxControlMax5_TextChanged(sender As System.Object, e As System.EventArgs) Handles RadTextBoxControlMin5.TextChanged, RadTextBoxControlMin4.TextChanged, RadTextBoxControlMin3.TextChanged, RadTextBoxControlMin2.TextChanged, RadTextBoxControlMin1.TextChanged, RadTextBoxControlMax5.TextChanged, RadTextBoxControlMax4.TextChanged, RadTextBoxControlMax3.TextChanged, RadTextBoxControlMax2.TextChanged, RadTextBoxControlMax1.TextChanged, RadTextBoxControlAnzeige5.TextChanged, RadTextBoxControlAnzeige4.TextChanged, RadTextBoxControlAnzeige3.TextChanged, RadTextBoxControlAnzeige2.TextChanged, RadTextBoxControlAnzeige1.TextChanged
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True
    End Sub

    Private Sub RadCheckBoxAbdruck5_Click(sender As System.Object, e As System.EventArgs) Handles RadCheckBoxAbdruck5.Click, RadCheckBoxAbdruck4.Click, RadCheckBoxAbdruck3.Click, RadCheckBoxAbdruck2.Click, RadCheckBoxAbdruck1.Click
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True
    End Sub
#End Region

#Region "Methods"
    Private Sub LadePruefungen() Implements IRhewaPruefungDialog.LadePruefungen
        'Nur laden wenn es sich um eine Bearbeitung handelt (sonst würde das in Memory Objekt überschrieben werden)
        If Not DialogModus = enuDialogModus.lesend And Not DialogModus = enuDialogModus.korrigierend Then
            LadePruefungenBearbeitungsModus()
        Else
            LadePruefungenRHEWAKorrekturModus()

        End If
    End Sub

    Private Sub LadePruefungenRHEWAKorrekturModus() Implements IRhewaPruefungDialog.LadePruefungenRHEWAKorrekturModus
        Try
            'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen

            For Each obj In objEichprozess.Eichprotokoll.PruefungStabilitaetGleichgewichtslage
                obj.Eichprotokoll = objEichprozess.Eichprotokoll

                _ListPruefungStabilitaet.Add(obj)
            Next
        Catch ex As System.ObjectDisposedException 'fehler im Clientseitigen Lesemodus (bei bereits abegschickter Eichung)
            Using context As New Entities
                'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                Dim query = From a In context.PruefungStabilitaetGleichgewichtslage Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungStabilitaet = query.ToList
            End Using
        End Try
    End Sub

    Private Sub LadePruefungenBearbeitungsModus() Implements IRhewaPruefungDialog.LadePruefungenBearbeitungsModus
        Using context As New Entities
            'neu laden des Objekts, diesmal mit den lookup Objekten
            objEichprozess = (From a In context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault

            'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
            Dim query = From a In context.PruefungStabilitaetGleichgewichtslage Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
            _ListPruefungStabilitaet = query.ToList

        End Using
    End Sub


    Private Sub UpdatePruefungsObject(ByVal PObjPruefung As PruefungStabilitaetGleichgewichtslage)
        If PObjPruefung.Durchlauf = 1 Then
            PObjPruefung.Last = RadTextBoxControlLast1.Text
            PObjPruefung.Anzeige = RadTextBoxControlAnzeige1.Text
            PObjPruefung.MIN = RadTextBoxControlMin1.Text
            PObjPruefung.MAX = RadTextBoxControlMax1.Text
            PObjPruefung.AbdruckOK = RadCheckBoxAbdruck1.Checked
        ElseIf PObjPruefung.Durchlauf = 2 Then
            'durchlauf 2
            PObjPruefung.Last = RadTextBoxControlLast2.Text
            PObjPruefung.Anzeige = RadTextBoxControlAnzeige2.Text
            PObjPruefung.MIN = RadTextBoxControlMin2.Text
            PObjPruefung.MAX = RadTextBoxControlMax2.Text
            PObjPruefung.AbdruckOK = RadCheckBoxAbdruck2.Checked
        ElseIf PObjPruefung.Durchlauf = 3 Then
            'durchlauf 3
            PObjPruefung.Last = RadTextBoxControlLast3.Text
            PObjPruefung.Anzeige = RadTextBoxControlAnzeige3.Text
            PObjPruefung.MIN = RadTextBoxControlMin3.Text
            PObjPruefung.MAX = RadTextBoxControlMax3.Text
            PObjPruefung.AbdruckOK = RadCheckBoxAbdruck3.Checked
        ElseIf PObjPruefung.Durchlauf = 4 Then
            'durchlauf 4
            PObjPruefung.Last = RadTextBoxControlLast4.Text
            PObjPruefung.Anzeige = RadTextBoxControlAnzeige4.Text
            PObjPruefung.MIN = RadTextBoxControlMin4.Text
            PObjPruefung.MAX = RadTextBoxControlMax4.Text
            PObjPruefung.AbdruckOK = RadCheckBoxAbdruck4.Checked
        ElseIf PObjPruefung.Durchlauf = 5 Then
            'durchlauf 5
            PObjPruefung.Last = RadTextBoxControlLast5.Text
            PObjPruefung.Anzeige = RadTextBoxControlAnzeige5.Text
            PObjPruefung.MIN = RadTextBoxControlMin5.Text
            PObjPruefung.MAX = RadTextBoxControlMax5.Text
            PObjPruefung.AbdruckOK = RadCheckBoxAbdruck5.Checked

        End If
    End Sub
    Private Sub UeberschreibePruefungsobjekte()
        objEichprozess.Eichprotokoll.PruefungStabilitaetGleichgewichtslage.Clear()
        For Each obj In _ListPruefungStabilitaet
            objEichprozess.Eichprotokoll.PruefungStabilitaetGleichgewichtslage.Add(obj)
        Next
    End Sub

    Private Sub UeberschreibeLast(sender As Object)
        'damit keine Event Kettenreaktion durchgeführt wird, werden die Events ab hier unterbrochen
        _suspendEvents = True
        If sender.name.Equals(RadTextBoxControlLast1.Name) Then
            RadTextBoxControlLast1.Text = RadTextBoxControlLast1.Text
            RadTextBoxControlLast2.Text = RadTextBoxControlLast1.Text
            RadTextBoxControlLast3.Text = RadTextBoxControlLast1.Text
            RadTextBoxControlLast4.Text = RadTextBoxControlLast1.Text
            RadTextBoxControlLast5.Text = RadTextBoxControlLast1.Text
        ElseIf sender.name.Equals(RadTextBoxControlLast2.Name) Then
            RadTextBoxControlLast1.Text = RadTextBoxControlLast2.Text
            RadTextBoxControlLast2.Text = RadTextBoxControlLast2.Text
            RadTextBoxControlLast3.Text = RadTextBoxControlLast2.Text
            RadTextBoxControlLast4.Text = RadTextBoxControlLast2.Text
            RadTextBoxControlLast5.Text = RadTextBoxControlLast2.Text
        ElseIf sender.name.Equals(RadTextBoxControlLast3.Name) Then
            RadTextBoxControlLast1.Text = RadTextBoxControlLast3.Text
            RadTextBoxControlLast2.Text = RadTextBoxControlLast3.Text
            RadTextBoxControlLast3.Text = RadTextBoxControlLast3.Text
            RadTextBoxControlLast4.Text = RadTextBoxControlLast3.Text
            RadTextBoxControlLast5.Text = RadTextBoxControlLast3.Text
        ElseIf sender.name.Equals(RadTextBoxControlLast4.Name) Then
            RadTextBoxControlLast1.Text = RadTextBoxControlLast4.Text
            RadTextBoxControlLast2.Text = RadTextBoxControlLast4.Text
            RadTextBoxControlLast3.Text = RadTextBoxControlLast4.Text
            RadTextBoxControlLast4.Text = RadTextBoxControlLast4.Text
            RadTextBoxControlLast5.Text = RadTextBoxControlLast4.Text
        ElseIf sender.name.Equals(RadTextBoxControlLast5.Name) Then
            RadTextBoxControlLast1.Text = RadTextBoxControlLast5.Text
            RadTextBoxControlLast2.Text = RadTextBoxControlLast5.Text
            RadTextBoxControlLast3.Text = RadTextBoxControlLast5.Text
            RadTextBoxControlLast4.Text = RadTextBoxControlLast5.Text
            RadTextBoxControlLast5.Text = RadTextBoxControlLast5.Text
        End If
        _suspendEvents = False
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
            DisableControls(RadScrollablePanel1.PanelContainer)

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
        'anzeige KG Nur laden wenn schon etwas eingegeben wurde
        'durchlauf 1
        _currentObjPruefungStabilitaetGleichgewichtslage = Nothing
        _currentObjPruefungStabilitaetGleichgewichtslage = (From o In _ListPruefungStabilitaet Where o.Durchlauf = "1").FirstOrDefault

        If Not _currentObjPruefungStabilitaetGleichgewichtslage Is Nothing Then
            RadTextBoxControlLast1.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Last
            RadTextBoxControlAnzeige1.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Anzeige
            RadTextBoxControlMin1.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MIN
            RadTextBoxControlMax1.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MAX
            RadCheckBoxAbdruck1.Checked = _currentObjPruefungStabilitaetGleichgewichtslage.AbdruckOK
        End If

        'durchlauf 2
        _currentObjPruefungStabilitaetGleichgewichtslage = Nothing
        _currentObjPruefungStabilitaetGleichgewichtslage = (From o In _ListPruefungStabilitaet Where o.Durchlauf = "2").FirstOrDefault

        If Not _currentObjPruefungStabilitaetGleichgewichtslage Is Nothing Then
            RadTextBoxControlLast2.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Last
            RadTextBoxControlAnzeige2.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Anzeige
            RadTextBoxControlMin2.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MIN
            RadTextBoxControlMax2.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MAX
            RadCheckBoxAbdruck2.Checked = _currentObjPruefungStabilitaetGleichgewichtslage.AbdruckOK
        End If

        'durchlauf 3
        _currentObjPruefungStabilitaetGleichgewichtslage = Nothing
        _currentObjPruefungStabilitaetGleichgewichtslage = (From o In _ListPruefungStabilitaet Where o.Durchlauf = "3").FirstOrDefault

        If Not _currentObjPruefungStabilitaetGleichgewichtslage Is Nothing Then
            RadTextBoxControlLast3.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Last
            RadTextBoxControlAnzeige3.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Anzeige
            RadTextBoxControlMin3.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MIN
            RadTextBoxControlMax3.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MAX
            RadCheckBoxAbdruck3.Checked = _currentObjPruefungStabilitaetGleichgewichtslage.AbdruckOK
        End If

        'durchlauf 4
        _currentObjPruefungStabilitaetGleichgewichtslage = Nothing
        _currentObjPruefungStabilitaetGleichgewichtslage = (From o In _ListPruefungStabilitaet Where o.Durchlauf = "4").FirstOrDefault

        If Not _currentObjPruefungStabilitaetGleichgewichtslage Is Nothing Then
            RadTextBoxControlLast4.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Last
            RadTextBoxControlAnzeige4.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Anzeige
            RadTextBoxControlMin4.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MIN
            RadTextBoxControlMax4.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MAX
            RadCheckBoxAbdruck4.Checked = _currentObjPruefungStabilitaetGleichgewichtslage.AbdruckOK
        End If

        'durchlauf 5
        _currentObjPruefungStabilitaetGleichgewichtslage = Nothing
        _currentObjPruefungStabilitaetGleichgewichtslage = (From o In _ListPruefungStabilitaet Where o.Durchlauf = "5").FirstOrDefault

        If Not _currentObjPruefungStabilitaetGleichgewichtslage Is Nothing Then
            RadTextBoxControlLast5.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Last
            RadTextBoxControlAnzeige5.Text = _currentObjPruefungStabilitaetGleichgewichtslage.Anzeige
            RadTextBoxControlMin5.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MIN
            RadTextBoxControlMax5.Text = _currentObjPruefungStabilitaetGleichgewichtslage.MAX
            RadCheckBoxAbdruck5.Checked = _currentObjPruefungStabilitaetGleichgewichtslage.AbdruckOK
        End If

        'fokus setzen auf erstes Steuerelement
        RadTextBoxControlLast1.Focus()

    End Sub

    Protected Friend Overrides Function ValidateControls() As Boolean Implements IRhewaEditingDialog.ValidateControls
        'prüfen ob alle Felder ausgefüllt sind
        Me.AbortSaving = False

        If RadCheckBoxAbdruck1.Checked = False Or
     RadCheckBoxAbdruck2.Checked = False Or
     RadCheckBoxAbdruck3.Checked = False Or
     RadCheckBoxAbdruck4.Checked = False Or
      RadCheckBoxAbdruck5.Checked = False Then
            AbortSaving = True
        End If

        lblPflichtfeld1.Visible = IIf(RadCheckBoxAbdruck1.Checked = False, True, False)
        lblPflichtfeld2.Visible = IIf(RadCheckBoxAbdruck2.Checked = False, True, False)
        lblPflichtfeld3.Visible = IIf(RadCheckBoxAbdruck3.Checked = False, True, False)
        lblPflichtfeld4.Visible = IIf(RadCheckBoxAbdruck4.Checked = False, True, False)
        lblPflichtfeld5.Visible = IIf(RadCheckBoxAbdruck5.Checked = False, True, False)

        'fehlermeldung anzeigen bei falscher validierung
        'sonderfall Kopierte Waage
        If objEichprozess.AusStandardwaageErzeugt Then
            If Not AbortSaving Then
                Return Me.ShowValidationErrorBoxStandardwaage(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage)
            Else
                Dim result = Me.ShowValidationErrorBox(False)
                Return ProcessResult(result)

            End If
        Else
            'fehlermeldung anzeigen bei falscher validierung
            Dim result = Me.ShowValidationErrorBox(False)
            Return ProcessResult(result)


        End If
    End Function

    Protected Friend Overrides Sub OverwriteIstSoll() Implements IRhewaEditingDialog.OverwriteIstSoll
        RadCheckBoxAbdruck1.Checked = True
        RadCheckBoxAbdruck2.Checked = True
        RadCheckBoxAbdruck3.Checked = True
        RadCheckBoxAbdruck4.Checked = True
        RadCheckBoxAbdruck5.Checked = True
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

                    'wenn es defintiv noch keine pruefungen gibt, neue Anlegen
                    If _ListPruefungStabilitaet.Count = 0 Then
                        'anzahl Bereiche auslesen um damit die anzahl der benötigten Iterationen und Objekt Erzeugungen zu erfahren

                        For i = 1 To 5
                            Dim objPruefung = Context.PruefungStabilitaetGleichgewichtslage.Create
                            'wenn es die eine itereation mehr ist:
                            objPruefung.Durchlauf = i

                            UpdatePruefungsObject(objPruefung)

                            Context.SaveChanges()

                            objEichprozess.Eichprotokoll.PruefungStabilitaetGleichgewichtslage.Add(objPruefung)
                            Context.SaveChanges()

                            _ListPruefungStabilitaet.Add(objPruefung)
                        Next
                    Else ' es gibt bereits welche
                        'jedes objekt initialisieren und aus context laden und updaten
                        For Each objPruefung In _ListPruefungStabilitaet
                            objPruefung = Context.PruefungStabilitaetGleichgewichtslage.FirstOrDefault(Function(value) value.ID = objPruefung.ID)
                            UpdatePruefungsObject(objPruefung)
                            Context.SaveChanges()
                        Next

                    End If

                    'Speichern in Datenbank
                    Context.SaveChanges()
                End If
            End If
        End Using
    End Sub
    Protected Friend Overrides Sub AktualisiereStatus() Implements IRhewaEditingDialog.AktualisiereStatus
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
                    If AktuellerStatusDirty = False Then
                        ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                        If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                            objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                        End If
                    ElseIf AktuellerStatusDirty = True Then
                        objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                        AktuellerStatusDirty = False
                    End If

                    'Speichern in Datenbank
                    Context.SaveChanges()
                End If
            End If
        End Using
    End Sub

    Protected Friend Overrides Function CheckDialogModus() As Boolean Implements IRhewaEditingDialog.CheckDialogModus
        If DialogModus = enuDialogModus.lesend Or DialogModus = enuDialogModus.korrigierend Then
            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
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
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStabilitaet)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStabilitaet
            Catch ex As Exception
            End Try
        End If
    End Sub


    Protected Friend Overrides Sub Lokalisiere() Implements IRhewaEditingDialog.Lokalisiere
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uco15PruefungStabilitaet))
        Lokalisierung(Me, resources)
    End Sub




    Protected Friend Overrides Sub Entsperrung() Implements IRhewaEditingDialog.Entsperrung
        'Hiermit wird ein lesender Vorgang wieder entsperrt.
        For Each Control In Me.RadScrollablePanel1.PanelContainer.Controls
            Try
                Control.readonly = Not Control.readonly
            Catch ex As Exception
                Try
                    Control.isreadonly = Not Control.isReadonly
                Catch ex2 As Exception
                    Try
                        Control.enabled = Not Control.enabled
                    Catch ex3 As Exception
                    End Try
                End Try
            End Try
        Next

        'ändern des Moduses
        DialogModus = enuDialogModus.korrigierend
        ParentFormular.DialogModus = FrmMainContainer.enuDialogModus.korrigierend
    End Sub

    Protected Friend Overrides Sub UpdateObjekt() Implements IRhewaEditingDialog.UpdateObjekt
        'neuen Context aufbauen

        Using Context As New Entities
            'jedes objekt initialisieren und aus context laden und updaten
            For Each obj In _ListPruefungStabilitaet
                Dim objPruefung = Context.PruefungStabilitaetGleichgewichtslage.FirstOrDefault(Function(value) value.ID = obj.ID)
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


    Protected Friend Overrides Sub Versenden() Implements IRhewaEditingDialog.Versenden
        UpdateObjekt()
        UeberschreibePruefungsobjekte()
        'Erzeugen eines Server Objektes auf basis des aktuellen DS. Setzt es auf es ausserdem auf Fehlerhaft
        CloneAndSendServerObjekt()
    End Sub

#End Region

End Class