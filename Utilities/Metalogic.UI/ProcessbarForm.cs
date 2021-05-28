using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Metalogic.UI
{
   

    public partial class ProcessbarForm : Form
    {
        private ProcessbarForm()
        {
            InitializeComponent();

        }
        
        
        private readonly int _totalFilesCount;


        public ProcessbarForm(int totalFilesCount)
        {
            InitializeComponent();
            
            _totalFilesCount = totalFilesCount;
            Setup();
            this.Load += ProcessbarForm_Load;
            this.Closing += ProcessbarForm_Closing;
        }

        private void ProcessbarForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void ProcessbarForm_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                BackendWork?.Invoke(this);
            }).Start();
        }

        private void Setup()
        {
            pBar1.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            pBar1.Minimum = 1;
            // Set the initial value of the ProgressBar.
            pBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            pBar1.Step = 1;
            pBar1.Maximum = _totalFilesCount;
            this.Size = new Size(this.Width + 100, this.Height);
        }

        
        
        public event BackendWorkEvent BackendWork;

        public delegate void BackendWorkEvent(object sender);





        public delegate void IncreaseCounterd();

        public void IncreaseCounter()
        {
            BeginInvoke(new Action(() =>
            {
                pBar1.PerformStep();
            }));
        }


        public void AskForClose()
        {
            BeginInvoke(new Action(() =>
            {
                Closing -= ProcessbarForm_Closing;
                Close();
            }));
        }
    }
        
        
        
        
    
}
