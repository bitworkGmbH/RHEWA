﻿Public Class uco15PruefungStabilitaet
    Inherits ucoContent

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
        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStabilitaet)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStabilitaet
            Catch ex As Exception
            End Try
        End If
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage

        'daten füllen
        LoadFromDatabase()
    End Sub
#End Region

#Region "Methods"
    Private Sub LoadFromDatabase()

        objEichprozess = ParentFormular.CurrentEichprozess
        'events abbrechen
        _suspendEvents = True
        'Nur laden wenn es sich um eine Bearbeitung handelt (sonst würde das in Memory Objekt überschrieben werden)
        If Not DialogModus = enuDialogModus.lesend And Not DialogModus = enuDialogModus.korrigierend Then
            Using context As New EichsoftwareClientdatabaseEntities1
                'neu laden des Objekts, diesmal mit den lookup Objekten
                objEichprozess = (From a In context.Eichprozess.Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault

                'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                Dim query = From a In context.PruefungStabilitaetGleichgewichtslage Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungStabilitaet = query.ToList

            End Using
        Else
            Try
                'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen

                For Each obj In objEichprozess.Eichprotokoll.PruefungStabilitaetGleichgewichtslage
                    obj.Eichprotokoll = objEichprozess.Eichprotokoll

                    _ListPruefungStabilitaet.Add(obj)
                Next
            Catch ex As System.ObjectDisposedException 'fehler im Clientseitigen Lesemodus (bei bereits abegschickter Eichung)
                Using context As New EichsoftwareClientdatabaseEntities1
                    'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                    Dim query = From a In context.PruefungStabilitaetGleichgewichtslage Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                    _ListPruefungStabilitaet = query.ToList
                End Using
            End Try

        End If

        'steuerelemente mit werten aus DB füllen
        FillControls()

        If DialogModus = enuDialogModus.lesend Then
            'falls der Eichvorgang nur lesend betrchtet werden soll, wird versucht alle Steuerlemente auf REadonly zu setzen. Wenn das nicht klappt,werden sie disabled
            For Each Control In Me.RadScrollablePanel1.PanelContainer.Controls
                Try
                    Control.readonly = True
                Catch ex As Exception
                    Try
                        Control.isreadonly = True
                    Catch ex2 As Exception
                        Try
                            Control.enabled = False
                        Catch ex3 As Exception
                        End Try
                    End Try
                End Try
            Next
        End If
        'events abbrechen
        _suspendEvents = False
    End Sub

    ''' <summary>
    ''' aktualisieren der Oberfläche wenn nötig
    ''' </summary>
    ''' <param name="UserControl"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub UpdateNeeded(UserControl As UserControl)
        If Me.Equals(UserControl) Then
            MyBase.UpdateNeeded(UserControl)
            'Hilfetext setzen
            ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStabilitaet)
            'Überschrift setzen
            ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStabilitaet
            '   FillControls()
            LoadFromDatabase() 'war mal auskommentiert. ich weiß gerade nicht mehr wieso. Ergänzung: war ausdokumentiert, weil damit die Werte der NSW und WZ übeschrieben werden wenn man auf zurück klickt. Wenn es allerdings ausdokumenterit ist, funktioniert das anlegen einer neuen WZ nicht
        End If
    End Sub


    ''' <summary>
    ''' Lädt die Werte aus dem Objekt in die Steuerlemente
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Private Sub FillControls()
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


    Private Function ValidateControls() As Boolean
        'prüfen ob alle Felder ausgefüllt sind
        Me.AbortSaveing = False

        If RadCheckBoxAbdruck1.Checked = False Or _
     RadCheckBoxAbdruck2.Checked = False Or _
     RadCheckBoxAbdruck3.Checked = False Or _
     RadCheckBoxAbdruck4.Checked = False Or _
      RadCheckBoxAbdruck5.Checked = False Then
            AbortSaveing = True
        End If

         'fehlermeldung anzeigen bei falscher validierung
        Return Me.ShowValidationErrorBox()
    End Function


    'Speicherroutine
    Protected Overrides Sub SaveNeeded(ByVal UserControl As UserControl)
        If Me.Equals(UserControl) Then

            If DialogModus = enuDialogModus.lesend Then
                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                End If
                ParentFormular.CurrentEichprozess = objEichprozess
                Exit Sub
            End If


            If DialogModus = enuDialogModus.korrigierend Then
                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung Then
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung
                End If
                ParentFormular.CurrentEichprozess = objEichprozess
                Exit Sub
            End If

            If ValidateControls() = True Then


                'neuen Context aufbauen
                Using Context As New EichsoftwareClientdatabaseEntities1


                    'prüfen ob CREATE oder UPDATE durchgeführt werden muss
                    If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                        'prüfen ob das Objekt anhand der ID gefunden werden kann
                        Dim dobjEichprozess As Eichprozess = Context.Eichprozess.FirstOrDefault(Function(value) value.Vorgangsnummer = objEichprozess.Vorgangsnummer)
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

                ParentFormular.CurrentEichprozess = objEichprozess
            End If

        End If
    End Sub


    Protected Overrides Sub SaveWithoutValidationNeeded(usercontrol As UserControl)
        MyBase.SaveWithoutValidationNeeded(usercontrol)
        If Me.Equals(usercontrol) Then


            'neuen Context aufbauen
            Using Context As New EichsoftwareClientdatabaseEntities1
                If DialogModus = enuDialogModus.lesend Then
                    ParentFormular.CurrentEichprozess = objEichprozess
                    Exit Sub
                End If

                'prüfen ob CREATE oder UPDATE durchgeführt werden muss
                If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                    'prüfen ob das Objekt anhand der ID gefunden werden kann
                    Dim dobjEichprozess As Eichprozess = Context.Eichprozess.Include("Eichprotokoll").FirstOrDefault(Function(value) value.Vorgangsnummer = objEichprozess.Vorgangsnummer)
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

                    End If
                End If
            End Using
            ParentFormular.CurrentEichprozess = objEichprozess
        End If
    End Sub

#End Region

    Protected Overrides Sub LokalisierungNeeded(UserControl As System.Windows.Forms.UserControl)
        If Me.Equals(UserControl) = False Then Exit Sub

        MyBase.LokalisierungNeeded(UserControl)

        'lokalisierung: Leider kann ich den automatismus von .NET nicht nutzen. Dieser funktioniert nur sauber, wenn ein Dialog erzeugt wird. Zur Laufzeit aber gibt es diverse Probleme mit dem Automatischen Ändern der Sprache,
        'da auch informationen wie Positionen und Größen "lokalisiert" gespeichert werden. Wenn nun zur Laufzeit, also das Fenster größer gemacht wurde, setzt er die Anchor etc. auf die Ursprungsgröße 
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uco15PruefungStabilitaet))

        Me.lblAbdruck.Text = resources.GetString("lblAbdruck.Text")
        Me.lblAnzeige.Text = resources.GetString("lblAnzeige.Text")
        Me.lblBeschreibung.Text = resources.GetString("lblBeschreibung.Text")
        Me.lblLast.Text = resources.GetString("lblLast.Text")
        Me.lblPrintBeschreibung.Text = resources.GetString("lblPrintBeschreibung.Text")


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



    ''' <summary>
    ''' LAst auf alle Lasten ausweiten
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControlLast1_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlLast5.TextChanged, RadTextBoxControlLast4.TextChanged, RadTextBoxControlLast3.TextChanged, RadTextBoxControlLast2.TextChanged, RadTextBoxControlLast1.TextChanged
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True

        'damit keine Event Kettenreaktion durchgeführt wird, werden die Events ab hier unterbrochen
        _suspendEvents = True
        'bereich 1
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

    'Entsperrroutine
    Protected Overrides Sub EntsperrungNeeded()
        MyBase.EntsperrungNeeded()

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
    Private Sub UpdateObject()
        'neuen Context aufbauen

        Using Context As New EichsoftwareClientdatabaseEntities1
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
    Protected Overrides Sub VersendenNeeded(TargetUserControl As UserControl)


        If Me.Equals(TargetUserControl) Then
            MyBase.VersendenNeeded(TargetUserControl)
            Using dbcontext As New EichsoftwareClientdatabaseEntities1
                '   objEichprozess = (From a In dbcontext.Eichprozess.Include("Eichprotokoll").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Beschaffenheitspruefung").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault

                Dim objServerEichprozess As New EichsoftwareWebservice.ServerEichprozess
                'auf fehlerhaft Status setzen
                objEichprozess.FK_Bearbeitungsstatus = 2
                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Stammdateneingabe 'auf die erste Seite "zurückblättern" damit Eichbevollmächtigter sich den DS von Anfang angucken muss
                UpdateObject()
                UeberschreibePruefungsobjekte()

                'erzeuegn eines Server Objektes auf basis des aktuellen DS
                objServerEichprozess = clsClientServerConversionFunctions.CopyObjectProperties(objServerEichprozess, objEichprozess, clsClientServerConversionFunctions.enuModus.RHEWASendetAnClient)
                Using Webcontext As New EichsoftwareWebservice.EichsoftwareWebserviceClient
                    Try
                        Webcontext.Open()
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung, My.Resources.GlobaleLokalisierung.Fehler, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit Sub
                    End Try

                    Dim objLiz = (From db In dbcontext.Lizensierung Select db).FirstOrDefault

                    Try
                        'add prüft anhand der Vorgangsnummer automatisch ob ein neuer Prozess angelegt, oder ein vorhandener aktualisiert wird
                        Webcontext.AddEichprozess(objLiz.HEKennung, objLiz.Lizenzschluessel, objServerEichprozess, My.User.Name, System.Environment.UserDomainName, My.Computer.Name)

                        'schließen des dialoges
                        ParentFormular.Close()
                    Catch ex As Exception
                        MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error)
                        ' Status zurück setzen
                        Exit Sub
                    End Try
                End Using
            End Using
        End If
    End Sub

    Private Sub RadTextBoxControlMax5_TextChanged(sender As System.Object, e As System.EventArgs) Handles RadTextBoxControlMin5.TextChanged, RadTextBoxControlMin4.TextChanged, RadTextBoxControlMin3.TextChanged, RadTextBoxControlMin2.TextChanged, RadTextBoxControlMin1.TextChanged, RadTextBoxControlMax5.TextChanged, RadTextBoxControlMax4.TextChanged, RadTextBoxControlMax3.TextChanged, RadTextBoxControlMax2.TextChanged, RadTextBoxControlMax1.TextChanged, RadTextBoxControlAnzeige5.TextChanged, RadTextBoxControlAnzeige4.TextChanged, RadTextBoxControlAnzeige3.TextChanged, RadTextBoxControlAnzeige2.TextChanged, RadTextBoxControlAnzeige1.TextChanged
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True
    End Sub

    Private Sub RadCheckBoxAbdruck5_Click(sender As System.Object, e As System.EventArgs) Handles RadCheckBoxAbdruck5.Click, RadCheckBoxAbdruck4.Click, RadCheckBoxAbdruck3.Click, RadCheckBoxAbdruck2.Click, RadCheckBoxAbdruck1.Click
        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True
    End Sub
End Class
