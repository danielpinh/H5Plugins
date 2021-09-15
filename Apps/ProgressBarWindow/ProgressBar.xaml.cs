using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace H5Plugins
{
    /// <summary>
    /// Interação lógica para ProgressBar.xam
    /// </summary>
    public partial class ProgressBar : System.Windows.Window, IDisposable
    {      
        
        public bool IsClosed { get; private set; }
        public ProgressBar(string title = "")
        {
            InitializeComponent();
            InitializeCommands();            
            this.Title = title;
            this.Closed += (s, e) =>
            {
                IsClosed = true;
            };
        }       

        public bool RunSheets<Sheets>(string title, List<Sheets> collection, Action<Sheets> action)
        {
            this.Title = title;
            return RunSheets(collection, action);
        }

        public bool RunSheets<Sheets>(List<Sheets> collection, Action<Sheets> action)
        {
            if (IsClosed) return IsClosed;
            Show();
            this.progressBar.Value = 0;
            this.progressBar.Maximum = collection.Count();

            foreach (Sheets item in collection)
            {
                action?.Invoke(item);
                if (Update()) break;
            }
            
            return IsClosed;
        }

        public bool RunString<T>(string title, List<string> collection, Action<string> action)
        {
            this.Title = title;
            return RunString<T>(collection, action);
        }

        public bool RunString<T>(List<string> collection, Action<string> action)
        {
            if (IsClosed) return IsClosed;
            Show();
            this.progressBar.Value = 0;
            this.progressBar.Maximum = collection.Count();

            foreach (string item in collection)
            {
                action?.Invoke(item);
                if (Update()) break;
            }
            return IsClosed;
        }

        private bool Update (double value = 1.0)
        {          
            DoEvents();
            progressBar.Value += value;
            return IsClosed;
        }

        public void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        private void InitializeCommands()
        {
            this.Topmost = true;
            this.ShowInTaskbar = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
        }

        public void Dispose()
        {
            if (!IsClosed) Close();
        }
    }
}
