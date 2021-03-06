﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace Battlelogium
{
    public partial class BattlelogiumConfigEditor : Form
    {
        BattlelogiumConfiguration config;

        public BattlelogiumConfigEditor(BattlelogiumConfiguration config)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            InitializeComponent();
            this.config = config;

            //Load General Settings
            this.waitTimeToKillOrigin_input.Value = config.WaitTimeToKillOrigin;
            this.customJsEnabled_input.Checked = config.CustomJsEnabled;
            this.directToCampaign_input.Checked = config.DirectToCampaign;
            this.checkUpdates_input.Checked = config.CheckUpdates;
            this.useSoftwareRender_input.Checked = config.UseSoftwareRender;
            this.startOrigin_input.Checked = config.HandleOrigin;

            //Apply window height maximum and minimums
            this.windowHeight_input.Maximum = (decimal) SystemParameters.PrimaryScreenHeight;
            this.windowWidth_input.Maximum = (decimal) SystemParameters.PrimaryScreenWidth;

            this.windowHeight_input.Minimum = 400;
            this.windowWidth_input.Minimum = 400;  

            //Load Window Settings
            this.windowedMode_input.Checked = config.WindowedMode;
            this.startMaximized_input.Checked = config.StartMaximized;
            this.noBorder_input.Checked = config.NoBorder;
            this.windowHeight_input.Value = config.WindowHeight;
            this.windowWidth_input.Value = config.WindowWidth;

            if (this.CheckForParFix()) this.parFixBtn.Text = "Restore Origin Requirement";
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BattlelogiumParManager.exe"))) this.parFixBtn.Enabled = false;
            
            this.Refresh();
   
        }

        private void windowedMode_input_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.windowedMode_input.Checked)
            {
                this.startMaximized_input.Enabled = false;
                this.noBorder_input.Enabled = false;
                this.windowHeight_input.Enabled = false;
                this.windowWidth_input.Enabled = false;

            }
            if (this.windowedMode_input.Checked)
            {
                this.startMaximized_input.Enabled = true;
                this.noBorder_input.Enabled = true;
                this.windowHeight_input.Enabled = true;
                this.windowWidth_input.Enabled = true;

            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            this.config.WriteConfig("waitTimeToKillOrigin", ((int) this.waitTimeToKillOrigin_input.Value).ToString());
            this.config.WriteConfig("customJsEnabled", this.customJsEnabled_input.Checked.ToString());
            this.config.WriteConfig("directToCampaign", this.directToCampaign_input.Checked.ToString());
            this.config.WriteConfig("checkUpdates", this.checkUpdates_input.Checked.ToString());
            this.config.WriteConfig("useSoftwareRender", this.useSoftwareRender_input.Checked.ToString());
            this.config.WriteConfig("handleOrigin", this.startOrigin_input.Checked.ToString());

            this.config.WriteConfig("windowedMode", this.windowedMode_input.Checked.ToString());
            this.config.WriteConfig("startMaximized", this.startMaximized_input.Checked.ToString());
            this.config.WriteConfig("noBorder", this.noBorder_input.Checked.ToString());
            this.config.WriteConfig("windowWidth", ((int)this.windowWidth_input.Value).ToString());
            this.config.WriteConfig("windowHeight", ((int)this.windowHeight_input.Value).ToString());

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (Utilities.ShowChoiceDialog("Are you sure you want to discard your changes?", "Discard Changes", "Discard Changes", "Cancel"))
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                return;
            }

        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 1 && e.Column == 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.CellBounds);
            }
            if (e.Row == 1 && e.Column == 1)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.CellBounds);
            }
        }

        /// <summary>Checks if the Par Fix is installed</summary>
        private bool CheckForParFix()
        {
            try
            {
                if (!File.Exists(Path.Combine(Utilities.GetBF3Path(), "bf3.par.orig")))
                {
                    return false; //return false if fix is not installed
                }
                if (File.Exists(Path.Combine(Utilities.GetBF3Path(), "bf3.par.orig")))
                {
                    return true; //return true if fix is installed
                }
                return false;
            }
            catch (FileNotFoundException)
            {
                return false; //Return false if Battlefield 3 not found. It's ParManager's job to tell the user.
            }
        }

        private void parFixBtn_Click(object sender, EventArgs e)
        {
            ProcessStartInfo parFix = new ProcessStartInfo("BattlelogiumParManager.exe");
            bool startOrigin = true;
            if(!CheckForParFix()){
                parFix.Arguments = "remove";
                startOrigin = false;
            }
            if(CheckForParFix()){
                parFix.Arguments = "restore";
                startOrigin = true;
            }
            Process parFixProcess = Process.Start(parFix);
            parFixProcess.WaitForExit();
            if (parFixProcess.ExitCode == 0) this.startOrigin_input.Checked = startOrigin;
            this.parFixBtn.Enabled = false;
        }


    }
}
