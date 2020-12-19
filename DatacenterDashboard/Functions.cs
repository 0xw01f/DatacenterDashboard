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

    class Functions
    {
        //  Check IPv4, from https://stackoverflow.com/a/11412991/14304544
        public bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        // Softblink function From https://stackoverflow.com/a/44467337/14304544
        public async void SoftBlink(Guna2Panel ctrl, Color c1, Color c2, short CycleTime_ms, bool BkClr)
        {
            var sw = new Stopwatch(); sw.Start();
            short halfCycle = (short)Math.Round(CycleTime_ms * 0.5);
            while (true)
            {

                await Task.Delay(1);
                var n = sw.ElapsedMilliseconds % CycleTime_ms;
                var per = (double)Math.Abs(n - halfCycle) / halfCycle;
                var red = (short)Math.Round((c2.R - c1.R) * per) + c1.R;
                var grn = (short)Math.Round((c2.G - c1.G) * per) + c1.G;
                var blw = (short)Math.Round((c2.B - c1.B) * per) + c1.B;
                var clr = Color.FromArgb(red, grn, blw);

                if (BkClr) ctrl.FillColor = clr; else ctrl.ForeColor = clr;
            }
        }

        // Set a panel on error status
        public void setError(Guna2Panel targetPanel, Label statusLabel, List<string> lstOffline)
        {

            var match = lstOffline
                .FirstOrDefault(stringToCheck => stringToCheck.Contains(targetPanel.Name));
            if (match == null)
            {

                SoftBlink(targetPanel, Color.FromArgb(30, 30, 30), Color.FromArgb(79, 35, 47), 2000, true);

                targetPanel.BorderColor = Color.FromArgb(210, 44, 50);

                statusLabel.Text = "● OFFLINE";
                statusLabel.ForeColor = Color.FromArgb(210, 44, 50);

                lstOffline.Add(targetPanel.Name);
            }
        }


        public void removePanel(string pName, Control cntrl, List<string> lstOffline, List<string> lstOnline)
        {
            //to remove control by Name
            foreach (Guna2Panel item in cntrl.Controls.OfType<Guna2Panel>())
            {
                if (item.Name == pName)
                {
                    cntrl.Controls.Remove(item);
                    lstOffline.Remove(pName);
                    lstOnline.Remove(pName);
                }

            }
        }
    }
}
