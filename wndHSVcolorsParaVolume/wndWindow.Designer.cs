namespace wndHSVcolorsParaVolume
{
    partial class wndWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirImagemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarImagemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbErros = new System.Windows.Forms.ComboBox();
            this.grpBxMensagens = new System.Windows.Forms.GroupBox();
            this.cmbBxFormatChoice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBxTypePerspective = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.grpBxMensagens.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1001, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirImagemToolStripMenuItem,
            this.salvarImagemToolStripMenuItem,
            this.sairToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // abrirImagemToolStripMenuItem
            // 
            this.abrirImagemToolStripMenuItem.Name = "abrirImagemToolStripMenuItem";
            this.abrirImagemToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.abrirImagemToolStripMenuItem.Text = "Abrir Imagem";
            this.abrirImagemToolStripMenuItem.Click += new System.EventHandler(this.abrirImagemToolStripMenuItem_Click);
            // 
            // salvarImagemToolStripMenuItem
            // 
            this.salvarImagemToolStripMenuItem.Name = "salvarImagemToolStripMenuItem";
            this.salvarImagemToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.salvarImagemToolStripMenuItem.Text = "Salvar Imagem";
            this.salvarImagemToolStripMenuItem.Click += new System.EventHandler(this.salvarImagemToolStripMenuItem_Click);
            // 
            // sairToolStripMenuItem
            // 
            this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            this.sairToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sairToolStripMenuItem.Text = "Sair";
            this.sairToolStripMenuItem.Click += new System.EventHandler(this.sairToolStripMenuItem_Click);
            // 
            // cmbErros
            // 
            this.cmbErros.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbErros.FormattingEnabled = true;
            this.cmbErros.Location = new System.Drawing.Point(0, 23);
            this.cmbErros.Name = "cmbErros";
            this.cmbErros.Size = new System.Drawing.Size(872, 23);
            this.cmbErros.TabIndex = 1;
            // 
            // grpBxMensagens
            // 
            this.grpBxMensagens.Controls.Add(this.cmbErros);
            this.grpBxMensagens.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxMensagens.Location = new System.Drawing.Point(0, 519);
            this.grpBxMensagens.Name = "grpBxMensagens";
            this.grpBxMensagens.Size = new System.Drawing.Size(1001, 73);
            this.grpBxMensagens.TabIndex = 9;
            this.grpBxMensagens.TabStop = false;
            this.grpBxMensagens.Text = "Mensagens:";
            // 
            // cmbBxFormatChoice
            // 
            this.cmbBxFormatChoice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxFormatChoice.FormattingEnabled = true;
            this.cmbBxFormatChoice.Items.AddRange(new object[] {
            "Grid format",
            "Fill format"});
            this.cmbBxFormatChoice.Location = new System.Drawing.Point(731, 37);
            this.cmbBxFormatChoice.Name = "cmbBxFormatChoice";
            this.cmbBxFormatChoice.Size = new System.Drawing.Size(243, 28);
            this.cmbBxFormatChoice.TabIndex = 10;
            this.cmbBxFormatChoice.SelectedIndexChanged += new System.EventHandler(this.cmbBxFormatChoice_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(616, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "Format Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(580, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Perspective Type";
            // 
            // cmbBxTypePerspective
            // 
            this.cmbBxTypePerspective.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBxTypePerspective.FormattingEnabled = true;
            this.cmbBxTypePerspective.Items.AddRange(new object[] {
            "Isometric",
            "Geometric",
            "Formal Isometric",
            "Dimetric NEN-ISO",
            "Dimetric Chinese Scroll Paints",
            "Dimetric Computer Games Side View",
            "Dimetric Computer Games top View",
            "Isometric Computer Games"});
            this.cmbBxTypePerspective.Location = new System.Drawing.Point(731, 109);
            this.cmbBxTypePerspective.Name = "cmbBxTypePerspective";
            this.cmbBxTypePerspective.Size = new System.Drawing.Size(243, 28);
            this.cmbBxTypePerspective.TabIndex = 13;
            this.cmbBxTypePerspective.SelectedIndexChanged += new System.EventHandler(this.cmbBxTypePerspective_SelectedIndexChanged);
            // 
            // wndWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 620);
            this.Controls.Add(this.cmbBxTypePerspective);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbBxFormatChoice);
            this.Controls.Add(this.grpBxMensagens);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "wndWindow";
            this.Text = "Project  Brightness-Volume";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpBxMensagens.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirImagemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarImagemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpBxMensagens;
        public System.Windows.Forms.ComboBox cmbErros;
        private System.Windows.Forms.ComboBox cmbBxFormatChoice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBxTypePerspective;
    }
}

