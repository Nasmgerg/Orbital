namespace Orbital.View
{
    partial class Add
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            nameLabel = new Label();
            nameTextBox = new TextBox();
            roleLabel = new Label();
            roleComboBox = new ComboBox();
            sectorLabel = new Label();
            sectorComboBox = new ComboBox();
            specialtyLabel = new Label();
            specialtyTextBox = new TextBox();
            shiftLabel = new Label();
            shiftUpDown = new NumericUpDown();
            addButton = new Button();
            cancelButton = new Button();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)shiftUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            //
            // nameLabel
            //
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(12, 23);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(34, 15);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Név:";
            //
            // nameTextBox
            //
            nameTextBox.Location = new Point(130, 20);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(230, 23);
            nameTextBox.TabIndex = 1;
            //
            // roleLabel
            //
            roleLabel.AutoSize = true;
            roleLabel.Location = new Point(12, 58);
            roleLabel.Name = "roleLabel";
            roleLabel.Size = new Size(62, 15);
            roleLabel.TabIndex = 2;
            roleLabel.Text = "Szerepkör:";
            //
            // roleComboBox
            //
            roleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            roleComboBox.FormattingEnabled = true;
            roleComboBox.Items.AddRange(new object[] { "kutató", "technikus", "egyéb" });
            roleComboBox.Location = new Point(130, 55);
            roleComboBox.Name = "roleComboBox";
            roleComboBox.Size = new Size(230, 23);
            roleComboBox.TabIndex = 3;
            //
            // sectorLabel
            //
            sectorLabel.AutoSize = true;
            sectorLabel.Location = new Point(12, 93);
            sectorLabel.Name = "sectorLabel";
            sectorLabel.Size = new Size(48, 15);
            sectorLabel.TabIndex = 4;
            sectorLabel.Text = "Szektor:";
            //
            // sectorComboBox
            //
            sectorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sectorComboBox.FormattingEnabled = true;
            sectorComboBox.Items.AddRange(new object[] { "Labor", "Irányítás", "Műszaki", "Orvosi" });
            sectorComboBox.Location = new Point(130, 90);
            sectorComboBox.Name = "sectorComboBox";
            sectorComboBox.Size = new Size(230, 23);
            sectorComboBox.TabIndex = 5;
            //
            // specialtyLabel
            //
            specialtyLabel.AutoSize = true;
            specialtyLabel.Location = new Point(12, 128);
            specialtyLabel.Name = "specialtyLabel";
            specialtyLabel.Size = new Size(73, 15);
            specialtyLabel.TabIndex = 6;
            specialtyLabel.Text = "Szakterület:";
            //
            // specialtyTextBox
            //
            specialtyTextBox.Location = new Point(130, 125);
            specialtyTextBox.Name = "specialtyTextBox";
            specialtyTextBox.Size = new Size(230, 23);
            specialtyTextBox.TabIndex = 7;
            //
            // shiftLabel
            //
            shiftLabel.AutoSize = true;
            shiftLabel.Location = new Point(12, 163);
            shiftLabel.Name = "shiftLabel";
            shiftLabel.Size = new Size(94, 15);
            shiftLabel.TabIndex = 8;
            shiftLabel.Text = "Műszak (1–3):";
            //
            // shiftUpDown
            //
            shiftUpDown.Location = new Point(130, 160);
            shiftUpDown.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            shiftUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            shiftUpDown.Name = "shiftUpDown";
            shiftUpDown.Size = new Size(230, 23);
            shiftUpDown.TabIndex = 9;
            shiftUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // addButton
            //
            addButton.Location = new Point(130, 210);
            addButton.Name = "addButton";
            addButton.Size = new Size(110, 30);
            addButton.TabIndex = 10;
            addButton.Text = "Mentés";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += Save;
            //
            // cancelButton
            //
            cancelButton.Location = new Point(250, 210);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(110, 30);
            cancelButton.TabIndex = 11;
            cancelButton.Text = "Mégse";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += Cancel;
            //
            // errorProvider
            //
            errorProvider.ContainerControl = this;
            //
            // Add
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 261);
            Controls.Add(cancelButton);
            Controls.Add(addButton);
            Controls.Add(shiftUpDown);
            Controls.Add(shiftLabel);
            Controls.Add(specialtyTextBox);
            Controls.Add(specialtyLabel);
            Controls.Add(sectorComboBox);
            Controls.Add(sectorLabel);
            Controls.Add(roleComboBox);
            Controls.Add(roleLabel);
            Controls.Add(nameTextBox);
            Controls.Add(nameLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Add";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Új személy felvétele";
            ((System.ComponentModel.ISupportInitialize)shiftUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label nameLabel;
        private TextBox nameTextBox;
        private Label roleLabel;
        private ComboBox roleComboBox;
        private Label sectorLabel;
        private ComboBox sectorComboBox;
        private Label specialtyLabel;
        private TextBox specialtyTextBox;
        private Label shiftLabel;
        private NumericUpDown shiftUpDown;
        private Button addButton;
        private Button cancelButton;
        private ErrorProvider errorProvider;
    }
}
