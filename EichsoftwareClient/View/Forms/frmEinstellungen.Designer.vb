<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmEinstellungen

    Inherits Telerik.WinControls.UI.RadForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmEinstellungen))
        Me.RadGroupBox1 = New Telerik.WinControls.UI.RadGroupBox()
        Me.RadDateTimePickerEnd = New Telerik.WinControls.UI.RadDateTimePicker()
        Me.RadDateTimePickerStart = New Telerik.WinControls.UI.RadDateTimePicker()
        Me.RadDateTimePickerSince = New Telerik.WinControls.UI.RadDateTimePicker()
        Me.Label1 = New Telerik.WinControls.UI.RadLabel()
        Me.RadRadioButtonSyncZwischen = New Telerik.WinControls.UI.RadRadioButton()
        Me.RadRadioButtonSyncSeit = New Telerik.WinControls.UI.RadRadioButton()
        Me.RadRadioButtonSyncAlles = New Telerik.WinControls.UI.RadRadioButton()
        Me.RadButtonOK = New Telerik.WinControls.UI.RadButton()
        Me.RadButtonAbbrechen = New Telerik.WinControls.UI.RadButton()
        Me.RadButtonGridSettingsZuruecksetzen = New Telerik.WinControls.UI.RadButton()
        Me.cmdSendDiagnosticData = New Telerik.WinControls.UI.RadButton()
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RadGroupBox1.SuspendLayout()
        CType(Me.RadDateTimePickerEnd, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadDateTimePickerStart, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadDateTimePickerSince, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Label1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadRadioButtonSyncZwischen, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadRadioButtonSyncSeit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadRadioButtonSyncAlles, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadButtonOK, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadButtonAbbrechen, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadButtonGridSettingsZuruecksetzen, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cmdSendDiagnosticData, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadGroupBox1
        '
        resources.ApplyResources(Me.RadGroupBox1, "RadGroupBox1")
        Me.RadGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping
        Me.RadGroupBox1.Controls.Add(Me.RadDateTimePickerEnd)
        Me.RadGroupBox1.Controls.Add(Me.RadDateTimePickerStart)
        Me.RadGroupBox1.Controls.Add(Me.RadDateTimePickerSince)
        Me.RadGroupBox1.Controls.Add(Me.Label1)
        Me.RadGroupBox1.Controls.Add(Me.RadRadioButtonSyncZwischen)
        Me.RadGroupBox1.Controls.Add(Me.RadRadioButtonSyncSeit)
        Me.RadGroupBox1.Controls.Add(Me.RadRadioButtonSyncAlles)
        Me.RadGroupBox1.Name = "RadGroupBox1"
        '
        'RadDateTimePickerEnd
        '
        resources.ApplyResources(Me.RadDateTimePickerEnd, "RadDateTimePickerEnd")
        Me.RadDateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.RadDateTimePickerEnd.Name = "RadDateTimePickerEnd"
        Me.RadDateTimePickerEnd.TabStop = False
        Me.RadDateTimePickerEnd.Value = New Date(2014, 2, 12, 11, 45, 34, 423)
        '
        'RadDateTimePickerStart
        '
        resources.ApplyResources(Me.RadDateTimePickerStart, "RadDateTimePickerStart")
        Me.RadDateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.RadDateTimePickerStart.Name = "RadDateTimePickerStart"
        Me.RadDateTimePickerStart.TabStop = False
        Me.RadDateTimePickerStart.Value = New Date(2014, 2, 12, 11, 45, 34, 423)
        '
        'RadDateTimePickerSince
        '
        resources.ApplyResources(Me.RadDateTimePickerSince, "RadDateTimePickerSince")
        Me.RadDateTimePickerSince.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.RadDateTimePickerSince.Name = "RadDateTimePickerSince"
        Me.RadDateTimePickerSince.TabStop = False
        Me.RadDateTimePickerSince.Value = New Date(2014, 2, 12, 11, 45, 34, 423)
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'RadRadioButtonSyncZwischen
        '
        resources.ApplyResources(Me.RadRadioButtonSyncZwischen, "RadRadioButtonSyncZwischen")
        Me.RadRadioButtonSyncZwischen.Name = "RadRadioButtonSyncZwischen"
        '
        'RadRadioButtonSyncSeit
        '
        resources.ApplyResources(Me.RadRadioButtonSyncSeit, "RadRadioButtonSyncSeit")
        Me.RadRadioButtonSyncSeit.Name = "RadRadioButtonSyncSeit"
        '
        'RadRadioButtonSyncAlles
        '
        resources.ApplyResources(Me.RadRadioButtonSyncAlles, "RadRadioButtonSyncAlles")
        Me.RadRadioButtonSyncAlles.CheckState = System.Windows.Forms.CheckState.Checked
        Me.RadRadioButtonSyncAlles.Name = "RadRadioButtonSyncAlles"
        Me.RadRadioButtonSyncAlles.ToggleState = Telerik.WinControls.Enumerations.ToggleState.[On]
        '
        'RadButtonOK
        '
        resources.ApplyResources(Me.RadButtonOK, "RadButtonOK")
        Me.RadButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.RadButtonOK.Image = Global.EichsoftwareClient.My.Resources.Resources.disk
        Me.RadButtonOK.Name = "RadButtonOK"
        '
        'RadButtonAbbrechen
        '
        resources.ApplyResources(Me.RadButtonAbbrechen, "RadButtonAbbrechen")
        Me.RadButtonAbbrechen.Name = "RadButtonAbbrechen"
        '
        'RadButtonGridSettingsZuruecksetzen
        '
        resources.ApplyResources(Me.RadButtonGridSettingsZuruecksetzen, "RadButtonGridSettingsZuruecksetzen")
        Me.RadButtonGridSettingsZuruecksetzen.Name = "RadButtonGridSettingsZuruecksetzen"
        '
        'cmdSendDiagnosticData
        '
        resources.ApplyResources(Me.cmdSendDiagnosticData, "cmdSendDiagnosticData")
        Me.cmdSendDiagnosticData.Image = Global.EichsoftwareClient.My.Resources.Resources.cog
        Me.cmdSendDiagnosticData.Name = "cmdSendDiagnosticData"
        '
        'FrmEinstellungen
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.cmdSendDiagnosticData)
        Me.Controls.Add(Me.RadButtonGridSettingsZuruecksetzen)
        Me.Controls.Add(Me.RadButtonAbbrechen)
        Me.Controls.Add(Me.RadButtonOK)
        Me.Controls.Add(Me.RadGroupBox1)
        Me.Name = "FrmEinstellungen"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        CType(Me.RadGroupBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RadGroupBox1.ResumeLayout(False)
        Me.RadGroupBox1.PerformLayout()
        CType(Me.RadDateTimePickerEnd, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadDateTimePickerStart, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadDateTimePickerSince, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Label1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadRadioButtonSyncZwischen, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadRadioButtonSyncSeit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadRadioButtonSyncAlles, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadButtonOK, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadButtonAbbrechen, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadButtonGridSettingsZuruecksetzen, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cmdSendDiagnosticData, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents RadGroupBox1 As Telerik.WinControls.UI.RadGroupBox
    Friend WithEvents RadRadioButtonSyncZwischen As Telerik.WinControls.UI.RadRadioButton
    Friend WithEvents RadRadioButtonSyncSeit As Telerik.WinControls.UI.RadRadioButton
    Friend WithEvents RadRadioButtonSyncAlles As Telerik.WinControls.UI.RadRadioButton
    Friend WithEvents Label1 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadButtonOK As Telerik.WinControls.UI.RadButton
    Friend WithEvents RadButtonAbbrechen As Telerik.WinControls.UI.RadButton
    Friend WithEvents RadDateTimePickerEnd As Telerik.WinControls.UI.RadDateTimePicker
    Friend WithEvents RadDateTimePickerStart As Telerik.WinControls.UI.RadDateTimePicker
    Friend WithEvents RadDateTimePickerSince As Telerik.WinControls.UI.RadDateTimePicker
    Friend WithEvents RadButtonGridSettingsZuruecksetzen As Telerik.WinControls.UI.RadButton
    Friend WithEvents cmdSendDiagnosticData As Telerik.WinControls.UI.RadButton
End Class

