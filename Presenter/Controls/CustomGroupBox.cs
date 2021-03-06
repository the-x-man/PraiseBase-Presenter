﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PraiseBase.Presenter.Controls
{
    public partial class CustomGroupBox : Panel
    {
        [Description("Title of the groupbox"), Category("CustomGroupBox"), DefaultValue("Title"), Localizable(true)]
        public String Title 
        { 
            get 
            { 
                return labelTitle.Text; 
            } 
            set
            { 
                labelTitle.Text = value; 
            } 
        }

        public CustomGroupBox()
        {
            InitializeComponent();
        }

        private void CustomGroupBox_Paint(object sender, PaintEventArgs e)
        {
            panelTitleBG.Size = new System.Drawing.Size(Width, 28);
        }
    }
}