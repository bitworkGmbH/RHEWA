<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEingabeNeueLizenz

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
        Dim TableViewDefinition1 As Telerik.WinControls.UI.TableViewDefinition = New Telerik.WinControls.UI.TableViewDefinition()
        Me.RadButton1 = New Telerik.WinControls.UI.RadButton()
        Me.RadLabel1 = New Telerik.WinControls.UI.RadLabel()
        Me.RadTextBoxControl1 = New Telerik.WinControls.UI.RadTextBoxControl()
        Me.RadLabel2 = New Telerik.WinControls.UI.RadLabel()
        Me.RadTextBoxControl2 = New Telerik.WinControls.UI.RadTextBoxControl()
        Me.RadCheckBox1 = New Telerik.WinControls.UI.RadCheckBox()
        Me.RadCheckBox2 = New Telerik.WinControls.UI.RadCheckBox()
        Me.RadLabel3 = New Telerik.WinControls.UI.RadLabel()
        Me.RadMultiColumnComboBoxBenutzer = New Telerik.WinControls.UI.RadMultiColumnComboBox()
        Me.RadButton2 = New Telerik.WinControls.UI.RadButton()
        CType(Me.RadButton1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadTextBoxControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadTextBoxControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadCheckBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadCheckBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBoxBenutzer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBoxBenutzer.EditorControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RadButton2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RadButton1
        '
        Me.RadButton1.Location = New System.Drawing.Point(487, 263)
        Me.RadButton1.Name = "RadButton1"
        Me.RadButton1.Size = New System.Drawing.Size(110, 24)
        Me.RadButton1.TabIndex = 8
        Me.RadButton1.Text = "OK"
        '
        'RadLabel1
        '
        Me.RadLabel1.Location = New System.Drawing.Point(12, 125)
        Me.RadLabel1.Name = "RadLabel1"
        Me.RadLabel1.Size = New System.Drawing.Size(74, 16)
        Me.RadLabel1.TabIndex = 2
        Me.RadLabel1.Text = "HE-Kennung:"
        '
        'RadTextBoxControl1
        '
        Me.RadTextBoxControl1.IsReadOnly = True
        Me.RadTextBoxControl1.Location = New System.Drawing.Point(99, 125)
        Me.RadTextBoxControl1.Name = "RadTextBoxControl1"
        Me.RadTextBoxControl1.Size = New System.Drawing.Size(498, 20)
        Me.RadTextBoxControl1.TabIndex = 3
        '
        'RadLabel2
        '
        Me.RadLabel2.Location = New System.Drawing.Point(12, 151)
        Me.RadLabel2.Name = "RadLabel2"
        Me.RadLabel2.Size = New System.Drawing.Size(86, 16)
        Me.RadLabel2.TabIndex = 4
        Me.RadLabel2.Text = "Lizenzschlüssel"
        '
        'RadTextBoxControl2
        '
        Me.RadTextBoxControl2.IsReadOnly = True
        Me.RadTextBoxControl2.Location = New System.Drawing.Point(99, 151)
        Me.RadTextBoxControl2.Name = "RadTextBoxControl2"
        Me.RadTextBoxControl2.Size = New System.Drawing.Size(498, 20)
        Me.RadTextBoxControl2.TabIndex = 5
        Me.RadTextBoxControl2.TabStop = False
        '
        'RadCheckBox1
        '
        Me.RadCheckBox1.AutoSize = False
        Me.RadCheckBox1.CheckAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.RadCheckBox1.Location = New System.Drawing.Point(12, 200)
        Me.RadCheckBox1.Name = "RadCheckBox1"
        Me.RadCheckBox1.Size = New System.Drawing.Size(387, 18)
        Me.RadCheckBox1.TabIndex = 6
        Me.RadCheckBox1.Text = "RHEWA Lizenz (Sonderrechte)"
        '
        'RadCheckBox2
        '
        Me.RadCheckBox2.AutoSize = False
        Me.RadCheckBox2.CheckAlignment = System.Drawing.ContentAlignment.MiddleRight
        Me.RadCheckBox2.Location = New System.Drawing.Point(12, 224)
        Me.RadCheckBox2.Name = "RadCheckBox2"
        Me.RadCheckBox2.Size = New System.Drawing.Size(387, 18)
        Me.RadCheckBox2.TabIndex = 7
        Me.RadCheckBox2.Text = "Aktiv (inaktive dürfen Anwendung nicht verwenden)"
        '
        'RadLabel3
        '
        Me.RadLabel3.Location = New System.Drawing.Point(12, 12)
        Me.RadLabel3.Name = "RadLabel3"
        Me.RadLabel3.Size = New System.Drawing.Size(55, 16)
        Me.RadLabel3.TabIndex = 0
        Me.RadLabel3.Text = "Benutzer:"
        '
        'RadMultiColumnComboBoxBenutzer
        '
        Me.RadMultiColumnComboBoxBenutzer.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList
        '
        'RadMultiColumnComboBoxBenutzer.NestedRadGridView
        '
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.BackColor = System.Drawing.SystemColors.Window
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.ForeColor = System.Drawing.SystemColors.ControlText
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.Location = New System.Drawing.Point(0, 0)
        '
        '
        '
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate.AllowAddNewRow = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate.AllowCellContextMenu = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate.AllowColumnChooser = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate.EnableGrouping = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate.ShowFilteringRow = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate.ViewDefinition = TableViewDefinition1
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.Name = "NestedRadGridView"
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.ReadOnly = True
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.ShowGroupPanel = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.ShowNoDataText = False
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.Size = New System.Drawing.Size(240, 150)
        Me.RadMultiColumnComboBoxBenutzer.EditorControl.TabIndex = 0
        Me.RadMultiColumnComboBoxBenutzer.Enabled = False
        Me.RadMultiColumnComboBoxBenutzer.ImeMode = System.Windows.Forms.ImeMode.[On]
        Me.RadMultiColumnComboBoxBenutzer.Location = New System.Drawing.Point(99, 12)
        Me.RadMultiColumnComboBoxBenutzer.Name = "RadMultiColumnComboBoxBenutzer"
        Me.RadMultiColumnComboBoxBenutzer.NullText = "Bitte Benutzer aus Liste auswählen"
        '
        '
        '
        Me.RadMultiColumnComboBoxBenutzer.RootElement.ToolTipText = "Die Benutzer dieser Liste stammen aus einer SQL Server Tabelle (Benutzer) die aus" & _
    " dem CRM System gefüllt wird. Wenn kein Benutzer angezeigt wird, haben alle verf" & _
    "ügbaren Benutzer bereits eine Lizenz."
        Me.RadMultiColumnComboBoxBenutzer.Size = New System.Drawing.Size(498, 20)
        Me.RadMultiColumnComboBoxBenutzer.TabIndex = 1
        Me.RadMultiColumnComboBoxBenutzer.TabStop = False
        '
        'RadButton2
        '
        Me.RadButton2.Enabled = False
        Me.RadButton2.Location = New System.Drawing.Point(99, 34)
        Me.RadButton2.Name = "RadButton2"
        Me.RadButton2.Size = New System.Drawing.Size(498, 66)
        Me.RadButton2.TabIndex = 9
        Me.RadButton2.Text = "Die Benutzer dieser Liste stammen aus einer SQL Server Tabelle (Benutzer) die aus" & _
    " dem CRM System gefüllt wird. Wenn kein Benutzer angezeigt wird, haben alle verf" & _
    "ügbaren Benutzer bereits eine Lizenz."
        Me.RadButton2.TextWrap = True
        '
        'frmEingabeNeueLizenz
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(609, 295)
        Me.Controls.Add(Me.RadButton2)
        Me.Controls.Add(Me.RadMultiColumnComboBoxBenutzer)
        Me.Controls.Add(Me.RadLabel3)
        Me.Controls.Add(Me.RadCheckBox2)
        Me.Controls.Add(Me.RadCheckBox1)
        Me.Controls.Add(Me.RadButton1)
        Me.Controls.Add(Me.RadTextBoxControl2)
        Me.Controls.Add(Me.RadLabel2)
        Me.Controls.Add(Me.RadTextBoxControl1)
        Me.Controls.Add(Me.RadLabel1)
        Me.Name = "frmEingabeNeueLizenz"
        '
        '
        '
        Me.RootElement.ApplyShapeToControl = True
        Me.Text = "Lizenzverwaltung"
        CType(Me.RadButton1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadTextBoxControl1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadTextBoxControl2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadCheckBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadCheckBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadLabel3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBoxBenutzer.EditorControl.MasterTemplate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBoxBenutzer.EditorControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadMultiColumnComboBoxBenutzer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RadButton2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RadButton1 As Telerik.WinControls.UI.RadButton
    Friend WithEvents RadLabel1 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadTextBoxControl1 As Telerik.WinControls.UI.RadTextBoxControl
    Friend WithEvents RadLabel2 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadTextBoxControl2 As Telerik.WinControls.UI.RadTextBoxControl
    Friend WithEvents RadCheckBox1 As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents RadCheckBox2 As Telerik.WinControls.UI.RadCheckBox
    Friend WithEvents RadLabel3 As Telerik.WinControls.UI.RadLabel
    Friend WithEvents RadMultiColumnComboBoxBenutzer As Telerik.WinControls.UI.RadMultiColumnComboBox
    Friend WithEvents RadButton2 As Telerik.WinControls.UI.RadButton
End Class

