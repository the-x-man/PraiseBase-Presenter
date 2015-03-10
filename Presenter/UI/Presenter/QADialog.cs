﻿using System;
using System.Windows.Forms;

namespace PraiseBase.Presenter.UI.Presenter
{
    public partial class QADialog : Form
    {
        public QADialog()
        {
            InitializeComponent();
        }

        private void QADialog_Load(object sender, EventArgs e)
        {
            textBoxComment.Text = SongManager.Instance.CurrentSong.Song.Comment;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            SongManager.Instance.CurrentSong.Song.Comment = textBoxComment.Text;
            try
            {
                SongManager.Instance.SaveCurrentSong();

                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}