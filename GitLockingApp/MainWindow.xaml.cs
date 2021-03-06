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
        private string[] selectedLockID = new string[20];
        private string[] selectedLockUrl = new string[20];
        private string path = "";
        #if !DEBUG
            private string gitPath = "";
        #else
            private string gitPath = "C:/Users/cornnieu/Desktop/Programming/P3/game/GitLockApp";
        #endif

        public MainWindow()
        {
            InitializeComponent(); 
            path = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
#if !DEBUG
            gitPath = path.Substring(8);
            gitPath = gitPath.Split(new string[] { "/GitLockApp" }, StringSplitOptions.None)[0];
#endif
            path = Path.GetDirectoryName(path);
            UpdateLockList();
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
                    selectedFileUrl[i] = split[1].Replace(@"\", @"/");
                }
            }
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            UpdateLockList();
        }

        private void UpdateLockList()
        {
            UpdateLockListPS();
        }

        private async void UpdateLockListPS()
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript($"cd {gitPath}");
                ps.AddScript(@"git lfs locks");

                PSDataCollection<PSObject> results = await Task.Factory.FromAsync(ps.BeginInvoke(), aResults => ps.EndInvoke(aResults));

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
            ClearStringArray(selectedLockID);
            FileUrl.Content = "...";
        }

        private void Lock(object sender, RoutedEventArgs e)
        {
            if (CountString(selectedFileUrl) > 0)
            {
                LockingFile();
            }
        }

        private async void LockingFile()
        {
            ResponseWindow response = new ResponseWindow(CountString(selectedFileUrl));
            response.Show();

            for (int i = 0; i < CountString(selectedFileUrl); i++)
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript($"cd {gitPath}");
                    ps.AddScript($"git lfs lock {selectedFileUrl[i]}");

                    response.UpdateCounter("Locking " + selectedFileUrl[i]);

                    PSDataCollection<PSObject> result = await Task.Factory.FromAsync(ps.BeginInvoke(), results => ps.EndInvoke(results));

                    if (result.Count > 0)
                    {
                        if (!result[0].ToString().Contains("Locked"))
                        {
                            response.Close();
                            ErrorWindow error = new ErrorWindow(result[0].ToString());
                            error.Show();
                            break;
                        }
                    }
                }
            }
            UpdateLockList();
            if (response.ShowActivated)
            {
                response.Close();
            }
        }

        private void Unlock(object sender, RoutedEventArgs e)
        {
            if (CountString(selectedLockID) > 0)
            {
                UnlockingFile();
            }
        }

        private async void UnlockingFile()
        {
            ResponseWindow response = new ResponseWindow(CountString(selectedLockID));
            response.Show();

            for (int i = 0; i < CountString(selectedLockID); i++)
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddScript($"cd {gitPath}");
                    ps.AddScript($"git lfs unlock --id={selectedLockID[i]}");

                    response.UpdateCounter("Unlocking " + selectedLockUrl[i]);

                    PSDataCollection<PSObject> result = await Task.Factory.FromAsync(ps.BeginInvoke(), results => ps.EndInvoke(results));

                    if (result.Count > 0)
                    {
                        if (!result[0].ToString().Contains("Unlocked"))
                        {
                            response.Close();
                            ErrorWindow error = new ErrorWindow(result[0].ToString());
                            error.Show();
                            break;
                        }
                    }
                }
            }
            UpdateLockList();
            if (response.ShowActivated)
            {
                response.Close();
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < List.SelectedItems.Count; i++)
            {
                string[] selectedString = List.SelectedItems[i].ToString().Split(new string[] { "\t" }, StringSplitOptions.None);
                selectedLockUrl[i] = selectedString[0];
                selectedLockID[i] = selectedString[2].Substring(3);
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