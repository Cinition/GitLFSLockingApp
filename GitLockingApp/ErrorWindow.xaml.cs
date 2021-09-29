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
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(string errorString)
        {
            InitializeComponent();

            int textWidth = (int)MeasureString(errorString).Width;

            ErrorText.Content = errorString;
            ErrorText.Width = textWidth + 10;

            this.Width = textWidth + 30;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(ErrorText.FontFamily, ErrorText.FontStyle, ErrorText.FontWeight, ErrorText.FontStretch),
                ErrorText.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
