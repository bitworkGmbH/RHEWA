Imports System
Imports System.Data.Entity

Friend Class uco_8PruefungNullstellungUndAussermittigeBelastung

    Inherits ucoContent
    Implements IRhewaEditingDialog
    Implements IRhewaPruefungDialog

#Region "Member Variables"
    Private _suspendEvents As Boolean = False 'Variable zum temporären stoppen der Eventlogiken
    'Private AktuellerStatusDirty As Boolean = False 'variable die genutzt wird, um bei öffnen eines existierenden Eichprozesses speichern zu können wenn grundlegende Änderungen vorgenommen wurden.
    'Wie das ändern der Waagenart und der Waegezelle. Dann wird der Vorgang auf Komptabilitätsnachweis zurückgesetzt
    Private _ListPruefungAussermittigeBelastung As New List(Of PruefungAussermittigeBelastung)
    Private _currentObjPruefungAussermittigeBelastung As PruefungAussermittigeBelastung
    Private _currentObjPruefungWiederholbarkeit As PruefungWiederholbarkeit
    Private _ListPruefungWiederholbarkeit As New List(Of PruefungWiederholbarkeit)

    Private _currentObjVerfahren As Lookup_Konformitaetsbewertungsverfahren
#End Region

#Region "Constructors"
    Sub New()
        MyBase.New()
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
    End Sub

    Sub New(ByRef pParentform As FrmMainContainer, ByRef pObjEichprozess As Eichprozess, Optional ByRef pPreviousUco As ucoContent = Nothing, Optional ByRef pNextUco As ucoContent = Nothing, Optional ByVal pEnuModus As enuDialogModus = enuDialogModus.normal)
        MyBase.New(pParentform, pObjEichprozess, pPreviousUco, pNextUco, pEnuModus)
        _suspendEvents = True
        InitializeComponent()
        _suspendEvents = False

        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderGenauigkeitderNullstellungUndAussermittigeBelastung

    End Sub

#End Region

#Region "Events"

    Private Sub ucoBeschaffenheitspruefung_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SetzeUeberschrift()
        EichprozessStatusReihenfolge = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderGenauigkeitderNullstellungUndAussermittigeBelastung

        'daten füllen
        LoadFromDatabase()
    End Sub


    ''' <summary>
    ''' Öffnen der Eichfehlergrenzen
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadButtonShowEFG_Click(sender As Object, e As EventArgs) Handles RadButtonShowEFG.Click, RadButtonShowEFG2.Click
        ShowEichfehlergrenzenDialog()
    End Sub

    Private Sub RadTextBoxControlWeight_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles RadTextBoxControlWeight3.Validating, RadTextBoxControlWeight2.Validating, RadTextBoxControlWeight1.Validating,
        RadTextBoxControlDisplayWeight3.Validating, RadTextBoxControlDisplayWeight2.Validating, RadTextBoxControlDisplayWeight1.Validating, RadTextBoxControlBereich3Weight9.Validating,
        RadTextBoxControlBereich3Weight8.Validating, RadTextBoxControlBereich3Weight7.Validating, RadTextBoxControlBereich3Weight6.Validating, RadTextBoxControlBereich3Weight5.Validating, RadTextBoxControlBereich3Weight4.Validating,
        RadTextBoxControlBereich3Weight3.Validating, RadTextBoxControlBereich3Weight2.Validating, RadTextBoxControlBereich3Weight12.Validating, RadTextBoxControlBereich3Weight11.Validating, RadTextBoxControlBereich3Weight10.Validating,
        RadTextBoxControlBereich3Weight1.Validating, RadTextBoxControlBereich3DisplayWeight9.Validating, RadTextBoxControlBereich3DisplayWeight8.Validating,
        RadTextBoxControlBereich3DisplayWeight7.Validating, RadTextBoxControlBereich3DisplayWeight6.Validating, RadTextBoxControlBereich3DisplayWeight5.Validating, RadTextBoxControlBereich3DisplayWeight4.Validating,
RadTextBoxControlBereich3DisplayWeight3.Validating, RadTextBoxControlBereich3DisplayWeight2.Validating, RadTextBoxControlBereich3DisplayWeight12.Validating, RadTextBoxControlBereich3DisplayWeight11.Validating,
RadTextBoxControlBereich3DisplayWeight10.Validating, RadTextBoxControlBereich3DisplayWeight1.Validating, RadTextBoxControlBereich2Weight9.Validating, RadTextBoxControlBereich2Weight8.Validating,
RadTextBoxControlBereich2Weight7.Validating, RadTextBoxControlBereich2Weight6.Validating, RadTextBoxControlBereich2Weight5.Validating, RadTextBoxControlBereich2Weight4.Validating, RadTextBoxControlBereich2Weight3.Validating,
RadTextBoxControlBereich2Weight2.Validating, RadTextBoxControlBereich2Weight12.Validating, RadTextBoxControlBereich2Weight11.Validating, RadTextBoxControlBereich2Weight10.Validating, RadTextBoxControlBereich2Weight1.Validating, RadTextBoxControlBereich2DisplayWeight9.Validating, RadTextBoxControlBereich2DisplayWeight8.Validating, RadTextBoxControlBereich2DisplayWeight7.Validating,
RadTextBoxControlBereich2DisplayWeight6.Validating, RadTextBoxControlBereich2DisplayWeight5.Validating, RadTextBoxControlBereich2DisplayWeight4.Validating, RadTextBoxControlBereich2DisplayWeight3.Validating,
RadTextBoxControlBereich2DisplayWeight2.Validating, RadTextBoxControlBereich2DisplayWeight12.Validating, RadTextBoxControlBereich2DisplayWeight11.Validating, RadTextBoxControlBereich2DisplayWeight10.Validating,
RadTextBoxControlBereich2DisplayWeight1.Validating, RadTextBoxControlBereich1WeightMitte.Validating, RadTextBoxControlBereich1Weight9.Validating, RadTextBoxControlBereich1Weight8.Validating, RadTextBoxControlBereich1Weight7.Validating,
RadTextBoxControlBereich1Weight6.Validating, RadTextBoxControlBereich1Weight5.Validating, RadTextBoxControlBereich1Weight4.Validating, RadTextBoxControlBereich1Weight3.Validating, RadTextBoxControlBereich1Weight2.Validating,
RadTextBoxControlBereich1Weight12.Validating, RadTextBoxControlBereich1Weight11.Validating, RadTextBoxControlBereich1Weight10.Validating, RadTextBoxControlBereich1Weight1.Validating, RadTextBoxControlBereich1DisplayWeightMitte.Validating,
RadTextBoxControlBereich1DisplayWeight9.Validating, RadTextBoxControlBereich1DisplayWeight8.Validating, RadTextBoxControlBereich1DisplayWeight7.Validating, RadTextBoxControlBereich1DisplayWeight6.Validating,
RadTextBoxControlBereich1DisplayWeight5.Validating, RadTextBoxControlBereich1DisplayWeight4.Validating, RadTextBoxControlBereich1DisplayWeight3.Validating, RadTextBoxControlBereich1DisplayWeight2.Validating,
RadTextBoxControlBereich1DisplayWeight12.Validating, RadTextBoxControlBereich1DisplayWeight11.Validating, RadTextBoxControlBereich1DisplayWeight10.Validating, RadTextBoxControlBereich1DisplayWeight1.Validating

        'basis validierung
        BasicTextboxValidation(sender, e)
    End Sub

    Private Sub RadCheckBoxBereich1VEL1_MouseClick(sender As Object, e As MouseEventArgs) Handles RadCheckBoxBereich3VEL9.MouseClick, RadCheckBoxBereich3VEL8.MouseClick, RadCheckBoxBereich3VEL7.MouseClick,
          RadCheckBoxBereich3VEL6.MouseClick, RadCheckBoxBereich3VEL5.MouseClick, RadCheckBoxBereich3VEL4.MouseClick, RadCheckBoxBereich3VEL3.MouseClick, RadCheckBoxBereich3VEL2.MouseClick, RadCheckBoxBereich3VEL12.MouseClick,
          RadCheckBoxBereich3VEL11.MouseClick, RadCheckBoxBereich3VEL10.MouseClick, RadCheckBoxBereich3VEL1.MouseClick, RadCheckBoxBereich2VEL9.MouseClick, RadCheckBoxBereich2VEL8.MouseClick,
          RadCheckBoxBereich2VEL7.MouseClick, RadCheckBoxBereich2VEL6.MouseClick, RadCheckBoxBereich2VEL5.MouseClick, RadCheckBoxBereich2VEL4.MouseClick, RadCheckBoxBereich2VEL3.MouseClick, RadCheckBoxBereich2VEL2.MouseClick,
          RadCheckBoxBereich2VEL12.MouseClick, RadCheckBoxBereich2VEL11.MouseClick, RadCheckBoxBereich2VEL10.MouseClick, RadCheckBoxBereich2VEL1.MouseClick, RadCheckBoxBereich1VELMitte.MouseClick, RadCheckBoxBereich1VEL9.MouseClick,
          RadCheckBoxBereich1VEL8.MouseClick, RadCheckBoxBereich1VEL7.MouseClick, RadCheckBoxBereich1VEL6.MouseClick, RadCheckBoxBereich1VEL5.MouseClick, RadCheckBoxBereich1VEL4.MouseClick, RadCheckBoxBereich1VEL3.MouseClick,
          RadCheckBoxBereich1VEL2.MouseClick, RadCheckBoxBereich1VEL12.MouseClick, RadCheckBoxBereich1VEL11.MouseClick, RadCheckBoxBereich1VEL10.MouseClick, RadCheckBoxBereich1VEL1.MouseClick, RadCheckBoxVEL1.Click

        'negieren der checkbox
        CType(sender, Telerik.WinControls.UI.RadCheckBox).Checked = Not CType(sender, Telerik.WinControls.UI.RadCheckBox).Checked
    End Sub

#Region "Wiederholbarkeit Text Changed Events"
    Private Sub RadTextBoxControlErrorLimit1_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlErrorLimit1.TextChanged
        GetBetragNormallast()
    End Sub

    ''' <summary>
    ''' wenn sich eine der Last Werte ändert, muss es in allen anderen Textboxen nachgezogen werden
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControl_TextChanged(sender As Object, e As EventArgs) Handles _
    RadTextBoxControlWeight3.TextChanged, RadTextBoxControlWeight2.TextChanged, RadTextBoxControlWeight1.TextChanged

        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True

        'damit keine Event Kettenreaktion durchgeführt wird, werden die Events ab hier unterbrochen
        _suspendEvents = True

        FuelleLastBeiAenderungen(sender)

        'neu berechnen der Fehler und EFG
        CalculateEFGWiederholungen(RadTextBoxControlWeight1)
        CalculateEFGWiederholungen(RadTextBoxControlWeight2)
        CalculateEFGWiederholungen(RadTextBoxControlWeight3)

        _suspendEvents = False
    End Sub

    Private Sub RadTextBoxControlDisplayWeight1_TextChanged(sender As Object, e As EventArgs) Handles RadTextBoxControlDisplayWeight1.TextChanged, RadTextBoxControlDisplayWeight2.TextChanged, RadTextBoxControlDisplayWeight3.TextChanged
        CalculateEFGWiederholungen(sender)
    End Sub

#End Region

#Region "Wiederholbarkeit Methoden"

    Private Sub FuelleLastBeiAenderungen(sender As Object)
        RadTextBoxControlWeight1.Text = CType(sender, Telerik.WinControls.UI.RadTextBox).Text
        RadTextBoxControlWeight2.Text = CType(sender, Telerik.WinControls.UI.RadTextBox).Text
        RadTextBoxControlWeight3.Text = CType(sender, Telerik.WinControls.UI.RadTextBox).Text
    End Sub

    Private Sub GetBetragNormallast()
        Try

            Dim MAX20 As Decimal = 0
            Dim MAX35 As Decimal = 0
            Dim MAX50 As Decimal = 0
            Dim newLast As Decimal = 0

            GetMaxWerte(MAX20, MAX35, MAX50)

            'wiederholung1

            Dim AnzeigeMax1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlDisplayWeight{0}", 1))
            Dim AnzeigeMax2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlDisplayWeight{0}", 2))
            Dim AnzeigeMax3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlDisplayWeight{0}", 3))

            Dim listdecimals As New List(Of Decimal)
            If IsNumeric(AnzeigeMax1.Text) Then
                listdecimals.Add(AnzeigeMax1.Text)
            End If
            If IsNumeric(AnzeigeMax2.Text) Then
                listdecimals.Add(AnzeigeMax2.Text)
            End If
            If IsNumeric(AnzeigeMax3.Text) Then
                listdecimals.Add(AnzeigeMax3.Text)
            End If
            Dim max As Decimal = 0
            Dim min As Decimal = 0
            If listdecimals.Count = 3 Then
                max = listdecimals.Max
                min = listdecimals.Min

                Dim differenz As Decimal = max - min

                Try
                    If (differenz) > (CDec(lblEFGSpeziallBerechnung.Text) * 0.33) Then
                        newLast = MAX50
                    ElseIf (differenz) < (CDec(lblEFGSpeziallBerechnung.Text) * 0.22) Then
                        newLast = MAX20
                    ElseIf (differenz) < (CDec(lblEFGSpeziallBerechnung.Text) * 0.33) Then
                        newLast = MAX35
                    Else
                        newLast = MAX20
                    End If
                    RadTextBoxControlBetragNormallast.Text = newLast
                Catch ex As Exception
                End Try
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GetMaxWerte(ByRef MAX20 As Decimal, ByRef MAX35 As Decimal, ByRef MAX50 As Decimal)
        If objEichprozess.Lookup_Waagenart.Art = "Einbereichswaage" Then
            MAX20 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1) * 0.2
            MAX35 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1) * 0.35
            MAX50 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1) * 0.5
        ElseIf objEichprozess.Lookup_Waagenart.Art = "Zweibereichswaage" Or objEichprozess.Lookup_Waagenart.Art = "Zweiteilungswaage" Then
            MAX20 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2) * 0.2
            MAX35 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2) * 0.35
            MAX50 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2) * 0.5
        ElseIf objEichprozess.Lookup_Waagenart.Art = "Dreibereichswaage" Or objEichprozess.Lookup_Waagenart.Art = "Dreiteilungswaage" Then
            MAX20 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3) * 0.2
            MAX35 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3) * 0.35
            MAX50 = CDec(objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3) * 0.5
        End If
    End Sub

    ''' <summary>
    ''' Berechnen ob EFG eingehalten wird
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <remarks></remarks>
    Private Sub CalculateEFGWiederholungen(ByVal sender As Object)
        Try

            Dim Wiederholung As String = CType(sender, Windows.Forms.Control).Name.Last() 'letzte Zahl auslesen
            Dim Fehler As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlErrorLimit{0}", 1)) ' gibt nur ein Fehlerelement
            Dim Last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlWeight{0}", Wiederholung))
            Dim EFG As Telerik.WinControls.UI.RadCheckBox = FindControl(String.Format("RadCheckBoxVEL{0}", 1)) ' gibt nur ein EFG Element
            Dim Spezial As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblEFGSpeziallBerechnung"))
            Dim min As Decimal
            Dim max As Decimal
            'Eichwert holen
            Dim eichwertBereich As Integer = GetEichwertBereich()
            'EFG Wert Berechnen

            Try
                Spezial.Text = GetEFG(Last.Text, eichwertBereich)
            Catch ex As InvalidCastException
                Exit Sub
            End Try

            'EFG durch die Differenz zwischen den 3 Belastungen. Mit anderen Worten: Die Differenz der Wägeergebnisse bei der 3maligen Belastung darf nicht größer sein, als der Absolutwert der für diese Belastung geltenden Fehlergrenze der Waage.

            Dim AnzeigeMax1 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlDisplayWeight{0}", 1))
            Dim AnzeigeMax2 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlDisplayWeight{0}", 2))
            Dim AnzeigeMax3 As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlDisplayWeight{0}", 3))

            GetEFGDifferenz(Fehler, EFG, Spezial, min, max, AnzeigeMax1, AnzeigeMax2, AnzeigeMax3)

        Catch ex As Exception
        End Try
    End Sub





#End Region

#Region "Aussermittige Belastung Text changed Events"
    ''' <summary>
    ''' Berechnen ob EFG eingehalten wird
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CalculateEFGAussermittigeBelastung(ByVal Bereich As Integer, ByVal Belastungsort As String)
        Dim Last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}Weight{1}", Bereich, Belastungsort))
        Dim Anzeige As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}DisplayWeight{1}", Bereich, Belastungsort))
        Dim Fehler As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}ErrorLimit{1}", Bereich, Belastungsort))
        Dim EFG As Telerik.WinControls.UI.RadCheckBox = FindControl(String.Format("RadCheckBoxBereich{0}VEL{1}", Bereich, Belastungsort))
        Dim Spezial As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblBereich{0}EFGSpeziallBerechnung", Bereich))

        Try
            Spezial.Text = GetEFG(Last.Text, Bereich)
        Catch ex As InvalidCastException
            Exit Sub
        End Try

        'Alte Formel
        Try
            Fehler.Text = CDec(Anzeige.Text) - CDec(Last.Text)
            If Anzeige.Text > CDec(Last.Text) + CDec(Spezial.Text) Then
                EFG.Checked = False
            ElseIf Anzeige.Text < CDec(Last.Text) - CDec(Spezial.Text) Then
                EFG.Checked = False
            Else
                EFG.Checked = True
            End If
        Catch ex As Exception
        End Try

        ''Neue EFG Formel nach Herrn Strack
        'Try
        '    Fehler.Text = CDec(Anzeige.Text) - CDec(Last.Text)

        '    Dim Faktor As Decimal = 0
        '    If CDec(Last.Text) <= 500 * CDec(Spezial.Text) Then
        '        Faktor = 0.5
        '    ElseIf CDec(Last.Text) > 500 * CDec(Spezial.Text) And CDec(Last.Text) <= 2000 * CDec(Spezial.Text) Then
        '        Faktor = 1
        '    ElseIf CDec(Last.Text) > 2000 * CDec(Spezial.Text) And CDec(Last.Text) <= 10000 * CDec(Spezial.Text) Then
        '        Faktor = 1.5
        '    Else
        '        Faktor = 1.5
        '    End If

        '    If CDec(Anzeige.Text) < (CDec(Last.Text) - (Faktor * CDec(Spezial.Text))) Or CDec(Anzeige.Text) > ((CDec(Last.Text) + (Faktor * CDec(Spezial.Text)))) Then
        '        EFG.Checked = False
        '    Else
        '        EFG.Checked = True
        '    End If
        'Catch ex As Exception

        'End Try

    End Sub

    ''' <summary>
    ''' wenn sich eine der Last Werte ändert, muss es in allen anderen Textboxen nachgezogen werden
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControlBereich_TextChanged(sender As Object, e As EventArgs) Handles _
        RadTextBoxControlBereich1WeightMitte.TextChanged,
        RadTextBoxControlBereich1Weight12.TextChanged,
        RadTextBoxControlBereich1Weight11.TextChanged,
        RadTextBoxControlBereich1Weight10.TextChanged,
        RadTextBoxControlBereich1Weight9.TextChanged,
        RadTextBoxControlBereich1Weight8.TextChanged,
        RadTextBoxControlBereich1Weight7.TextChanged,
        RadTextBoxControlBereich1Weight6.TextChanged,
        RadTextBoxControlBereich1Weight5.TextChanged,
        RadTextBoxControlBereich1Weight4.TextChanged,
        RadTextBoxControlBereich1Weight3.TextChanged,
        RadTextBoxControlBereich1Weight2.TextChanged,
        RadTextBoxControlBereich1Weight1.TextChanged,
        RadTextBoxControlBereich2Weight12.TextChanged,
        RadTextBoxControlBereich2Weight11.TextChanged,
        RadTextBoxControlBereich2Weight10.TextChanged,
        RadTextBoxControlBereich2Weight9.TextChanged,
        RadTextBoxControlBereich2Weight8.TextChanged,
        RadTextBoxControlBereich2Weight7.TextChanged,
        RadTextBoxControlBereich2Weight6.TextChanged,
        RadTextBoxControlBereich2Weight5.TextChanged,
        RadTextBoxControlBereich2Weight4.TextChanged,
        RadTextBoxControlBereich2Weight3.TextChanged,
        RadTextBoxControlBereich2Weight2.TextChanged,
        RadTextBoxControlBereich2Weight1.TextChanged,
        RadTextBoxControlBereich3Weight12.TextChanged,
        RadTextBoxControlBereich3Weight11.TextChanged,
        RadTextBoxControlBereich3Weight10.TextChanged,
        RadTextBoxControlBereich3Weight9.TextChanged,
        RadTextBoxControlBereich3Weight8.TextChanged,
        RadTextBoxControlBereich3Weight7.TextChanged,
        RadTextBoxControlBereich3Weight6.TextChanged,
        RadTextBoxControlBereich3Weight5.TextChanged,
        RadTextBoxControlBereich3Weight4.TextChanged,
        RadTextBoxControlBereich3Weight3.TextChanged,
        RadTextBoxControlBereich3Weight2.TextChanged,
        RadTextBoxControlBereich3Weight1.TextChanged

        If _suspendEvents = True Then Exit Sub
        AktuellerStatusDirty = True

        'damit keine Event Kettenreaktion durchgeführt wird, werden die Events ab hier unterbrochen
        _suspendEvents = True

        'bereich laden
        Dim bereich = GetBereich(sender).ToString

        'alle steuerlemente iterieren
        Try
            For i As Integer = 1 To 13
                Dim Belastungsort As String = i
                If Belastungsort = 13 Then 'sonderfall mitte
                    Belastungsort = "Mitte"
                End If

                'wenn sich eine der Last Werte ändert, muss es in allen anderen Textboxen nachgezogen werden
                Dim Last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}Weight{1}", bereich, Belastungsort))

                'vorzeitiger abrruch, ab dann wenn ein ausgeblendetes steuerelement gefunden wurde
                If Last.Visible = False Then Continue For

                Last.Text = CType(sender, Telerik.WinControls.UI.RadTextBox).Text

                'wenn weight1 leer ist, sind auch alle anderen leer
                Dim EFG As Telerik.WinControls.UI.RadCheckBox = FindControl(String.Format("RadCheckBoxBereich{0}VEL{1}", bereich, Belastungsort))
                If Last.Text.Equals("") Then
                    EFG.Checked = False
                End If

                'neu berechnen der Fehler und EFG
                CalculateEFGAussermittigeBelastung(bereich, Belastungsort)

            Next
        Catch ex As Exception
            'überspringen wenn Bereich nicht sichtbar
        End Try
        _suspendEvents = False
    End Sub

    ''' <summary>
    ''' EFG Berechnen bei änderung in einer Textbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RadTextBoxControlBereich1DisplayWeight1_TextChanged(sender As Object, e As EventArgs) Handles _
        RadTextBoxControlBereich1DisplayWeight1.TextChanged,
        RadTextBoxControlBereich1DisplayWeight2.TextChanged,
        RadTextBoxControlBereich1DisplayWeight3.TextChanged,
        RadTextBoxControlBereich1DisplayWeight4.TextChanged,
        RadTextBoxControlBereich1DisplayWeight5.TextChanged,
        RadTextBoxControlBereich1DisplayWeight6.TextChanged,
        RadTextBoxControlBereich1DisplayWeight7.TextChanged,
        RadTextBoxControlBereich1DisplayWeight8.TextChanged,
        RadTextBoxControlBereich1DisplayWeight9.TextChanged,
        RadTextBoxControlBereich1DisplayWeight10.TextChanged,
        RadTextBoxControlBereich1DisplayWeight11.TextChanged,
        RadTextBoxControlBereich1DisplayWeight12.TextChanged,
        RadTextBoxControlBereich1DisplayWeightMitte.TextChanged,
        RadTextBoxControlBereich2DisplayWeight1.TextChanged,
        RadTextBoxControlBereich2DisplayWeight2.TextChanged,
        RadTextBoxControlBereich2DisplayWeight3.TextChanged,
        RadTextBoxControlBereich2DisplayWeight4.TextChanged,
        RadTextBoxControlBereich2DisplayWeight5.TextChanged,
        RadTextBoxControlBereich2DisplayWeight6.TextChanged,
        RadTextBoxControlBereich2DisplayWeight7.TextChanged,
        RadTextBoxControlBereich2DisplayWeight8.TextChanged,
        RadTextBoxControlBereich2DisplayWeight9.TextChanged,
        RadTextBoxControlBereich2DisplayWeight10.TextChanged,
        RadTextBoxControlBereich2DisplayWeight11.TextChanged,
        RadTextBoxControlBereich2DisplayWeight12.TextChanged,
        RadTextBoxControlBereich3DisplayWeight1.TextChanged,
        RadTextBoxControlBereich3DisplayWeight2.TextChanged,
        RadTextBoxControlBereich3DisplayWeight3.TextChanged,
        RadTextBoxControlBereich3DisplayWeight4.TextChanged,
        RadTextBoxControlBereich3DisplayWeight5.TextChanged,
        RadTextBoxControlBereich3DisplayWeight6.TextChanged,
        RadTextBoxControlBereich3DisplayWeight7.TextChanged,
        RadTextBoxControlBereich3DisplayWeight8.TextChanged,
        RadTextBoxControlBereich3DisplayWeight9.TextChanged,
        RadTextBoxControlBereich3DisplayWeight10.TextChanged,
        RadTextBoxControlBereich3DisplayWeight11.TextChanged,
        RadTextBoxControlBereich3DisplayWeight12.TextChanged

        Try

            If _suspendEvents = True Then Exit Sub
            AktuellerStatusDirty = True
            Dim Bereich = GetBereich(sender)
            Dim Belastungsort = GetBelastungsort(sender)

            'neu berechnen der Fehler und EFG
            CalculateEFGAussermittigeBelastung(Bereich, Belastungsort)

        Catch ex As Exception
        End Try
    End Sub

#End Region


#Region "Hilfetexte"
    Private Sub RadGroupBoxPruefungGenaugikeit_MouseHover(sender As Object, e As EventArgs) Handles RadGroupBoxPruefungGenaugikeit.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_Nullstellung)
    End Sub

    Private Sub RadGroupBoxPruefungAussermittigeBelastung_MouseHover(sender As Object, e As EventArgs) Handles RadGroupBoxPruefungAussermittigeBelastung.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungAussermittigerBelastung)
    End Sub

    Private Sub RadGroupBoxWiederholungen_MouseEnter(sender As Object, e As EventArgs) Handles RadGroupBoxWiederholungen.MouseEnter
        ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungWiederholbarkeitStaffelverfahren)

    End Sub

#End Region
#End Region

#Region "Methods"

    Private Sub FillControlsNullstellung()
        If Not objEichprozess.Eichprotokoll.GenauigkeitNullstellung_InOrdnung Is Nothing Then
            RadCheckBoxNullstellungOK.Checked = objEichprozess.Eichprotokoll.GenauigkeitNullstellung_InOrdnung
        End If
    End Sub

    Private Sub FillControlsAussermittigeBelastung()
        'füllen der berechnenten Steuerelemente
        lblBereich1EFGSpeziallBerechnung.Mask = String.Format("F{0}", _intNullstellenE) 'anzahl nullstellen für Textcontrol definieren
        lblBereich2EFGSpeziallBerechnung.Mask = String.Format("F{0}", _intNullstellenE) 'anzahl nullstellen für Textcontrol definieren
        lblBereich3EFGSpeziallBerechnung.Mask = String.Format("F{0}", _intNullstellenE) 'anzahl nullstellen für Textcontrol definieren

        'hoechstlast berechnen
        BerechneHoechstlast()


        For bereich As Integer = 1 To 3
            Dim sBereich As String = bereich 'wegen LINQ eigenart
            For belastungsort As Integer = 1 To 13
                Dim sBelastungsortControl As String = belastungsort
                Dim sBelastungsortDB As String = belastungsort

                If belastungsort = 13 Then
                    sBelastungsortDB = "M" 'sonderfall mitte
                    sBelastungsortControl = "Mitte"
                End If

                Dim Last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}Weight{1}", CInt(bereich), sBelastungsortControl))
                Dim Anzeige As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}DisplayWeight{1}", CInt(bereich), sBelastungsortControl))
                Dim Fehler As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}ErrorLimit{1}", CInt(bereich), sBelastungsortControl))
                Dim EFG As Telerik.WinControls.UI.RadCheckBox = FindControl(String.Format("RadCheckBoxBereich{0}VEL{1}", CInt(bereich), sBelastungsortControl))

                'TODO prüfen wieso Bereich als Byte deklariert ist in SQL Kompakt Entity Framework
                _currentObjPruefungAussermittigeBelastung = Nothing
                _currentObjPruefungAussermittigeBelastung = (From o In _ListPruefungAussermittigeBelastung Where o.Belastungsort = sBelastungsortDB And o.Bereich = sBereich).FirstOrDefault

                If Not _currentObjPruefungAussermittigeBelastung Is Nothing Then
                    If Not Last Is Nothing Then

                        Last.Text = _currentObjPruefungAussermittigeBelastung.Last
                        Anzeige.Text = _currentObjPruefungAussermittigeBelastung.Anzeige
                        Fehler.Text = _currentObjPruefungAussermittigeBelastung.Fehler
                        EFG.Checked = _currentObjPruefungAussermittigeBelastung.EFG
                    End If

                End If
            Next
        Next

        Try
            lblBereich1EFGSpeziallBerechnung.Text = GetEFG(RadTextBoxControlBereich1Weight1.Text, 1)
            lblBereich2EFGSpeziallBerechnung.Text = GetEFG(RadTextBoxControlBereich2Weight1.Text, 2)
            lblBereich3EFGSpeziallBerechnung.Text = GetEFG(RadTextBoxControlBereich3Weight1.Text, 3)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub FillControlsWiederholbarkeit()
        'füllen der berechnenten Steuerelemente

        lblEFGSpeziallBerechnung.Mask = String.Format("F{0}", _intNullstellenE) 'anzahl nullstellen für Textcontrol definieren
        'berechnen der EFGs

        Try

            RadTextBoxControlBetragNormallast.Text = objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien
            'standardwert eintragen
            If RadTextBoxControlBetragNormallast.Text.Equals("") Then
                RadTextBoxControlBetragNormallast.Text = 0
            End If
        Catch ex As Exception
            If RadTextBoxControlBetragNormallast.Text.Equals("") Then
                RadTextBoxControlBetragNormallast.Text = 0
            End If
        End Try

        'bereich 1

        Select Case objEichprozess.Lookup_Waagenart.Art
            Case Is = "Einbereichswaage"
                'EFG Wert Berechnen

                RadTextBoxControlWeight1.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1 * 0.5
                RadTextBoxControlWeight2.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1 * 0.5
                RadTextBoxControlWeight3.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1 * 0.5

                lblEFGSpeziallBerechnung.Text = GetEFG(RadTextBoxControlWeight1.Text, 1) 'selber werte wie weight 2 und 3
            Case Is = "Zweibereichswaage", "Zweiteilungswaage"
                lblEFGSpeziallBerechnung.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert2

                RadTextBoxControlWeight1.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2 * 0.5
                RadTextBoxControlWeight2.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2 * 0.5
                RadTextBoxControlWeight3.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2 * 0.5
                lblEFGSpeziallBerechnung.Text = GetEFG(RadTextBoxControlWeight1.Text, 2) 'selber werte wie weight 2 und 3

            Case Is = "Dreibereichswaage", "Dreiteilungswaage"
                lblEFGSpeziallBerechnung.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Eichwert3

                RadTextBoxControlWeight1.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3 * 0.5
                RadTextBoxControlWeight2.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3 * 0.5
                RadTextBoxControlWeight3.Text = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3 * 0.5
                lblEFGSpeziallBerechnung.Text = GetEFG(RadTextBoxControlWeight2.Text, 3) 'selber werte wie weight 2 und 3
        End Select

        'anzeige KG Nur laden wenn schon etwas eingegeben wurde
        'bereich 1
        _currentObjPruefungWiederholbarkeit = Nothing
        _currentObjPruefungWiederholbarkeit = (From o In _ListPruefungWiederholbarkeit Where o.Wiederholung = "1" And o.Belastung = "halb").FirstOrDefault

        If Not _currentObjPruefungWiederholbarkeit Is Nothing Then
            RadTextBoxControlWeight1.Text = _currentObjPruefungWiederholbarkeit.Last
            RadTextBoxControlDisplayWeight1.Text = _currentObjPruefungWiederholbarkeit.Anzeige
            RadTextBoxControlErrorLimit1.Text = _currentObjPruefungWiederholbarkeit.Fehler
            RadCheckBoxVEL1.Checked = _currentObjPruefungWiederholbarkeit.EFG
        End If

        'anzeige KG Nur laden wenn schon etwas eingegeben wurde
        _currentObjPruefungWiederholbarkeit = Nothing
        _currentObjPruefungWiederholbarkeit = (From o In _ListPruefungWiederholbarkeit Where o.Wiederholung = "2" And o.Belastung = "halb").FirstOrDefault

        If Not _currentObjPruefungWiederholbarkeit Is Nothing Then
            RadTextBoxControlWeight2.Text = _currentObjPruefungWiederholbarkeit.Last
            RadTextBoxControlDisplayWeight2.Text = _currentObjPruefungWiederholbarkeit.Anzeige
        End If

        'anzeige KG Nur laden wenn schon etwas eingegeben wurde
        _currentObjPruefungWiederholbarkeit = Nothing
        _currentObjPruefungWiederholbarkeit = (From o In _ListPruefungWiederholbarkeit Where o.Wiederholung = "3" And o.Belastung = "halb").FirstOrDefault

        If Not _currentObjPruefungWiederholbarkeit Is Nothing Then
            RadTextBoxControlWeight3.Text = _currentObjPruefungWiederholbarkeit.Last
            RadTextBoxControlDisplayWeight3.Text = _currentObjPruefungWiederholbarkeit.Anzeige
        End If

    End Sub

    Private Sub SaveAussermittigeBelastung(ByRef Context As Entities)
        'anzahl Bereiche auslesen um damit die anzahl der benötigten Iterationen und Objekt Erzeugungen zu erfahren
        Dim intBereiche As Integer = GetAnzahlBereiche()

        'alte löschen

        Dim query = From a In Context.PruefungAussermittigeBelastung Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
        For Each pruefung In query
            Context.PruefungAussermittigeBelastung.Remove(pruefung)
        Next
        Context.SaveChanges()

        _ListPruefungAussermittigeBelastung.Clear()

        'wenn es defintiv noch keine pruefungen gibt, neue Anlegen
        '  If _ListPruefungAussermittigeBelastung.Count = 0 Then

        For j = 1 To intBereiche
            'sonderfall eine Wägezelle
            If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen = 1 Then
                SaveAussermittigeBelastungEinwaegezelle(Context, j)
            Else
                SaveAussermittigeBelastungMehrwaegezelle(Context, j)
            End If
        Next
    End Sub

    ''' <summary>
    ''' prüfungstabellen laden
    ''' </summary>
    Private Sub LadePruefungen() Implements IRhewaPruefungDialog.LadePruefungen
        'Nur laden wenn es sich um eine Bearbeitung handelt (sonst würde das in Memory Objekt überschrieben werden)
        If Not DialogModus = enuDialogModus.lesend And Not DialogModus = enuDialogModus.korrigierend Then
            LadePruefungenBearbeitungsModus()
        Else
            LadePruefungenRHEWAKorrekturModus()
        End If
    End Sub

    Private Sub LadePruefungenRHEWAKorrekturModus() Implements IRhewaPruefungDialog.LadePruefungenRHEWAKorrekturModus
        _ListPruefungWiederholbarkeit.Clear()
        _ListPruefungAussermittigeBelastung.Clear()
        Try
            'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
            For Each obj In objEichprozess.Eichprotokoll.PruefungAussermittigeBelastung
                obj.Eichprotokoll = objEichprozess.Eichprotokoll
                _ListPruefungAussermittigeBelastung.Add(obj)
            Next

            For Each obj In objEichprozess.Eichprotokoll.PruefungWiederholbarkeit
                obj.Eichprotokoll = objEichprozess.Eichprotokoll
                _ListPruefungWiederholbarkeit.Add(obj)
            Next
        Catch ex As System.ObjectDisposedException 'fehler im Clientseitigen Lesemodus (bei bereits abegschickter Eichung)
            Using context As New Entities
                Dim query = From a In context.PruefungAussermittigeBelastung Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungAussermittigeBelastung = query.ToList

                Dim query2 = From a In context.PruefungWiederholbarkeit Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
                _ListPruefungWiederholbarkeit = query2.ToList
            End Using
        End Try

        _currentObjVerfahren = objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren
    End Sub

    Private Sub LadePruefungenBearbeitungsModus() Implements IRhewaPruefungDialog.LadePruefungenBearbeitungsModus
        Using context As New Entities

            'neu laden des Objekts, diesmal mit den lookup Objekten
            objEichprozess = (From a In context.Eichprozess.Include("Eichprotokoll").Include("Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren").Include("Lookup_Bearbeitungsstatus").Include("Lookup_Vorgangsstatus").Include("Lookup_Auswertegeraet").Include("Kompatiblitaetsnachweis").Include("Lookup_Waegezelle").Include("Lookup_Waagenart").Include("Lookup_Waagentyp").Include("Mogelstatistik") Select a Where a.Vorgangsnummer = objEichprozess.Vorgangsnummer).FirstOrDefault

            'abrufen aller Prüfungs entitäten die sich auf dieses eichprotokoll beziehen
            Dim query = From a In context.PruefungAussermittigeBelastung Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
            _ListPruefungAussermittigeBelastung = query.ToList

            Dim query2 = From a In context.PruefungWiederholbarkeit Where a.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID
            _ListPruefungWiederholbarkeit = query2.ToList

            _currentObjVerfahren = (From a In context.Lookup_Konformitaetsbewertungsverfahren Where a.ID = objEichprozess.Eichprotokoll.FK_Identifikationsdaten_Konformitaetsbewertungsverfahren).FirstOrDefault
        End Using
    End Sub

    ''' <summary>
    ''' 'zurücksetzten der Groupboxen größen auf default (designer) werte. Sonst würden die Groupboxen immer kleiner gerechnet
    ''' </summary>
    Private Sub ResetGroupboxSizes()
        RadGroupBoxBereich1.Size = New Size(503, 489)
        RadGroupBoxBereich2.Size = New Size(503, 489)
        RadGroupBoxBereich3.Size = New Size(503, 489)

        RadGroupBoxBereich1.Location = New Size(21, 149)
        RadGroupBoxBereich2.Location = New Size(21, 644)
        RadGroupBoxBereich3.Location = New Size(21, 1139)
    End Sub



    Private Sub SaveAussermittigeBelastungMehrwaegezelle(Context As Entities, j As Integer)
        For intBelastungsort As Integer = 1 To (objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen + 1) 'eine Mehr für Mitte
            'bei mehrbereichswagen gibt es die Mitte nur im ersten Durchlauf
            If j > 1 And intBelastungsort = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen + 1 Then
                Continue For
            End If

            Dim objPruefung = Context.PruefungAussermittigeBelastung.Create
            'wenn es die eine itereation mehr ist:
            If intBelastungsort = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen + 1 Then
                'mitte anlegen
                objPruefung.Belastungsort = "M"
            Else 'sonst bereich zuweisen
                objPruefung.Belastungsort = intBelastungsort
            End If
            objPruefung.Bereich = j
            UpdatePruefungsObject(objPruefung)

            Try
                Context.SaveChanges()

            Catch ex As Validation.DbEntityValidationException
                For Each e In ex.EntityValidationErrors
                    MessageBox.Show(e.ValidationErrors(0).ErrorMessage)
                Next
            End Try

            objEichprozess.Eichprotokoll.PruefungAussermittigeBelastung.Add(objPruefung)
            Try
                Context.SaveChanges()

            Catch ex As Validation.DbEntityValidationException
                For Each e In ex.EntityValidationErrors
                    MessageBox.Show(e.ValidationErrors(0).ErrorMessage)
                Next
            End Try

            _ListPruefungAussermittigeBelastung.Add(objPruefung)
        Next
    End Sub

    Private Sub SaveAussermittigeBelastungEinwaegezelle(Context As Entities, j As Integer)
        For intBelastungsort As Integer = 1 To 5 'eine Mehr für Mitte
            If j > 1 And intBelastungsort = 5 Then
                Continue For
            End If
            Dim objPruefung = Context.PruefungAussermittigeBelastung.Create
            'wenn es die eine itereation mehr ist:
            If intBelastungsort = 5 Then
                'mitte anlegen
                objPruefung.Belastungsort = "M"
            Else 'sonst bereich zuweisen
                objPruefung.Belastungsort = intBelastungsort
            End If
            objPruefung.Bereich = j
            UpdatePruefungsObject(objPruefung)
            Try
                Context.SaveChanges()

            Catch ex As Validation.DbEntityValidationException
                For Each e In ex.EntityValidationErrors
                    MessageBox.Show(e.ValidationErrors(0).ErrorMessage)
                Next
            End Try

            objEichprozess.Eichprotokoll.PruefungAussermittigeBelastung.Add(objPruefung)
            Try
                Context.SaveChanges()

            Catch ex As Validation.DbEntityValidationException
                For Each e In ex.EntityValidationErrors
                    MessageBox.Show(e.ValidationErrors(0).ErrorMessage)
                Next
            End Try

            _ListPruefungAussermittigeBelastung.Add(objPruefung)
        Next
    End Sub

    Private Sub SaveWiederholungen(ByRef Context As Entities)
        If RadGroupBoxWiederholungen.Visible = True Then

            'wenn es defintiv noch keine pruefungen gibt, neue Anlegen
            If _ListPruefungWiederholbarkeit.Count = 0 Then
                'anzahl Wiederholungen beträgt 3 um damit die anzahl der benötigten Iterationen und Objekt Erzeugungen zu erfahren
                For i As Integer = 1 To 3

                    'halbe Last
                    Dim objPruefung = Context.PruefungWiederholbarkeit.Create
                    'wenn es die eine itereation mehr ist:
                    objPruefung.Wiederholung = i
                    objPruefung.Belastung = "halb"
                    UpdatePruefungsObject(objPruefung)

                    Try
                        Context.SaveChanges()

                    Catch ex As Validation.DbEntityValidationException
                        For Each e In ex.EntityValidationErrors
                            MessageBox.Show(e.ValidationErrors(0).ErrorMessage)
                        Next
                    End Try

                    objEichprozess.Eichprotokoll.PruefungWiederholbarkeit.Add(objPruefung)
                    Try
                        Context.SaveChanges()

                    Catch ex As Validation.DbEntityValidationException
                        For Each e In ex.EntityValidationErrors
                            MessageBox.Show(e.ValidationErrors(0).ErrorMessage)
                        Next
                    End Try

                    _ListPruefungWiederholbarkeit.Add(objPruefung)

                Next
            Else ' es gibt bereits welche
                'jedes objekt initialisieren und aus context laden und updaten
                For Each objPruefung In _ListPruefungWiederholbarkeit
                    objPruefung = Context.PruefungWiederholbarkeit.FirstOrDefault(Function(value) value.ID = objPruefung.ID)
                    UpdatePruefungsObject(objPruefung)
                    Context.SaveChanges()
                Next
            End If

        End If

    End Sub

    ''' <summary>
    ''' je nach anzahl der WZ entsprechendes Bild laden
    ''' </summary>
    Private Sub LadeBilder()
        'je nach anzahl der WZ entsprechendes Bild laden
        If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen <= 12 Then
            PictureBox4LC.Visible = False
            PictureBox6LC.Visible = False
            PictureBox8LC.Visible = False
            PictureBox10LC.Visible = False
            PictureBox12LC.Visible = True
        End If
        If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen <= 10 Then
            PictureBox4LC.Visible = False
            PictureBox6LC.Visible = False
            PictureBox8LC.Visible = False
            PictureBox10LC.Visible = True
            PictureBox12LC.Visible = False
        End If
        If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen <= 8 Then
            PictureBox4LC.Visible = False
            PictureBox6LC.Visible = False
            PictureBox8LC.Visible = True
            PictureBox10LC.Visible = False
            PictureBox12LC.Visible = False
        End If
        If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen <= 6 Then
            PictureBox4LC.Visible = False
            PictureBox6LC.Visible = True
            PictureBox8LC.Visible = False
            PictureBox10LC.Visible = False
            PictureBox12LC.Visible = False
        End If
        If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen <= 4 Then
            PictureBox4LC.Visible = True
            PictureBox6LC.Visible = False
            PictureBox8LC.Visible = False
            PictureBox10LC.Visible = False
            PictureBox12LC.Visible = False
        End If
    End Sub

    Private Sub BereichsgruppenAusblenden()
        'je nach Art der Waage andere Bereichsgruppen ausblenden
        If objEichprozess.Lookup_Waagenart.Art = "Einbereichswaage" Then
            RadGroupBoxBereich2.Visible = False
            RadGroupBoxBereich3.Visible = False

        ElseIf objEichprozess.Lookup_Waagenart.Art = "Zweibereichswaage" Or objEichprozess.Lookup_Waagenart.Art = "Zweiteilungswaage" Then
            RadGroupBoxBereich2.Visible = True
            RadGroupBoxBereich3.Visible = False
        ElseIf objEichprozess.Lookup_Waagenart.Art = "Dreibereichswaage" Or objEichprozess.Lookup_Waagenart.Art = "Dreiteilungswaage" Then
            RadGroupBoxBereich2.Visible = True
            RadGroupBoxBereich3.Visible = True
        End If
    End Sub

    ''' <summary>
    ''' Hoechstlast berechnen
    ''' </summary>
    Private Sub BerechneHoechstlast()

        Dim teilungsfaktor As Integer = 3
        Dim wert As String = "" 'hilfsvariable zum zuweisen des Textwertes
        'je nach Zahl der Wägezellen ändert sich die Berechnung. Bei mehr als 4 WZ ändert sich die Formel von Hoechstlast / 3 auf Hoechstlast/(Anzahl WZ-1)
        If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen > 4 Then
            teilungsfaktor = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen - 1
        End If

        'alle textboxen den entsprechenden Wert zuordnen
        For bereich As Integer = 1 To 3

            'neue Werte berechnen ab Bereich 2 und 3
            If bereich = 1 Then
                wert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast1 / teilungsfaktor
            ElseIf bereich = 2 Then
                If Not objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2.Equals("") Then
                    wert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast2 / teilungsfaktor
                End If
            ElseIf bereich = 3 Then
                If Not objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3.Equals("") Then
                    wert = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_Hoechstlast3 / teilungsfaktor
                End If
            End If

            If Not wert = "-1" Then 'abbruch bei nicht zutreffenden Bereichen
                For belastungsort As Integer = 1 To 13
                    Dim sBelastungsOrt As String = belastungsort
                    If belastungsort = 13 Then sBelastungsOrt = "Mitte" 'sonderfall

                    Dim last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}Weight{1}", CInt(bereich), sBelastungsOrt))
                    If Not last Is Nothing Then
                        last.Text = wert
                    End If

                Next
            End If
            wert = "-1"
        Next
    End Sub



    Private Sub EinAusblendenVonWZBereichenen()
        Dim AnzlWZ As Integer = objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen

        'sonderfall 1 WZ = dann trotzdem 4 Belastungsorte
        If AnzlWZ = 1 Then AnzlWZ = 4

        'aus dem Designer sind alle panel bis auf 1 und Mitte visible = false und werden heir auf sichtbar geschaltet
        For i As Integer = 1 To AnzlWZ Step 1
            For bereich As Integer = 1 To 3
                Dim PanelWZ As Telerik.WinControls.UI.RadPanel = FindControl(String.Format("PanelBereich{0}WZ{1}", CInt(bereich), i))
                PanelWZ.Visible = True
            Next
        Next
        Dim PanelWZMAX As Telerik.WinControls.UI.RadPanel = FindControl(String.Format("PanelBereich{0}WZ{1}", 1, AnzlWZ))
        PanelBereich1WZMitte.Location = New Size(PanelWZMAX.Location.X, PanelWZMAX.Location.Y + 30)
    End Sub

    Private Sub BerechneNeueHoehe()
        Try
            'berechnen der neuen höhe für Groupboxen

            'höhe = höhe - ((max WZ - Anzahl WZ)*Höhe VoN WZ Panel - Abstand zwischen den Panels (etwa 10))
            Dim NeueHoehe As Integer

            'sonderfall bei 1 WZ werden trotzdem 4 Bereiche und mitte angezeigt
            If objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen = 1 Then
                NeueHoehe = RadGroupBoxBereich1.Height - ((12 - 4) * PanelBereich1WZ1.Height) - ((12 - 4) * 8)

            Else
                NeueHoehe = RadGroupBoxBereich1.Height - ((12 - objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen) * PanelBereich1WZ1.Height) - ((12 - objEichprozess.Kompatiblitaetsnachweis.Kompatiblitaet_Waage_AnzahlWaegezellen) * 8)

            End If

            RadGroupBoxBereich1.Height = NeueHoehe + 20
            RadGroupBoxBereich2.Height = NeueHoehe
            RadGroupBoxBereich3.Height = NeueHoehe

            'nicht nur werden die Groupboxen kleiner, sie müssen auch verschoben werden . (Passiert in Relation zur Vorherigen Groupbox)
            RadGroupBoxBereich2.Location = New Size(RadGroupBoxBereich2.Location.X, RadGroupBoxBereich1.Location.Y + NeueHoehe + 40)
            'dritte Groupbox muss sogar doppelt so hoch verschoben werden
            RadGroupBoxBereich3.Location = New Size(RadGroupBoxBereich3.Location.X, RadGroupBoxBereich1.Location.Y + (NeueHoehe * 2) + 60)

            'berechnen der äußeren Group Box Hoehe.
            'Der wert ergibt sich aus der neuen Position der letzten sichtbaren Groupbox + deren neuer höher+ einige Pixel Puffer

            If RadGroupBoxBereich3.Visible = True Then
                RadGroupBoxPruefungAussermittigeBelastung.Size = New Size(RadGroupBoxPruefungAussermittigeBelastung.Size.Width, RadGroupBoxBereich3.Location.Y + RadGroupBoxBereich3.Size.Height + 30)
            ElseIf RadGroupBoxBereich2.Visible = True Then
                RadGroupBoxPruefungAussermittigeBelastung.Size = New Size(RadGroupBoxPruefungAussermittigeBelastung.Size.Width, RadGroupBoxBereich2.Location.Y + RadGroupBoxBereich2.Size.Height + 30)
            ElseIf RadGroupBoxBereich1.Visible = True Then
                RadGroupBoxPruefungAussermittigeBelastung.Size = New Size(RadGroupBoxPruefungAussermittigeBelastung.Size.Width, RadGroupBoxBereich1.Location.Y + RadGroupBoxBereich1.Size.Height + 30)
            End If

        Catch ex As Exception

        End Try

    End Sub



    Private Sub UeberschreibePruefungsobjekte()
        objEichprozess.Eichprotokoll.PruefungAussermittigeBelastung.Clear()
        For Each obj In _ListPruefungAussermittigeBelastung
            objEichprozess.Eichprotokoll.PruefungAussermittigeBelastung.Add(obj)
        Next
        objEichprozess.Eichprotokoll.PruefungWiederholbarkeit.Clear()
        For Each obj In _ListPruefungWiederholbarkeit
            objEichprozess.Eichprotokoll.PruefungWiederholbarkeit.Add(obj)
        Next
    End Sub

    Private Sub UpdatePruefungsObject(ByVal PObjPruefung As PruefungAussermittigeBelastung)
        PObjPruefung.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID

        Dim Belastungsort As String = PObjPruefung.Belastungsort
        Dim Bereich As String = PObjPruefung.Bereich

        'sonderfall
        If Belastungsort = "M" And Bereich = 1 Then
            Belastungsort = "Mitte"
        ElseIf Belastungsort = "M" And Bereich <> 1 Then
            Exit Sub
        End If

        Dim Last As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}Weight{1}", CInt(Bereich), Belastungsort))
        Dim Anzeige As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}DisplayWeight{1}", CInt(Bereich), Belastungsort))
        Dim Fehler As Telerik.WinControls.UI.RadTextBox = FindControl(String.Format("RadTextBoxControlBereich{0}ErrorLimit{1}", CInt(Bereich), Belastungsort))
        Dim EFG As Telerik.WinControls.UI.RadCheckBox = FindControl(String.Format("RadCheckBoxBereich{0}VEL{1}", CInt(Bereich), Belastungsort))
        Dim EFGExtra As Telerik.WinControls.UI.RadMaskedEditBox = FindControl(String.Format("lblBereich{0}EFGSpeziallBerechnung", CInt(Bereich)))

        'überschreiben der Objekteigenschaften
        PObjPruefung.Last = Last.Text
        PObjPruefung.Anzeige = Anzeige.Text
        PObjPruefung.Fehler = Fehler.Text
        PObjPruefung.EFG = EFG.Checked
        PObjPruefung.EFGExtra = EFGExtra.Text
    End Sub

    Private Sub UpdatePruefungsObject(ByVal PObjPruefung As PruefungWiederholbarkeit)
        PObjPruefung.FK_Eichprotokoll = objEichprozess.Eichprotokoll.ID

        If PObjPruefung.Belastung = "halb" Then
            If PObjPruefung.Wiederholung = "1" Then
                PObjPruefung.Last = RadTextBoxControlWeight1.Text
                PObjPruefung.Anzeige = RadTextBoxControlDisplayWeight1.Text
                PObjPruefung.Fehler = RadTextBoxControlErrorLimit1.Text
                PObjPruefung.EFG = RadCheckBoxVEL1.Checked
                PObjPruefung.EFG_Extra = lblEFGSpeziallBerechnung.Text
            End If
            If PObjPruefung.Wiederholung = "2" Then
                PObjPruefung.Last = RadTextBoxControlWeight2.Text
                PObjPruefung.Anzeige = RadTextBoxControlDisplayWeight2.Text
                PObjPruefung.Fehler = RadTextBoxControlErrorLimit1.Text
                PObjPruefung.EFG = RadCheckBoxVEL1.Checked
                PObjPruefung.EFG_Extra = lblEFGSpeziallBerechnung.Text
            End If
            If PObjPruefung.Wiederholung = "3" Then
                PObjPruefung.Last = RadTextBoxControlWeight3.Text
                PObjPruefung.Anzeige = RadTextBoxControlDisplayWeight3.Text
                PObjPruefung.Fehler = RadTextBoxControlErrorLimit1.Text
                PObjPruefung.EFG = RadCheckBoxVEL1.Checked
                PObjPruefung.EFG_Extra = lblEFGSpeziallBerechnung.Text
            End If

        End If
    End Sub




#Region "validate Controls"
#Region "Nullstellung"
    Private Function ValidateControlsNullstellung() As Boolean
        If RadCheckBoxNullstellungOK.Checked = False Then
            RadCheckBoxNullstellungOK.Focus()
            AbortSaving = True
            RadCheckBoxNullstellungOK.Focus()
            Return False
        End If
        Return True
    End Function

#End Region
#Region "Wiederholbarkeit"
    Private Function ValidatecontrolsWiederholungen() As Boolean
        If RadGroupBoxWiederholungen.Visible = True Then
            If RadCheckBoxVEL1.Checked = False And RadCheckBoxVEL1.Visible = True Then
                AbortSaving = True
                Return False
            End If
        End If
        Return True
    End Function

#End Region

#Region "aussermittige Belastung"
    Private Function ValidateControlsAussermittigeBelastung() As Boolean
        If RadCheckBoxBereich1VEL1.Checked = False And RadCheckBoxBereich1VEL1.Visible = True Then RadTextBoxControlBereich1DisplayWeight1.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL2.Checked = False And RadCheckBoxBereich1VEL2.Visible = True Then RadTextBoxControlBereich1DisplayWeight2.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL3.Checked = False And RadCheckBoxBereich1VEL3.Visible = True Then RadTextBoxControlBereich1DisplayWeight3.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL4.Checked = False And RadCheckBoxBereich1VEL4.Visible = True Then RadTextBoxControlBereich1DisplayWeight4.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL5.Checked = False And RadCheckBoxBereich1VEL5.Visible = True Then RadTextBoxControlBereich1DisplayWeight5.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL6.Checked = False And RadCheckBoxBereich1VEL6.Visible = True Then RadTextBoxControlBereich1DisplayWeight6.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL7.Checked = False And RadCheckBoxBereich1VEL7.Visible = True Then RadTextBoxControlBereich1DisplayWeight7.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL8.Checked = False And RadCheckBoxBereich1VEL8.Visible = True Then RadTextBoxControlBereich1DisplayWeight8.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL9.Checked = False And RadCheckBoxBereich1VEL9.Visible = True Then RadTextBoxControlBereich1DisplayWeight9.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL10.Checked = False And RadCheckBoxBereich1VEL10.Visible = True Then RadTextBoxControlBereich1DisplayWeight10.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL11.Checked = False And RadCheckBoxBereich1VEL11.Visible = True Then RadTextBoxControlBereich1DisplayWeight11.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VEL12.Checked = False And RadCheckBoxBereich1VEL12.Visible = True Then RadTextBoxControlBereich1DisplayWeight12.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich1VELMitte.Checked = False And RadCheckBoxBereich1VELMitte.Visible = True Then RadTextBoxControlBereich1DisplayWeightMitte.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL1.Checked = False And RadCheckBoxBereich2VEL1.Visible = True Then RadTextBoxControlBereich2DisplayWeight1.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL2.Checked = False And RadCheckBoxBereich2VEL2.Visible = True Then RadTextBoxControlBereich2DisplayWeight2.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL3.Checked = False And RadCheckBoxBereich2VEL3.Visible = True Then RadTextBoxControlBereich2DisplayWeight3.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL4.Checked = False And RadCheckBoxBereich2VEL4.Visible = True Then RadTextBoxControlBereich2DisplayWeight4.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL5.Checked = False And RadCheckBoxBereich2VEL5.Visible = True Then RadTextBoxControlBereich2DisplayWeight5.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL6.Checked = False And RadCheckBoxBereich2VEL6.Visible = True Then RadTextBoxControlBereich2DisplayWeight6.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL7.Checked = False And RadCheckBoxBereich2VEL7.Visible = True Then RadTextBoxControlBereich2DisplayWeight7.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL8.Checked = False And RadCheckBoxBereich2VEL8.Visible = True Then RadTextBoxControlBereich2DisplayWeight8.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL9.Checked = False And RadCheckBoxBereich2VEL9.Visible = True Then RadTextBoxControlBereich2DisplayWeight9.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL10.Checked = False And RadCheckBoxBereich2VEL10.Visible = True Then RadTextBoxControlBereich2DisplayWeight10.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL11.Checked = False And RadCheckBoxBereich2VEL11.Visible = True Then RadTextBoxControlBereich2DisplayWeight11.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich2VEL12.Checked = False And RadCheckBoxBereich2VEL12.Visible = True Then RadTextBoxControlBereich2DisplayWeight12.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL1.Checked = False And RadCheckBoxBereich3VEL1.Visible = True Then RadTextBoxControlBereich3DisplayWeight1.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL2.Checked = False And RadCheckBoxBereich3VEL2.Visible = True Then RadTextBoxControlBereich3DisplayWeight2.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL3.Checked = False And RadCheckBoxBereich3VEL3.Visible = True Then RadTextBoxControlBereich3DisplayWeight3.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL4.Checked = False And RadCheckBoxBereich3VEL4.Visible = True Then RadTextBoxControlBereich3DisplayWeight4.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL5.Checked = False And RadCheckBoxBereich3VEL5.Visible = True Then RadTextBoxControlBereich3DisplayWeight5.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL6.Checked = False And RadCheckBoxBereich3VEL6.Visible = True Then RadTextBoxControlBereich3DisplayWeight6.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL7.Checked = False And RadCheckBoxBereich3VEL7.Visible = True Then RadTextBoxControlBereich3DisplayWeight7.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL8.Checked = False And RadCheckBoxBereich3VEL8.Visible = True Then RadTextBoxControlBereich3DisplayWeight8.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL9.Checked = False And RadCheckBoxBereich3VEL9.Visible = True Then RadTextBoxControlBereich3DisplayWeight9.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL10.Checked = False And RadCheckBoxBereich3VEL10.Visible = True Then RadTextBoxControlBereich3DisplayWeight10.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL11.Checked = False And RadCheckBoxBereich3VEL11.Visible = True Then RadTextBoxControlBereich3DisplayWeight11.TextBoxElement.Border.ForeColor = Color.Red
        If RadCheckBoxBereich3VEL12.Checked = False And RadCheckBoxBereich3VEL12.Visible = True Then RadTextBoxControlBereich3DisplayWeight12.TextBoxElement.Border.ForeColor = Color.Red

        If RadCheckBoxBereich1VEL1.Checked = False And RadCheckBoxBereich1VEL1.Visible = True Or
        RadCheckBoxBereich1VEL2.Checked = False And RadCheckBoxBereich1VEL2.Visible = True Or
        RadCheckBoxBereich1VEL3.Checked = False And RadCheckBoxBereich1VEL3.Visible = True Or
        RadCheckBoxBereich1VEL4.Checked = False And RadCheckBoxBereich1VEL4.Visible = True Or
        RadCheckBoxBereich1VEL5.Checked = False And RadCheckBoxBereich1VEL5.Visible = True Or
        RadCheckBoxBereich1VEL6.Checked = False And RadCheckBoxBereich1VEL6.Visible = True Or
        RadCheckBoxBereich1VEL7.Checked = False And RadCheckBoxBereich1VEL7.Visible = True Or
        RadCheckBoxBereich1VEL8.Checked = False And RadCheckBoxBereich1VEL8.Visible = True Or
        RadCheckBoxBereich1VEL9.Checked = False And RadCheckBoxBereich1VEL9.Visible = True Or
        RadCheckBoxBereich1VEL10.Checked = False And RadCheckBoxBereich1VEL10.Visible = True Or
        RadCheckBoxBereich1VEL11.Checked = False And RadCheckBoxBereich1VEL11.Visible = True Or
        RadCheckBoxBereich1VEL12.Checked = False And RadCheckBoxBereich1VEL12.Visible = True Or
        RadCheckBoxBereich1VELMitte.Checked = False And RadCheckBoxBereich1VELMitte.Visible = True Or
        RadCheckBoxBereich2VEL1.Checked = False And RadCheckBoxBereich2VEL1.Visible = True Or
        RadCheckBoxBereich2VEL2.Checked = False And RadCheckBoxBereich2VEL2.Visible = True Or
        RadCheckBoxBereich2VEL3.Checked = False And RadCheckBoxBereich2VEL3.Visible = True Or
        RadCheckBoxBereich2VEL4.Checked = False And RadCheckBoxBereich2VEL4.Visible = True Or
        RadCheckBoxBereich2VEL5.Checked = False And RadCheckBoxBereich2VEL5.Visible = True Or
        RadCheckBoxBereich2VEL6.Checked = False And RadCheckBoxBereich2VEL6.Visible = True Or
        RadCheckBoxBereich2VEL7.Checked = False And RadCheckBoxBereich2VEL7.Visible = True Or
        RadCheckBoxBereich2VEL8.Checked = False And RadCheckBoxBereich2VEL8.Visible = True Or
        RadCheckBoxBereich2VEL9.Checked = False And RadCheckBoxBereich2VEL9.Visible = True Or
        RadCheckBoxBereich2VEL10.Checked = False And RadCheckBoxBereich2VEL10.Visible = True Or
        RadCheckBoxBereich2VEL11.Checked = False And RadCheckBoxBereich2VEL11.Visible = True Or
        RadCheckBoxBereich2VEL12.Checked = False And RadCheckBoxBereich2VEL12.Visible = True Or
        RadCheckBoxBereich3VEL1.Checked = False And RadCheckBoxBereich3VEL1.Visible = True Or
        RadCheckBoxBereich3VEL2.Checked = False And RadCheckBoxBereich3VEL2.Visible = True Or
        RadCheckBoxBereich3VEL3.Checked = False And RadCheckBoxBereich3VEL3.Visible = True Or
        RadCheckBoxBereich3VEL4.Checked = False And RadCheckBoxBereich3VEL4.Visible = True Or
        RadCheckBoxBereich3VEL5.Checked = False And RadCheckBoxBereich3VEL5.Visible = True Or
        RadCheckBoxBereich3VEL6.Checked = False And RadCheckBoxBereich3VEL6.Visible = True Or
        RadCheckBoxBereich3VEL7.Checked = False And RadCheckBoxBereich3VEL7.Visible = True Or
        RadCheckBoxBereich3VEL8.Checked = False And RadCheckBoxBereich3VEL8.Visible = True Or
        RadCheckBoxBereich3VEL9.Checked = False And RadCheckBoxBereich3VEL9.Visible = True Or
        RadCheckBoxBereich3VEL10.Checked = False And RadCheckBoxBereich3VEL10.Visible = True Or
        RadCheckBoxBereich3VEL11.Checked = False And RadCheckBoxBereich3VEL11.Visible = True Or
        RadCheckBoxBereich3VEL12.Checked = False And RadCheckBoxBereich3VEL12.Visible = True Then
            AbortSaving = True
            Return False

        End If
        Return True
    End Function

#End Region
#End Region



#End Region


#Region "Interface Methods"

    Protected Friend Overrides Sub SetzeUeberschrift() Implements IRhewaEditingDialog.SetzeUeberschrift
        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen
                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungAussermittigerBelastung)
                'Überschrift setzen
                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungNullstellungundAussermittigeBelastung
            Catch ex As Exception
            End Try
        End If
    End Sub


    Protected Friend Overrides Sub LoadFromDatabase() Implements IRhewaEditingDialog.LoadFromDatabase
        Me.SuspendLayout()
        'zurücksetzten der Groupboxen größen auf default (designer) werte. Sonst würden die Groupboxen immer kleiner gerechnet
        ResetGroupboxSizes()

        objEichprozess = ParentFormular.CurrentEichprozess
        'events abbrechen
        _suspendEvents = True

        LadePruefungen()

        'steuerelemente mit werten aus DB füllen
        FillControls()

        If DialogModus = enuDialogModus.lesend Then
            'falls der Konformitätsbewertungsvorgang nur lesend betrchtet werden soll, wird versucht alle Steuerlemente auf REadonly zu setzen. Wenn das nicht klappt,werden sie disabled
            DisableControls(RadGroupBoxBereich1)
            DisableControls(RadGroupBoxBereich2)
            DisableControls(RadGroupBoxBereich3)
            DisableControls(RadGroupBoxPruefungAussermittigeBelastung)
            DisableControls(RadGroupBoxPruefungGenaugikeit)
            DisableControls(RadGroupBoxWiederholungen)

        End If
        'events abbrechen
        _suspendEvents = False
        Me.ResumeLayout()
    End Sub
    ''' <summary>
    ''' Lädt die Werte aus dem Objekt in die Steuerlemente
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    '''
    Protected Friend Overrides Sub FillControls() Implements IRhewaEditingDialog.FillControls
        'Steuerlemente füllen
        FillControlsNullstellung()
        'dynamisches laden der Nullstellen:
        HoleNullstellen()
        LadeBilder()
        BereichsgruppenAusblenden()
        'ein ausblenden von WZ Bereichenen
        EinAusblendenVonWZBereichenen()
        BerechneNeueHoehe()
        FillControlsAussermittigeBelastung()

        'Nur wenn es sich um das Staffel oder Fahrzeugwaagen verfahren handelt wird an dieser Stelle die Wiederholbarkeit geprüft. sonst erfolgt dies an einer anderen Stelle
        Select Case objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren.Verfahren
            Case Is = "über 60kg mit Normalien"
                RadGroupBoxWiederholungen.Visible = False

            Case Is = "Fahrzeugwaagen", "über 60kg im Staffelverfahren"
                RadGroupBoxWiederholungen.Visible = True
                FillControlsWiederholbarkeit()
        End Select

        'fokus setzen auf erstes Steuerelement
        RadCheckBoxNullstellungOK.Focus()
    End Sub
    ''' <summary>
    ''' Füllt das Objekt mit den Werten aus den Steuerlementen
    ''' </summary>
    ''' <remarks></remarks>
    ''' <author></author>
    ''' <commentauthor></commentauthor>
    Protected Friend Overrides Sub UpdateObjekt() Implements IRhewaEditingDialog.UpdateObjekt
        If DialogModus = enuDialogModus.normal Then objEichprozess.Bearbeitungsdatum = Date.Now
        objEichprozess.Eichprotokoll.GenauigkeitNullstellung_InOrdnung = RadCheckBoxNullstellungOK.Checked
        If RadTextBoxControlBetragNormallast.Visible = True Then
            objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien = RadTextBoxControlBetragNormallast.Text
        Else
            objEichprozess.Eichprotokoll.Wiederholbarkeit_Staffelverfahren_MINNormalien = 0
        End If
    End Sub




    Protected Friend Overrides Function CheckDialogModus() As Boolean Implements IRhewaEditingDialog.CheckDialogModus
        If DialogModus = enuDialogModus.korrigierend Or DialogModus = enuDialogModus.lesend Then
            If DialogModus = enuDialogModus.korrigierend Then
                UpdateObjekt()
            End If
            Select Case objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren.Verfahren
                Case Is = "über 60kg mit Normalien"
                    ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                    If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet Then
                        objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet
                    End If
                Case Is = "Fahrzeugwaagen", "über 60kg im Staffelverfahren"
                    ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                    If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast Then
                        objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast
                    End If
            End Select
            ParentFormular.CurrentEichprozess = objEichprozess
            Return False
        End If
        Return True
    End Function




    Protected Friend Overrides Sub SaveObjekt() Implements IRhewaEditingDialog.SaveObjekt
        'neuen Context aufbauen
        Using Context As New Entities
            'prüfen ob CREATE oder UPDATE durchgeführt werden muss
            If objEichprozess.ID <> 0 Then 'an dieser stelle muss eine ID existieren
                'prüfen ob das Objekt anhand der ID gefunden werden kann
                Dim dobjEichprozess As Eichprozess = Context.Eichprozess.Include("Lookup_Waagenart").FirstOrDefault(Function(value) value.Vorgangsnummer = objEichprozess.Vorgangsnummer)
                If Not dobjEichprozess Is Nothing Then
                    'lokale Variable mit Instanz aus DB überschreiben. Dies ist notwendig, damit das Entity Framework weiß, das ein Update vorgenommen werden muss.
                    objEichprozess = dobjEichprozess
                    'neuen Status zuweisen

                    SaveAussermittigeBelastung(Context)
                    SaveWiederholungen(Context)

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
                Dim dobjEichprozess As Eichprozess = Context.Eichprozess.Include("Lookup_Waagenart").FirstOrDefault(Function(value) value.Vorgangsnummer = objEichprozess.Vorgangsnummer)
                If Not dobjEichprozess Is Nothing Then
                    'lokale Variable mit Instanz aus DB überschreiben. Dies ist notwendig, damit das Entity Framework weiß, das ein Update vorgenommen werden muss.
                    objEichprozess = dobjEichprozess
                    'neuen Status zuweisen
                    'die reihenfolge wird hier je nach Verfahren verändert
                    Select Case objEichprozess.Eichprotokoll.Lookup_Konformitaetsbewertungsverfahren.Verfahren
                        Case Is = "über 60kg mit Normalien"
                            If AktuellerStatusDirty = False Then
                                ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet Then
                                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet
                                End If
                            ElseIf AktuellerStatusDirty = True Then
                                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitNormallastLinearitaet
                                AktuellerStatusDirty = False
                            End If
                        Case Is = "Fahrzeugwaagen", "über 60kg im Staffelverfahren"
                            If AktuellerStatusDirty = False Then
                                ' Wenn der aktuelle Status kleiner ist als der für die Beschaffenheitspruefung, wird dieser überschrieben. Sonst würde ein aktuellere Status mit dem vorherigen überschrieben
                                If objEichprozess.FK_Vorgangsstatus < GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast Then
                                    objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast
                                End If
                            ElseIf AktuellerStatusDirty = True Then
                                objEichprozess.FK_Vorgangsstatus = GlobaleEnumeratoren.enuEichprozessStatus.PrüfungderRichtigkeitmitErsatzlast
                                AktuellerStatusDirty = False
                            End If
                    End Select

                    'Füllt das Objekt mit den Werten aus den Steuerlementen
                    UpdateObjekt()
                    'Speichern in Datenbank
                    Context.SaveChanges()
                End If
            End If
        End Using
    End Sub

    Protected Friend Overrides Sub Lokalisiere() Implements IRhewaEditingDialog.Lokalisiere
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(uco_8PruefungNullstellungUndAussermittigeBelastung))
        Lokalisierung(Me, resources)

        If Not ParentFormular Is Nothing Then
            Try
                'Hilfetext setzen

                ParentFormular.SETContextHelpText(My.Resources.GlobaleLokalisierung.Hilfe_PruefungAussermittigerBelastung)
                'Überschrift setzen

                ParentFormular.GETSETHeaderText = My.Resources.GlobaleLokalisierung.Ueberschrift_PruefungNullstellungundAussermittigeBelastung
            Catch ex As Exception
            End Try
        End If

        ''dynamisches laden der Nullstellen:
        'LadeBilder()
        'BereichsgruppenAusblenden()
        'EinAusblendenVonWZBereichenen()
        'BerechneNeueHoehe()
    End Sub




    Protected Friend Overrides Sub Entsperrung() Implements IRhewaEditingDialog.Entsperrung
        'Hiermit wird ein lesender Vorgang wieder entsperrt.
        EnableControls(RadGroupBoxBereich1)
        EnableControls(RadGroupBoxBereich2)
        EnableControls(RadGroupBoxBereich3)
        EnableControls(RadGroupBoxPruefungAussermittigeBelastung)
        EnableControls(RadGroupBoxPruefungGenaugikeit)
        EnableControls(RadGroupBoxWiederholungen)

        'ändern des Moduses
        DialogModus = enuDialogModus.korrigierend
        ParentFormular.DialogModus = FrmMainContainer.enuDialogModus.korrigierend
    End Sub


    Protected Friend Overrides Sub Versenden() Implements IRhewaEditingDialog.Versenden
        UeberschreibePruefungsobjekte()
        UpdateObjekt()
        'Erzeugen eines Server Objektes auf basis des aktuellen DS. Setzt es auf es ausserdem auf Fehlerhaft
        CloneAndSendServerObjekt()
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

        If ValidateControlsNullstellung() = False Then
            ValidateControlsAussermittigeBelastung()
            ValidatecontrolsWiederholungen()
            'fehlermeldung anzeigen bei falscher validierung

            'fehlermeldung anzeigen bei falscher validierung
            Dim result = Me.ShowValidationErrorBox(False)
            Return ProcessResult(result)

        Else
            ValidateControlsAussermittigeBelastung()
            ValidatecontrolsWiederholungen()
            'fehlermeldung anzeigen bei falscher validierung
            Dim result = Me.ShowValidationErrorBox(False, My.Resources.GlobaleLokalisierung.EichfehlergrenzenNichtEingehalten)
            Return ProcessResult(result)

        End If

        'Speichern soll nicht abgebrochen werden, da alles okay ist
        AbortSaving = False
        Return True

    End Function

    Protected Friend Overrides Sub OverwriteIstSoll() Implements IRhewaEditingDialog.OverwriteIstSoll
        RadCheckBoxNullstellungOK.Checked = True
        RadTextBoxControlDisplayWeight1.Text = RadTextBoxControlWeight1.Text
        RadTextBoxControlDisplayWeight2.Text = RadTextBoxControlWeight2.Text
        RadTextBoxControlDisplayWeight3.Text = RadTextBoxControlWeight3.Text

        RadTextBoxControlBereich1DisplayWeight1.Text = RadTextBoxControlBereich1Weight1.Text
        RadTextBoxControlBereich1DisplayWeight2.Text = RadTextBoxControlBereich1Weight2.Text
        RadTextBoxControlBereich1DisplayWeight3.Text = RadTextBoxControlBereich1Weight3.Text
        RadTextBoxControlBereich1DisplayWeight4.Text = RadTextBoxControlBereich1Weight4.Text
        RadTextBoxControlBereich1DisplayWeight5.Text = RadTextBoxControlBereich1Weight5.Text
        RadTextBoxControlBereich1DisplayWeight6.Text = RadTextBoxControlBereich1Weight6.Text
        RadTextBoxControlBereich1DisplayWeight7.Text = RadTextBoxControlBereich1Weight7.Text
        RadTextBoxControlBereich1DisplayWeight8.Text = RadTextBoxControlBereich1Weight8.Text
        RadTextBoxControlBereich1DisplayWeight9.Text = RadTextBoxControlBereich1Weight9.Text
        RadTextBoxControlBereich1DisplayWeight10.Text = RadTextBoxControlBereich1Weight10.Text
        RadTextBoxControlBereich1DisplayWeight11.Text = RadTextBoxControlBereich1Weight11.Text
        RadTextBoxControlBereich1DisplayWeight12.Text = RadTextBoxControlBereich1Weight12.Text
        RadTextBoxControlBereich1DisplayWeightMitte.Text = RadTextBoxControlBereich1WeightMitte.Text

        RadTextBoxControlBereich2DisplayWeight1.Text = RadTextBoxControlBereich2Weight1.Text
        RadTextBoxControlBereich2DisplayWeight2.Text = RadTextBoxControlBereich2Weight2.Text
        RadTextBoxControlBereich2DisplayWeight3.Text = RadTextBoxControlBereich2Weight3.Text
        RadTextBoxControlBereich2DisplayWeight4.Text = RadTextBoxControlBereich2Weight4.Text
        RadTextBoxControlBereich2DisplayWeight5.Text = RadTextBoxControlBereich2Weight5.Text
        RadTextBoxControlBereich2DisplayWeight6.Text = RadTextBoxControlBereich2Weight6.Text
        RadTextBoxControlBereich2DisplayWeight7.Text = RadTextBoxControlBereich2Weight7.Text
        RadTextBoxControlBereich2DisplayWeight8.Text = RadTextBoxControlBereich2Weight8.Text
        RadTextBoxControlBereich2DisplayWeight9.Text = RadTextBoxControlBereich2Weight9.Text
        RadTextBoxControlBereich2DisplayWeight10.Text = RadTextBoxControlBereich2Weight10.Text
        RadTextBoxControlBereich2DisplayWeight11.Text = RadTextBoxControlBereich2Weight11.Text
        RadTextBoxControlBereich2DisplayWeight12.Text = RadTextBoxControlBereich2Weight12.Text

        RadTextBoxControlBereich3DisplayWeight1.Text = RadTextBoxControlBereich3Weight1.Text
        RadTextBoxControlBereich3DisplayWeight2.Text = RadTextBoxControlBereich3Weight2.Text
        RadTextBoxControlBereich3DisplayWeight3.Text = RadTextBoxControlBereich3Weight3.Text
        RadTextBoxControlBereich3DisplayWeight4.Text = RadTextBoxControlBereich3Weight4.Text
        RadTextBoxControlBereich3DisplayWeight5.Text = RadTextBoxControlBereich3Weight5.Text
        RadTextBoxControlBereich3DisplayWeight6.Text = RadTextBoxControlBereich3Weight6.Text
        RadTextBoxControlBereich3DisplayWeight7.Text = RadTextBoxControlBereich3Weight7.Text
        RadTextBoxControlBereich3DisplayWeight8.Text = RadTextBoxControlBereich3Weight8.Text
        RadTextBoxControlBereich3DisplayWeight9.Text = RadTextBoxControlBereich3Weight9.Text
        RadTextBoxControlBereich3DisplayWeight10.Text = RadTextBoxControlBereich3Weight10.Text
        RadTextBoxControlBereich3DisplayWeight11.Text = RadTextBoxControlBereich3Weight11.Text
        RadTextBoxControlBereich3DisplayWeight12.Text = RadTextBoxControlBereich3Weight12.Text

    End Sub
#End Region

End Class