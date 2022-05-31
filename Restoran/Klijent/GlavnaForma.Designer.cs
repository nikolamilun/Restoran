
namespace Klijent
{
    partial class GlavnaForma
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlavnaForma));
            this.tabovi = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.oAplikaciji = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.meniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jeloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sastojciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.naMenijuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seSastojiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tabovi.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabovi
            // 
            this.tabovi.Controls.Add(this.tabPage1);
            this.tabovi.Controls.Add(this.oAplikaciji);
            this.tabovi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabovi.Location = new System.Drawing.Point(0, 0);
            this.tabovi.Name = "tabovi";
            this.tabovi.SelectedIndex = 0;
            this.tabovi.Size = new System.Drawing.Size(800, 450);
            this.tabovi.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Prikaz i izmena podataka";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // oAplikaciji
            // 
            this.oAplikaciji.Location = new System.Drawing.Point(4, 22);
            this.oAplikaciji.Name = "oAplikaciji";
            this.oAplikaciji.Padding = new System.Windows.Forms.Padding(3);
            this.oAplikaciji.Size = new System.Drawing.Size(792, 424);
            this.oAplikaciji.TabIndex = 1;
            this.oAplikaciji.Text = "O Aplikaciji";
            this.oAplikaciji.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(786, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "alati";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.meniToolStripMenuItem,
            this.jeloToolStripMenuItem,
            this.sastojciToolStripMenuItem,
            this.naMenijuToolStripMenuItem,
            this.seSastojiToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(53, 22);
            this.toolStripDropDownButton1.Text = "Tabele";
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(158, 22);
            this.toolStripDropDownButton2.Text = "Specijalne funkcionalnosti";
            // 
            // meniToolStripMenuItem
            // 
            this.meniToolStripMenuItem.Name = "meniToolStripMenuItem";
            this.meniToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.meniToolStripMenuItem.Text = "Meni";
            // 
            // jeloToolStripMenuItem
            // 
            this.jeloToolStripMenuItem.Name = "jeloToolStripMenuItem";
            this.jeloToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.jeloToolStripMenuItem.Text = "Jelo";
            // 
            // sastojciToolStripMenuItem
            // 
            this.sastojciToolStripMenuItem.Name = "sastojciToolStripMenuItem";
            this.sastojciToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sastojciToolStripMenuItem.Text = "Sastojci";
            // 
            // naMenijuToolStripMenuItem
            // 
            this.naMenijuToolStripMenuItem.Name = "naMenijuToolStripMenuItem";
            this.naMenijuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.naMenijuToolStripMenuItem.Text = "Na meniju";
            // 
            // seSastojiToolStripMenuItem
            // 
            this.seSastojiToolStripMenuItem.Name = "seSastojiToolStripMenuItem";
            this.seSastojiToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.seSastojiToolStripMenuItem.Text = "Se sastoji";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Size = new System.Drawing.Size(786, 393);
            this.splitContainer1.SplitterDistance = 269;
            this.splitContainer1.TabIndex = 1;
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(513, 393);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // GlavnaForma
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabovi);
            this.Name = "GlavnaForma";
            this.Text = "Restoran";
            this.tabovi.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabovi;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage oAplikaciji;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem meniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jeloToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sastojciToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem naMenijuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seSastojiToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
    }
}

