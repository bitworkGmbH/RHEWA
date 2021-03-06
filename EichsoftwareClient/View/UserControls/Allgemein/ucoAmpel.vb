﻿Imports System.Drawing
Imports System.Drawing.Imaging
Imports EichsoftwareClient
''' <summary>
''' UCO für die Amplefunktion oben Rechts. Navigation und Anzeige des Statuses
''' </summary>
''' <remarks></remarks>
''' <author></author>
''' <commentauthor></commentauthor>
Public Class ucoAmpel

#Region "Events"
    Public Event Navigieren(ByVal GewaehlterVorgang As GlobaleEnumeratoren.enuEichprozessStatus)
    Private Event NotifyPropertyChanged()
#End Region

#Region "Instanzvariablen"
    Private Datasource As New DataTable
    Private WithEvents _ParentForm As FrmMainContainer
    Private _aktuellerGewaehlterVorgang As GlobaleEnumeratoren.enuEichprozessStatus = GlobaleEnumeratoren.enuEichprozessStatus.Stammdateneingabe
#End Region

#Region "Konstruktoren"
    ''' <summary>
    ''' methode welche in beiden Konstrkturen verwendet wird, zum zuweisen von standardwerten
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitConstructor()
        Me.RadListView1.AllowEdit = False
        Me.RadListView1.AllowRemove = False
        Me.RadListView1.FullRowSelect = True
        Me.RadListView1.ItemSize = New Size(32, 32)
        Me.RadListView1.Location = New Point(0, 0)
        Me.RadListView1.Name = "radListView1"
        Me.RadListView1.Size = New Size(287, 106)
        Me.RadListView1.TabIndex = 0
        Me.RadListView1.Text = "radListView1"
        Me.RadListView1.ViewType = Telerik.WinControls.UI.ListViewType.IconsView
        Me.RadListView1.ItemSize = New Size(150, 100)
        Me.RadListView1.ItemSpacing = 5
        Me.RadListView1.EnableKineticScrolling = False
        Me.RadListView1.ListViewElement.ViewElement.ViewElement.Margin = New Padding(0, 5, 0, 5)
        Me.RadListView1.ListViewElement.ViewElement.Orientation = Orientation.Horizontal
        RadListView1.ListViewElement.NotifyParentOnMouseInput = True
        RadListView1.ListViewElement.ShouldHandleMouseInput = True
        Datasource.TableName = "Bullets"
        Datasource.Columns.Add("Title", GetType(String))
        Datasource.Columns.Add("Status", GetType(String))

        Try
            Dim colArray(1) As Data.DataColumn
            colArray(0) = Datasource.Columns("Status")
            Datasource.PrimaryKey = colArray
        Catch e As Exception
        End Try
        Datasource.Columns.Add("Image", GetType(Byte()))
        'füllen des breadcrumbs
        FillDataset()
    End Sub

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        InitConstructor()
    End Sub

    Sub New(ByVal pParentForm As Form)
        ' This call is required by the designer.
        InitializeComponent()
        InitConstructor()
        Try
            _ParentForm = pParentForm
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "properties"
    Public Property AktuellerGewaehlterVorgang As GlobaleEnumeratoren.enuEichprozessStatus
        Get
            Return _aktuellerGewaehlterVorgang
        End Get
        Set(value As GlobaleEnumeratoren.enuEichprozessStatus)
            If _aktuellerGewaehlterVorgang <> value Then
                _aktuellerGewaehlterVorgang = value
                RaiseEvent NotifyPropertyChanged()
            End If
        End Set
    End Property
#End Region

#Region "enumeratoren"
    Public Enum enuImage
        grün = 1
        gelb = 2
        rot = 3
    End Enum
#End Region

#Region "Methoden"
    ''' <summary>
    ''' Füllt die Breadcrumb neu, bei wechselnder Lokalisierungc
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LokalisierungNeeded() Handles _ParentForm.LokalisierungNeeded
        FillDataset(Update:=True)
    End Sub

    ''' <summary>
    ''' Ändert die Statusbilder von rot, gelb und grün je nach Status
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Changes() Handles Me.NotifyPropertyChanged
        'ändern des bildes
        ChangeImageOfElement(AktuellerGewaehlterVorgang)

        If Not _ParentForm.CurrentEichprozess Is Nothing Then
            'erneutes überprüfen auf Stati die nun ungültig sind
            HideElement(GetListeUngueltigeStati(_ParentForm.CurrentEichprozess))
        End If
    End Sub

    ''' <summary>
    ''' Ändert die Icons des gewählten Elements und alle denen davor. Wenn ein Element auf grün ist, müssen davor auch alle grün sein. Dann kann es max 1 gelbes geben und alles danach ist rot
    ''' </summary>
    ''' <param name="pStatus"></param>
    ''' <remarks></remarks>
    Private Sub ChangeImageOfElement(ByVal pStatus As GlobaleEnumeratoren.enuEichprozessStatus)
        Try
            For Each item As DataRow In Datasource.Rows
                If CInt(item("Status")) < pStatus Then
                    item("Image") = ConvertBitmapToByteArray(My.Resources.bullet_green)

                ElseIf CInt(item("Status")) = pStatus Then
                    'sonderfall versenden = fertig
                    If pStatus = GlobaleEnumeratoren.enuEichprozessStatus.Versenden Then
                        If Not _ParentForm.CurrentEichprozess Is Nothing Then
                            If _ParentForm.CurrentEichprozess.FK_Bearbeitungsstatus = GlobaleEnumeratoren.enuBearbeitungsstatus.noch_nicht_versendet Or
                               _ParentForm.CurrentEichprozess.FK_Bearbeitungsstatus = GlobaleEnumeratoren.enuBearbeitungsstatus.Fehlerhaft Then
                                item("Image") = ConvertBitmapToByteArray(My.Resources.bullet_yellow)
                            Else
                                item("Image") = ConvertBitmapToByteArray(My.Resources.bullet_green)
                                Datasource.AcceptChanges()
                            End If
                        Else
                            item("Image") = ConvertBitmapToByteArray(My.Resources.bullet_yellow)
                        End If
                    Else
                        item("Image") = ConvertBitmapToByteArray(My.Resources.bullet_yellow)
                    End If
                ElseIf CInt(item("Status")) > pStatus Then
                    item("Image") = ConvertBitmapToByteArray(My.Resources.bullet_red)
                End If
            Next
            Datasource.AcceptChanges()

            FindeElementUndSelektiere(pStatus)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Markiert das übergebenen Element als Aktives und setzt den Fokus
    ''' </summary>
    ''' <param name="pStatus"></param>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Public Sub FindeElementUndSelektiere(ByVal pStatus As GlobaleEnumeratoren.enuEichprozessStatus)
        Try
            'prüfen ob das aktuelle element welchse das Event triggert bereits fokusiert ist, wenn ja => überspringen
            Dim ListItemTmp = (From raditem In RadListView1.Items Where IsNumeric(raditem.Value) And raditem.Value = pStatus And raditem.Visible = True And raditem.Selected = True).FirstOrDefault
            If Not ListItemTmp Is Nothing Then 'es ist markiert/fokusiert => abbruch
                Exit Sub
            Else

                Dim items(0) As Telerik.WinControls.UI.ListViewDataItem

                'workaounrd für fokussierungsfehler. Damit das aktuell gewählte element in der Mitte angezeigt wird, wird zunächst das vorherige Element ausgewählt und dann erst das korrekte

                'vorheriges Element finden und selektieren
                Dim Listitem = (From raditem In RadListView1.Items Where raditem.Value = pStatus - 1 And raditem.Visible = True).FirstOrDefault
                If Listitem Is Nothing Then
                    Listitem = (From raditem In RadListView1.Items Where raditem.Value = pStatus - 2 And raditem.Visible = True).FirstOrDefault
                    If Listitem Is Nothing Then
                        Listitem = (From raditem In RadListView1.Items Where raditem.Value = pStatus And raditem.Visible = True).FirstOrDefault
                    End If
                End If

                Dim priorcontol As Control = Nothing
                If Not Me._ParentForm Is Nothing Then
                    If Not Me._ParentForm.CurrentUCO Is Nothing Then
                        priorcontol = Me._ParentForm.CurrentUCO.ActiveControl

                    End If
                End If

                items(0) = Listitem
                Try
                    RadListView1.Select(items)

                Catch ex As StackOverflowException 'bisher einmal ohne Grund aufgetreten beim Ändern der Sprache...
                Catch ex As Exception
                End Try
                RadListView1.Focus()

                'tatsächliches element finden und selektieren
                Listitem = (From raditem In RadListView1.Items Where raditem.Value = pStatus And raditem.Visible = True).FirstOrDefault
                If Listitem Is Nothing Then
                    Listitem = (From raditem In RadListView1.Items Where raditem.Value = pStatus - 1 And raditem.Visible = True).FirstOrDefault
                End If
                items(0) = Listitem
                RadListView1.SelectedItem = Nothing
                Try
                    RadListView1.Select(items)

                Catch ex As StackOverflowException 'bisher einmal ohne Grund aufgetreten beim Ändern der Sprache...
                Catch ex As Exception
                End Try
                RadListView1.Focus()
                If Not priorcontol Is Nothing Then
                    priorcontol.Focus()
                End If

            End If

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Füllt das Grid mit den bisher bekannten Vorgangsstati
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor>In dem Grid stehen dann aber alle Stati. Es werden nicht unbenötigte ausgeblendet</commentauthor>
    Private Sub FillDataset(Optional Update As Boolean = False)
        If Update = True Then
            UpdateDataset()
        Else
            CreateDataset()
        End If

    End Sub

    Private Sub CreateDataset()
        Try
            Datasource.AcceptChanges()
            Datasource.Clear()
            Datasource.AcceptChanges()
        Catch ex As Exception

        End Try
        Dim nrow As DataRow = Datasource.NewRow
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_Stammdaten, GlobaleEnumeratoren.enuEichprozessStatus.Stammdateneingabe, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_Kompatiblitaetsnachweis, GlobaleEnumeratoren.enuEichprozessStatus.Kompatbilitaetsnachweis, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_KompatiblitaetsnachweisErgebnis, GlobaleEnumeratoren.enuEichprozessStatus.KompatbilitaetsnachweisErgebnis, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_Beschaffenheitspruefung, GlobaleEnumeratoren.enuEichprozessStatus.Beschaffenheitspruefung, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_Eichprotokollverfahrensauswahl, GlobaleEnumeratoren.enuEichprozessStatus.AuswahlKonformitätsverfahren, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_EichprotokollStammdaten, GlobaleEnumeratoren.enuEichprozessStatus.EichprotokollStammdaten, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungNullstellungundAussermittigeBelastung, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderGenauigkeitderNullstellungUndAussermittigeBelastung, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungDerRichtigkeitMitNormallast, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStaffelverfahren, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungDerWiederholbarkeit, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungUeberlastAnzeige, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderÜberlastanzeige, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungRollendelasten, GlobaleEnumeratoren.enuEichprozessStatus.WaagenFuerRollendeLasten, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungAnsprechvermoegen, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungdesAnsprechvermögens, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStabilitaet, GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungTaraEinrichtung, GlobaleEnumeratoren.enuEichprozessStatus.Taraeinrichtung, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungEignungAchslastwaegungen, GlobaleEnumeratoren.enuEichprozessStatus.EignungfürAchslastwägungen, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungFallbeschleunigung, GlobaleEnumeratoren.enuEichprozessStatus.BerücksichtigungderFallbeschleunigung, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungEichtechnischeSicherung, GlobaleEnumeratoren.enuEichprozessStatus.EichtechnischeSicherungundDatensicherung, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_Exports, GlobaleEnumeratoren.enuEichprozessStatus.Export, My.Resources.bullet_red)
        createRow(nrow, My.Resources.GlobaleLokalisierung.Ueberschrift_Versenden, GlobaleEnumeratoren.enuEichprozessStatus.Versenden, My.Resources.bullet_red)


        RadListView1.DataSource = Datasource
        RadListView1.DisplayMember = "Title"
        RadListView1.ValueMember = "Status"

        ' give each of the data items a key (= the id of the usercontrol)
        For Each item As Telerik.WinControls.UI.ListViewDataItem In RadListView1.Items
            item.Key = item.Value
        Next

        Datasource.AcceptChanges()
    End Sub

    Private Sub UpdateDataset()
        Datasource.Rows(0)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_Stammdaten
        Datasource.Rows(1)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_Kompatiblitaetsnachweis
        Datasource.Rows(2)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_KompatiblitaetsnachweisErgebnis
        Datasource.Rows(3)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_Beschaffenheitspruefung
        Datasource.Rows(4)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_Eichprotokollverfahrensauswahl
        Datasource.Rows(5)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_EichprotokollStammdaten
        Datasource.Rows(6)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungNullstellungundAussermittigeBelastung
        Datasource.Rows(7)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungDerRichtigkeitMitNormallast
        Datasource.Rows(8)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStaffelverfahren
        Datasource.Rows(9)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungDerWiederholbarkeit
        Datasource.Rows(10)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungUeberlastAnzeige
        Datasource.Rows(11)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungRollendelasten
        Datasource.Rows(12)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungAnsprechvermoegen
        Datasource.Rows(13)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStabilitaet
        Datasource.Rows(14)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungTaraEinrichtung
        Datasource.Rows(15)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungEignungAchslastwaegungen
        Datasource.Rows(16)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungFallbeschleunigung
        Datasource.Rows(17)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungEichtechnischeSicherung
        Datasource.Rows(18)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_Exports
        Datasource.Rows(19)("Title") = My.Resources.GlobaleLokalisierung.Ueberschrift_Versenden
        Datasource.AcceptChanges()
    End Sub

    Private Sub createRow(nrow As DataRow, ueberschrift As String, status As GlobaleEnumeratoren.enuEichprozessStatus, bullet As Bitmap)
        nrow = Datasource.NewRow
        nrow("Title") = ueberschrift
        nrow("Status") = CInt(status)
        nrow("Image") = ConvertBitmapToByteArray(bullet)
        Datasource.Rows.Add(nrow)
    End Sub



    ''' <summary>
    ''' Blendet Vorgangs Element aus
    ''' </summary>
    ''' <param name="pStatus"></param>
    ''' <remarks></remarks>
    Public Sub HideElement(ByVal pStatus As List(Of GlobaleEnumeratoren.enuEichprozessStatus))
        If pStatus Is Nothing Then Exit Sub
        Try
            For Each item In RadListView1.Items
                If pStatus.Contains(item.Value) Then
                    item.Visible = False
                Else
                    item.Visible = True
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub
#End Region

#Region "telerik"
    ''' <summary>
    ''' Konvertierungsfunktion von Bitmap zu Bytearray
    ''' </summary>
    ''' <param name="img"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DebuggerStepThrough()>
    Private Function ConvertBitmapToByteArray(ByVal img As Bitmap) As Byte()
        Dim stream As New IO.MemoryStream()
        img.Save(stream, ImageFormat.Png)

        Dim byteArray As Byte() = stream.GetBuffer()
        Return byteArray
    End Function

    Private Sub RadListView1_CurrentItemChanged(sender As Object, e As Telerik.WinControls.UI.ListViewItemEventArgs) Handles RadListView1.CurrentItemChanged
        'abbruch falls Status noch rot. hier darf nicht hingesprungen werden
        Try
            Dim ItemImage = DirectCast(e.Item("Image"), Byte())
            Dim CompareImage = ConvertBitmapToByteArray(My.Resources.bullet_red)

            'prüfen ob das gewählte element rot ist. wenn nicht das letzte gelbe element wählen
            If ItemImage.SequenceEqual(CompareImage) Then
                Me.FindeElementUndSelektiere(Me.AktuellerGewaehlterVorgang)
                Exit Sub
            End If
            'End If

            RaiseEvent Navigieren(e.Item("Status"))
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Event welches von Telerik gebraucht wird, um unser Custom Visual Item in der Listbox zu erzeugen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub radListView1_VisualItemCreating(ByVal sender As Object, ByVal e As Telerik.WinControls.UI.ListViewVisualItemCreatingEventArgs) Handles RadListView1.VisualItemCreating
        e.VisualItem = New CustomVisualItem()
    End Sub

#End Region

#Region "Hilfsfunktionen"

    ''' <summary>
    ''' Erzeugt eine Liste von Stati die für den aktuellen Eichrpozess ungültig sind. Z.B. Staffelverfahren bei einbereichwaagen
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Wird z.b. für UCOAmpel genutzt umben�tigte Felder auszublenden</remarks>
    Private Function GetListeUngueltigeStati(objEichprozess As Eichprozess) As List(Of GlobaleEnumeratoren.enuEichprozessStatus)
        Try
            Dim returnlist As New List(Of GlobaleEnumeratoren.enuEichprozessStatus)

            'wenn es sich nicht um ein flüchtiges Object handelt (server seitiges objekt (kopie von Server objekt)
            'versuche aufruf,
            Try
                'Versuch
                Dim obj = objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren.Verfahren
                'ansonsten reinitialsiere Objekt aus lokaler DB
                SammelUngueltigeStati(objEichprozess, returnlist)
            Catch ex As NullReferenceException
                'neu laden des Objekts, diesmal mit den lookup Objekten
                ReloadEichprozess(objEichprozess, returnlist)
            Catch ex As ObjectDisposedException
                'neu laden des Objekts, diesmal mit den lookup Objekten
                ReloadEichprozess(objEichprozess, returnlist)

            End Try

            Return returnlist
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
            Debug.WriteLine(ex.StackTrace)

            Return Nothing
        End Try
    End Function
    ''' <summary>
    '''neu laden des Objekts, diesmal mit den lookup Objekten
    ''' </summary>
    ''' <param name="objEichprozess"></param>
    ''' <param name="returnlist"></param>
    Private Sub ReloadEichprozess(ByRef objEichprozess As Eichprozess, ByRef returnlist As List(Of GlobaleEnumeratoren.enuEichprozessStatus))
        Using context As New Entities
            Dim vorgangsnummer As String = objEichprozess.Vorgangsnummer
            objEichprozess = (From a In context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = vorgangsnummer).FirstOrDefault

            SammelUngueltigeStati(objEichprozess, returnlist)
        End Using
    End Sub

    ''' <summary>
    '''  Methode welche abhängig von diversen Faktoren Listitems aus der auslistung entfernt oder hinzufügt. So gibt es einige Prüfungen z.b. nur für Achlastwägungen
    ''' </summary>
    ''' <param name="objEichprozess"></param>
    ''' <param name="returnlist"></param>
    ''' <remarks></remarks>
    Private Sub SammelUngueltigeStati(ByRef objEichprozess As Eichprozess, ByRef returnlist As List(Of GlobaleEnumeratoren.enuEichprozessStatus))
        If Not objEichprozess Is Nothing Then
            'Achslastwägungen
            If Not objEichprozess.Eichprotokoll Is Nothing Then
                Select Case objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren.Verfahren
                    Case Is = "über 60kg mit Normalien", "über 60kg im Staffelverfahren"
                        ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                        returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.EignungfürAchslastwägungen)
                        returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.WaagenFuerRollendeLasten)
                    Case Is = "Fahrzeugwaagen"
                End Select

                Select Case objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren.Verfahren
                    Case Is = "über 60kg mit Normalien"
                        ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                        returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast)
                    Case Is = "Fahrzeugwaagen", "über 60kg im Staffelverfahren"
                        ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                        returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet)
                End Select

                If objEichprozess.Eichprotokoll.Verwendungszweck_Drucker = False Then
                    returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage)
                End If
            Else
                returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet)
                returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.EignungfürAchslastwägungen)
                returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.WaagenFuerRollendeLasten)
                returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast)
                returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage)
            End If
        Else
            returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet)
            returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.EignungfürAchslastwägungen)
            returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.WaagenFuerRollendeLasten)
            returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast)
            returnlist.Add(GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderStabilitätderGleichgewichtslage)
        End If
    End Sub

#End Region

End Class