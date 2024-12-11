using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ClickNextPrint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectDriverButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the system open file dialog, limited to .inf files.
            FileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".inf";
            dialog.Filter = "Driver manifests (*.inf)|*.inf";

            bool? result = dialog.ShowDialog();

            // If a file was picked, try to process it.
            if (result == true)
            {
                try
                {
                    // Update the driver path display box.
                    DriverPathBox.Text = dialog.FileName;
                    DriverPathBox.ToolTip = dialog.FileName;

                    // Open the file and read the entire contents as a single string.
                    string manifestText = File.ReadAllText(DriverPathBox.Text);

                    // Create a new driver manifest object from the manifest text.
                    DriverManifest driverManifest = new DriverManifest(manifestText);

                    // No errors so far, we are okay to update the printer list box.
                    PrinterListBox.Items.Clear();
                    foreach (string driver in driverManifest.GetDriverList())
                    {
                        PrinterListBox.Items.Add(driver);
                    }
                }
                catch (Exception ex)
                {
                    // Some kind of error while reading the manifest file. Pop-up a dialog box with the error message.
                    MessageBox.Show(ex.Message, "Error parsing manifest", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DriverPathBox.Text = null;
                    DriverPathBox.ToolTip = null;
                }
            }
        }

        private void BuildIntuneButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify a valid driver has been selected.
            if (DriverPathBox.Text == "" || Path.GetDirectoryName(DriverPathBox.Text) == null || PrinterListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a valid driver first", "No driver selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate a printer name and address have been provided
            if (PrinterNameBox.Text == null || PrinterNameBox.Text.Length == 0)
            {
                MessageBox.Show("Please provide a printer name first", "No printer name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Regex ipv4Regex = new Regex(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$");
            if (!ipv4Regex.IsMatch(PrinterAddressBox.Text))
            {
                MessageBox.Show("Please provide a valid printer IPv4 address", "No printer address", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Show the system save file dialog, limited to .intunewin files.
            FileDialog dialog = new SaveFileDialog();
            dialog.FileName = PrinterNameBox.Text.Replace(" ", "");
            dialog.DefaultExt = ".intunewin";
            dialog.Filter = "Intune win32 app (*.intunewin)|*.intunewin";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    PrinterBundle bundle = new PrinterBundle(
                        Path.GetFileNameWithoutExtension(dialog.FileName).Replace(" ", ""),
                        (string)PrinterListBox.SelectedItem,
                        PrinterNameBox.Text,
                        PrinterAddressBox.Text,
                        (string)DuplexingComboBox.SelectedValue,
                        ColorCheckBox.IsChecked ?? false,
                        CollateCheckBox.IsChecked ?? false
                    );
                    
                    bundle.Build(Path.GetDirectoryName(dialog.FileName));
                }
                catch (Exception ex)
                {
                    // Some kind of error while building the scripts and intune bundle. Pop-up a dialog box with the error message.
                    MessageBox.Show(ex.Message, "Error building output", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
