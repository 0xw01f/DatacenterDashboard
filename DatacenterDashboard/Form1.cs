using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Guna.UI2.WinForms;
using System.Drawing.Drawing2D;

namespace DatacenterDashboard
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        Functions functions = new Functions();

        List<string> pnlsOnline = new List<string>();
        List<string> pnlsOffline = new List<string>();
        static Random rnd = new Random();

        string selectedPanelName = "";
        public Form1()
        {
            InitializeComponent();
        }


        // =========================
        // Dynamic Form REGION
        #region "Dynamic Form"

        //  From https://stackoverflow.com/a/31200177/14304544
        protected override void WndProc(ref Message m)
        {
            const int RESIZE_HANDLE_SIZE = 10;

            switch (m.Msg)
            {
                case 0x0084/*NCHITTEST*/ :
                    base.WndProc(ref m);

                    if ((int)m.Result == 0x01/*HTCLIENT*/)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13/*HTTOPLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12/*HTTOP*/ ;
                            else
                                m.Result = (IntPtr)14/*HTTOPRIGHT*/ ;
                        }
                        else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE))
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10/*HTLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)2/*HTCAPTION*/ ;
                            else
                                m.Result = (IntPtr)11/*HTRIGHT*/ ;
                        }
                        else
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16/*HTBOTTOMLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15/*HTBOTTOM*/ ;
                            else
                                m.Result = (IntPtr)17/*HTBOTTOMRIGHT*/ ;
                        }
                    }
                    return;
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }

        #endregion
        // =========================
        // =========================
        // Add server panel REGION
        #region "AddServer"
        public void addServer(string srvHostName, string srvAddress)
        {
            Guna2Panel newPanel = new Guna2Panel();

            PictureBox picServer = new PictureBox();

            //  === Picture
            picServer.BackColor = System.Drawing.Color.Transparent;
            //  Set random picture or set depending on a parameter

            if (drpSrvType.SelectedItem.ToString() == "Server")
            {
                picServer.Image = global::DatacenterDashboard.Properties.Resources.globe_80px;

            }
            else if (drpSrvType.SelectedItem.ToString() == "Database")
            {
                picServer.Image = global::DatacenterDashboard.Properties.Resources.database_80px;
            }



            picServer.Location = new System.Drawing.Point(6, 13);
            picServer.Size = new System.Drawing.Size(50, 50);
            picServer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            picServer.MouseClick += PicServer_MouseClick;
            //  ===

            //  === Labels
            for (int i = 0; i < 7; i++)
            {
                Label lblDescr = new Label();

                lblDescr.AutoSize = true;
                lblDescr.BackColor = System.Drawing.Color.Transparent;
                lblDescr.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                lblDescr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(198)))), ((int)(((byte)(203)))));
                lblDescr.Size = new System.Drawing.Size(75, 19);
                lblDescr.Tag = i;
                lblDescr.MouseClick += LblDescr_MouseClick;
                if (i == 1)
                {
                    lblDescr.Location = new System.Drawing.Point(63, 10);
                    lblDescr.Text = "Hostname:";
                    lblDescr.Tag = "lblsHost";
                }
                else if (i == 2)
                {
                    lblDescr.Location = new System.Drawing.Point(63, 30);
                    lblDescr.Text = "Status:";
                    lblDescr.Tag = "lblsStatus";
                }
                else if (i == 3)
                {
                    lblDescr.Location = new System.Drawing.Point(63, 50);
                    lblDescr.Text = "Address:";
                    lblDescr.Tag = "lblsAddress";
                }
                else if (i == 4)
                {
                    lblDescr.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblDescr.Location = new System.Drawing.Point(151, 10);
                    lblDescr.Text = srvHostName;
                }
                else if (i == 5)
                {
                    lblDescr.Location = new System.Drawing.Point(151, 30);
                    lblDescr.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblDescr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(179)))), ((int)(((byte)(90)))));
                    lblDescr.Text = "● ONLINE";
                    lblDescr.Tag = "lblStatus";
                }
                else if (i == 6)
                {
                    lblDescr.Location = new System.Drawing.Point(151, 50);
                    lblDescr.Text = srvAddress;
                }


                newPanel.Controls.Add(lblDescr);
            }

            //  ===


            newPanel.BorderColor = System.Drawing.Color.Gray;
            newPanel.BorderRadius = 4;
            newPanel.BorderThickness = 1;

            newPanel.Controls.Add(picServer);


            newPanel.Cursor = System.Windows.Forms.Cursors.Hand;
            newPanel.CustomBorderColor = System.Drawing.Color.Gray;
            newPanel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(67)))));
            newPanel.Location = new System.Drawing.Point(10, 10);
            newPanel.Name = "pnl" + txtAddress.Text;
            newPanel.ShadowDecoration.Parent = newPanel;
            newPanel.Size = new System.Drawing.Size(240, 77);
            //newPanel.ContextMenuStrip = contextMenuStrip1;
            newPanel.MouseClick += NewPanel_MouseClick;
            pnlsOnline.Add(newPanel.Name);
            flowLayoutPanel1.Controls.Add(newPanel);

        }

        #endregion
        // =========================



        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }



        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if(txtHostName.Text == "" || txtAddress.Text == "")
            {
                MessageBox.Show("Error: Set text.");
            } else
            {
                if (functions.ValidateIPv4(txtAddress.Text))
                {
                    var match = pnlsOffline
                   .FirstOrDefault(stringToCheck => stringToCheck.Contains("pnl" + txtAddress.Text));
                    var match2 = pnlsOnline
                        .FirstOrDefault(stringToCheck => stringToCheck.Contains("pnl" + txtAddress.Text));
                    if (match == null && match2 == null)
                    {
                        addServer(txtHostName.Text, txtAddress.Text);
               
                    }
                    else
                    {
                        MessageBox.Show("Error: This address exists already, please choose another one.");
                    }
                } else
                {
                    MessageBox.Show("Error: IPv4 address incorrect.");
                }
               
            }
            
        }

       


        private void PicServer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                string pnlName = ((PictureBox)sender).Parent.Name;
                selectedPanelName = pnlName;
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void LblDescr_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                string pnlName = ((Label)sender).Parent.Name;
                selectedPanelName = pnlName;
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void NewPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                string pnlName = ((Guna2Panel)sender).Name;
                selectedPanelName = pnlName;
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
          
        }


        private void testeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you confirm that you want to remove this server ?", "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                functions.removePanel(selectedPanelName, flowLayoutPanel1, pnlsOffline, pnlsOnline);
            }
          
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {

            int r = rnd.Next(pnlsOnline.Count);
            //MessageBox.Show((string)pnlsOnline[r]);


            foreach (Guna2Panel cnP in flowLayoutPanel1.Controls)
            {
                if (cnP is Guna2Panel)
                {
                    foreach (Control cnL in cnP.Controls)
                    {                     
                        if (cnL is Label)
                        {
                            Label lb = (Label)cnL;
                            if (lb.Tag.ToString() == "lblStatus") { functions.setError(cnP, lb, pnlsOffline); }
                        }
                    }
                }
            }

  
              
         


        }
        public IEnumerable<Guna2Panel> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Guna2Panel>();
            


            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
       
        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {


            foreach (Guna2Panel item in flowLayoutPanel1.Controls.OfType<Guna2Panel>())
            {

                if (item.Name == selectedPanelName)
                {
                    foreach (Control cnL in item.Controls)
                    {
                        if (cnL is Label)
                        {
                            Label lb = (Label)cnL;
                            if (lb.Tag.ToString() == "lblStatus") { functions.setError(item, lb, pnlsOffline); }
                        }
                    }
                }

            }
        }


        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Guna2Panel item in flowLayoutPanel1.Controls.OfType<Guna2Panel>())
            {

                if (item.Name == selectedPanelName)
                {
                    foreach (Control cnL in item.Controls)
                    {
                        if (cnL is Label)
                        {
                            Label lb = (Label)cnL;
                            if (lb.Tag.ToString() == "lblStatus") {


                                    item.FillColor = Color.FromArgb(45, 55, 67);
                                    item.BorderColor = Color.Gray;
                                    lb.Text = "● ONLINE";
                                    lb.ForeColor = Color.FromArgb(70, 179, 90);
                                    pnlsOnline.Add(item.Name);
                            }
                        }
                    }
                }

            }
        }
    }
}
