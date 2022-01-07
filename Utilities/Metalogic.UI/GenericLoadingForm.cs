using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metalogic.UI
{
    public class LoadFileEvent<T> : EventArgs
    {
        public T File { get; set; }
        public int CurIndex { get; set; }

        public int FilesCount { get; set; }
    }

    public partial class GenericLoadingForm<T> : Form
    {
        private GenericLoadingForm()
        {
            InitializeComponent();
        }

        void LoadingForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private int _treadNo;
        private int _totalFilesCount;
        public GenericLoadingForm(IEnumerable<T> files, int treadNo = 1)
        {
            InitializeComponent();

            _treadNo = treadNo;
            _files = files.ToList();
            _totalFilesCount = _files.Count;
            Setup();
        }

        public GenericLoadingForm(IEnumerable<List<T>> filebatches, int treadNo = 1)
        {
            InitializeComponent();

            _treadNo = treadNo;
            _fileBatches = filebatches.ToList();
            _totalFilesCount = _fileBatches.Sum(x => x.Count);
            Setup();
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
            Load += LoadingFormLoad;
            Closing += LoadingForm_Closing;
            this.Size = new Size(this.Width + 100, this.Height);
        }

        
        private void LoadingFormLoad(object sender, EventArgs e)
        {
            if (_totalFilesCount < 1)
            {
                Closing -= LoadingForm_Closing;
                Close();
            }

            // Set Maximum to the total number of files to copy.
            pBar1.Maximum = _totalFilesCount;
            var workerThread = new Thread(ExecuteJobs);
            workerThread.Start();
        }

        public event FileLoadEvent ProcessFile;

        public delegate void FileLoadEvent(object sender, LoadFileEvent<T> e);


        private readonly List<T> _files = null;

        private readonly List<List<T>> _fileBatches = null;





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

        
        private void ExecuteJobs()
        {
            HandleBeginProcessFiles();

            if (_files != null)
            {
                HandleFileBatchesInternal(_files);
            }
            else
            {
                foreach (var fileBatch in _fileBatches)
                {
                    HandleFileBatchesInternal(fileBatch);
                }
            }

            HandleFinishedProcessFiles();

            BeginInvoke(new IncreaseCounterd(SendClose), null);
        }

        private void HandleFileBatchesInternal(IEnumerable<T> files)
        {
            if (_treadNo > 1)
            {
                Parallel.ForEach(
                    files,
                    new ParallelOptions { MaxDegreeOfParallelism = _treadNo },
                    (line, state, index) =>
                    {
                        HandleFileInternal(line, (int)index);
                    }
                );
            }
            else
            {
                var index = 0;
                foreach (var file in files)
                {
                    HandleFileInternal(file, index++);
                }
            }
        }

        private void HandleFileInternal(T file, int index)
        {
            HandleFile(file);
            ProcessFile?.Invoke(this, new LoadFileEvent<T> { File = file, CurIndex = index, FilesCount = _files.Count });

            // Perform the increment on the ProgressBar.
            BeginInvoke(new IncreaseCounterd(IncreaseCounter), null);
        }

        protected virtual void HandleBeginProcessFiles()
        {
        }
        protected virtual void HandleFile(T file)
        {
        }

        protected virtual void HandleFinishedProcessFiles()
        {
            
        }

        public void SetTitle(string title)
        {
            BeginInvoke(new Action(() =>
            {
                Text = title;
            }));
        }

    }
}
