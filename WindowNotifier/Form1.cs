using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowNotifier
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Displaynotify();
        }

        protected void Displaynotify()
        {
            try
            {
                notifier.Icon = new System.Drawing.Icon(Path.GetFullPath(@"image\window.ico"));
                notifier.Text = "Пожалуйста, выполните следующее";
                notifier.Visible = true;
                notifier.BalloonTipTitle = "Откройте окно на уровне: ПОЛНОСТЬЮ";
                notifier.BalloonTipText = "Нажмите, когда выполните!";
                notifier.ShowBalloonTip(10000);
                notifier.Click += Notifier_Click;
            }
            catch (Exception ex)
            {
            }
        }

        private void Notifier_Click(object sender, EventArgs e)
        {
            
        }
    }
}
