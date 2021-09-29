using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace GitLockingApp
{

    public partial class MainWindow : Window
    {
        private string[] selectedFileUrl = new string[20];
        private string[] selectedLockUrl = new string[20];
        private string path = "";
        private string gitPath = "C:/Users/cornnieu/Desktop/Programming/P3/game";

        public MainWindow()
        {
            InitializeComponent(); 
            path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            //gitPath = path.Substring(8, path.Length - (18 + 8));
            path = Path.GetDirectoryName(path);
            updateLockList();
        }

        private void SelectFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = path;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    if (openFileDialog.FileNames.Length > 1)
                    {
                        FileUrl.Content = "Multiple Files Selected";
                    }
                    else
                    {
                        FileUrl.Content = Path.GetFileName(openFileDialog.FileNames[i]);
                    }

                    string[] split = openFileDialog.FileNames[i].Split(new string[] { "game\\" }, StringSplitOptions.None);
                    selectedFileUrl[i] = split[1];
                }
            }
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            updateLockList();
        }

        private void updateLockList()
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript($"cd {gitPath}");
                ps.AddScript(@"git lfs locks");

                Collection<PSObject> results = ps.Invoke();

                List.Items.Clear();

                if (results.Count > 0)
                {
                    foreach (var result in results)
                    {
                        List.Items.Add(result.ToString());
                    }
                }
            }

            ClearStringArray(selectedFileUrl);
            ClearStringArray(selectedLockUrl);
            FileUrl.Content = "...";
        }

        private void Lock(object sender, RoutedEventArgs e)
        {
            if (CountString(selectedFileUrl) > 0)
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript($"cd {gitPath}");

                    foreach (var url in selectedFileUrl)
                    {
                        ps.AddScript($"git lfs lock {url}");
                    }

                    Collection<PSObject> results = ps.Invoke();

                    if (results.Count > 0)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            ErrorWindow error = new ErrorWindow(results[i].ToString());
                            error.Show();
                        }
                    }
                }

                updateLockList();
            }
        }

        private void Unlock(object sender, RoutedEventArgs e)
        {
            if (CountString(selectedLockUrl) > 0)
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript($"cd {gitPath}");

                    foreach (var url in selectedLockUrl)
                    {
                        ps.AddScript($"git lfs unlock {url}");
                    }

                    Collection<PSObject> results = ps.Invoke();

                    if (results.Count > 0)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            ErrorWindow error = new ErrorWindow(results[i].ToString());
                            error.Show();
                        }
                    }
                }

                updateLockList();
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < List.SelectedItems.Count; i++)
            {
                string[] selectedString = List.SelectedItems[i].ToString().Split(new string[] { "\t" }, StringSplitOptions.None);
                selectedLockUrl[i] = selectedString[0];
            }

            if (List.SelectedItems.Count > 1)
            {
                FileUrl.Content = "Locked Files Selected";
            }
            else
            {
                FileUrl.Content = "Locked File Selected";
            }

        }

        private int CountString(string[] stringArray)
        {
            int counter = 0;

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] != "")
                {
                    counter++;
                }
            }

            return counter;
        }

        private void ClearStringArray(string[] stringArray)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                stringArray[i] = "";
            }
        }
    }
}