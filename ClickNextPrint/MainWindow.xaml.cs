using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            FileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".inf";
            dialog.Filter = "Driver manifests (*.inf)|*.inf";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    DriverPathBox.Text = dialog.FileName;
                    DriverPathBox.ToolTip = dialog.FileName;

                    string manifestText = File.ReadAllText(DriverPathBox.Text);
                    DriverManifest driverManifest = new DriverManifest(manifestText);

                    PrinterListBox.Items.Clear();
                    foreach (string driver in driverManifest.GetDriverList())
                    {
                        PrinterListBox.Items.Add(driver);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error parsing manifest", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DriverPathBox.Text = null;
                    DriverPathBox.ToolTip = null;
                }
            }
        }

        private void DuplexingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(DuplexingComboBox.SelectedValue);
        }

        private void BuildIntuneButton_Click(object sender, RoutedEventArgs e)
        {
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