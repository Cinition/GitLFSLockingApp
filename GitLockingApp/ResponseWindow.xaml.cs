using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace GitLockingApp
{
    /// <summary>
    /// Interaction logic for ResponseWindow.xaml
    /// </summary>
    public partial class ResponseWindow : Window
    {
        public int fileCount = 1;
        public int fileTotalCount;

        public ResponseWindow(int _fileTotalCount)
        {
            InitializeComponent();
            fileTotalCount = _fileTotalCount;
        }

        public void UpdateCounter(string currentFileName)
        {
            Counter.Content = $"{fileCount} / {fileTotalCount}";
            FileName.Content = currentFileName;
            int newWidth = (int)MeasureString(currentFileName).Width;
            FileName.Width = newWidth + 10;
            this.Width = newWidth + 30;
            fileCount++;
        }

        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FileName.FontFamily, FileName.FontStyle, FileName.FontWeight, FileName.FontStretch),
                FileName.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
