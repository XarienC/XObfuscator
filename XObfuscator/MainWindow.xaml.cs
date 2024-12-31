using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using XObfuscator.Functions;
using XObfuscator.Methods.AntiProtection;
using XObfuscator.Methods.AssemblySpoofer;
using XObfuscator.Methods.ProxyProtection;
using XObfuscator.Methods.ControlFlow;
using XObfuscator.Methods.FakeAttributes;
using XObfuscator.Methods.FakeMethodCalls;
using XObfuscator.Methods.FieldEncapsulation;
using XObfuscator.Methods.IntegerProtection;
using XObfuscator.Methods.JunkAdder;
using XObfuscator.Methods.Local2Field;
using XObfuscator.Methods.Opaque_Predicates;
using XObfuscator.Methods.Renaming;
using XObfuscator.Methods.StackProtection;
using XObfuscator.Methods.Watermark;
using XObfuscator.Methods.ProxyProtections;

namespace XObfuscator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // Select the program to obfuscate.
        private void SelectEXEButton_Click(object sender, RoutedEventArgs e)
        {
            // Calls the LoadExecutable method from the FileLoader class
            FileLoader.LoadExecutable();

            // Updates the TextBox to display the selected executable's path
            EXEPathTB.Text = FileLoader.SelectedFilePath;
        }
        private void OutputLocationToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Enable the TextBox and Button for selecting output location
            OutputLocationTB.IsEnabled = true;
            SelectOutputLocationButton.IsEnabled = true;
        }

        private void OutputLocationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            // Disable the TextBox and Button for selecting output location
            OutputLocationTB.IsEnabled = false;
            SelectOutputLocationButton.IsEnabled = false;

            // Clear the TextBox as the user isn't using a custom output location
            OutputLocationTB.Text = string.Empty;
        }
        private void WatermarkToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Enable the textbox when the checkbox is checked
            WatermarkTB.IsEnabled = true;
        }

        private void WatermarkToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            WatermarkTB.IsEnabled = false;
        }

        // Uses WPF's OpenFileDialog for the folder selection as adding Windows Forms was too much of a pain.
        private void SelectOutputLocationButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an OpenFileDialog instance
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                CheckFileExists = false, // Allow selecting folders (by disabling file check)
                FileName = "Select Folder", // Set a default display name
                Title = "Select Output Folder"
            };

            // Show the dialog and check if a folder (file) is selected
            if (openFileDialog.ShowDialog() == true)
            {
                // Extract the directory path
                string folderPath = Path.GetDirectoryName(openFileDialog.FileName);

                // Set the folder path to the TextBox
                if (!string.IsNullOrEmpty(folderPath))
                {
                    OutputLocationTB.Text = folderPath;
                }
                else
                {
                    MessageBox.Show("Invalid selection. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No folder selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Obfuscation button for selected executable.
        private void ObfuscateButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure an executable file has been loaded
            if (string.IsNullOrEmpty(Functions.FileLoader.SelectedFilePath))
            {
                MessageBox.Show("No executable selected. Please load an executable first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Load the module
                var module = ModuleDefMD.Load(Functions.FileLoader.SelectedFilePath);

                if (RenameToggle.IsChecked == true)
                {
                    RenameProtection.Obfuscate(module);
                }

                if (IntProtectionToggle.IsChecked == true)
                {
                    IntProtection.Obfuscate(module);
                }

                if (FakeAttribToggle.IsChecked == true)
                {
                    FakeAttributes.Obfuscate(module);
                }

                if (ControlFlowToggle.IsChecked == true)
                {
                    ControlFlow.Obfuscate(module);
                }

                if (JunkAdderToggle.IsChecked == true)
                {
                    JunkInsertion.Obfuscate(module);
                }

                if (FakeMethodCallToggle.IsChecked == true)
                {
                    FakeMethodCalls.Obfuscate(module);
                }

                if (Local2FieldToggle.IsChecked == true)
                {
                    Local2Field.Obfuscate(module);
                }

                if (ProxyIntToggle.IsChecked == true)
                {
                    ProxyInt.Obfuscate(module);
                }

                if (ProxyStringToggle.IsChecked == true)
                {
                    ProxyString.Obfuscate(module);
                }

                if (ProxyManyToggle.IsChecked == true)
                {
                    ManyProxy.Obfuscate(module);
                }

                if (WatermarkToggle.IsChecked == true)
                {
                    string watermarkText = WatermarkTB.Text;
                    Watermark.AddWatermark(module, watermarkText);
                }

                if (OpaquePredicatesToggle.IsChecked == true)
                {
                    OpaquePredicates.Apply(module);
                }

                if (FieldEncapToggle.IsChecked == true)
                {
                    FieldEncap.Obfuscate(module);
                }

                if (SpoofAssemblyToggle.IsChecked == true)
                {
                    SpoofAssembly.Obfuscate(module);
                }

                if (StackProtectionToggle.IsChecked == true)
                {
                    StackObf.Obfuscate(module);
                }

                if (AntiDnSpyToggle.IsChecked == true)
                {
                    AntiDnSpy.Obfuscate(module);
                }

                if (AntiDe4DotToggle.IsChecked == true)
                {
                    AntiDe4Dot.Obfuscate(module);
                }

                if (AntiIldasmToggle.IsChecked == true)
                {
                    AntiIldasm.Obfuscate(module);
                }

                string outputDirectory;
                if (OutputLocationToggle.IsChecked == true && !string.IsNullOrEmpty(OutputLocationTB.Text))
                {
                    outputDirectory = OutputLocationTB.Text;
                }
                else
                {
                    // Default to the same directory as the input file
                    outputDirectory = Path.GetDirectoryName(Functions.FileLoader.SelectedFilePath);
                }

                // Construct the output file path
                string outputPath = Path.Combine(outputDirectory,
                    Path.GetFileNameWithoutExtension(Functions.FileLoader.SelectedFilePath) + "_XObfuscated" + Path.GetExtension(Functions.FileLoader.SelectedFilePath));

                // Save the obfuscated assembly
                module.Write(outputPath);

                MessageBox.Show($"Obfuscation complete. Saved to: {outputPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during obfuscation:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}