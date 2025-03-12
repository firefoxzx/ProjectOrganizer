using System.Windows;
using Microsoft.Data.Sqlite;

namespace ProjectOrganizer.Core;
public partial class CreateProjectWindow : Window
{
    public CreateProjectWindow()
    {
        InitializeComponent();
    }
    private void CreateProject_Click(Object sender,RoutedEventArgs e){
        using (var connection = new SqliteConnection("Data Source= Data/Projects.db")){
                connection.Open();
                string inserTableQuery = @"INSERT INTO Projects (Name, Description,CreatedDate, State) VALUES (@name,@desc,@createddate,@state);";

                using (var command = new SqliteCommand(inserTableQuery, connection)){
                    command.Parameters.AddWithValue("@name",ProjectName.Text);
                    command.Parameters.AddWithValue("@desc","Desc");
                    command.Parameters.AddWithValue("@createddate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@state","Defined");
                    command.ExecuteNonQuery();
                }
        }
        this.Close();
    }
}