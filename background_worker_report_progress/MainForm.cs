using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace background_worker_report_progress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Progress += (e) =>
            {
                Invoke((MethodInvoker)delegate 
                {
                    labelStatus.Text = $"{e.Count} of {e.Total}";
                });
            };
        }
        public event ProgressEventHandler Progress;

        private void btnWorker_CheckedChanged(object sender, EventArgs e)
        {
            if(btnWorker.Checked)
            {
                _cts = new CancellationTokenSource();
                Task.Run(() => 
                {
                    var formCount = 10000;
                    for (int i = 0; i < 10000; i++)
                    {
                        if(_cts.IsCancellationRequested)
                        {
                            return;
                        }
                        // Notify every 100 times.
                        if((i % 100) == 0)
                        {
                            Progress?.Invoke(new ProgressEventArgs(count: i, total: formCount));
                        }
                        MockCreateForm();
                    }
                    Progress?.Invoke(new ProgressEventArgs(count: formCount, total: formCount));
                }, _cts.Token);
            }
            else
            {
                _cts.Cancel();
                labelStatus.Text = "Idle";
            }
        }
        CancellationTokenSource _cts = null;
        public void MockCreateForm()
        {
            Task.Delay(5).Wait();
        }
    }

    public delegate void ProgressEventHandler(ProgressEventArgs e);
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int count, int total)
        {
            Count = count;
            Total = total;
        }
        public int Count { get; }
        public int Total { get; }
    }
}
