' 13.05.2014 hill EichsoftwareClient uco10PruefungStaffelverfahren.vb
Imports System

Public Class uco10PruefungStaffelverfahren

    Inherits ucoContent
    Implements IRhewaEditingDialog

#Region "Member Variables"
    Private _suspendEvents As Boolean = False 'Variable zum temporären stoppen der Eventlogiken
    Private _ListPruefungStaffelverfahrenNormallast As New List(Of PruefungStaffelverfahrenNormallast)
    Private _ListPruefungStaffelverfahrenErsatzlast As New List(Of PruefungStaffelverfahrenErsatzlast)

    Private AnzahlBereiche As Integer
#End Region

#Region "Constructors"
    Sub New()
        MyBase.New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        Telerik.WinControls.ThemeResolutionService.LoadPackageResource("EichsoftwareClient.RHEWAGREEN.tssp") 'Pfad zur Themedatei
        Telerik.WinControls.ThemeResolutionService.ApplicationThemeName = "RHEWAGREEN" 'standard Themename
        Telerik.WinControls.ThemeResolutionService.ApplyThemeToControlTree(Me, "RHEWAGREEN")

    End Sub

    Sub New(ByRef pParentform As FrmMainContainer, ByRef pObjEichprozess As Eichprozess, Optional ByRef pPreviousUco As ucoContent = Nothing, Optional ByRef pNextUco As ucoContent = Nothing, Optional ByVal pEnuModus As enuDialogModus = enuDialogModus.normal)
        MyBase.New(pParentform, pObjEichprozess, pPreviousUco, pNextUco, pEnuModus)
        _suspendEvents = True
        InitializeComponent()
        _suspendEvents = False
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast
    End Sub

#End Region

#Region "Events"

    Private Sub ucoBeschaffenheitspruefung_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SetzeUeberschrift()
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast

        'daten füllen
        LoadFromDatabase()

    End Sub

    ''' <summary>
    ''' pflichtfelder rot umranden bei falschen oder fehlenden eingaben
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControl_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles _
    RadTextBoxControlStaffel5Bereich3Last2.Validating,
    RadTextBoxControlStaffel5Bereich3Last1.Validating,
    RadTextBoxControlStaffel5Bereich3Anzeige1.Validating,
    RadTextBoxControlStaffel5Bereich2Last2.Validating,
    RadTextBoxControlStaffel5Bereich2Last1.Validating,
    RadTextBoxControlStaffel5Bereich2Anzeige1.Validating,
    RadTextBoxControlStaffel5Bereich1Last2.Validating,
    RadTextBoxControlStaffel5Bereich1Last1.Validating,
    RadTextBoxControlStaffel5Bereich1Anzeige1.Validating,
    RadTextBoxControlStaffel4Bereich3Last2.Validating,
    RadTextBoxControlStaffel4Bereich3Last1.Validating,
    RadTextBoxControlStaffel4Bereich3Anzeige1.Validating,
    RadTextBoxControlStaffel4Bereich2Last2.Validating,
    RadTextBoxControlStaffel4Bereich2Last1.Validating,
    RadTextBoxControlStaffel4Bereich2Anzeige1.Validating,
    RadTextBoxControlStaffel4Bereich1Last2.Validating,
    RadTextBoxControlStaffel4Bereich1Last1.Validating,
    RadTextBoxControlStaffel4Bereich1Anzeige1.Validating,
    RadTextBoxControlStaffel3Bereich3Last2.Validating,
    RadTextBoxControlStaffel3Bereich3Last1.Validating,
    RadTextBoxControlStaffel3Bereich3Anzeige1.Validating,
    RadTextBoxControlStaffel3Bereich2Last2.Validating,
    RadTextBoxControlStaffel3Bereich2Last1.Validating,
    RadTextBoxControlStaffel3Bereich2Anzeige1.Validating,
    RadTextBoxControlStaffel3Bereich1Last2.Validating,
    RadTextBoxControlStaffel3Bereich1Last1.Validating,
    RadTextBoxControlStaffel3Bereich1Anzeige1.Validating,
    RadTextBoxControlStaffel2Bereich3Last2.Validating,
    RadTextBoxControlStaffel2Bereich3Last1.Validating,
    RadTextBoxControlStaffel2Bereich3Anzeige1.Validating,
    RadTextBoxControlStaffel2Bereich2Last2.Validating,
    RadTextBoxControlStaffel2Bereich2Last1.Validating,
    RadTextBoxControlStaffel2Bereich2Anzeige1.Validating,
    RadTextBoxControlStaffel2Bereich1Last2.Validating,
    RadTextBoxControlStaffel2Bereich1Last1.Validating,
    RadTextBoxControlStaffel2Bereich1Anzeige1.Validating,
    RadTextBoxControlStaffel1Bereich3Last4.Validating,
    RadTextBoxControlStaffel1Bereich3Last3.Validating,
    RadTextBoxControlStaffel1Bereich3Last1.Validating,
    RadTextBoxControlStaffel1Bereich3Anzeige4.Validating,
    RadTextBoxControlStaffel1Bereich3Anzeige3.Validating,
    RadTextBoxControlStaffel1Bereich3Anzeige1.Validating,
    RadTextBoxControlStaffel1Bereich2Last4.Validating,
    RadTextBoxControlStaffel1Bereich2Last3.Validating,
    RadTextBoxControlStaffel1Bereich2Last1.Validating,
    RadTextBoxControlStaffel1Bereich2Anzeige4.Validating,
    RadTextBoxControlStaffel1Bereich2Anzeige3.Validating,
    RadTextBoxControlStaffel1Bereich2Anzeige1.Validating,
    RadTextBoxControlStaffel1Bereich1Last4.Validating,
    RadTextBoxControlStaffel1Bereich1Last3.Validating,
    RadTextBoxControlStaffel1Bereich1Last1.Validating,
    RadTextBoxControlStaffel1Bereich1Anzeige4.Validating,
    RadTextBoxControlStaffel1Bereich1Anzeige3.Validating,
    RadTextBoxControlStaffel1Bereich1Anzeige1.Validating,
    RadTextBoxControlStaffel1Bereich1Last2.Validating,
    RadTextBoxControlStaffel1Bereich1Fehler2.Validating,
    RadTextBoxControlStaffel1Bereich1Anzeige2.Validating
        Try
            BasicTextboxValidation(sender, e)
        Catch ex As Exception
        End Try
    End Sub


    ''' <summary>
    ''' Event welches bei allen Textboxen triggerd und die Berechnungsroutinen anstößt
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControlEingaben_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel5Bereich1Last4.Leave, RadTextBoxControlStaffel5Bereich1Last3.Leave, RadTextBoxControlStaffel5Bereich1Last2.Leave, RadTextBoxControlStaffel5Bereich1Last1.Leave, RadTextBoxControlStaffel5Bereich1Anzeige1.Leave, RadTextBoxControlStaffel4Bereich1Last4.Leave, RadTextBoxControlStaffel4Bereich1Last3.Leave, RadTextBoxControlStaffel4Bereich1Last2.Leave, RadTextBoxControlStaffel4Bereich1Last1.Leave, RadTextBoxControlStaffel4Bereich1Anzeige1.Leave, RadTextBoxControlStaffel3Bereich1Last2.Leave, RadTextBoxControlStaffel3Bereich1Last1.Leave, RadTextBoxControlStaffel3Bereich1Anzeige1.Leave, RadTextBoxControlStaffel2Bereich1Last2.Leave, RadTextBoxControlStaffel2Bereich1Last1.Leave, RadTextBoxControlStaffel2Bereich1Anzeige1.Leave, RadTextBoxControlStaffel1Bereich1Last4.Leave, RadTextBoxControlStaffel1Bereich1Last3.Leave, RadTextBoxControlStaffel1Bereich1Last2.Leave, RadTextBoxControlStaffel1Bereich1Last1.Leave, RadTextBoxControlStaffel1Bereich1Anzeige4.Leave, RadTextBoxControlStaffel1Bereich1Anzeige3.Leave, RadTextBoxControlStaffel1Bereich1Anzeige2.Leave, RadTextBoxControlStaffel1Bereich1Anzeige1.Leave,
    RadTextBoxControlStaffel5Bereich2Last4.Leave, RadTextBoxControlStaffel5Bereich2Last3.Leave, RadTextBoxControlStaffel5Bereich2Last2.Leave, RadTextBoxControlStaffel5Bereich2Last1.Leave, RadTextBoxControlStaffel5Bereich2Anzeige1.Leave, RadTextBoxControlStaffel4Bereich2Last4.Leave, RadTextBoxControlStaffel4Bereich2Last3.Leave, RadTextBoxControlStaffel4Bereich2Last2.Leave, RadTextBoxControlStaffel4Bereich2Last1.Leave, RadTextBoxControlStaffel4Bereich2Anzeige1.Leave, RadTextBoxControlStaffel3Bereich2Last2.Leave, RadTextBoxControlStaffel3Bereich2Last1.Leave, RadTextBoxControlStaffel3Bereich2Anzeige1.Leave, RadTextBoxControlStaffel2Bereich2Last2.Leave, RadTextBoxControlStaffel2Bereich2Last1.Leave, RadTextBoxControlStaffel2Bereich2Anzeige1.Leave, RadTextBoxControlStaffel1Bereich2Last4.Leave, RadTextBoxControlStaffel1Bereich2Last3.Leave, RadTextBoxControlStaffel1Bereich2Last1.Leave, RadTextBoxControlStaffel1Bereich2Anzeige4.Leave, RadTextBoxControlStaffel1Bereich2Anzeige3.Leave, RadTextBoxControlStaffel1Bereich2Anzeige1.Leave,
    RadTextBoxControlStaffel5Bereich3Last4.Leave, RadTextBoxControlStaffel5Bereich3Last3.Leave, RadTextBoxControlStaffel5Bereich3Last2.Leave, RadTextBoxControlStaffel5Bereich3Last1.Leave, RadTextBoxControlStaffel5Bereich3Anzeige1.Leave, RadTextBoxControlStaffel4Bereich3Last4.Leave, RadTextBoxControlStaffel4Bereich3Last3.Leave, RadTextBoxControlStaffel4Bereich3Last2.Leave, RadTextBoxControlStaffel4Bereich3Last1.Leave, RadTextBoxControlStaffel4Bereich3Anzeige1.Leave, RadTextBoxControlStaffel3Bereich3Last2.Leave, RadTextBoxControlStaffel3Bereich3Last1.Leave, RadTextBoxControlStaffel3Bereich3Anzeige1.Leave, RadTextBoxControlStaffel2Bereich3Last2.Leave, RadTextBoxControlStaffel2Bereich3Last1.Leave, RadTextBoxControlStaffel2Bereich3Anzeige1.Leave, RadTextBoxControlStaffel1Bereich3Last4.Leave, RadTextBoxControlStaffel1Bereich3Last3.Leave, RadTextBoxControlStaffel1Bereich3Last1.Leave, RadTextBoxControlStaffel1Bereich3Anzeige4.Leave, RadTextBoxControlStaffel1Bereich3Anzeige3.Leave, RadTextBoxControlStaffel1Bereich3Anzeige1.Leave,
    RadTextBoxControlStaffel5Bereich3Anzeige4.Leave, RadTextBoxControlStaffel5Bereich3Anzeige3.Leave, RadTextBoxControlStaffel5Bereich2Anzeige4.Leave, RadTextBoxControlStaffel5Bereich2Anzeige3.Leave, RadTextBoxControlStaffel5Bereich1Anzeige4.Leave, RadTextBoxControlStaffel5Bereich1Anzeige3.Leave, RadTextBoxControlStaffel4Bereich3Anzeige4.Leave, RadTextBoxControlStaffel4Bereich3Anzeige3.Leave, RadTextBoxControlStaffel4Bereich2Anzeige4.Leave, RadTextBoxControlStaffel4Bereich2Anzeige3.Leave, RadTextBoxControlStaffel4Bereich1Anzeige4.Leave, RadTextBoxControlStaffel4Bereich1Anzeige3.Leave, RadTextBoxControlStaffel3Bereich3Anzeige4.Leave, RadTextBoxControlStaffel3Bereich3Anzeige3.Leave, RadTextBoxControlStaffel3Bereich2Anzeige4.Leave, RadTextBoxControlStaffel3Bereich2Anzeige3.Leave, RadTextBoxControlStaffel3Bereich1Anzeige4.Leave, RadTextBoxControlStaffel3Bereich1Anzeige3.Leave, RadTextBoxControlStaffel2Bereich3Anzeige4.Leave, RadTextBoxControlStaffel2Bereich3Anzeige3.Leave, RadTextBoxControlStaffel2Bereich2Anzeige4.Leave, RadTextBoxControlStaffel2Bereich2Anzeige3.Leave, RadTextBoxControlStaffel2Bereich1Anzeige4.Leave, RadTextBoxControlStaffel2Bereich1Anzeige3.Leave

        If _suspendEvents = True Then Exit Sub
        'If TimerRunning Then Exit Sub Else StartTimer()
        _suspendEvents = True

        AktuellerStatusDirty = True

        Dim staffel As String = GetStaffel(sender)
        Dim bereich As String = GetBereich(sender)
        BerechneStaffelBereich(staffel, bereich)
        BerechneMessabweichung(bereich)
        _suspendEvents = False
    End Sub

    Private Sub RadButtonZwischenwerte_Click(sender As Object, e As EventArgs) Handles RadButtonStaffel1Bereich1Zwischenwerte.Click, RadButtonStaffel5Bereich3Zwischenwerte.Click, RadButtonStaffel5Bereich2Zwischenwerte.Click, RadButtonStaffel5Bereich1Zwischenwerte.Click, RadButtonStaffel3Bereich3Zwischenwerte.Click, RadButtonStaffel3Bereich2Zwischenwerte.Click, RadButtonStaffel3Bereich1Zwischenwerte.Click, RadButtonStaffel2Bereich3Zwischenwerte.Click, RadButtonStaffel2Bereich2Zwischenwerte.Click, RadButtonStaffel2Bereich1Zwischenwerte.Click, RadButtonStaffel1Bereich3Zwischenwerte.Click, RadButtonStaffel1Bereich2Zwischenwerte.Click
        'TODO Dennis Ostroga
        'Hier muss der neue Dialog geöffnet werden
        Dim staffel As String = GetStaffel(sender)
        Dim bereich As String = GetBereich(sender)
        Dim ersatzgewicht As Decimal
        Try

            'TODO Dennis OStroga - bitwork GmbH - Aktuelle TextBox einlesen
            Dim ControlBlank = Controls("RadTextBoxControlStaffel1Bereich1Last1")
            Dim Control As Telerik.WinControls.UI.RadTextBox = CType(ControlBlank, Telerik.WinControls.UI.RadTextBox)

            'RadTextBoxControlStaffel2Bereich1Last1
            ersatzgewicht = Control.Text 'RadTextBoxControlStaffel1Bereich1Last1.Text

        Catch ex As Exception

        End Try

        Dim FZwischenwerte As New frmZwischenwerte(Me.objEichprozess, staffel, bereich, ersatzgewicht)
        FZwischenwerte.ShowDialog()
    End Sub

#Region "Hilfetexte"
    Private Sub RadButtonStaffel1Bereich1Zwischenwerte_MouseEnter(sender As Object, e As EventArgs) Handles RadButtonStaffel1Bereich1Zwischenwerte.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenZwischenwerte)
    End Sub
    Private Sub RadTextBoxControlStaffel1Bereich1Last1_MouseEnter(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel1Bereich3Last4.MouseEnter, RadTextBoxControlStaffel1Bereich3Last3.MouseEnter, RadTextBoxControlStaffel1Bereich3Last1.MouseEnter, RadTextBoxControlStaffel1Bereich2Last4.MouseEnter, RadTextBoxControlStaffel1Bereich2Last3.MouseEnter, RadTextBoxControlStaffel1Bereich2Last1.MouseEnter, RadTextBoxControlStaffel1Bereich1Last4.MouseEnter, RadTextBoxControlStaffel1Bereich1Last3.MouseEnter, RadTextBoxControlStaffel1Bereich1Last1.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenNormallast)
    End Sub

    Private Sub RadGroupBoxStaffel1Bereich1_MouseEnter(sender As Object, e As EventArgs) Handles RadGroupBoxStaffel5Bereich3.MouseEnter, RadGroupBoxStaffel5Bereich2.MouseEnter, RadGroupBoxStaffel5Bereich1.MouseEnter, RadGroupBoxStaffel3Bereich3.MouseEnter, RadGroupBoxStaffel3Bereich2.MouseEnter, RadGroupBoxStaffel3Bereich1.MouseEnter, RadGroupBoxStaffel2Bereich3.MouseEnter, RadGroupBoxStaffel2Bereich2.MouseEnter, RadGroupBoxStaffel2Bereich1.MouseEnter, RadGroupBoxStaffel1Bereich3.MouseEnter, RadGroupBoxStaffel1Bereich2.MouseEnter, RadGroupBoxStaffel1Bereich1.MouseEnter
        'zurücksetzen der Hilfetexte
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahren)
    End Sub

    Private Sub RadTextBoxControlStaffel2Bereich1Last1_MouseEnter(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel5Bereich3Last1.MouseEnter, RadTextBoxControlStaffel5Bereich2Last1.MouseEnter, RadTextBoxControlStaffel5Bereich1Last1.MouseEnter, RadTextBoxControlStaffel4Bereich3Last1.MouseEnter, RadTextBoxControlStaffel4Bereich2Last1.MouseEnter, RadTextBoxControlStaffel4Bereich1Last1.MouseEnter, RadTextBoxControlStaffel3Bereich3Last1.MouseEnter, RadTextBoxControlStaffel3Bereich2Last1.MouseEnter, RadTextBoxControlStaffel3Bereich1Last1.MouseEnter, RadTextBoxControlStaffel2Bereich3Last1.MouseEnter, RadTextBoxControlStaffel2Bereich2Last1.MouseEnter, RadTextBoxControlStaffel2Bereich1Last1.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenErsatzlastSoll)
    End Sub

    Private Sub RadTextBoxControlStaffel2Bereich1Anzeige1_MouseEnter(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel5Bereich3Anzeige1.MouseEnter, RadTextBoxControlStaffel5Bereich2Anzeige1.MouseEnter, RadTextBoxControlStaffel5Bereich1Anzeige1.MouseEnter, RadTextBoxControlStaffel4Bereich3Anzeige1.MouseEnter, RadTextBoxControlStaffel4Bereich2Anzeige1.MouseEnter, RadTextBoxControlStaffel4Bereich1Anzeige1.MouseEnter, RadTextBoxControlStaffel3Bereich3Anzeige1.MouseEnter, RadTextBoxControlStaffel3Bereich2Anzeige1.MouseEnter, RadTextBoxControlStaffel3Bereich1Anzeige1.MouseEnter, RadTextBoxControlStaffel2Bereich3Anzeige1.MouseEnter, RadTextBoxControlStaffel2Bereich2Anzeige1.MouseEnter, RadTextBoxControlStaffel2Bereich1Anzeige1.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenErsatzlastIst)
    End Sub

    Private Sub RadTextBoxControlStaffel2Bereich1Last2_MouseEnter(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel5Bereich3Last2.MouseEnter, RadTextBoxControlStaffel5Bereich2Last2.MouseEnter, RadTextBoxControlStaffel5Bereich1Last2.MouseEnter, RadTextBoxControlStaffel4Bereich3Last2.MouseEnter, RadTextBoxControlStaffel4Bereich2Last2.MouseEnter, RadTextBoxControlStaffel4Bereich1Last2.MouseEnter, RadTextBoxControlStaffel3Bereich3Last2.MouseEnter, RadTextBoxControlStaffel3Bereich2Last2.MouseEnter, RadTextBoxControlStaffel3Bereich1Last2.MouseEnter, RadTextBoxControlStaffel2Bereich3Last2.MouseEnter, RadTextBoxControlStaffel2Bereich2Last2.MouseEnter, RadTextBoxControlStaffel2Bereich1Last2.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenErsatzlastNormallast)
    End Sub

    Private Sub RadTextBoxControlStaffel5Bereich3Anzeige3_MouseEnter(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel5Bereich3Anzeige3.MouseEnter, RadTextBoxControlStaffel5Bereich2Anzeige3.MouseEnter, RadTextBoxControlStaffel5Bereich1Anzeige3.MouseEnter, RadTextBoxControlStaffel4Bereich3Anzeige3.MouseEnter, RadTextBoxControlStaffel4Bereich2Anzeige3.MouseEnter, RadTextBoxControlStaffel4Bereich1Anzeige3.MouseEnter, RadTextBoxControlStaffel3Bereich3Anzeige3.MouseEnter, RadTextBoxControlStaffel3Bereich2Anzeige3.MouseEnter, RadTextBoxControlStaffel3Bereich1Anzeige3.MouseEnter, RadTextBoxControlStaffel2Bereich3Anzeige3.MouseEnter, RadTextBoxControlStaffel2Bereich2Anzeige3.MouseEnter, RadTextBoxControlStaffel2Bereich1Anzeige3.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenErsatzlastErsatzPlusNormallast)
    End Sub

    Private Sub RadTextBoxControlStaffel2Bereich1Anzeige4_MouseEnter(sender As Object, e As EventArgs) Handles RadTextBoxControlStaffel5Bereich3Anzeige4.MouseEnter, RadTextBoxControlStaffel5Bereich2Anzeige4.MouseEnter, RadTextBoxControlStaffel5Bereich1Anzeige4.MouseEnter, RadTextBoxControlStaffel4Bereich3Anzeige4.MouseEnter, RadTextBoxControlStaffel4Bereich2Anzeige4.MouseEnter, RadTextBoxControlStaffel4Bereich1Anzeige4.MouseEnter, RadTextBoxControlStaffel3Bereich3Anzeige4.MouseEnter, RadTextBoxControlStaffel3Bereich2Anzeige4.MouseEnter, RadTextBoxControlStaffel3Bereich1Anzeige4.MouseEnter, RadTextBoxControlStaffel2Bereich3Anzeige4.MouseEnter, RadTextBoxControlStaffel2Bereich2Anzeige4.MouseEnter, RadTextBoxControlStaffel2Bereich1Anzeige4.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahrenErsatzlastErsatzlast2)
    End Sub

#End Region
#End Region

#Region "Methods"


    ''' <summary>
    ''' Entsprechend der Vorgaben die Anzahl der Nullstellen für alle erforderlichen Steuerelemente setzen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetzeNullstellen()
        Try
            'nullstellen maske für EFG erzwingen
            For Each Control In RadScrollablePanel1.PanelContainer.Controls
                If TypeOf Control Is Telerik.WinControls.UI.RadGroupBox Then
                    If CType(Control, Telerik.WinControls.UI.RadGroupBox).Visible = True Then
                        For Each control2 In CType(Control, Telerik.WinControls.UI.RadGroupBox).Controls
                            If TypeOf control2 Is Telerik.WinControls.UI.RadGroupBox Then
                                For Each control3 In CType(control2, Telerik.WinControls.UI.RadGroupBox).Controls
                                    If TypeOf control3 Is Telerik.WinControls.UI.RadMaskedEditBox Then
                                        If CType(control3, Telerik.WinControls.UI.RadMaskedEditBox).Name.Contains("Wert") Then
                                            CType(control3, Telerik.WinControls.UI.RadMaskedEditBox).Mask = String.Format("F{0}", _intNullstellenE)
                                        End If
                                    End If
                                Next
                            End If
                        Next
                    End If
                End If
            Next
        Catch e As Exception
        End Try
    End Sub

    ''' <summary>
    '''    je nach Art der Waage andere Bereichsgruppen ausblenden
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub VersteckeBereiche()
        'je nach Art der Waage andere Bereichsgruppen ausblenden
        If objEichprozess.Lookup_Waagenart.Art = "Einbereichswaage" Then
            AnzahlBereiche = 1
            RadGroupBoxStaffel1Bereich2.Visible = False
            RadGroupBoxStaffel1Bereich3.Visible = False
            RadGroupBoxStaffel2Bereich2.Visible = False
            RadGroupBoxStaffel2Bereich3.Visible = False
            RadGroupBoxStaffel3Bereich2.Visible = False
            RadGroupBoxStaffel3Bereich3.Visible = False
            RadGroupBoxStaffel4Bereich2.Visible = False
            RadGroupBoxStaffel4Bereich3.Visible = False
            RadGroupBoxStaffel5Bereich2.Visible = False
            RadGroupBoxStaffel5Bereich3.Visible = False
        ElseIf objEichprozess.Lookup_Waagenart.Art = "Zweibereichswaage" Or objEichprozess.Lookup_Waagenart.Art = "Zweiteilungswaage" Then
            AnzahlBereiche = 2
            RadGroupBoxStaffel1Bereich2.Visible = True
            RadGroupBoxStaffel1Bereich3.Visible = False
            RadGroupBoxStaffel2Bereich2.Visible = True
            RadGroupBoxStaffel2Bereich3.Visible = False
            RadGroupBoxStaffel3Bereich2.Visible = True
            RadGroupBoxStaffel3Bereich3.Visible = False
            RadGroupBoxStaffel4Bereich2.Visible = True
            RadGroupBoxStaffel4Bereich3.Visible = False
            RadGroupBoxStaffel5Bereich2.Visible = True
            RadGroupBoxStaffel5Bereich3.Visible = False
        ElseIf objEichprozess.Lookup_Waagenart.Art = "Dreibereichswaage" Or objEichprozess.Lookup_Waagenart.Art = "Dreiteilungswaage" Then
            AnzahlBereiche = 3
            RadGroupBoxStaffel1Bereich2.Visible = True
            RadGroupBoxStaffel1Bereich3.Visible = True
            RadGroupBoxStaffel2Bereich2.Visible = True
            RadGroupBoxStaffel2Bereich3.Visible = True
            RadGroupBoxStaffel3Bereich2.Visible = True
            RadGroupBoxStaffel3Bereich3.Visible = True
            RadGroupBoxStaffel4Bereich2.Visible = True
            RadGroupBoxStaffel4Bereich3.Visible = True
            RadGroupBoxStaffel5Bereich2.Visible = True
            RadGroupBoxStaffel5Bereich3.Visible = True
        End If
    End Sub



    ''' <summary>
    ''' Methode welche Steuerelemente mit Werten aus übergebener Staffel und Bereich füllt. Lädt die Daten wenn vorhanden aus DB ansonsten werden Standardformeln verwendet
    ''' </summary>
    ''' <param name="Staffel"></param>
    ''' <param name="Bereich"></param>
    ''' <remarks></remarks>
    Private Sub LadeStaffel(ByVal Staffel As String, ByVal Bereich As String)
        Try
            If Staffel = "1" Then
                Dim _currentObjPruefungStaffelverfahrenNormallast As PruefungStaffelverfahrenNormallast

                _currentObjPruefungStaffelverfahrenNormallast = Nothing
                _currentObjPruefungStaffelverfahrenNormallast = (From o In _ListPruefungStaffelverfahrenNormallast Where o.Bereich = CStr(Bereich) And o.Staffel = CStr(Staffel)).FirstOrDefault

                If Not _currentObjPruefungStaffelverfahrenNormallast Is Nothing Then

                    For messpunkt As Integer = 1 To 7
                        Dim Last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), CInt(messpunkt)))
                        Dim Anzeige As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), CInt(messpunkt)))
                        Dim Fehler As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), CInt(messpunkt)))
                        Dim EFG As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), CInt(messpunkt)))

                        If messpunkt <= 4 Then
                            If Not Last Is Nothing Then
                                Last.Text = _currentObjPruefungStaffelverfahrenNormallast.GetType().GetProperty(String.Format("NormalLast_Last_{0}", messpunkt)).GetValue(_currentObjPruefungStaffelverfahrenNormallast, Nothing)
                            End If

                            If Not Anzeige Is Nothing Then
                                Anzeige.Text = _currentObjPruefungStaffelverfahrenNormallast.GetType().GetProperty(String.Format("NormalLast_Anzeige_{0}", messpunkt)).GetValue(_currentObjPruefungStaffelverfahrenNormallast, Nothing)
                            End If

                            If Not Fehler Is Nothing Then
                                Fehler.Text = _currentObjPruefungStaffelverfahrenNormallast.GetType().GetProperty(String.Format("NormalLast_Fehler_{0}", messpunkt)).GetValue(_currentObjPruefungStaffelverfahrenNormallast, Nothing)
                            End If

                            If Not EFG Is Nothing Then
                                EFG.Text = _currentObjPruefungStaffelverfahrenNormallast.GetType().GetProperty(String.Format("NormalLast_EFG_{0}", messpunkt)).GetValue(_currentObjPruefungStaffelverfahrenNormallast, Nothing)
                            End If

                        ElseIf messpunkt = 5 Then
                            If Not Fehler Is Nothing Then
                                Fehler.Text = _currentObjPruefungStaffelverfahrenNormallast.DifferenzAnzeigewerte_Fehler
                            End If
                            If Not EFG Is Nothing Then
                                EFG.Text = _currentObjPruefungStaffelverfahrenNormallast.DifferenzAnzeigewerte_EFG
                            End If
                        ElseIf messpunkt = 6 Then
                            If Not Fehler Is Nothing Then
                                Fehler.Text = _currentObjPruefungStaffelverfahrenNormallast.MessabweichungStaffel_Fehler
                            End If
                            If Not EFG Is Nothing Then
                                EFG.Text = _currentObjPruefungStaffelverfahrenNormallast.MessabweichungStaffel_EFG
                            End If
                        ElseIf messpunkt = 7 Then
                            If Not Fehler Is Nothing Then
                                Fehler.Text = _currentObjPruefungStaffelverfahrenNormallast.MessabweichungWaage_Fehler
                            End If
                            If Not EFG Is Nothing Then
                                EFG.Text = _currentObjPruefungStaffelverfahrenNormallast.MessabweichungWaage_EFG
                            End If
                        End If

                    Next

                Else 'Bereich wurde nicht gefunden => DB Ojekt ist leer. also mit Standardwerten füllen

                    ' standardwerte eintragen
                    If Bereich = 1 AndAlso AnzahlBereiche >= 1 Then 'die Anzahlbereich Abfrage sorgt dafür, dass wenn es z.b. keinen 3. bereich gibt, dieser auch nicht gefüllt wird.
                        RadTextBoxControlStaffel1Bereich1Last1.Text = "0"
                        RadTextBoxControlStaffel1Bereich1Last2.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min1

                        lblStaffel1Bereich1EFGWert5.Text = Math.Round(CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert1), _intNullstellenE) / 5
                        'last 2 Berechnen Wenn es mehr Normallast gibt, als Min benötigt wird, kann der MIN wert übernommen werden. sonst muss soviel genommen werden, wie da ist
                        If CDec(objEichprozess.Eichprotokoll.Pruefverfahren_BetragNormallast) > CDec(objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien) Then
                            RadTextBoxControlStaffel1Bereich1Last3.Text = objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien
                        Else
                            RadTextBoxControlStaffel1Bereich1Last3.Text = objEichprozess.Eichprotokoll.Pruefverfahren_BetragNormallast
                        End If
                        RadTextBoxControlStaffel1Bereich1Last4.Text = "0"
                    ElseIf Bereich = 2 AndAlso AnzahlBereiche >= 2 Then
                        RadTextBoxControlStaffel1Bereich2Last1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min2

                        'last 2 Berechnen Wenn es mehr Normallast gibt, als Min benötigt wird, kann der MIN wert übernommen werden. sonst muss soviel genommen werden, wie da ist
                        If CDec(objEichprozess.Eichprotokoll.Pruefverfahren_BetragNormallast) > CDec(objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien) Then
                            RadTextBoxControlStaffel1Bereich2Last3.Text = objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien
                        Else
                            RadTextBoxControlStaffel1Bereich2Last3.Text = objEichprozess.Eichprotokoll.Pruefverfahren_BetragNormallast
                        End If

                        RadTextBoxControlStaffel1Bereich2Last4.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min2
                        RadTextBoxControlStaffel1Bereich2Last1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min2

                        lblStaffel1Bereich2EFGWert5.Text = Math.Round(CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert2), _intNullstellenE) / 5
                    ElseIf Bereich = 3 AndAlso AnzahlBereiche >= 3 Then
                        RadTextBoxControlStaffel1Bereich3Last1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min3
                        'last 2 Berechnen
                        'last 2 Berechnen Wenn es mehr Normallast gibt, als Min benötigt wird, kann der MIN wert übernommen werden. sonst muss soviel genommen werden, wie da ist
                        If CDec(objEichprozess.Eichprotokoll.Pruefverfahren_BetragNormallast) > CDec(objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien) Then
                            RadTextBoxControlStaffel1Bereich2Last4.Text = objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien
                        Else
                            RadTextBoxControlStaffel1Bereich2Last4.Text = objEichprozess.Eichprotokoll.Pruefverfahren_BetragNormallast
                        End If
                        RadTextBoxControlStaffel1Bereich3Last4.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min3
                        RadTextBoxControlStaffel1Bereich3Last1.Text = objEichprozess.Eichprotokoll.Identifikationsdaten_Min3

                        lblStaffel1Bereich3EFGWert5.Text = Math.Round(CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert3), _intNullstellenE) / 5
                    End If

                End If

            Else 'staffel 2- 5 => andere Berechnungsroutinen
                Dim _currentObjPruefungStaffelverfahrenErsatzlast As PruefungStaffelverfahrenErsatzlast

                _currentObjPruefungStaffelverfahrenErsatzlast = Nothing
                _currentObjPruefungStaffelverfahrenErsatzlast = (From o In _ListPruefungStaffelverfahrenErsatzlast Where o.Bereich = CStr(Bereich) And o.Staffel = CStr(Staffel)).FirstOrDefault

                Dim Last1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 1))
                Dim Last2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 2))
                Dim Last3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 3))
                Dim Last4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 4))

                Dim Anzeige1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 1))
                Dim Anzeige3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 3))
                Dim Anzeige4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 4))

                Dim Fehler5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 5))
                Dim Fehler6 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 6))
                Dim Fehler7 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 7))

                Dim EFG5 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 5))
                Dim EFG6 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 6))
                Dim EFG7 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 7))

                If Not _currentObjPruefungStaffelverfahrenErsatzlast Is Nothing Then
                    Last1.Text = _currentObjPruefungStaffelverfahrenErsatzlast.Ersatzlast_Soll
                    Last2.Text = _currentObjPruefungStaffelverfahrenErsatzlast.ZusaetzlicheErsatzlast_Soll
                    Last3.Text = _currentObjPruefungStaffelverfahrenErsatzlast.ErsatzUndNormallast_Soll
                    Last4.Text = _currentObjPruefungStaffelverfahrenErsatzlast.Ersatzlast2_Soll

                    Anzeige1.Text = _currentObjPruefungStaffelverfahrenErsatzlast.Ersatzlast_Ist
                    Anzeige3.Text = _currentObjPruefungStaffelverfahrenErsatzlast.ErsatzUndNormallast_Ist
                    Anzeige4.Text = _currentObjPruefungStaffelverfahrenErsatzlast.Ersatzlast2_Ist

                    Fehler5.Text = _currentObjPruefungStaffelverfahrenErsatzlast.DifferenzAnzeigewerte_Fehler
                    Fehler6.Text = _currentObjPruefungStaffelverfahrenErsatzlast.MessabweichungStaffel_Fehler
                    Fehler7.Text = _currentObjPruefungStaffelverfahrenErsatzlast.MessabweichungWaage_Fehler

                    EFG5.Text = _currentObjPruefungStaffelverfahrenErsatzlast.DifferenzAnzeigewerte_EFG
                    EFG6.Text = _currentObjPruefungStaffelverfahrenErsatzlast.MessabweichungStaffel_EFG
                    EFG7.Text = _currentObjPruefungStaffelverfahrenErsatzlast.MessabweichungWaage_EFG
                Else 'standardweret eintragen, wenn DB Objekt nothing ist
                    Dim eichwert As Decimal
                    'eichwert auslesen
                    If Bereich = "1" AndAlso AnzahlBereiche >= 1 Then
                        eichwert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert1
                    ElseIf Bereich = "2" AndAlso AnzahlBereiche >= 2 Then
                        eichwert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert2
                    ElseIf Bereich = "3" AndAlso AnzahlBereiche >= 3 Then
                        eichwert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert3
                    End If
                    'EFG Normallast
                    EFG5.Text = Math.Round(eichwert, _intNullstellenE) / 5
                    'EFG Differenz der Staffel
                    'EFG differenz der Waage
                    '=WENN(B142<$B$50;WERT(1*$B$15);(1,5*$B$15))
                    If IsNumeric(Last3.Text) AndAlso IsNumeric(eichwert) Then
                        If CDec(Last3.Text) < Math.Round(CDec(eichwert * 2000), _intNullstellenE, MidpointRounding.AwayFromZero) Then
                            EFG6.Text = Math.Round(CDec(eichwert), _intNullstellenE)
                            EFG7.Text = Math.Round(CDec(eichwert), _intNullstellenE)
                        Else
                            EFG6.Text = Math.Round(CDec(eichwert), _intNullstellenE) * 1.5
                            EFG7.Text = Math.Round(CDec(eichwert), _intNullstellenE) * 1.5
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.StackTrace, ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' lädt alle Staffeln
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LadeStaffeln()
        For Staffel As Integer = 1 To 5
            For Bereich As Integer = 1 To 3
                LadeStaffel(Staffel, Bereich)
            Next
        Next
    End Sub


    Private Sub UeberschreibePruefungsobjekte()
        objEichprozess.Eichprotokoll.PruefungStaffelverfahrenErsatzlast.Clear()
        For Each obj In _ListPruefungStaffelverfahrenErsatzlast
            objEichprozess.Eichprotokoll.PruefungStaffelverfahrenErsatzlast.Add(obj)
        Next
        objEichprozess.Eichprotokoll.PruefungStaffelverfahrenNormallast.Clear()
        For Each obj In _ListPruefungStaffelverfahrenNormallast
            objEichprozess.Eichprotokoll.PruefungStaffelverfahrenNormallast.Add(obj)
        Next
    End Sub

    Private Sub UpdatePruefungsObject(ByVal PObjPruefung As PruefungStaffelverfahrenNormallast)
        Dim Staffel As String = PObjPruefung.Staffel
        Dim Bereich As String = PObjPruefung.Bereich

        Dim Last1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Last2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Last3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Last4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Anzeige1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Anzeige2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Anzeige3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Anzeige4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Fehler1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Fehler2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Fehler3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Fehler4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Fehler5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim Fehler6 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim Fehler7 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 7))
        Dim EFG1 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim EFG2 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim EFG3 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim EFG4 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim EFG5 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim EFG6 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim EFG7 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 7))

        PObjPruefung.NormalLast_Last_1 = Last1.Text

        If Bereich = "1" Then 'diese Felder gibt es nur im Bereich 1 weil dort zustäzlich gegen 0 geprüft werden muss
            PObjPruefung.NormalLast_Last_2 = Last2.Text
            PObjPruefung.NormalLast_Anzeige_2 = Anzeige2.Text
            PObjPruefung.NormalLast_Fehler_2 = Fehler2.Text
            PObjPruefung.NormalLast_EFG_2 = EFG2.Text
        Else
            PObjPruefung.NormalLast_Last_2 = 0
            PObjPruefung.NormalLast_Anzeige_2 = 0
            PObjPruefung.NormalLast_Fehler_2 = 0
            PObjPruefung.NormalLast_EFG_2 = 0
        End If

        PObjPruefung.NormalLast_Last_3 = Last3.Text
        PObjPruefung.NormalLast_Last_4 = Last4.Text
        PObjPruefung.NormalLast_Anzeige_1 = Anzeige1.Text

        PObjPruefung.NormalLast_Anzeige_3 = Anzeige3.Text
        PObjPruefung.NormalLast_Anzeige_4 = Anzeige4.Text
        PObjPruefung.NormalLast_Fehler_1 = Fehler1.Text
        PObjPruefung.NormalLast_Fehler_3 = Fehler3.Text
        PObjPruefung.NormalLast_Fehler_4 = Fehler4.Text
        PObjPruefung.NormalLast_EFG_1 = EFG1.Text
        PObjPruefung.NormalLast_EFG_3 = EFG3.Text
        PObjPruefung.NormalLast_EFG_4 = EFG4.Text
        PObjPruefung.DifferenzAnzeigewerte_Fehler = Fehler5.Text
        PObjPruefung.DifferenzAnzeigewerte_EFG = EFG5.Text
        PObjPruefung.MessabweichungStaffel_Fehler = Fehler6.Text
        PObjPruefung.MessabweichungStaffel_EFG = EFG6.Text
        PObjPruefung.MessabweichungWaage_Fehler = Fehler7.Text
        PObjPruefung.MessabweichungWaage_EFG = EFG7.Text
    End Sub

    Private Sub UpdatePruefungsObject(ByVal PObjPruefung As PruefungStaffelverfahrenErsatzlast)
        Dim Staffel As String = PObjPruefung.Staffel
        Dim Bereich As String = PObjPruefung.Bereich

        'suchen der Steuerelemente
        Dim Last1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Last2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Last3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Last4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Anzeige1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Anzeige3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Anzeige4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Fehler5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim Fehler6 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim Fehler7 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 7))
        Dim EFG5 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim EFG6 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim EFG7 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 7))

        'überschreiben der Objekteigenschaften
        PObjPruefung.Ersatzlast_Soll = Last1.Text
        PObjPruefung.Ersatzlast_Ist = Anzeige1.Text
        PObjPruefung.ZusaetzlicheErsatzlast_Soll = Last2.Text
        PObjPruefung.ErsatzUndNormallast_Soll = Last3.Text
        PObjPruefung.ErsatzUndNormallast_Ist = Anzeige3.Text
        PObjPruefung.Ersatzlast2_Soll = Last4.Text
        PObjPruefung.Ersatzlast2_Ist = Anzeige4.Text
        PObjPruefung.DifferenzAnzeigewerte_Fehler = Fehler5.Text
        PObjPruefung.DifferenzAnzeigewerte_EFG = EFG5.Text
        PObjPruefung.MessabweichungStaffel_Fehler = Fehler6.Text
        PObjPruefung.MessabweichungStaffel_EFG = EFG6.Text
        PObjPruefung.MessabweichungWaage_Fehler = Fehler7.Text
        PObjPruefung.MessabweichungWaage_EFG = EFG7.Text
    End Sub

    ''' <summary>
    ''' prüft innerhalb einer Staffel ob alle Felder ausgefüllt sind und gibt true zurück wenn dem so ist, prüft nur wenn überhaupt eintragungen vorgenommen wurden
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Private Function ValidateStaffelAusgefuellt(BereichGroupBox As Telerik.WinControls.UI.RadGroupBox, ByRef einFeldGefuelltInStaffel As Boolean) As Boolean
        Dim returnvalue As Boolean = True

        If BereichGroupBox.Visible = True Then 'Nur wenn der Bereich auch sichtbar ist
            For Each Control In BereichGroupBox.Controls
                Try
                    If TypeOf (Control) Is Telerik.WinControls.UI.RadTextBox Then
                        If CType(Control, Telerik.WinControls.UI.RadTextBox).ReadOnly = False And CType(Control, Telerik.WinControls.UI.RadTextBox).Enabled = True Then
                            If CType(Control, Telerik.WinControls.UI.RadTextBox).Text.Trim = "" Then
                                returnvalue = False
                                If einFeldGefuelltInStaffel = True Then
                                    CType(Control, Telerik.WinControls.UI.RadTextBox).TextBoxElement.Border.ForeColor = Color.Red
                                End If
                            Else
                                CType(Control, Telerik.WinControls.UI.RadTextBox).TextBoxElement.Border.ForeColor = Color.Transparent
                                einFeldGefuelltInStaffel = True
                            End If
                        End If
                    End If

                Catch ex As Exception
                End Try
            Next
        End If
        Return returnvalue
    End Function

    Private Sub ValidateStaffelEFG(staffel As Integer, bereich As Integer)
        Dim Fehler7 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(staffel), CInt(bereich), 7))
        Dim EFGWertStaffel As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(staffel), CInt(bereich), 7))

        Dim decAbsoluteFehlergrenze As Decimal = 0
        Try
            If Fehler7.Text.Trim.StartsWith("-") Then
                decAbsoluteFehlergrenze = CDec(Fehler7.Text.Replace("-", "")) 'entfernen des minuszeichens
            Else
                decAbsoluteFehlergrenze = CDec(Fehler7.Text)
            End If

            If decAbsoluteFehlergrenze > CDec(EFGWertStaffel.Text) Then 'eichwerte unterschritten/überschritten
                Fehler7.TextBoxElement.Border.ForeColor = Color.Red
                AbortSaving = True
            Else
                Fehler7.TextBoxElement.Border.ForeColor = Color.Transparent
            End If
        Catch ex As InvalidCastException
            Fehler7.TextBoxElement.Border.ForeColor = Color.Red
            AbortSaving = True

        End Try
    End Sub

    Private Sub BerechneStaffelBereich(ByVal Staffel As String, ByVal Bereich As String)
        Try
            Dim Last1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 1))
            If Last1.Visible = False Then Exit Sub

            Dim Eichwert As String = ""
            If Bereich = "1" AndAlso AnzahlBereiche >= 1 Then
                Eichwert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert1
            ElseIf Bereich = "2" AndAlso AnzahlBereiche >= 2 Then
                Eichwert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert2
            ElseIf Bereich = "3" AndAlso AnzahlBereiche >= 3 Then
                Eichwert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert3
            End If

            If Staffel = "1" Then
                BerechneStaffel1(Bereich, Staffel, Eichwert)
            Else 'staffel 2 - 7 werden anders berechnet
                BerechneStaffel2To7(Bereich, Staffel, Eichwert)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.StackTrace, ex.Message)
        End Try
    End Sub


    Private Sub BerechneStaffel1(Bereich As String, Staffel As String, Eichwert As String) ', Last1 As Telerik.WinControls.UI.RadTextBox, Last2 As Telerik.WinControls.UI.RadTextBox, Last3Normallast As Telerik.WinControls.UI.RadTextBox, ByRef Last4 As Telerik.WinControls.UI.RadTextBox, Anzeige1 As Telerik.WinControls.UI.RadTextBox, Anzeige2 As Telerik.WinControls.UI.RadTextBox, Anzeige3Normallast As Telerik.WinControls.UI.RadTextBox, Anzeige4 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler1 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler2 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler3 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler4 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler5 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler6 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler7 As Telerik.WinControls.UI.RadTextBox, ByRef EFG1 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG2 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG3 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG4 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG5 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG6 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG7 As Telerik.WinControls.UI.RadMaskedEditBox, Eichwert As String)
        Dim Last1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Last2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Last3Normallast As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Last4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Anzeige1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Anzeige2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Anzeige3Normallast As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Anzeige4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Fehler1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Fehler2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Fehler3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Fehler4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Fehler5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim Fehler6 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim Fehler7 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 7))
        Dim EFG1 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim EFG2 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim EFG3 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim EFG4 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim EFG5 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim EFG6 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim EFG7 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 7))

        'Im Bereich 1 wird gegen den nullwert geprüft. Ab bereich 2 gegen 20e
        If Bereich = "1" Then
            'fehler berechnen
            Try
                Fehler1.Text = CDec(Anzeige1.Text) - CDec(Last1.Text)
            Catch e As InvalidCastException
            End Try
            Try
                Fehler2.Text = CDec(Anzeige2.Text) - CDec(Last2.Text)
            Catch e As InvalidCastException
            End Try
        Else
            'fehler berechnen
            Try
                Fehler1.Text = CDec(Anzeige1.Text) - CDec(Last1.Text)
            Catch e As InvalidCastException
            End Try
        End If

        Try
            Fehler3.Text = CDec(Anzeige3Normallast.Text) - CDec(Last3Normallast.Text)
        Catch e As InvalidCastException
        End Try
        Try
            Fehler4.Text = CDec(Anzeige4.Text) - CDec(Last4.Text)
        Catch e As InvalidCastException
        End Try

        '4. last entspricht der ersten Last (üblicherweise 0)
        Last4.Text = Last1.Text

        'EFG Wert 1 und 2 Berechnen

        Try
            If CDec(Last1.Text) < Math.Round(CDec(Eichwert * 500), _intNullstellenE, MidpointRounding.AwayFromZero) Then
                'Im Bereich 1 wird gegen den nullwert geprüft. Ab bereich 2 gegen 20e. dort fällt EFG2 weg. stattdessen wird EFG3 genutzt
                If Bereich = "1" Then
                    EFG1.Text = Math.Round(CDec(Eichwert) * 0.5, _intNullstellenE)
                    EFG2.Text = Math.Round(CDec(Eichwert) * 0.5, _intNullstellenE)
                Else
                    EFG1.Text = Math.Round(CDec(Eichwert) * 0.5, _intNullstellenE)
                    EFG3.Text = Math.Round(CDec(Eichwert) * 0.5, _intNullstellenE)
                End If
            Else
                'Im Bereich 1 wird gegen den nullwert geprüft. Ab bereich 2 gegen 20e. dort fällt EFG2 weg. stattdessen wird EFG3 genutzt
                If Bereich = "1" Then
                    EFG1.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                    EFG2.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                Else

                    EFG1.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                    EFG3.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                End If
            End If
        Catch ex As InvalidCastException
        End Try

        'berechnen der Differenzen
        Try
            Fehler5.Text = CDec(Anzeige4.Text) - CDec(Anzeige1.Text)
        Catch ex As InvalidCastException
        End Try

        'messabweichung berechnen (abgeändert von Excel mappe. hier wird statt der min. normalien die eingebene Normalien Menge genommen
        Try
            'Fehler6.Text = CDec(Anzeige3.Text) - CDec(Anzeige1.Text) - CDec(Last3.Text) //alte Rechnung

            'weitere Anpassung nach Absprache mit Herrn Lüling und Herrn Strack:
            'Ermittlung der Messabweichung für eine Staffel

            'Für die Ermittlung der Messabweichung der Staffel 1 für den Bereich 1 gilt folgende Formel:
            'Messabweichung = Anzeigewert (Normallast) – Anzeigewert (Nullwert) – Normallast
            If Bereich = "1" Then
                Fehler6.Text = CDec(Anzeige3Normallast.Text) - CDec(Anzeige1.Text) - CDec(Last3Normallast.Text)
            Else
                'Für die Ermittlung der Messabweichung der Staffel 1 für die Bereiche 2 und 3 wird in der bestehenden Prüfanweisung keine Vorgehensweise beschrieben. Deshalb wird an dieser Stelle der Berechnung folgende Vereinbarung zugrunde gelegt: Die Mindestlast bleibt bei der Ermittlung der Messabweichung an dieser Stelle unberücksichtigt. Lediglich die Differenz von SOLL und IST der Mindestlast wird berücksichtigt.
                'Folgende Formel gilt:
                'Messabweichung = Anzeigewert (Normallast) – (Anzeigewert (Mindestlast) – Mindestlast) – Normallast
                Fehler6.Text = (CDec(Anzeige3Normallast.Text)) - (CDec(Anzeige1.Text) - CDec(Last1.Text)) - CDec(Last3Normallast.Text)
            End If

        Catch ex As InvalidCastException
        End Try

        'messabweichung zur WAage
        Try
            Fehler7.Text = Fehler6.Text
        Catch e As InvalidCastException
        End Try

        'EFG'Wert 3 Berechnen
        Try
            '=WENN(B130<$B$49;WERT(0,5*$B$15);(1*$B$15))
            If CDec(Last3Normallast.Text) < Math.Round(CDec(Eichwert * 500), _intNullstellenE, MidpointRounding.AwayFromZero) Then
                EFG3.Text = Math.Round(CDec(Eichwert * 0.5), _intNullstellenE)
            Else
                EFG3.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
            End If
        Catch e As InvalidCastException
        End Try

        'Berechnen von EFG  4
        Try
            '=WENN(B130<$B$49;WERT(0,5*$B$15);(1*$B$15))
            If CDec(Last4.Text) < Math.Round(CDec(Eichwert * 500), _intNullstellenE, MidpointRounding.AwayFromZero) Then
                EFG4.Text = Math.Round(CDec(Eichwert * 0.5), _intNullstellenE)
            Else
                EFG4.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
            End If
        Catch e As InvalidCastException
        End Try

        'berechnen von EFG 5
        Try
            EFG5.Text = Math.Round(CDec(Eichwert), _intNullstellenE) / 5
        Catch e As InvalidCastException
        End Try

        'Berechnen von EFG   6 und 7
        Try
            If CDec(Last3Normallast.Text) < Math.Round(CDec(Eichwert * 500), _intNullstellenE, MidpointRounding.AwayFromZero) Then
                EFG6.Text = Math.Round(CDec(Eichwert * 0.5), _intNullstellenE)
                EFG7.Text = Math.Round(CDec(Eichwert * 0.5), _intNullstellenE)
            Else
                EFG6.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                EFG7.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
            End If
        Catch e As InvalidCastException
        End Try
    End Sub

    Private Sub BerechneStaffel2To7(Bereich As String, Staffel As String, Eichwert As String) 'Last2 As Telerik.WinControls.UI.RadTextBox, ByRef Last3Normallast As Telerik.WinControls.UI.RadTextBox, ByRef Last4 As Telerik.WinControls.UI.RadTextBox, Anzeige1 As Telerik.WinControls.UI.RadTextBox, Anzeige3Normallast As Telerik.WinControls.UI.RadTextBox, Anzeige4 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler5 As Telerik.WinControls.UI.RadTextBox, ByRef Fehler6 As Telerik.WinControls.UI.RadTextBox, ByRef EFG5 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG6 As Telerik.WinControls.UI.RadMaskedEditBox, ByRef EFG7 As Telerik.WinControls.UI.RadMaskedEditBox, Eichwert As String)
        Dim Last1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Last2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 2))
        Dim Last3Normallast As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Last4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Last{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Anzeige1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 1))
        Dim Anzeige3Normallast As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 3))
        Dim Anzeige4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Anzeige{2}", CInt(Staffel), CInt(Bereich), 4))
        Dim Fehler5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim Fehler6 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim EFG5 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 5))
        Dim EFG6 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 6))
        Dim EFG7 As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblStaffel{0}Bereich{1}EFGWert{2}", CInt(Staffel), CInt(Bereich), 7))

        'lasten
        Try
            If IsNumeric(Last2.Text) And IsNumeric(Anzeige1.Text) Then
                Last3Normallast.Text = CDec(Last2.Text) + CDec(Anzeige1.Text)
            End If
        Catch e As Exception
        End Try
        Try
            Last4.Text = Anzeige1.Text
        Catch e As Exception
        End Try

        Try
            'Differenz Staffel
            If Last3Normallast.Text = "0" Then
                Fehler6.Text = "0"
            Else
                If IsNumeric(Anzeige3Normallast.Text) And IsNumeric(Anzeige1.Text) And IsNumeric(Last2.Text) Then
                    Fehler6.Text = (CDec(Anzeige3Normallast.Text) - CDec(Anzeige1.Text)) - CDec(Last2.Text)
                End If
            End If
        Catch e As Exception
        End Try

        Try
            'Differenze Normallast
            If IsNumeric(Anzeige4.Text) And IsNumeric(Anzeige1.Text) Then
                Fehler5.Text = CDec(Anzeige4.Text) - CDec(Anzeige1.Text)
            End If

        Catch e As Exception
        End Try

        Try
            'berechnen von EFG 5
            EFG5.Text = Math.Round(CDec(Eichwert), _intNullstellenE) / 5
        Catch e As Exception
        End Try
        Try
            'Berechne EFG Wert 6 und 7
            If IsNumeric(Last3Normallast.Text) Then

                If CDec(Last3Normallast.Text) < Math.Round(CDec(Eichwert * 2000), _intNullstellenE, MidpointRounding.AwayFromZero) Then
                    EFG6.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                    EFG7.Text = Math.Round(CDec(Eichwert), _intNullstellenE)
                Else
                    EFG6.Text = Math.Round(CDec(Eichwert), _intNullstellenE) * 1.5
                    EFG7.Text = Math.Round(CDec(Eichwert), _intNullstellenE) * 1.5
                End If
            End If

        Catch e As Exception
        End Try
    End Sub


#Region " Messabweichung der Waage [SEwi]"

    ''' <summary>
    ''' Messabweichung ergibt sich aus Summe der Fehler aller Staffeln
    ''' </summary>
    ''' <param name="Bereich"></param>
    ''' <remarks></remarks>
    Private Sub BerechneMessabweichung(ByVal Bereich As String)
        Try

            'die Messabweichung durch alle Staffeln durchreichen
            Dim Fehler6Staffel1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 1, CInt(Bereich), 6))
            If Fehler6Staffel1.Visible = False Then Exit Sub
            Dim Fehler7Staffel1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 1, CInt(Bereich), 7))

            'staffel 1
            If IsNumeric(Fehler6Staffel1.Text) Then
                'differenz der Waage Berechnen
                Fehler7Staffel1.Text = CDec(Fehler6Staffel1.Text)

                'staffel 2
                Dim Fehler6Staffel2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 2, CInt(Bereich), 6))
                Dim Fehler7Staffel2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 2, CInt(Bereich), 7))

                If IsNumeric(Fehler6Staffel2.Text) Then
                    Fehler7Staffel2.Text = CDec(Fehler6Staffel1.Text) + CDec(Fehler6Staffel2.Text)
                    'staffel 3
                    Dim Fehler6Staffel3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 3, CInt(Bereich), 6))
                    Dim Fehler7Staffel3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 3, CInt(Bereich), 7))

                    If IsNumeric(Fehler6Staffel3.Text) Then
                        Fehler7Staffel3.Text = CDec(Fehler6Staffel1.Text) + CDec(Fehler6Staffel2.Text) + CDec(Fehler6Staffel3.Text)

                        'staffel 4
                        Dim Fehler6Staffel4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 4, CInt(Bereich), 6))
                        Dim Fehler7Staffel4 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 4, CInt(Bereich), 7))

                        If IsNumeric(Fehler6Staffel4.Text) Then
                            Fehler7Staffel4.Text = CDec(Fehler6Staffel1.Text) + CDec(Fehler6Staffel2.Text) + CDec(Fehler6Staffel3.Text) + CDec(Fehler6Staffel4.Text)

                            'staffel 5
                            Dim Fehler6Staffel5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 5, CInt(Bereich), 6))
                            Dim Fehler7Staffel5 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlStaffel{0}Bereich{1}Fehler{2}", 5, CInt(Bereich), 7))
                            If IsNumeric(Fehler6Staffel5.Text) Then
                                Fehler7Staffel5.Text = CDec(Fehler6Staffel1.Text) + CDec(Fehler6Staffel2.Text) + CDec(Fehler6Staffel3.Text) + CDec(Fehler6Staffel4.Text) + CDec(Fehler6Staffel5.Text)
                            Else
                                Fehler7Staffel5.Text = ""
                                Exit Sub
                            End If
                        Else
                            Fehler7Staffel4.Text = ""
                            Exit Sub
                        End If
                    Else
                        Fehler7Staffel3.Text = ""
                        Exit Sub
                    End If
                Else
                    Fehler7Staffel2.Text = ""
                    Exit Sub
                End If
            Else
                Fehler7Staffel1.Text = ""
                Exit Sub
            End If

        Catch ex As Exception
        End Try
    End Sub

#End Region
#End Region


#Region "Interface Methods"
    Protected Friend Overrides Sub SetzeUeberschrift() Implements IRhewaEditingDialog.SetzeUeberschrift
        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungStaffelverfahren)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungStaffelverfahren
            Catch ex As Exception
            End Try
        End If
    End Sub
    Protected Friend Overrides Sub LoadFromDatabase() Implements IRhewaEditingDialog.LoadFromDatabase
        SuspendLayout()
        objEichprozess = ParentFormular.CurrentEichprozess
        'events abbrechen
        _suspendEvents = True
        'Nur laden wenn es sich um eine Bearbeitung handelt (sonst würde das in Memory Objekt überschrieben werden)
        If Not DialogModus = enuDialogModus.lesend And Not DialogModus = enuDialogModus.korrigierend Then
            Using context As New Entities
                'neu laden des Objekts, diesmal mit den lookup Objekten
                objEichprozess = (From a In context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault

                'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                Dim query = From a In context.PruefungStaffelverfahrenNormallast Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungStaffelverfahrenNormallast = query.ToList

                'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                Dim query2 = From a In context.PruefungStaffelverfahrenErsatzlast Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungStaffelverfahrenErsatzlast = query2.ToList

            End Using

        Else
            _ListPruefungStaffelverfahrenNormallast.Clear()
            _ListPruefungStaffelverfahrenErsatzlast.Clear()
            Try
                For Each obj In objEichprozess.Eichprotokoll.PruefungStaffelverfahrenNormallast
                    obj.Eichprotokoll = objEichprozess.Eichprotokoll

                    _ListPruefungStaffelverfahrenNormallast.Add(obj)
                Next
                For Each obj In objEichprozess.Eichprotokoll.PruefungStaffelverfahrenErsatzlast
                    obj.Eichprotokoll = objEichprozess.Eichprotokoll

                    _ListPruefungStaffelverfahrenErsatzlast.Add(obj)
                Next
            Catch ex As System.ObjectDisposedException 'fehler im Clientseitigen Lesemodus (bei bereits abegschickter Eichung)
                Using context As New Entities
                    'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen

                    Dim query = From a In context.PruefungStaffelverfahrenNormallast Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                    _ListPruefungStaffelverfahrenNormallast = query.ToList

                    'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
                    Dim query2 = From a In context.PruefungStaffelverfahrenErsatzlast Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                    _ListPruefungStaffelverfahrenErsatzlast = query2.ToList
                End Using
            End Try

        End If

        'steuerelemente mit werten aus DB füllen
        FillControls()

        If DialogModus = enuDialogModus.lesend Then
            DisableControls(RadGroupBoxStaffel1Bereich1)
            DisableControls(RadGroupBoxStaffel1Bereich2)
            DisableControls(RadGroupBoxStaffel1Bereich3)
            DisableControls(RadGroupBoxStaffel2Bereich1)
            DisableControls(RadGroupBoxStaffel2Bereich2)
            DisableControls(RadGroupBoxStaffel2Bereich3)
            DisableControls(RadGroupBoxStaffel3Bereich1)
            DisableControls(RadGroupBoxStaffel3Bereich2)
            DisableControls(RadGroupBoxStaffel3Bereich3)
            DisableControls(RadGroupBoxStaffel4Bereich1)
            DisableControls(RadGroupBoxStaffel4Bereich2)
            DisableControls(RadGroupBoxStaffel4Bereich3)
            DisableControls(RadGroupBoxStaffel5Bereich1)
            DisableControls(RadGroupBoxStaffel5Bereich2)
            DisableControls(RadGroupBoxStaffel5Bereich3)
        End If

        'events abbrechen
        _suspendEvents = False
        ResumeLayout()
    End Sub

    ''' <summary>
    ''' Lädt die Werte aus dem Objekt in die Steuerlemente
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub FillControls() Implements IRhewaEditingDialog.FillControls
        HoleNullstellen()
        SetzeNullstellen()

        'je nach Art der Waage andere Bereichsgruppen ausblenden
        VersteckeBereiche()

        'füllen der berechnenten Steuerelemente
        LadeStaffeln()

        'fokus setzen auf erstes Steuerelement
        RadTextBoxControlStaffel1Bereich1Anzeige1.Focus()
    End Sub

    ''' <summary>
    ''' Füllt das Objekt mit den Werten aus den Steuerlementen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub UpdateObjekt() Implements IRhewaEditingDialog.UpdateObjekt
        If DialogModus = enuDialogModus.normal Then objEichprozess.Bearbeitungsdatum = Date.Now

        'neuen Context aufbauen
        Using Context As New Entities
            'jedes objekt initialisieren und aus context laden und updaten
            For Each obj In _ListPruefungStaffelverfahrenNormallast
                Dim objPruefung = Context.PruefungStaffelverfahrenNormallast.FirstOrDefault(Function(value) value.ID = obj.ID)
                If Not objPruefung Is Nothing Then
                    'in lokaler DB gucken
                    UpdatePruefungsObject(objPruefung)
                Else 'es handelt sich um eine Serverobjekt im => Korrekturmodus
                    If DialogModus = enuDialogModus.korrigierend Then
                        UpdatePruefungsObject(obj)
                    End If
                End If
            Next

            'jedes objekt initialisieren und aus context laden und updaten
            For Each obj In _ListPruefungStaffelverfahrenErsatzlast
                Dim objPruefung = Context.PruefungStaffelverfahrenErsatzlast.FirstOrDefault(Function(value) value.ID = obj.ID)
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
        AbortSaving = False

        For staffel As Integer = 1 To 5
            For bereich As Integer = 1 To 3
                BerechneStaffelBereich(staffel, bereich)
                BerechneMessabweichung(bereich)
            Next
        Next

        Dim intausgefuellteStaffeln As Integer = 0
        Dim einFeldGefuelltInStaffel As Boolean = False
        If ValidateStaffelAusgefuellt(RadGroupBoxStaffel1Bereich1, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel1Bereich2, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel1Bereich3, einFeldGefuelltInStaffel) Or einFeldGefuelltInStaffel Then
            intausgefuellteStaffeln = 1
            einFeldGefuelltInStaffel = False
            If ValidateStaffelAusgefuellt(RadGroupBoxStaffel2Bereich1, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel2Bereich2, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel2Bereich3, einFeldGefuelltInStaffel) Or einFeldGefuelltInStaffel Then
                intausgefuellteStaffeln = 2
                einFeldGefuelltInStaffel = False
                If ValidateStaffelAusgefuellt(RadGroupBoxStaffel3Bereich1, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel3Bereich2, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel3Bereich3, einFeldGefuelltInStaffel) Or einFeldGefuelltInStaffel Then
                    intausgefuellteStaffeln = 3
                    einFeldGefuelltInStaffel = False
                    If ValidateStaffelAusgefuellt(RadGroupBoxStaffel4Bereich1, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel4Bereich2, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel4Bereich3, einFeldGefuelltInStaffel) Or einFeldGefuelltInStaffel Then
                        intausgefuellteStaffeln = 4
                        einFeldGefuelltInStaffel = False
                        If ValidateStaffelAusgefuellt(RadGroupBoxStaffel5Bereich1, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel5Bereich2, einFeldGefuelltInStaffel) And ValidateStaffelAusgefuellt(RadGroupBoxStaffel5Bereich3, einFeldGefuelltInStaffel) Or einFeldGefuelltInStaffel Then
                            intausgefuellteStaffeln = 5
                        End If
                    End If
                End If
            End If
        Else : AbortSaving = True 'eine Staffel muss ausgefüllt sein
        End If

        If AbortSaving Then
            'fehlermeldung anzeigen bei falscher validierung
            Dim result = Me.ShowValidationErrorBox(False)
            Return ProcessResult(result)


        End If

        'logik zum Valideren der Eichfehlergrenzen der einzelnen Staffeln. Abhängig davon wieviele Staffeln überhaupt ausgefüllt sind
        If (intausgefuellteStaffeln >= 1) Then
            'Staffel 1
            ValidateStaffelEFG(1, 1)
            If AnzahlBereiche >= 2 Then ValidateStaffelEFG(1, 2)
            If AnzahlBereiche >= 3 Then ValidateStaffelEFG(1, 3)
        End If

        'staffel 2
        If (intausgefuellteStaffeln >= 2) Then
            ValidateStaffelEFG(2, 1)
            If AnzahlBereiche >= 2 Then ValidateStaffelEFG(2, 2)
            If AnzahlBereiche >= 3 Then ValidateStaffelEFG(2, 3)
        End If
        'staffel 3
        If (intausgefuellteStaffeln >= 3) Then
            ValidateStaffelEFG(3, 1)
            If AnzahlBereiche >= 2 Then ValidateStaffelEFG(3, 2)
            If AnzahlBereiche >= 3 Then ValidateStaffelEFG(3, 3)
        End If
        'staffel 4
        If (intausgefuellteStaffeln >= 4) Then
            ValidateStaffelEFG(4, 1)
            If AnzahlBereiche >= 2 Then ValidateStaffelEFG(4, 2)
            If AnzahlBereiche >= 3 Then ValidateStaffelEFG(4, 3)
        End If
        'staffel 5
        If (intausgefuellteStaffeln >= 5) Then
            ValidateStaffelEFG(5, 1)
            If AnzahlBereiche >= 2 Then ValidateStaffelEFG(5, 2)
            If AnzahlBereiche >= 3 Then ValidateStaffelEFG(5, 3)
        End If

        'fehlermeldung anzeigen bei falscher validierung

        If AbortSaving Then
            'fehlermeldung anzeigen bei falscher validierung
            Dim result = Me.ShowValidationErrorBox(False, My.Resources.GlobaleLokalisierung.EichfehlergrenzenNichtEingehalten)
            Return ProcessResult(result)

        End If

        Return True

    End Function




    Protected Friend Overrides Sub Lokalisiere() Implements IRhewaEditingDialog.Lokalisiere
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(uco10PruefungStaffelverfahren))
        Lokalisierung(Me, resources)
    End Sub


    Protected Friend Overrides Sub Entsperrung() Implements IRhewaEditingDialog.Entsperrung
        'Hiermit wird ein lesender Vorgang wieder entsperrt.
        EnableControls(RadGroupBoxStaffel1Bereich1)
        EnableControls(RadGroupBoxStaffel1Bereich2)
        EnableControls(RadGroupBoxStaffel1Bereich3)
        EnableControls(RadGroupBoxStaffel2Bereich1)
        EnableControls(RadGroupBoxStaffel2Bereich2)
        EnableControls(RadGroupBoxStaffel2Bereich3)
        EnableControls(RadGroupBoxStaffel3Bereich1)
        EnableControls(RadGroupBoxStaffel3Bereich2)
        EnableControls(RadGroupBoxStaffel3Bereich3)
        EnableControls(RadGroupBoxStaffel4Bereich1)
        EnableControls(RadGroupBoxStaffel4Bereich2)
        EnableControls(RadGroupBoxStaffel4Bereich3)
        EnableControls(RadGroupBoxStaffel5Bereich1)
        EnableControls(RadGroupBoxStaffel5Bereich2)
        EnableControls(RadGroupBoxStaffel5Bereich3)

        'ändern des Moduses
        DialogModus = enuDialogModus.korrigierend
        ParentFormular.DialogModus = FrmMainContainer.enuDialogModus.korrigierend
    End Sub


    Protected Friend Overrides Sub Versenden() Implements IRhewaEditingDialog.Versenden
        UpdateObjekt()
        UeberschreibePruefungsobjekte()

        'Erzeugen eines Server Objektes auf basis des aktuellen DS. Setzt es auf es ausserdem auf Fehlerhaft
        CloneAndSendServerObjekt()
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
                    If _ListPruefungStaffelverfahrenNormallast.Count = 0 Then
                        'anzahl Bereiche auslesen um damit die anzahl der benötigten Iterationen und Objekt Erzeugungen zu erfahren
                        Dim intBereiche As Integer = GetAnzahlBereiche()

                        For intBereich = 1 To intBereiche

                            Dim objPruefung = Context.PruefungStaffelverfahrenNormallast.Create
                            'wenn es die eine itereation mehr ist:
                            objPruefung.Bereich = intBereich
                            objPruefung.Staffel = 1

                            UpdatePruefungsObject(objPruefung)

                            Context.SaveChanges()

                            objEichprozess.Eichprotokoll.PruefungStaffelverfahrenNormallast.Add(objPruefung)
                            Try
                                Context.SaveChanges()

                            Catch ex As Exception
                                lblStaffel1Bereich1Anzeige.Text = ""
                            End Try

                            _ListPruefungStaffelverfahrenNormallast.Add(objPruefung)
                        Next

                    Else ' es gibt bereits welche
                        'jedes objekt initialisieren und aus context laden und updaten
                        For Each objPruefung In _ListPruefungStaffelverfahrenNormallast
                            objPruefung = Context.PruefungStaffelverfahrenNormallast.FirstOrDefault(Function(value) value.ID = objPruefung.ID)
                            UpdatePruefungsObject(objPruefung)
                            Context.SaveChanges()
                        Next

                    End If

                    'ersatzlasten
                    'wenn es defintiv noch keine pruefungen gibt, neue Anlegen
                    If _ListPruefungStaffelverfahrenErsatzlast.Count = 0 Then
                        'anzahl Bereiche auslesen um damit die anzahl der benötigten Iterationen und Objekt Erzeugungen zu erfahren
                        Dim intBereiche As Integer = GetAnzahlBereiche()

                        For intStaffel As Integer = 2 To 5

                            For intBereich = 1 To intBereiche

                                Dim objPruefung = Context.PruefungStaffelverfahrenErsatzlast.Create
                                'wenn es die eine itereation mehr ist:
                                objPruefung.Bereich = intBereich
                                objPruefung.Staffel = intStaffel

                                UpdatePruefungsObject(objPruefung)

                                Context.SaveChanges()

                                objEichprozess.Eichprotokoll.PruefungStaffelverfahrenErsatzlast.Add(objPruefung)
                                Context.SaveChanges()

                                _ListPruefungStaffelverfahrenErsatzlast.Add(objPruefung)
                            Next
                        Next

                    Else ' es gibt bereits welche
                        'jedes objekt initialisieren und aus context laden und updaten
                        For Each objPruefung In _ListPruefungStaffelverfahrenErsatzlast
                            objPruefung = Context.PruefungStaffelverfahrenErsatzlast.FirstOrDefault(Function(value) value.ID = objPruefung.ID)
                            UpdatePruefungsObject(objPruefung)
                            Context.SaveChanges()
                        Next
                    End If
                End If
            End If



        End Using

        AktualisiereStatus()

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

                    'neuen Status zuweisen
                    If AktuellerStatusDirty = False Then
                        ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                        If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit Then
                            objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit
                        End If
                    ElseIf AktuellerStatusDirty = True Then
                        objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit
                        AktuellerStatusDirty = False
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
            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit Then
                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit
            End If
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If

        If DialogModus = enuDialogModus.korrigierend Then
            UpdateObjekt()
            If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit Then
                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderWiederholbarkeit
            End If
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If
        Return True
    End Function




#End Region

End Class