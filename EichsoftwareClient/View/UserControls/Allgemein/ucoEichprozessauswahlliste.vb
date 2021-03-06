Imports Telerik.WinControls.UI

Public Class ucoEichprozessauswahlliste

    Inherits ucoContent
    Implements IRhewaEditingDialog

#Region "Member Variables"
    Private WithEvents _ParentForm As FrmMainContainer
    Private objWebserviceFunctions As New clsWebserviceFunctions
    Private WithEvents objFTP As New clsFTP
    Private objPage As Telerik.WinControls.UI.RadPageViewPage
#End Region

#Region "Constructors"
    Sub New()
        MyBase.New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        _ParentForm = Nothing
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    Sub New(pParentForm As FrmMainContainer)
        MyBase.New(pParentForm, Nothing, Nothing)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        Me.RadGridViewRHEWAAlle.MasterTemplate.AutoExpandGroups = True
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _ParentForm = pParentForm
    End Sub
#End Region

#Region "Properties"
    Private ReadOnly Property VorgangsnummerGridClient As String
        Get
            Try
                If RadGridViewAuswahlliste.SelectedRows.Count > 0 Then
                    'prüfen ob das ausgewählte element eine REcord Row und kein Groupheader, Filter oder anderes ist
                    If TypeOf RadGridViewAuswahlliste.SelectedRows(0) Is Telerik.WinControls.UI.GridViewDataRowInfo Then
                        Return RadGridViewAuswahlliste.SelectedRows(0).Cells("Vorgangsnummer").Value
                    End If
                End If
                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property
    Private ReadOnly Property VorgangsnummerGridServer As String
        Get
            Try
                If RadGridViewRHEWAAlle.SelectedRows.Count > 0 Then
                    'prüfen ob das ausgewählte element eine REcord Row und kein Groupheader, Filter oder anderes ist
                    If TypeOf RadGridViewRHEWAAlle.SelectedRows(0) Is Telerik.WinControls.UI.GridViewDataRowInfo Then
                        Return RadGridViewRHEWAAlle.SelectedRows(0).Cells("Vorgangsnummer").Value
                    End If
                End If
                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End Get
    End Property
#End Region

#Region "Events"
    Private Sub ucoEichprozessauswahlliste_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.SuspendLayout()
        Me.Visible = False
        'laden des eingestellten Moants für den nächsten Programmstart
        LadeDateTimePickerRHEWAListe()
        LadeRoutine()
    End Sub

    ''' <summary>
    ''' aktivieren und deaktiveren der Schalter zum Genehmigen und Ablehnen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Private Sub RadGridView1_SelectionChanged(sender As System.Object, e As System.EventArgs) Handles RadGridViewRHEWAAlle.SelectionChanged
        TriggerEnabledStateGenehmigungsButtonns()
    End Sub

    ''' <summary>
    ''' Konfigurationsdialog anzeigen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadButtonEinstellungen_Click(sender As Object, e As EventArgs) Handles RadButtonEinstellungen.Click
        ZeigeKonfigurationsDialog()
    End Sub

    Private Sub RadButtonRefresh_Click(sender As Object, e As EventArgs) Handles RadButtonRefresh.Click
        LoadFromDatabase()

        'speichern des eingestellen Monats für den nächsten Programmstart
        SpeichereRhewaDatumsfilterEinstellung()
    End Sub

    ''' <summary>
    ''' Neuen Eichprozess anlegen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadButtonNeu_Click(sender As System.Object, e As System.EventArgs) Handles RadButtonClientNeu.Click
        OeffneNeuenEichprozess()
    End Sub

    ''' <summary>
    ''' Eichprozess bearbeiten
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadButtonBearbeiten_Click(sender As System.Object, e As System.EventArgs) Handles RadButtonClientBearbeiten.Click
        BearbeiteEichprozess()
    End Sub

    ''' <summary>
    ''' Eichprozess bearbeiten
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridViewAuswahlliste_DoubleClick(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles RadGridViewAuswahlliste.CellDoubleClick
        If RadGridViewAuswahlliste.SelectedRows.Count = 0 Then Exit Sub
        If Not TypeOf e.Row Is Telerik.WinControls.UI.GridViewDataRowInfo Then Exit Sub

        BearbeiteEichprozess()
    End Sub

    ''' <summary>
    ''' ausblenden bzw wieder einblenden des aktuellen Konformitätsbewertungsvorgangs
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Private Sub RadButtonAusblenden_Click(sender As System.Object, e As System.EventArgs) Handles RadButtonClientAusblenden.Click
        EichprozessAusblendenEinblenden()
    End Sub

    ''' <summary>
    ''' ein und ausblenden der als gelöscht markierten Elementen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Private Sub RadCheckBoxAusblendenGeloeschterDokumente_ToggleStateChanged(sender As System.Object, args As Telerik.WinControls.UI.StateChangedEventArgs) Handles RadCheckBoxAusblendenClientGeloeschterDokumente.ToggleStateChanged
        'laden der benötigten Liste mit nur den benötigten Spalten
        LoadFromDatabase()
    End Sub

    ''' <summary>
    ''' Lokale Eichprozesse für Gridview laden
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorkerLoadFromDatabase_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerLoadFromDatabase.DoWork
        'background worker result enthält anschließend alle lokalen Eichprozesse
        e.Result = clsDBFunctions.LadeLokaleEichprozessListe(RadCheckBoxAusblendenClientGeloeschterDokumente.Checked)
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub BackgroundWorkerLoadFromDatabase_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerLoadFromDatabase.RunWorkerCompleted
        LoadFromDatabaseCompleted(e)
    End Sub

    Private Sub BackgroundWorkerSyncAlles_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerSyncAlles.DoWork
        e = SyncAlles(e)
    End Sub



    Private Sub BackgroundWorkerSyncAlles_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorkerSyncAlles.ProgressChanged
        RadProgressBar1.Value1 = e.ProgressPercentage
        Me.Enabled = False
        Me.Parent.Enabled = False
    End Sub

    Private Sub BackgroundWorkerSyncAlles_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerSyncAlles.RunWorkerCompleted
        RadProgressBar1.Value1 = 100
        If Not e.Result Is Nothing Then
            MessageBox.Show(e.Result)
        End If

        FlowLayoutPanel2.Visible = False
        RadProgressBar1.Visible = False
        Me.Enabled = True
        Me.Parent.Enabled = True

        'aktualisieren des Grids
        LoadFromDatabase()
    End Sub

    ''' <summary>
    ''' Methode welche sich mit dem Webservice verbinduet und nach aktualisierungen für WZ, AWGs und eigenen Eichungen guckt
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RadButtonClientUpdateDatabase_Click(sender As System.Object, e As System.EventArgs) Handles RadButtonClientUpdateDatabase.Click
        VerbindeMitWebServiceUndAktualisiere()
    End Sub


    Private Sub RadButtonProtokollAblegen_Click(sender As Object, e As EventArgs) Handles RadButtonProtokollAblegen.Click
        VorgangAblegen()
    End Sub




#Region "Rhewa Funktionen"

    Private Sub RadButtonNeuStandardwaage_Click(sender As Object, e As EventArgs) Handles RadButtonNeuStandardwaage.Click
        Dim f As New FrmAuswahlStandardwaage
        f.Show()
        AddHandler f.FormClosed, AddressOf LoadFromDatabase
    End Sub

    'limitieren der Min MAx Werte des Datetime Pcikers
    Private Sub RadDateTimePickerFilterMonatLadeAlleEichprozesseVon_ValueChanged(sender As Object, e As EventArgs) Handles RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.ValueChanged
        RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.MinDate = RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.Value
    End Sub

    Private Sub RadDateTimePickerFilterMonatLadeAlleEichprozesseBis_ValueChanged(sender As Object, e As EventArgs) Handles RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.ValueChanged
        RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.MaxDate = RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.Value
    End Sub
    ''' <summary>
    ''' Lade eichprozessliste vom Server
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorkerLoadFromDatabaseRHEWA_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerLoadFromDatabaseRHEWA.DoWork
        'background worker result enthält anschließend alle serverseitigen Eichprozesse
        If RadCheckBoxLadeAlleEichprozesse.Checked Then
            e.Result = clsWebserviceFunctions.GetServerEichprotokollListe()
        Else
            e.Result = clsWebserviceFunctions.GetServerEichprotokollListe(RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.Value.Year, RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.Value.Month, RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.Value.Year, RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.Value.Month)
        End If
    End Sub

    ''' <summary>
    ''' Databinding und Formatierung
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorkerLoadFromDatabaseRHEWA_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs, Optional invalid As String = "Invalid") Handles BackgroundWorkerLoadFromDatabaseRHEWA.RunWorkerCompleted
        LoadFromDatabaseRHEWACompleted(e)
    End Sub


    '''' <summary>
    '''' Einblenden eines Anhang Symbols für Anträge mit Dateianhang
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    '''' <remarks></remarks>
    'Private Sub RadGridViewRHEWAAlle_CellFormatting(sender As Object, e As Telerik.WinControls.UI.CellFormattingEventArgs) Handles RadGridViewRHEWAAlle.CellFormatting
    '    e = FormatCellRHEWA(e)

    'End Sub


    'Private Sub RadGridView_ViewCellFormatting(sender As Object, e As CellFormattingEventArgs) Handles RadGridViewAuswahlliste.ViewCellFormatting, RadGridViewRHEWAAlle.ViewCellFormatting 'Handles RadGridViewAuswahlliste.ViewCellFormatting, RadGridViewRHEWAAlle.ViewCellFormatting
    '    If (TypeOf e.CellElement Is GridHeaderCellElement) Then
    '        e.CellElement.TextWrap = True
    '    End If

    '    If sender.Equals(RadGridViewRHEWAAlle) Then
    '        e = FormatCellRHEWA(e)

    '    End If
    'End Sub

    ''' <summary>
    ''' öffnen des Dateianhangs
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadGridViewRHEWAAlle_CellDoubleClick(sender As Object, e As Telerik.WinControls.UI.GridViewCellEventArgs) Handles RadGridViewRHEWAAlle.CellDoubleClick
        If RadGridViewRHEWAAlle.SelectedRows.Count = 0 Then Exit Sub
        If Not TypeOf e.Row Is Telerik.WinControls.UI.GridViewDataRowInfo Then Exit Sub

        Try
            If (RadGridViewRHEWAAlle.Columns(e.ColumnIndex).Name = "Anhang") Then
                Dim AnhangPfad = e.Row.Cells("AnhangPfad").Value
                Dim Vorgangsnummer As String

                If AnhangPfad.Trim.Equals("") = False Then
                    Vorgangsnummer = e.Row.Cells("Vorgangsnummer").Value
                    Try
                        OeffneDateiVonFTP(AnhangPfad, Vorgangsnummer)
                    Catch ex As Exception
                        Debug.WriteLine(ex.ToString)
                    End Try
                Else
                    ZeigeServerEichprozess()
                End If
            Else
                ZeigeServerEichprozess()
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try
    End Sub

    Private Sub BackgroundWorkerDownloadFromFTP_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerDownloadFromFTP.DoWork
        Dim vorgangsnummer As String = e.Argument
        e.Result = clsWebserviceFunctions.GetFTPFile(vorgangsnummer, objFTP, Me.BackgroundWorkerDownloadFromFTP)
    End Sub

#Region "Events FTP"

    Private Sub BackgroundWorkerDownloadFromFTP_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerDownloadFromFTP.RunWorkerCompleted
        Me.Enabled = True
        Me.RadProgressBar.Visible = False
        Me.RadProgressBar.Value1 = 0
        RadProgressBar.Text = ""

        Dim filepath As String = e.Result
        If filepath.Equals("") = False Then
            Dim proc As Process = New Process()
            proc.StartInfo.FileName = filepath
            proc.StartInfo.UseShellExecute = True
            proc.Start()
        Else
            MessageBox.Show("Konnte Datei am Pfad nicht finden. Sie konnte auch nicht heruntergeladen werden.")
        End If
    End Sub

    Private Sub BackgroundWorkerDownloadFromFTP_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorkerDownloadFromFTP.ProgressChanged
        Try
            If e.ProgressPercentage = 0 Then
                RadProgressBar.Value1 = e.UserState
                RadProgressBar.Text = CInt(CInt(e.UserState) / 1024) & " KB/ " & CInt(CInt(RadProgressBar.Maximum) / 1024) & " KB"
                Me.Refresh()
            Else
                RadProgressBar.Maximum = e.ProgressPercentage
            End If
        Catch ex As Exception
            Debug.WriteLine(ex.ToString)
        End Try
    End Sub

    Private Sub objFTP_ReportFTPProgress(Progress As Integer) Handles objFTP.ReportFTPProgress
        Try
            BackgroundWorkerDownloadFromFTP.ReportProgress(0, Progress)
        Catch ex As Exception
        End Try
    End Sub

#End Region
    ''' <summary>
    ''' Eichprozess genehmigen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadButtonEichprozessGenehmigen_Click(sender As System.Object, e As System.EventArgs) Handles RadButtonEichprozessGenehmigenRHEWA.Click
        GenehnmigeGewaehltenVorgang()
    End Sub

    ''' <summary>
    ''' Eichprozess ablehnen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RadButtonEichprozessAblehnen_click(sender As System.Object, e As System.EventArgs) Handles RadButtonEichprozessAblehnenRHEWA.Click
        AblehnenGewaehlterVorgang()
    End Sub



    ''' <summary>
    ''' Eichprozess bearbeiten
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RadButtonBearbeitenRHEWA_Click(sender As System.Object, e As System.EventArgs) Handles RadButtonEichungAnsehenRHEWA.Click
        ZeigeServerEichprozess()
    End Sub

#End Region
#End Region

#Region "Methoden"

    Protected Friend Sub LadeRoutine()
        SetzeUeberschrift()
        'laden des Grid Layouts aus User Settings
        AktuellerBenutzer.LadeGridLayout(Me)

        If Not ParentFormular Is Nothing Then
            Try
                'benutzer auswahl einblenden
                Dim uco As New ucoBenutzerwechsel(Me) With {
                    .objLizenz = AktuellerBenutzer.Instance.Lizenz,
                    .Dock = DockStyle.Fill
                }
                ParentFormular.RadScrollablePanelTrafficLightBreadcrumb.Controls.Add(uco)
                ParentFormular.objUCOBenutzerwechsel = uco
            Catch ex As Exception

            End Try
        End If

        RadButtonEinstellungen.Visible = True
        RadButtonEinstellungen.Enabled = True

        ' nur mit RHEWA Lizenz die Seite mit allen Vorgängen anzeigen
        GetControllsRhewaLizenz()

        'für den Fall das die Anwendung gerade erst installiert wurde, oder die einstellung zur Synchronisierung geändert wurde, sollen alle Eichungen vom RHEWA Server geholt werden, die einmal angelegt wurden
        If Not AktuellerBenutzer.Instance.Lizenz Is Nothing And AktuellerBenutzer.Instance.HoleAlleeigenenEichungenVomServer = True Then
            VerbindeMitWebserviceUndHoleAlles()
        Else
            LoadFromDatabase()
        End If
    End Sub

    ''' <summary>
    ''' nur mit RHEWA Lizenz die Seite mit allen Vorgängen anzeigen
    ''' </summary>
    Private Sub GetControllsRhewaLizenz()
        'Tabelle mit Server Items ausblenden
        If AktuellerBenutzer.Instance.Lizenz.RHEWALizenz = False Then
            Try
                objPage = RadPageView1.Pages(1) 'falls der benutzer gewechselt wird, und so zwischen mit RHEWA Lizenz und ohne RHEWA Lizenz gewechselt wird
                RadPageView1.Pages.RemoveAt(1)
                RadPageView1.Pages(0).Text = ""
            Catch ex As ArgumentOutOfRangeException
                'ignorieren
            End Try
            'großer button
            RadButtonClientNeu.Width = 192
            RadButtonClientNeu.Height = 63
            RadButtonClientNeu.TextAlignment = ContentAlignment.MiddleCenter
            RadButtonNeuStandardwaage.Visible = False
        Else
            RadButtonClientNeu.Width = 94
            RadButtonClientNeu.Height = 63
            RadButtonClientNeu.TextAlignment = ContentAlignment.MiddleRight
            RadButtonNeuStandardwaage.Visible = True

            If RadPageView1.Pages.Count = 1 Then
                RadPageView1.Pages(0).Text = "Eigene"

                RadPageView1.Pages.Add(objPage) 'wieder hinzufügen der Page falls vorher entfernt
                RadPageView1.Pages(1).Text = "Alle"
            End If

        End If
    End Sub



    ''' <summary>
    ''' Ein und ausblenden sowie lokalisierung des Grids
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FormatTable()
        Try
            'ausblenden von internen spalten
            RadGridViewAuswahlliste.Columns("ID").IsVisible = False
            RadGridViewAuswahlliste.Columns("Ausgeblendet").IsVisible = False
            RadGridViewAuswahlliste.Columns("Vorgangsnummer").IsVisible = False

            'übersetzen der Spaltenköpfe
            RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").HeaderText = My.Resources.GlobaleLokalisierung.Bearbeitungsstatus
            RadGridViewAuswahlliste.Columns("Lookup_Waegezelle").HeaderText = My.Resources.GlobaleLokalisierung.Waegezelle
            RadGridViewAuswahlliste.Columns("Lookup_Auswertegeraet").HeaderText = My.Resources.GlobaleLokalisierung.AuswerteGeraet
            RadGridViewAuswahlliste.Columns("Lookup_Waagentyp").HeaderText = My.Resources.GlobaleLokalisierung.Waagentyp
            RadGridViewAuswahlliste.Columns("Lookup_Waagenart").HeaderText = My.Resources.GlobaleLokalisierung.Waagenart
            RadGridViewAuswahlliste.Columns("Fabriknummer").HeaderText = My.Resources.GlobaleLokalisierung.Fabriknummer

            RadGridViewAuswahlliste.Columns("Identifikationsdaten_Datum").HeaderText = My.Resources.GlobaleLokalisierung.HKBDatum

            RadGridViewAuswahlliste.Columns("Bearbeitungsdatum").HeaderText = My.Resources.GlobaleLokalisierung.Bearbeitungsdatum
            RadGridViewAuswahlliste.Columns("Bemerkung").HeaderText = My.Resources.GlobaleLokalisierung.Bemerkung

            RadGridViewAuswahlliste.ShowFilteringRow = True

            RadGridViewRHEWAAlle.ShowFilteringRow = True

            'spaltengrößen anpassen (so viel platz wie möglich nehmen)
            RadGridViewAuswahlliste.BestFitColumns()
            RadGridViewAuswahlliste.EnableAlternatingRowColor = True
            RadGridViewAuswahlliste.SelectionMode = Telerik.WinControls.UI.GridViewSelectionMode.FullRowSelect
            RadGridViewAuswahlliste.AutoExpandGroups = True
            Try
                'bedingte Formatierung - nur hinzufügen, falls sie nicht schon existiert
                Dim o = From i As Telerik.WinControls.UI.ConditionalFormattingObject In RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList
                        Where i.Name = "Fehlerhaft"

                If o.Count = 0 Or RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Count = 4 Then 'wurde um 2 spalten ergänzt von 4 auf 6
                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Clear()
                    Dim objCondition As New Telerik.WinControls.UI.ConditionalFormattingObject("Fehlerhaft", Telerik.WinControls.UI.ConditionTypes.Equal, "Fehlerhaft", "", True)
                    Dim objCondition2 As New Telerik.WinControls.UI.ConditionalFormattingObject("invalid", Telerik.WinControls.UI.ConditionTypes.Equal, "Invalid", "", True)
                    Dim objCondition3 As New Telerik.WinControls.UI.ConditionalFormattingObject("invalidPl", Telerik.WinControls.UI.ConditionTypes.Equal, "Błędne", "", True)

                    objCondition.RowBackColor = Color.FromArgb(254, 120, 110)
                    objCondition2.RowBackColor = Color.FromArgb(254, 120, 110)
                    objCondition3.RowBackColor = Color.FromArgb(254, 120, 110)

                    Dim objCondition4 As New Telerik.WinControls.UI.ConditionalFormattingObject("Genehmigt", Telerik.WinControls.UI.ConditionTypes.Equal, "Genehmigt", "", True)
                    Dim objCondition5 As New Telerik.WinControls.UI.ConditionalFormattingObject("Valid", Telerik.WinControls.UI.ConditionTypes.Equal, "Valid", "", True)
                    Dim objCondition6 As New Telerik.WinControls.UI.ConditionalFormattingObject("Zatwierdzono", Telerik.WinControls.UI.ConditionTypes.Equal, "Zatwierdzono", "", True)

                    objCondition4.RowBackColor = Color.FromArgb(204, 255, 153)
                    objCondition5.RowBackColor = Color.FromArgb(204, 255, 153)
                    objCondition6.RowBackColor = Color.FromArgb(204, 255, 153)

                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition)
                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition2)
                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition3)

                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition4)
                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition5)
                    RadGridViewAuswahlliste.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition6)

                    Dim descriptor As New Telerik.WinControls.Data.GroupDescriptor()
                    descriptor.GroupNames.Add("Bearbeitungsstatus", System.ComponentModel.ListSortDirection.Ascending)
                    Me.RadGridViewAuswahlliste.GroupDescriptors.Add(descriptor)
                End If

            Catch ex As Exception
                AktuellerBenutzer.ResetGridSettings()
            End Try
        Catch ex As Exception
            AktuellerBenutzer.ResetGridSettings()
        End Try
    End Sub


    ''' <summary>
    ''' Konfigurationsdialog anzeigen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ZeigeKonfigurationsDialog()
        Dim f As New FrmEinstellungen
        f.ShowDialog()
        If f.DialogResult = DialogResult.OK Then
            'neu aktualisierung der Eichungen
            VerbindeMitWebserviceUndHoleAlles()

        ElseIf f.DialogResult = DialogResult.Retry Then
            RadGridViewAuswahlliste.DataSource = Nothing

            RadGridViewRHEWAAlle.DataSource = Nothing
            'laden des Grid Layouts aus User Settings
            AktuellerBenutzer.LadeGridLayout(Me)
            'aktualisieren des Grids
            LoadFromDatabase()

        End If
    End Sub


    Private Sub TriggerEnabledStateGenehmigungsButtonns()
        Try
            RadButtonEichprozessAblehnenRHEWA.Enabled = False
            RadButtonEichprozessGenehmigenRHEWA.Enabled = False

            If RadGridViewRHEWAAlle.SelectedRows.Count > 0 Then
                'prüfen ob das ausgewählte element eine REcord Row und kein Groupheader, Filter oder anderes ist
                If TypeOf RadGridViewRHEWAAlle.SelectedRows(0) Is Telerik.WinControls.UI.GridViewDataRowInfo Then
                    Dim SelectedStatus As String = "" 'Variable zum Speichern des BearbeitungsStatuses des aktuellen Prozesses
                    SelectedStatus = RadGridViewRHEWAAlle.SelectedRows(0).Cells("Bearbeitungsstatus").Value

                    If SelectedStatus.ToLower = "genehmigt" Then
                        RadButtonEichprozessAblehnenRHEWA.Enabled = False
                        RadButtonEichprozessGenehmigenRHEWA.Enabled = False
                    ElseIf SelectedStatus.ToLower = "fehlerhaft" Then
                        RadButtonEichprozessAblehnenRHEWA.Enabled = False
                        RadButtonEichprozessGenehmigenRHEWA.Enabled = False
                    ElseIf SelectedStatus.ToLower = "wartet auf bearbeitung" Then
                        RadButtonEichprozessAblehnenRHEWA.Enabled = True
                        RadButtonEichprozessGenehmigenRHEWA.Enabled = True
                    Else
                        RadButtonEichprozessAblehnenRHEWA.Enabled = False
                        RadButtonEichprozessGenehmigenRHEWA.Enabled = False
                    End If
                End If
            End If
        Catch ex As Exception
            RadButtonEichprozessAblehnenRHEWA.Enabled = False
            RadButtonEichprozessGenehmigenRHEWA.Enabled = False
        End Try
    End Sub


    Private Sub LoadFromDatabaseCompleted(e As System.ComponentModel.RunWorkerCompletedEventArgs)
        'zuweisen der Ergebnismenge als Datenquelle für das Grid
        Dim index As Integer = 0
        Dim groupIndex As Integer = 0
        Try
            If RadGridViewAuswahlliste.SelectedRows.Count > 0 Then
                index = RadGridViewAuswahlliste.SelectedRows(0).Index
                groupIndex = RadGridViewAuswahlliste.SelectedRows(0).Group.GroupRow.Index
            End If

        Catch ex As Exception
        End Try
        RadGridViewAuswahlliste.DataSource = e.Result

        'Spalten ein und ausblenden und formatieren
        Try
            FormatTable()
        Catch ex As Exception
            AktuellerBenutzer.ResetGridSettings()
            FormatTable()
        End Try

        Try
            RadGridViewAuswahlliste.Groups(groupIndex).GroupRow.ChildRows(index).IsSelected = True
            RadGridViewAuswahlliste.Groups(groupIndex).GroupRow.ChildRows(index).IsCurrent = True
            RadGridViewAuswahlliste.TableElement.ScrollToRow(RadGridViewAuswahlliste.SelectedRows(0))

        Catch ex As Exception
            Try
                RadGridViewAuswahlliste.Groups(groupIndex).GroupRow.ChildRows(index - 1).IsSelected = True
                RadGridViewAuswahlliste.Groups(groupIndex).GroupRow.ChildRows(index - 1).IsCurrent = True
                RadGridViewAuswahlliste.TableElement.ScrollToRow(RadGridViewAuswahlliste.SelectedRows(0))

            Catch ex2 As Exception

            End Try
        End Try
        Me.Enabled = True
        Me.ResumeLayout()
        Me.Refresh()
        RadGridViewAuswahlliste.Refresh()
        RadGridViewAuswahlliste.MasterTemplate.Refresh()
        RadGridViewAuswahlliste.MasterView.Refresh()

        Me.Visible = True


    End Sub
    Private Function SyncAlles(e As System.ComponentModel.DoWorkEventArgs) As System.ComponentModel.DoWorkEventArgs
        Dim bolSyncData As Boolean = True 'Wert der genutzt wird um ggfs die Synchrosierung abzubrechen, falls ein Benutzer noch ungesendete Konformitätsbewertungsvorgänge hat
        If clsWebserviceFunctions.TesteVerbindung() Then
            BackgroundWorkerSyncAlles.ReportProgress(10)
            'variablen zur Ausgabe ob es änderungen gibt:
            Dim bolNeuStammdaten As Boolean = False
            Dim bolNeuWZ As Boolean = False
            Dim bolNeuAWG As Boolean = False
            Dim bolNeuGenehmigung As Boolean = False

            'prüfen ob noch nicht abgeschickte Eichungen vorlieren. Wenn ja Hinweismeldung und Abbruchmöglichkeit für Benutzer
            If clsDBFunctions.PruefeAufUngesendeteEichungen() = True Then
                If MessageBox.Show(My.Resources.GlobaleLokalisierung.Warnung_EichungenWerdenGeloescht, My.Resources.GlobaleLokalisierung.Frage, MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    bolSyncData = True
                Else
                    AktuellerBenutzer.Instance.HoleAlleeigenenEichungenVomServer = False
                    AktuellerBenutzer.SaveSettings()
                    bolSyncData = False
                End If
            End If
            BackgroundWorkerSyncAlles.ReportProgress(20)

            If bolSyncData Then

                'für den Fall das die Anwendung gerade erst installiert wurde, oder die einstellung zur Synchronisierung geändert wurde, sollen alle Eichungen vom RHEWA Server geholt werden, die einmal angelegt wurden
                If clsDBFunctions.LoescheLokaleDatenbank() Then
                    BackgroundWorkerSyncAlles.ReportProgress(30)

                    AktuellerBenutzer.Instance.LetztesUpdate = "01.01.2000"
                    AktuellerBenutzer.SaveSettings()
                    'neue Stammdaten zum Benutzer holen
                    clsWebserviceFunctions.GetBenutzerStammdaten(bolNeuStammdaten)
                    BackgroundWorkerSyncAlles.ReportProgress(40)

                    'hole alle WZ
                    clsWebserviceFunctions.GetNeueWZ(bolNeuWZ)
                    BackgroundWorkerSyncAlles.ReportProgress(50)

                    'prüfen ob es neue AWG gibt
                    clsWebserviceFunctions.GetNeuesAWG(bolNeuAWG)
                    BackgroundWorkerSyncAlles.ReportProgress(60)

                    'prüfen ob Eichprozesse die versendet wurden genehmigt oder abgelehnt wurden
                    clsWebserviceFunctions.GetGenehmigungsstatus(bolNeuGenehmigung)
                    BackgroundWorkerSyncAlles.ReportProgress(70)

                    AktuellerBenutzer.Instance.LetztesUpdate = Date.Now
                    AktuellerBenutzer.SaveSettings()
                    BackgroundWorkerSyncAlles.ReportProgress(80)

                    clsWebserviceFunctions.GetEichprotokolleVomServer()
                    BackgroundWorkerSyncAlles.ReportProgress(90)

                    AktuellerBenutzer.Instance.HoleAlleeigenenEichungenVomServer = False
                    AktuellerBenutzer.SaveSettings()

                    Dim returnMessage As String = My.Resources.GlobaleLokalisierung.Aktualisierung_Erfolgreich
                    If bolNeuWZ Then
                        returnMessage += vbNewLine & vbNewLine & My.Resources.GlobaleLokalisierung.Aktualisierung_NeuWZ & " "
                    End If
                    If bolNeuAWG Then
                        returnMessage += vbNewLine & vbNewLine & My.Resources.GlobaleLokalisierung.Aktualisierung_NeuAWG & " "
                    End If

                    If bolNeuGenehmigung Then
                        returnMessage += vbNewLine & vbNewLine & My.Resources.GlobaleLokalisierung.Aktualisierung_NeuEichung & " "
                    End If

                    If bolNeuStammdaten Then

                    End If
                End If
            End If
        Else
            e.Result = My.Resources.GlobaleLokalisierung.KeineVerbindung
        End If

        BackgroundWorkerSyncAlles.ReportProgress(100)
        Return e
    End Function



#Region "Datetime Picker"


    ''' <summary>
    '''  laden des eingestellten Moants für den nächsten Programmstart
    ''' </summary>
    Private Sub LadeDateTimePickerRHEWAListe()
        Try
            If My.Settings.RHEWAFilterMonatBis.Equals(New Date) Then
                My.Settings.RHEWAFilterMonatBis = New Date(Now.Year, Now.Month, 1).AddMonths(1).AddDays(-1)
                My.Settings.Save()
            End If
            If My.Settings.RHEWAFilterMonatVon.Equals(New Date) Then
                My.Settings.RHEWAFilterMonatVon = New Date(Now.Year, Now.Month, 1).AddMonths(-1)
                My.Settings.Save()

            End If
            RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.Value = My.Settings.RHEWAFilterMonatBis

            RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.Value = My.Settings.RHEWAFilterMonatVon
        Catch ex As Exception
            RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.Value = Date.Now.Date
        End Try
    End Sub


    ''' <summary>
    '''  speichern des eingestellen Monats für den nächsten Programmstart
    ''' </summary>
    Private Sub SpeichereRhewaDatumsfilterEinstellung()
        My.Settings.RHEWAFilterMonatVon = RadDateTimePickerFilterMonatLadeAlleEichprozesseVon.Value
        My.Settings.RHEWAFilterMonatBis = RadDateTimePickerFilterMonatLadeAlleEichprozesseBis.Value

        My.Settings.Save()
    End Sub

#End Region


#Region "Interface Methoden"
    Protected Friend Overrides Sub SetzeUeberschrift() Implements IRhewaEditingDialog.SetzeUeberschrift
        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_Hauptmenue)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_Hauptmenue

            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Friend Overrides Sub Lokalisiere() Implements IRhewaEditingDialog.Lokalisiere
        'übersetzen und formatierung der Tabelle
        If AktuellerBenutzer.Instance.AktuelleSprache = "de" Then
            Telerik.WinControls.UI.Localization.RadGridLocalizationProvider.CurrentProvider = New telerikgridlocalizerDE
        ElseIf AktuellerBenutzer.Instance.AktuelleSprache = "pl" Then
            Telerik.WinControls.UI.Localization.RadGridLocalizationProvider.CurrentProvider = New telerikgridlocalizerPL
        Else
            Telerik.WinControls.UI.Localization.RadGridLocalizationProvider.CurrentProvider = New telerikgridlocalizerEN
        End If
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucoEichprozessauswahlliste))
        Lokalisierung(Me, resources)

        'Hilfetext setzen
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_Auswahlliste)
        'Überschrift setzen
        ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_Hauptmenue

        LoadFromDatabase()
    End Sub

    ''' <summary>
    ''' Initiert background threads die aus lokaler Client DB und Server Webservice die vorhandenen Eichungen abrufen
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend Overrides Sub LoadFromDatabase() Implements IRhewaEditingDialog.LoadFromDatabase
        Me.Enabled = False

        If Not BackgroundWorkerLoadFromDatabase.IsBusy Then
            BackgroundWorkerLoadFromDatabase.RunWorkerAsync()
        End If

        If AktuellerBenutzer.Instance.Lizenz.RHEWALizenz Then
            If Not BackgroundWorkerLoadFromDatabaseRHEWA.IsBusy Then
                BackgroundWorkerLoadFromDatabaseRHEWA.RunWorkerAsync()
            End If
        End If
    End Sub
#End Region


#Region "RHEWA Methoden"
    ''' <summary>
    ''' Neuen Eichprozess anlegen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OeffneNeuenEichprozess()
        Dim objEichprozess As Eichprozess = clsDBFunctions.ErzeugeNeuenEichprozess
        If Not objEichprozess Is Nothing Then
            'anzeigen des Dialogs zur Bearbeitung der Eichung
            Dim f As New FrmMainContainer(objEichprozess)
            f.Show()
            AddHandler f.FormClosed, AddressOf LoadFromDatabase
        End If
    End Sub

    ''' <summary>
    ''' Eichprozess bearbeiten
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BearbeiteEichprozess()
        If Not Me.VorgangsnummerGridClient.Equals("") Then
            Dim objEichprozess = clsDBFunctions.HoleVorhandenenEichprozess(VorgangsnummerGridClient)
            If Not objEichprozess Is Nothing Then
                If objEichprozess.FK_Bearbeitungsstatus = 4 Or objEichprozess.FK_Bearbeitungsstatus = 2 Then 'nur wenn neu oder fehlerhaft darf eine Änderung vorgenommen werrden
                    'anzeigen des Dialogs zur Bearbeitung der Eichung
                    Dim f As New FrmMainContainer(objEichprozess)
                    f.Show()
                    AddHandler f.FormClosed, AddressOf LoadFromDatabase
                Else
                    'es gibt ihn schon und er ist bereits abgeschickt und genehmigt. nur lesend öffnen


                    objEichprozess = clsDBFunctions.HoleNachschlageListenFuerEichprozess(objEichprozess)
                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Versenden 'grüne ampel auf versenden setzen, damit direkt alles betrachtet werden kann
                    Dim f As New FrmMainContainer(objEichprozess, FrmMainContainer.enuDialogModus.lesend)
                    f.Show()
                    AddHandler f.FormClosed, AddressOf LoadFromDatabase
                End If

                ''nach dem schließen des Dialogs aktualisieren
                'LoadFromDatabase()
            End If
        End If
    End Sub



    ''' <summary>
    ''' ausblenden bzw wieder einblenden des aktuellen eichvorgangs
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Private Sub EichprozessAusblendenEinblenden()
        If Not Me.VorgangsnummerGridClient.Equals("") Then

            clsDBFunctions.BlendeEichprozessAus(VorgangsnummerGridClient)
            'neu laden der Liste
            LoadFromDatabase()
        End If
    End Sub


    Private Sub FormatTableRhewa(index As Integer, groupIndex As Integer)
        Try
            Try
                'Spalten ein und ausblenden und formatieren
                'ausblenden von internen spalten
                RadGridViewRHEWAAlle.Columns("ID").IsVisible = False
                RadGridViewRHEWAAlle.Columns("Vorgangsnummer").IsVisible = False
                RadGridViewRHEWAAlle.Columns("Gesperrtdurch").HeaderText = "Gesperrt durch"
                RadGridViewRHEWAAlle.Columns("AnhangPfad").IsVisible = False
                'RadGridViewRHEWAAlle.Columns("AnhangPfad").HeaderText = "Anhang"
                RadGridViewRHEWAAlle.Columns("Eichbevollmaechtigter").HeaderText = "Konformitätsbewertungsbevollmächtigter"
                RadGridViewRHEWAAlle.Columns("NeueWZ").HeaderText = "Neue WZ"
                RadGridViewRHEWAAlle.Columns("Gesperrtdurch").HeaderText = "Gesperrt durch"
                RadGridViewRHEWAAlle.Columns("HKBDatum").HeaderText = "HKB Datum"
                RadGridViewRHEWAAlle.Columns("HKBDatum").FormatString = "{0:dd.MM.yyyy}"

            Catch ex As Exception
            End Try

            'Column für Anhang Element hinzufügen, wenn nicht vorhanden
            AddImageColumn()

            RadGridViewRHEWAAlle.BestFitColumns()
            RadGridViewRHEWAAlle.EnableAlternatingRowColor = False
            RadGridViewRHEWAAlle.ShowNoDataText = False
            RadGridViewRHEWAAlle.AutoSizeRows = True

            Try
                'bedingte Formatierung - nur hinzufügen, falls sie nicht schon existiert
                Dim o = From i As Telerik.WinControls.UI.ConditionalFormattingObject In RadGridViewRHEWAAlle.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList
                        Where i.Name = "Fehlerhaft"

                If o.Count = 0 Then
                    Dim objCondition As New ConditionalFormattingObject("Fehlerhaft", Telerik.WinControls.UI.ConditionTypes.Equal, "Fehlerhaft", "", True) With {
                        .RowBackColor = Color.FromArgb(254, 120, 110)
                    }

                    Dim objCondition2 As New ConditionalFormattingObject("Genehmigt", Telerik.WinControls.UI.ConditionTypes.Equal, "Genehmigt", "", True) With {
                        .RowBackColor = Color.FromArgb(204, 255, 153)
                    }

                    RadGridViewRHEWAAlle.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition)
                    RadGridViewRHEWAAlle.Columns("Bearbeitungsstatus").ConditionalFormattingObjectList.Add(objCondition2)

                    Dim descriptor As New Telerik.WinControls.Data.GroupDescriptor()
                    descriptor.GroupNames.Add("Bearbeitungsstatus", System.ComponentModel.ListSortDirection.Ascending)
                    Me.RadGridViewRHEWAAlle.GroupDescriptors.Add(descriptor)
                End If

            Catch ex As Exception

            End Try

            Try
                RadGridViewRHEWAAlle.Groups(groupIndex).GroupRow.ChildRows(index).IsSelected = True
                RadGridViewRHEWAAlle.Groups(groupIndex).GroupRow.ChildRows(index).IsCurrent = True

            Catch ex As Exception
                Try
                    RadGridViewRHEWAAlle.Groups(groupIndex).GroupRow.ChildRows(index - 1).IsSelected = True
                    RadGridViewRHEWAAlle.Groups(groupIndex).GroupRow.ChildRows(index - 1).IsCurrent = True

                Catch ex2 As Exception

                End Try
            End Try

            RadGridViewRHEWAAlle.EnableFiltering = True
            RadGridViewRHEWAAlle.MasterTemplate.ShowHeaderCellButtons = True
            RadGridViewRHEWAAlle.MasterTemplate.ShowFilteringRow = True

            Me.Enabled = True
            Try
                If RadGridViewRHEWAAlle.SelectedRows.Count > 0 Then RadGridViewRHEWAAlle.TableElement.ScrollToRow(RadGridViewRHEWAAlle.SelectedRows(0))
            Catch ex As Exception
            End Try

            Me.ResumeLayout()
            Me.Visible = True
        Catch ex As Exception


        End Try
    End Sub

    Private Sub AddImageColumn()
        Dim found As Boolean = False
        For Each column In RadGridViewRHEWAAlle.Columns
            If column.Name.Equals("Anhang") Then
                found = True
                Exit For
            End If
        Next
        If found = False Then

            Dim GridViewImageCol As GridViewImageColumn = New GridViewImageColumn()
            GridViewImageCol.Name = "Anhang"
            GridViewImageCol.HeaderText = "Anhang"
            GridViewImageCol.IsPinned = True
            GridViewImageCol.PinPosition = PinnedColumnPosition.Left

            RadGridViewRHEWAAlle.Columns.Add(GridViewImageCol)
        End If
    End Sub

    ''' <summary>
    ''' Öffnet einen Eichprozess vom Server zum lesen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ZeigeServerEichprozess()
        If Not Me.VorgangsnummerGridServer.Equals("") Then
            Try
                Dim objClientEichprozess = clsWebserviceFunctions.GetReadonlyEichprozess(VorgangsnummerGridServer)
                'anzeigen des Dialogs zur Bearbeitung der Eichung
                If Not objClientEichprozess Is Nothing Then

                    ' lese modus, dann soll beliebig im Eichprozess hin und her gewechselt werden können => Eichprozessstatus auf Versenden setzen
                    objClientEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.Versenden

                    Dim f As New FrmMainContainer(objClientEichprozess, FrmMainContainer.enuDialogModus.lesend)
                    f.Show()
                    AddHandler f.FormClosed, AddressOf LoadFromDatabase
                End If

            Catch ex As Exception
                MessageBox.Show("Der Webservice ist gerade nicht erreichbar, versuchen Sie es später erneut")
            End Try
        End If
    End Sub


    Private Sub LoadFromDatabaseRHEWACompleted(e As System.ComponentModel.RunWorkerCompletedEventArgs)
        'zuweisen der Ergebnismenge als Datenquelle für das Grid
        Dim index As Integer = 0
        Dim groupIndex As Integer = 0
        Try
            If RadGridViewRHEWAAlle.SelectedRows.Count > 0 Then
                index = RadGridViewRHEWAAlle.SelectedRows(0).Index
                groupIndex = RadGridViewRHEWAAlle.SelectedRows(0).Group.GroupRow.Index
            End If
        Catch ex As Exception
        End Try

        RadGridViewRHEWAAlle.DataSource = e.Result
        FormatTableRhewa(index, groupIndex)

        FillImageColumn()
    End Sub

    Private Sub FillImageColumn()
        For Each row In RadGridViewRHEWAAlle.Rows
            Try
                Dim valuecell = row.Cells("Anhangpfad")
                Dim imagecell = row.Cells("Anhang")

                If Not valuecell.Value.Trim.Equals("") Then
                    imagecell.Value = My.Resources.attach
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub


#End Region




#Region "Genehmigen / Ablehnen"
    ''' <summary>
    ''' Eichprozess genehmigen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GenehnmigeGewaehltenVorgang()
        If Not Me.VorgangsnummerGridServer.Equals("") Then
            If MessageBox.Show(My.Resources.GlobaleLokalisierung.Frage_Genehmigen, My.Resources.GlobaleLokalisierung.Frage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If clsWebserviceFunctions.GenehmigeEichprozess(VorgangsnummerGridServer) Then
                    'nach dem schließen des Dialogs aktualisieren
                    LoadFromDatabase()
                End If
            End If
        End If
    End Sub

    Private Sub AblehnenGewaehlterVorgang()
        If Not Me.VorgangsnummerGridServer.Equals("") Then
            If MessageBox.Show(My.Resources.GlobaleLokalisierung.Frage_Ablehnen, My.Resources.GlobaleLokalisierung.Frage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                If clsWebserviceFunctions.SetEichprozessAbgelehnt(VorgangsnummerGridServer) Then
                    'nach dem schließen des Dialogs aktualisieren
                    LoadFromDatabase()
                End If
            End If
        End If
    End Sub


#End Region

#Region "FTP Download"
    Private Sub OeffneDateiVonFTP(ByVal LokalerPfad As String, ByVal vorgangsnummer As String)
        If IO.File.Exists(LokalerPfad) Then
            Dim proc As Process = New Process()
            proc.StartInfo.FileName = LokalerPfad
            proc.StartInfo.UseShellExecute = True
            proc.Start()
        Else
            If Not BackgroundWorkerDownloadFromFTP.IsBusy Then
                Me.Enabled = False
                Me.RadProgressBar.Visible = True

                Me.RadProgressBar.Maximum = 100
                BackgroundWorkerDownloadFromFTP.RunWorkerAsync(vorgangsnummer)
            End If
        End If

    End Sub
#End Region


#Region "Updates aus Webservice"

    ''' <summary>
    ''' Synchronisiert Daten wie WZ, AWG, Stammdaten mit Webservice
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub VerbindeMitWebserviceUndHoleAlles()
        Me.Enabled = False
        Me.Parent.Enabled = False

        FlowLayoutPanel2.Visible = True
        RadProgressBar1.Visible = True
        RadProgressBar1.Value1 = 0
        BackgroundWorkerSyncAlles.RunWorkerAsync()
    End Sub

    ''' <summary>
    ''' Methode welche sich mit dem Webservice verbinduet und nach aktualisierungen für WZ, AWGs und eigenen Eichungen guckt
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub VerbindeMitWebServiceUndAktualisiere()
        If clsWebserviceFunctions.TesteVerbindung() Then

            'variablen zur Ausgabe ob es änderungen gibt:
            Dim bolNeuStammdaten As Boolean = False
            Dim bolNeuWZ As Boolean = False
            Dim bolNeuAWG As Boolean = False
            Dim bolNeuGenehmigung As Boolean = False

            'neue Stammdaten zum Benutzer holen
            clsWebserviceFunctions.GetBenutzerStammdaten(bolNeuStammdaten)

            'prüfen ob es neue WZ gibt
            clsWebserviceFunctions.GetNeueWZ(bolNeuWZ)
            'prüfen ob es neue AWG gibt
            clsWebserviceFunctions.GetNeuesAWG(bolNeuAWG)

            AktuellerBenutzer.Instance.LetztesUpdate = Date.Now
            AktuellerBenutzer.SaveSettings()

            'prüfen ob Eichprozesse die versendet wurden genehmigt oder abgelehnt wurden
            clsWebserviceFunctions.GetGenehmigungsstatus(bolNeuGenehmigung)

            'abgelegte Datein abrufen
            clsWebserviceFunctions.GetEichprotokollAblage()

            'refresh
            LoadFromDatabase()

            Dim returnMessage As String = My.Resources.GlobaleLokalisierung.Aktualisierung_Erfolgreich
            If bolNeuWZ Then
                returnMessage += vbNewLine & vbNewLine & My.Resources.GlobaleLokalisierung.Aktualisierung_NeuWZ & " "
            End If
            If bolNeuAWG Then
                returnMessage += vbNewLine & vbNewLine & My.Resources.GlobaleLokalisierung.Aktualisierung_NeuAWG & " "
            End If

            If bolNeuGenehmigung Then
                returnMessage += vbNewLine & vbNewLine & My.Resources.GlobaleLokalisierung.Aktualisierung_NeuEichung & " "
            End If

            If bolNeuStammdaten Then

            End If
            MessageBox.Show(returnMessage)
        Else
            MessageBox.Show(My.Resources.GlobaleLokalisierung.KeineVerbindung)
        End If
    End Sub


#End Region

#Region "Protokoll in Cloud ablegen"
    Private Sub VorgangAblegen()
        If Not Me.VorgangsnummerGridClient.Equals("") Then
            If clsWebserviceFunctions.PutEichprotokollAblage(VorgangsnummerGridClient) Then
                'neu laden der Liste
                LoadFromDatabase()
            Else
                MessageBox.Show(My.Resources.GlobaleLokalisierung.Fehler_UngueltigeLizenz)
            End If

        End If
    End Sub
#End Region


#End Region

End Class