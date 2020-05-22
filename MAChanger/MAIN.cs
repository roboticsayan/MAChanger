﻿using System;
using ReaLTaiizor;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace MAChanger
{
    public partial class MAIN : LostForm
    {
        public MAIN()
        {
            InitializeComponent();
        }

        private void MAIN_Load(object sender, EventArgs e)
        {
            foreach (NetworkInterface Adapter in NetworkInterface.GetAllNetworkInterfaces().Where(FA => Adapter.ControlMAC(FA.GetPhysicalAddress().GetAddressBytes(), true)).OrderByDescending(FA => FA.IsReceiveOnly))
                Adapters_CB.Items.Add(new Adapter(Adapter));

            if (Adapters_CB.Items.Count > 0)
                Adapters_CB.SelectedIndex = Adapters_CB.Items.Count - 1;
        }

        public void UA()
        {
            Adapter MAC = Adapters_CB.SelectedItem as Adapter;
            Current_TB.Text = MAC.GMAC;
            if (!string.IsNullOrEmpty(MAC.RegistryMAC))
                New_TB.Text = MAC.RegistryMAC;
            else
                New_TB.Text = Current_TB.Text;
            Save_B.Enabled = (Current_TB.Text != New_TB.Text);
        }

        private void Refresh_B_Click(object sender, EventArgs e)
        {
            UA();
        }

        private void Adapters_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            UA();
        }

        private void Generate_B_Click(object sender, EventArgs e)
        {
            New_TB.Text = Adapter.CreateMAC();
        }

        private void New_TB_TextChanged(object sender, EventArgs e)
        {
            Save_B.Enabled = (Adapter.ControlMAC(New_TB.Text, false) == (Current_TB.Text != New_TB.Text));
        }

        private void Save_B_Click(object sender, EventArgs e)
        {
            if (Adapter.ControlMAC(New_TB.Text, false))
                SetMAC(New_TB.Text, "Change MAC Address");
            else
                MessageBox.Show("The MAC Address Entered is Invalid, It Will Not Be Updated!", "Invalid MAC Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Undo_B_Click(object sender, EventArgs e)
        {
            SetMAC("", "Undo MAC Address");
        }

        public void SetMAC(string MAC, string Title)
        {
            Adapter Adapter = Adapters_CB.SelectedItem as Adapter;

            if (Adapter.SetRegistryMAC(MAC, Title))
            {
                Thread.Sleep(111);
                MessageBox.Show("MAC Address Successfully Changed!", Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                UA();
            }
        }
    }
}