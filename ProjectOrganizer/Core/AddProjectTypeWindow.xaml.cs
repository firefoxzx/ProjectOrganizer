using System.Windows;

namespace ProjectOrganizer.Core
{
    public partial class AddProjectTypeWindow : Window
    {
        public AddProjectTypeWindow()
        {
            InitializeComponent();
        }

        private void AddType_Click(object sender, RoutedEventArgs e)
        {
            string typeName = TypeNameInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(typeName))
            {
                MessageBox.Show("Type name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save the new type
            DataCode.AddProjectType(typeName);
            MessageBox.Show($"Project type '{typeName}' added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
