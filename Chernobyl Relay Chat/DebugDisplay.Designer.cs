﻿namespace Chernobyl_Relay_Chat
{
    partial class DebugDisplay
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
            this.textBoxRaw = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxRaw
            // 
            this.textBoxRaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRaw.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxRaw.Location = new System.Drawing.Point(0, 0);
            this.textBoxRaw.Multiline = true;
            this.textBoxRaw.Name = "textBoxRaw";
            this.textBoxRaw.ReadOnly = true;
            this.textBoxRaw.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxRaw.Size = new System.Drawing.Size(1169, 339);
            this.textBoxRaw.TabIndex = 0;
            // 
            // DebugDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 339);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxRaw);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DebugDisplay";
            this.Text = "Raw Messages";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxRaw;
    }
}