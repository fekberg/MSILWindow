using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;

namespace MSILWindow.Window
{
    public partial class WindowControl : UserControl
    { 
        Thread thread;

        public WindowControl()
        {
            InitializeComponent();

            var c = new CommandServer(this);
            thread = new Thread(c.Start);
            thread.Start();
        }

        public void Update(string text)
        {
            result.BeginInvoke(new Action(() =>
            {
                result.Text = text;
            }));
        }
    }
}
