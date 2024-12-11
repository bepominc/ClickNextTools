using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
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

        private DriverManifest? SelectedDriverManifest;

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
                    this.SelectedDriverManifest = new DriverManifest(manifestText);

                    // No errors so far, we are okay to update the printer list box.
                    PrinterListBox.Items.Clear();
                    foreach (string driver in this.SelectedDriverManifest.GetDriverList())
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

        private void DuplexingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if DEBUG
            Debug.WriteLine(DuplexingComboBox.SelectedValue);
#endif
        }

        private void BuildIntuneButton_Click(object sender, RoutedEventArgs e)
        {
            // Verify a valid driver has been selected.
            string driverRoot = Path.GetDirectoryName(DriverPathBox.Text) ?? "";
            if (SelectedDriverManifest == null || driverRoot == "")
            {
                MessageBox.Show("Please select a valid driver first", "No driver selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Validate a printer name and address have been provided

            // Show the system save file dialog, limited to .intunewin files.
            FileDialog dialog = new SaveFileDialog();
            dialog.FileName = PrinterNameBox.Text;
            dialog.DefaultExt = ".intunewin";
            dialog.Filter = "Intune win32 app (*.intunewin)|*.intunewin";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                // TODO: Kick off printer bundle compilation.
            }
        }
    }
}