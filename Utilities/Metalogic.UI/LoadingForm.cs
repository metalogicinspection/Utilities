using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Metalogic.UI
{
    public partial class LoadingForm : Form
    {
        private LoadingForm()
        {
            InitializeComponent();
        }

        void LoadingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public LoadingForm(IEnumerable<string> files)
        {
            InitializeComponent();

            _files = new Queue<string>(files);
            pBar1.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            pBar1.Minimum = 1;
            // Set the initial value of the ProgressBar.
            pBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            pBar1.Step = 1;
            Load += LoadingFormLoad;
            Closing += LoadingForm_Closing;
            this.Size = new Size(this.Width + 100, this.Height);
        }

        private void LoadingFormLoad(object sender, EventArgs e)
        {
            if (!_files.Any())
            {
                Closing -= LoadingForm_Closing;
                Close();
            }

            // Set Maximum to the total number of files to copy.
            pBar1.Maximum = _files.Count;
            var workerThread = new Thread(ExecuteJobs);
            workerThread.Start();
        }
        


        private readonly Queue<string> _files;






        public delegate void IncreaseCounterd();

        public void IncreaseCounter()
        {
            pBar1.PerformStep();
        }

        public void SendClose()
        {
            Closing -= LoadingForm_Closing;
            Close();
        }


        private int _finishedItemCount = 0;
        private void ExecuteJobs()
        {
            HandleBeginProcessFiles();
            
            while (_files.Any())
            {
                var file = _files.Dequeue();
                HandleFile(file);
                ++_finishedItemCount;
                // Perform the increment on the ProgressBar.
                BeginInvoke(new IncreaseCounterd(IncreaseCounter), null);
            }
            HandleFinishedProcessFiles();

            BeginInvoke(new IncreaseCounterd(SendClose), null);
        }


        protected virtual void HandleBeginProcessFiles()
        {
            throw new NotImplementedException();
        }

        protected virtual void HandleFile(string file)
        {
            throw new NotImplementedException();
        }

        protected virtual void HandleFinishedProcessFiles()
        {

            throw new NotImplementedException();
        }
    }
}
