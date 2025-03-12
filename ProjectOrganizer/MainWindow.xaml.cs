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
using ProjectOrganizer.Core;

namespace ProjectOrganizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string CurrentProject="";
    public MainWindow()
    {
        InitializeComponent();
        ShowProjects();
    }
    private void CreateProject_Click(Object sender, RoutedEventArgs e){
        CreateProjectWindow createProject = new CreateProjectWindow();
        createProject.ShowDialog();
        ShowProjects();
    }
    private void ShowProjects(){ 
        ProjectsPanel.Children.Clear(); // Clear old buttons
        var projects = DataCode.LoadProjects();
        Button CreateProjectBtn = new Button{
                Width = 200,
                Height = 100,
                Margin = new Thickness(5),
                Background = Brushes.LightGray,
                Content = "Create Project"
            };
        CreateProjectBtn.Click += CreateProject_Click;
        ProjectsPanel.Children.Add(CreateProjectBtn);

        foreach (var project in projects){
            Button projectButton = new Button{
                Width = 200,
                Height = 100,
                Margin = new Thickness(5),
                Background = Brushes.LightGray,
                Content = CreateButtonContent(project.Name, project.State,project.ID),
                Tag = project.ID
            };

            projectButton.Click += ProjectButton_Click;
            ProjectsPanel.Children.Add(projectButton);
        }
    }

    private Grid CreateButtonContent(string name, string state,int projectID){
        Grid grid = new Grid{
            Height = 80,
            Width = 180
        };

        // Create two rows: one for spacing and one for content
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Fills available space
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Content at bottom

        // StackPanel for content
        StackPanel contentStack = new StackPanel{
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        TextBlock nameText = new TextBlock{
            Text = name,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        TextBlock stateText = new TextBlock{
            Text = state,
            FontSize = 12,
            Foreground = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        Button menuButton = new Button{
            Content = "⚙",
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            FontSize = 16,
            Padding = new Thickness(5),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
        };
        menuButton.Click += (s, e) => {
            e.Handled = true; // Stops the event from propagating
            menuButton.ContextMenu.IsOpen = true;
        };
        
        ContextMenu contextMenu = new ContextMenu();
    
        MenuItem editStateItem = new MenuItem { Header = "Change State" };
        MenuItem deleteItem = new MenuItem { Header = "Delete" };
        MenuItem stateDefined = new MenuItem { Header = "Defined" };
        MenuItem stateDone = new MenuItem { Header = "Done" };
        MenuItem stateInProgress = new MenuItem { Header = "In Progress" };
        MenuItem stateCanceled = new MenuItem { Header = "Canceled" };

        deleteItem.Click += (s,e) => {
            ShowProjects();
            DataCode.DeleteProject(projectID);
        };
        
        stateDefined.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "Defined");
            stateText.Text = "Defined";
        };
        stateDone.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "Done");
            stateText.Text = "Done";
        };
        stateInProgress.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "In Progress");
            stateText.Text = "In Progress";
        };

        stateCanceled.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "Canceled");
            stateText.Text = "Canceled";
        };

        // Add state options inside the "Edit" submenu
        editStateItem.Items.Add(stateDefined);
        editStateItem.Items.Add(stateDone);
        editStateItem.Items.Add(stateInProgress);
        editStateItem.Items.Add(stateCanceled);
        contextMenu.Items.Add(editStateItem);
        contextMenu.Items.Add(deleteItem);

        menuButton.Click += (s, e) => menuButton.ContextMenu.IsOpen = true;
        menuButton.ContextMenu = contextMenu;
        // Add elements to StackPanel
        contentStack.Children.Add(nameText);
        contentStack.Children.Add(stateText);
        contentStack.Children.Add(menuButton);

        // Place StackPanel in the second row (bottom)
        Grid.SetRow(contentStack, 1);
        grid.Children.Add(contentStack);

        return grid;
    }   

    // Handle button click (you can replace this with your action)
    private void ProjectButton_Click(object sender, RoutedEventArgs e){
        Button clickedButton = sender as Button;
        int projectId = (int)clickedButton.Tag;
        ProjectWindow window = new ProjectWindow(projectId);
        window.Show();
        this.Close();
    }
}