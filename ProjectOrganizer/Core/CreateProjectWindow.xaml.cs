using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace ProjectOrganizer.Core;
public partial class CreateProjectWindow : Window
{
    public CreateProjectWindow()
    {
        InitializeComponent();
        LoadProjectTypes();
    }
    
    private void LoadProjectTypes(){
        ProjectTypeDropdown.Items.Clear();
        List<ProjectType> types = DataCode.ReturnProjectTypes();
        foreach (var type in types){
            ProjectTypeDropdown.Items.Add($"{type.Name}");
        }
        ProjectTypeDropdown.Items.Add(new ComboBoxItem { Content = "➕ Add New Type", Tag = -1 });

        ProjectTypeDropdown.SelectedIndex = 0; // Default selection
    }
    private void ProjectTypeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (ProjectTypeDropdown.SelectedItem is ComboBoxItem selectedItem && (int)selectedItem.Tag == -1){
                // Open the "Add New Type" window
                AddProjectTypeWindow newTypeWindow = new AddProjectTypeWindow();
                newTypeWindow.ShowDialog();

                // Refresh the list after adding
                LoadProjectTypes();
            }
        }

    private void CreateProject_Click(Object sender,RoutedEventArgs e){
        string projectName = ProjectName.Text.Trim();
        if (string.IsNullOrWhiteSpace(projectName)){
            MessageBox.Show("Project name cannot be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        string inserTableQuery = @"INSERT INTO Projects (Name, Description,CreatedDate, State) VALUES (@name,@desc,@createddate,@state);";
        using (var command = new SqliteCommand(inserTableQuery, DataCode.projectDbConnection)){
            command.Parameters.AddWithValue("@name",ProjectName.Text);
            command.Parameters.AddWithValue("@desc","Desc");
            command.Parameters.AddWithValue("@createddate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@state","Defined");
            command.ExecuteNonQuery();
        }
        this.Close();
    }
}