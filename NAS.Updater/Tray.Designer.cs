using System.Windows.Forms;

namespace NAS.Updater
{
    partial class Tray
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private NotifyIcon ico;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                ico.Visible = false;
                ico.Icon = null;
                ico = null;
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Tray";

            ico = new NotifyIcon()
            {
                Icon = Properties.Resources.Iconmuseo_Mechanic_Folders_Arm,
                BalloonTipTitle = "Nas.Updater",
                BalloonTipText = "Nas wird gerade Upgedatet",
                BalloonTipIcon = ToolTipIcon.Info,
                Visible = true
            };


            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }

        #endregion
    }
}