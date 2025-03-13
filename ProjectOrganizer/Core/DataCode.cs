using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Data.Sqlite;

namespace ProjectOrganizer.Core;

public static class DataCode{    
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProjectOrganizer");
        private static readonly string tasksDbPath = Path.Combine(appDataFolder, "Tasks.db");
        private static readonly string projectsDbPath = Path.Combine(appDataFolder, "Projects.db");

        public static readonly SqliteConnection taskDbConnection;
        public static readonly SqliteConnection projectDbConnection;

        static DataCode(){
            EnsureDatabaseExists("Data.Tasks.db", tasksDbPath);
            EnsureDatabaseExists("Data.Projects.db", projectsDbPath);
            EnsureDatabaseExists("Data.Projects.db", projectsDbPath);

            taskDbConnection = new SqliteConnection($"Data Source={tasksDbPath}");
            projectDbConnection = new SqliteConnection($"Data Source={projectsDbPath}");

            taskDbConnection.Open();
            projectDbConnection.Open();
        }

        private static void EnsureDatabaseExists(string resourceName, string destinationPath){
            if (!Directory.Exists(appDataFolder)){
                Directory.CreateDirectory(appDataFolder);
            }

            if (!File.Exists(destinationPath)){
                ExtractEmbeddedDatabase(resourceName, destinationPath);
            }
            File.SetAttributes(destinationPath, FileAttributes.Normal);
        }

        private static void ExtractEmbeddedDatabase(string resourceName, string destinationPath)
        {
            string fullResourceName = typeof(DataCode).Assembly.GetName().Name + "." + resourceName;
            
            using (var resourceStream = typeof(DataCode).Assembly.GetManifestResourceStream(fullResourceName)){
                if (resourceStream == null){
                    throw new Exception($"Embedded database '{fullResourceName}' not found!");
                }

                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write)){
                    resourceStream.CopyTo(fileStream);
                }
            }
        }
    
    public static List<Project> LoadProjects(){
        List<Project> projects = new List<Project>();

        string selectString= "SELECT * FROM projects";
        using (var command = new SqliteCommand(selectString,projectDbConnection))
        using (var reader=command.ExecuteReader()){
            while(reader.Read()){
                projects.Add(new Project(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.IsDBNull(2) ? "" : reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4)
                ));
            }
        }
        return projects;
    }
    public static Project SearchProjectWithID(int ID){
        Project project = new Project(-1," "," "," "," ");
        
        string selectString= "SELECT * FROM projects WHERE ID = @id";
        using (var command = new SqliteCommand(selectString,projectDbConnection)) {
            command.Parameters.AddWithValue("@id",ID);
            using (var reader=command.ExecuteReader()){
                while(reader.Read()){
                    project = new Project(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.IsDBNull(2) ? "" : reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4)
                    );
                }
            }
        }
        return project;
    }
    public static List<Task> GetProjectTasks(int projectID){
        List<Task> tasks = new List<Task>();

            string searchString = "SELECT * FROM tasks WHERE ProjectID = @id";
            using (var command = new SqliteCommand(searchString, taskDbConnection)){
                command.Parameters.AddWithValue("@id",projectID);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        tasks.Add(new Task(
                            reader.GetInt32(0),
                            reader.IsDBNull(1) ? "" : reader.GetString(1),
                            reader.GetInt32(2),
                            reader.IsDBNull(3) ? "" : reader.GetString(3), 
                            reader.IsDBNull(4) ? "0001-01-01 00:00:00" : reader.GetString(4),
                            reader.IsDBNull(5) ? 0.00 : reader.GetDouble(5),
                            reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                        ));
                    }
                }
            }
        return tasks;
    }
    public static void AddTask(string name, int projectID,DateTime endDate,float money,float timeSpent){
        string insertQuery = "INSERT INTO tasks (Name, ProjectID, State, EndDate, Money, TimeSpent) VALUES (@name, @projectID, @state, @endDate, @money, @timeSpent)";
        using (var command = new SqliteCommand(insertQuery, taskDbConnection)){
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@projectID", projectID);
            command.Parameters.AddWithValue("@state", "OnGoing");
            command.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@money", money);
            command.Parameters.AddWithValue("@timeSpent", timeSpent);

            command.ExecuteNonQuery();
        }
    }
    public static void DeleteTask(int taskID){
        string delteString = "DELETE FROM tasks WHERE ID = @id";
        using (var command = new SqliteCommand(delteString,taskDbConnection)){
            command.Parameters.AddWithValue("@id",taskID);

            command.ExecuteNonQuery();
        }
    }
    public static void UpdateProjectState(int projectID,string state){
        try{
            string query = "UPDATE Projects SET State = @State WHERE ID = @id";
            using (var command = new SqliteCommand(query, projectDbConnection)){
                command.Parameters.AddWithValue("@State", state);
                command.Parameters.AddWithValue("@id", projectID);
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex){
            MessageBox.Show("Database Update Error: " + ex.Message);
        }
    }
    public static void DeleteProject(int ProjectID){
        string delteString = "DELETE FROM projects WHERE ID = @id";
        using (var command = new SqliteCommand(delteString,projectDbConnection)){
            command.Parameters.AddWithValue("@id",ProjectID);

            command.ExecuteNonQuery();
        }
        string taskdelteString = "DELETE FROM tasks WHERE ProjectID = @id";
        using (var command = new SqliteCommand(taskdelteString,taskDbConnection)){
            command.Parameters.AddWithValue("@id",ProjectID);
    
            command.ExecuteNonQuery();
        }
    }

    public static void UpdateTaskState(Task task){
        try{
            string query = "UPDATE Tasks SET State = @State WHERE Name = @Name";
            using (var command = new SqliteCommand(query, taskDbConnection)){
                command.Parameters.AddWithValue("@State", task.State);
                command.Parameters.AddWithValue("@Name", task.Name);
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex){
            MessageBox.Show("Database Update Error: " + ex.Message);
        }
    }

    public static void SaveRichText(RichTextBox richTextBox, string filePath){
        TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
        using (FileStream fs = new FileStream(filePath, FileMode.Create)){
            range.Save(fs, DataFormats.Rtf);
        }
    }
    public static void LoadRichText(RichTextBox richTextBox, string filePath){
        if (!File.Exists(filePath)) return;

        TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
        using (FileStream fs = new FileStream(filePath, FileMode.Open)){
            range.Load(fs, DataFormats.Rtf);
        }
    }
   

}



public class Project{
    public int ID;
    public string Name;
    public string Description;
    public string CreatedDate;
    public string State;

    public Project(int id, string name, string description, string createdDate,string state){
        ID = id;
        Name = name;
        Description = description;
        CreatedDate = createdDate;
        State = state;
    }
}

public class Task{
    public int ID { get; set;}
    public string Name { get; set;}
    private string _state;
    public int ProjectID { get; set;}
    public string State{
        get { return _state; }
        set{
            if (_state != value){
                _state = value;
                OnPropertyChanged();
                Application.Current.Dispatcher.Invoke(() => DataCode.UpdateTaskState(this));
            }
        }
    }
    public DateTime EndDate { get; set; }
    public double Money { get; set;}
    public int TimeSpent { get; set;}

    public Task(int id,string name,int projectID,string state,string endDate, double money, int timeSpent){
        ID = id;
        Name = name;
        ProjectID = projectID;
        State = state;
        DateTime parsedDate;
        EndDate = DateTime.TryParse(endDate, out parsedDate) ? parsedDate : DateTime.MinValue;
        Money = money;
        TimeSpent = timeSpent;
    }
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}