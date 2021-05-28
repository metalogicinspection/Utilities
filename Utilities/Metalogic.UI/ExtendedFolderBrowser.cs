using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MetaphaseQA
{
    public delegate bool ShowNewButtonHandler(string selectedPath);

    /// Created By:				Goldberg Royee
    /// Date:					6/8/2006
    /// Reason:					This class is an extended class
    /// for the folderBrowseDialog of .NET.
    /// This class add a functionality to disable the 'Make New Folder' Button
    /// whenever a CD path selected.
    public class ExtendedFolderBrowser
    {
        private int _x;
        private int _y;

        #region Private Members

        private FolderBrowserDialog m_InternalFolderBrowser = null;
        #endregion

        #region CTOR
        /// <summary>
        /// 
        /// </summary>
        public ExtendedFolderBrowser(int x, int y, string selectedPath)
        {
            _x = x;
            _y = y;

            m_InternalFolderBrowser = new FolderBrowserDialog();
            m_InternalFolderBrowser.SelectedPath = selectedPath;

        }
        #endregion


        #region FolderBrowserDialog Mathods

        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            ThreadStart threadDelegate = new ThreadStart(positionOpenDialog);
            Thread newThread = new Thread(threadDelegate);
            newThread.Start();
            return m_InternalFolderBrowser.ShowDialog(owner);
        }

        public string SelectedPath
        {
            get
            {
                return m_InternalFolderBrowser.SelectedPath;
            }
        }

        public string Description
        {
            get
            {
                return m_InternalFolderBrowser.Description;
            }
            set
            {
                m_InternalFolderBrowser.Description = value;
            }
        }

        #endregion

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


        /// 
        /// Find the OpenFileDialog window when it appears, and position it so
        /// that we can see both dialogs at once.  There is no easier way to
        /// do this (&^%$! Microsoft!).
        /// 
        private void positionOpenDialog()
        {
            int count = 0;
            IntPtr zero = (IntPtr)0;
            const int SWP_NOSIZE = 0x0001;
            IntPtr wind;

            while ((wind = FindWindowByCaption(zero, "Browse For Folder")) == (IntPtr)0)
                if (++count > 100000)
                    return;             // Find window failed.
                else
                    Thread.Sleep(5);

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(25);

                SetWindowPos(wind, 0, _x, _y, 0, 0, SWP_NOSIZE);

            }
        }
    }
}
