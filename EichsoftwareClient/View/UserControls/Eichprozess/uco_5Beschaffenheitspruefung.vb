Imports EichsoftwareClient

Public Class uco_5Beschaffenheitspruefung
    Inherits ucoContent
    Implements IRhewaEditingDialog

#Region "Member Variables"
    Private _suspendEvents As Boolean = False 'Variable zum temporären stoppen der Eventlogiken
    Private _objEichprotokoll As Eichprotokoll
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
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.Beschaffenheitspruefung
    End Sub
#End Region

#Region "Events"

    Private Sub ucoBeschaffenheitspruefung_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SetzeUeberschrift()
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.Beschaffenheitspruefung
        'daten füllen
        LoadFromDatabase()
    End Sub


    ''' <summary>
    ''' da die checkboxen keinen Text haben, sieht man den Fokus nicht. Workaround der border aktiviert
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadCheckBox_GotFocus(sender As Object, e As EventArgs)
        If _suspendEvents Then Exit Sub
        Dim c As Telerik.WinControls.UI.RadCheckBox

        If (TypeOf sender Is Telerik.WinControls.UI.RadCheckBox) Then
            c = CType(sender, Telerik.WinControls.UI.RadCheckBox)

            c.ButtonElement.CheckMarkPrimitive.Border.BottomColor = Color.FromArgb(255, 0, 102, 51)
            c.ButtonElement.CheckMarkPrimitive.Border.LeftColor = Color.FromArgb(255, 0, 102, 51)
            c.ButtonElement.CheckMarkPrimitive.Border.RightColor = Color.FromArgb(255, 0, 102, 51)
            c.ButtonElement.CheckMarkPrimitive.Border.TopColor = Color.FromArgb(255, 0, 102, 51)

        End If
    End Sub
    ''' <summary>
    ''' da die checkboxen keinen Text haben, sieht man den Fokus nicht. Workaround der border deaktivert
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadCheckBox_LostFocus(sender As Object, e As EventArgs)
        If _suspendEvents Then Exit Sub
        Dim c As Telerik.WinControls.UI.RadCheckBox

        If (TypeOf sender Is Telerik.WinControls.UI.RadCheckBox) Then
            c = CType(sender, Telerik.WinControls.UI.RadCheckBox)
            c.ButtonElement.CheckMarkPrimitive.Border.BottomColor = Color.FromArgb(255, 173, 176, 173)
            c.ButtonElement.CheckMarkPrimitive.Border.LeftColor = Color.FromArgb(255, 173, 176, 173)
            c.ButtonElement.CheckMarkPrimitive.Border.RightColor = Color.FromArgb(255, 173, 176, 173)
            c.ButtonElement.CheckMarkPrimitive.Border.TopColor = Color.FromArgb(255, 173, 176, 173)
        End If
    End Sub

    Private Sub RadCheckBoxAWG1_Click(sender As System.Object, e As System.EventArgs)
        If _suspendEvents Then Exit Sub
        AktuellerStatusDirty = True
    End Sub

#End Region

#Region "Interface Methods"

    Protected Friend Overrides Sub SetzeUeberschrift() Implements IRhewaEditingDialog.SetzeUeberschrift
        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_Beschaffenheitspruefung)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_Beschaffenheitspruefung
            Catch ex As Exception
            End Try
        End If
    End Sub
    Protected Friend Overrides Sub LoadFromDatabase() Implements IRhewaEditingDialog.LoadFromDatabase
        'Laden aus Datenbank
        Using Context As New Entities
            _suspendEvents = True
            objEichprozess = ParentFormular.CurrentEichprozess
            'Nur laden wenn es sich um eine Bearbeitung handelt (sonst würde das in Memory Objekt überschrieben werden)
            If Not DialogModus = enuDialogModus.lesend And Not DialogModus = enuDialogModus.korrigierend Then
                objEichprozess = (From a In Context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault
            End If
        End Using
        _objEichprotokoll = objEichprozess.Eichprotokoll

        'steuerelemente mit werten aus DB füllen
        FillControls()

        If DialogModus = enuDialogModus.lesend Then
            'falls der Konformitätsbewertungsvorgang nur lesend betrchtet werden soll, wird versucht alle Steuerlemente auf REadonly zu setzen. Wenn das nicht klappt,werden sie disabled
            DisableControls(RadGroupBoxAufstellbedingungen)
            DisableControls(RadGroupBoxAuswerteGeraete)
            DisableControls(RadGroupBoxVerbindungselemente)
            DisableControls(RadGroupBoxWaegebruecke)
            DisableControls(RadGroupBoxWaegezellen)
        End If
        _suspendEvents = False
    End Sub

    ''' <summary>
    ''' Lädt die Werte aus dem Beschaffenheitspruefungsobjekt in die Steuerlemente
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub FillControls() Implements IRhewaEditingDialog.FillControls
        If Not _objEichprotokoll Is Nothing Then
            If Not _objEichprotokoll.Beschaffenheitspruefung_Genehmigt Is Nothing Then 'kompatiblitaet zu alten DS mit alter Beschaffenheitspruefung
                RadCheckBoxApprove.Checked = _objEichprotokoll.Beschaffenheitspruefung_Genehmigt
            End If
        End If

    End Sub

    ''' <summary>
    ''' Füllt das Objekt mit den Werten aus den Steuerlementen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub UpdateObjekt() Implements IRhewaEditingDialog.UpdateObjekt
        If DialogModus = enuDialogModus.normal Then objEichprozess.Bearbeitungsdatum = Date.Now
        If Not _objEichprotokoll Is Nothing Then
            If _objEichprotokoll.FK_Identifikationsdaten_Konformitaetsbewertungsverfahren Is Nothing Then
                _objEichprotokoll.FK_Identifikationsdaten_Konformitaetsbewertungsverfahren = GlobaleEnumeratoren.enuVerfahrensauswahl.nichts
            End If
            _objEichprotokoll.Beschaffenheitspruefung_Genehmigt = RadCheckBoxApprove.Checked
        End If

    End Sub


    ''' <summary>
    ''' Gültigkeit der Eingaben überprüfen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Function ValidateControls() As Boolean Implements IRhewaEditingDialog.ValidateControls
        Me.AbortSaving = False
        'prüfen ob alle Felder ausgefüllt sind

        If RadCheckBoxApprove.Checked = False Then
            RadCheckBoxApprove.Focus()
            AbortSaving = True
        End If

        'fehlermeldung anzeigen bei falscher validierung
        Dim result = Me.ShowValidationErrorBox(True)
        Return ProcessResult(result)
    End Function


    Protected Friend Overrides Sub Lokalisiere() Implements IRhewaEditingDialog.Lokalisiere
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uco_5Beschaffenheitspruefung))
        Lokalisierung(Me, resources)

        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_Beschaffenheitspruefung)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_Beschaffenheitspruefung
            Catch ex As Exception
            End Try
        End If
    End Sub



    Protected Friend Overrides Sub SaveObjekt() Implements IRhewaEditingDialog.SaveObjekt
        'neuen Context aufbauen
        Using Context As New Entities
            If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                If _objEichprotokoll Is Nothing Then           'neues Eichprotokoll anlegen und verfahrens Art zuweisen
                    _objEichprotokoll = Context.Eichprotokoll.Create
                    Context.Eichprotokoll.Add(_objEichprotokoll)
                Else
                    Dim dobjEichprotkoll As Eichprotokoll = Context.Eichprotokoll.FirstOrDefault(Function(value) value.ID = _objEichprotokoll.ID)
                    _objEichprotokoll = dobjEichprotkoll
                End If

                'Füllt das Objekt mit den Werten aus den Steuerlementen
                UpdateObjekt()
                'Speichern in Datenbank
                Context.SaveChanges()

                'zuweisen des eichprotokolls an den eichprozess

                'prüfen ob das Objekt anhand der ID gefunden werden kann
                Dim dbobjEichprozess As Eichprozess = (From a In Context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault
                If Not dbobjEichprozess Is Nothing Then
                    'lokale Variable mit Instanz aus DB überschreiben. Dies ist notwendig, damit das Entity Framework weiß, das ein Update vorgenommen werden muss.
                    objEichprozess = dbobjEichprozess
                    objEichprozess.FK_Eichprotokoll = _objEichprotokoll.ID 'zuweisen des eichprotokolls an den eichprozess
                    objEichprozess.Eichprotokoll = _objEichprotokoll



                    UpdateObjekt()

                    'Speichern in Datenbank
                    Context.SaveChanges()
                End If

            End If

        End Using
    End Sub

    Protected Friend Overrides Sub AktualisiereStatus() Implements IRhewaEditingDialog.AktualisiereStatus
        'neuen Context aufbauen
        Using Context As New Entities
            If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                Dim dbobjEichprozess As Eichprozess = (From a In Context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault
                If Not dbobjEichprozess Is Nothing Then
                    'lokale Variable mit Instanz aus DB überschreiben. Dies ist notwendig, damit das Entity Framework weiß, das ein Update vorgenommen werden muss.
                    objEichprozess = dbobjEichprozess

                    'neuen Status zuweisen
                    If AktuellerStatusDirty = False Then
                        ' Wenn der aktuelle Status kleiner ist als der für die AuswahlKonformitätsverfahren, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                        If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren Then
                            objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren
                        End If
                    ElseIf AktuellerStatusDirty = True Then
                        objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren
                        AktuellerStatusDirty = False
                    End If

                    UpdateObjekt()
                    'Speichern in Datenbank
                    Context.SaveChanges()
                End If
            End If
        End Using
    End Sub

    Protected Friend Overrides Function CheckDialogModus() As Boolean Implements IRhewaEditingDialog.CheckDialogModus
        If DialogModus = enuDialogModus.lesend Then
            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren Then
                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren
            End If
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If
        If DialogModus = enuDialogModus.korrigierend Then
            UpdateObjekt()
            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren Then
                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren
            End If
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If
        Return True
    End Function

    Protected Friend Overrides Sub OverwriteIstSoll() Implements IRhewaEditingDialog.OverwriteIstSoll
        RadCheckBoxApprove.Checked = True
    End Sub

    Protected Friend Overrides Sub Entsperrung() Implements IRhewaEditingDialog.Entsperrung
        'Hiermit wird ein lesender Vorgang wieder entsperrt.
        EnableControls(RadGroupBoxAufstellbedingungen)
        EnableControls(RadGroupBoxAuswerteGeraete)
        EnableControls(RadGroupBoxVerbindungselemente)
        EnableControls(RadGroupBoxWaegebruecke)
        EnableControls(RadGroupBoxWaegezellen)

        'ändern des Moduses
        DialogModus = enuDialogModus.korrigierend
        ParentFormular.DialogModus = FrmMainContainer.enuDialogModus.korrigierend
    End Sub



    Protected Friend Overrides Sub Versenden() Implements IRhewaEditingDialog.Versenden
        UpdateObjekt()
        'Erzeugen eines Server Objektes auf basis des aktuellen DS. Setzt es auf es ausserdem auf Fehlerhaft
        CloneAndSendServerObjekt()
    End Sub
#End Region

End Class