namespace Orbital.View
{
    partial class StartUp
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
            crewMemberGridView = new DataGridView();
            listButton = new Button();
            addButton = new Button();
            ((System.ComponentModel.ISupportInitialize)crewMemberGridView).BeginInit();
            SuspendLayout();
            //
            // crewMemberGridView
            //
            crewMemberGridView.AllowUserToAddRows = false;
            crewMemberGridView.AllowUserToDeleteRows = false;
            crewMemberGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            crewMemberGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            crewMemberGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            crewMemberGridView.Location = new Point(12, 52);
            crewMemberGridView.Name = "crewMemberGridView";
            crewMemberGridView.ReadOnly = true;
            crewMemberGridView.RowHeadersVisible = false;
            crewMemberGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            crewMemberGridView.Size = new Size(598, 386);
            crewMemberGridView.TabIndex = 2;
            //
            // listButton
            //
            listButton.Location = new Point(12, 12);
            listButton.Name = "listButton";
            listButton.Size = new Size(100, 30);
            listButton.TabIndex = 0;
            listButton.Text = "Listázás";
            listButton.UseVisualStyleBackColor = true;
            listButton.Click += ListCrewMembers;
            //
            // addButton
            //
            addButton.Location = new Point(120, 12);
            addButton.Name = "addButton";
            addButton.Size = new Size(100, 30);
            addButton.TabIndex = 1;
            addButton.Text = "Hozzáadás";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddCrewMember;
            //
            // StartUp
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(622, 450);
            Controls.Add(addButton);
            Controls.Add(listButton);
            Controls.Add(crewMemberGridView);
            Name = "StartUp";
            Text = "Orbitális Személyzeti Rendszer";
            ((System.ComponentModel.ISupportInitialize)crewMemberGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView crewMemberGridView;
        private Button listButton;
        private Button addButton;
    }
}
