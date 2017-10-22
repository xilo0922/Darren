using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<LoginEntities> login = new List<LoginEntities>();
            login.Add(ControlsUtilities.GetControls<LoginEntities>(this));
            LoginEntities Getle = ControlsUtilities.GetControls<LoginEntities>(this);
            Getle.UserName = "李四";
            Getle.TimeZoneId = null;
            Getle.ToggleSwitch = true;
            LoginEntities Setle = ControlsUtilities.SetControls<LoginEntities>(this, Getle);
            this.dataGridView1.DataSource = login;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioGroup1.SelectedIndex = 0;
        }
    }
}