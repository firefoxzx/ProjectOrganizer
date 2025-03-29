using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectOrganizer.Core;
using Task = ProjectOrganizer.Core.Task;

namespace ProjectOrganizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    string CurrentProject="";
    private bool isSidebarVisible = false;
    private int typeid = 0;
    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
        ShowProjectTypes();
        ShowProjects();

        //Yes i know it's ugly
        TaskViewModel ViewModel = new TaskViewModel(DataCode.LoadProjects()
                                                    .SelectMany(p => DataCode.GetProjectTasks(p.ID))
                                                    .Where(t => t.State != "Done")
                                                    .ToList());
        DataContext = ViewModel;
    }
    private void CreateProject_Click(Object sender, RoutedEventArgs e){
        CreateProjectWindow createProject = new CreateProjectWindow();
        createProject.ShowDialog();
        ShowProjects();
    }
    private void ShowProjects(int typeid = 0){ 
        ProjectsPanel.Children.Clear(); // Clear old buttons
        var projects = DataCode.LoadProjects(typeid);

        foreach (var project in projects){
            Button projectButton = new Button{
                Width = 200,
                Height = 120,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Content = CreateButtonContent(project.Name, project.State,project.ID),
                Tag = project.ID,
                Cursor = Cursors.Hand,
                Style = (Style)FindResource("RoundedButtonStyle")
            };

            projectButton.Click += ProjectButton_Click;
            ProjectsPanel.Children.Add(projectButton);
        }
    }

    private Grid CreateButtonContent(string name, string state,int projectID){
        Grid grid = new Grid{
            Height = 100,
            Width = 180
        };

        // Create two rows: one for spacing and one for content
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Project name
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Status
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Button

        // StackPanel for content
        StackPanel contentStack = new StackPanel{
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        TextBlock nameText = new TextBlock{
            Text = name,
            FontSize = 14,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Left,
            Foreground = Brushes.Black,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(5, 0, 5, 2)
        };

        TextBlock stateText = new TextBlock{
            Text = state,
            FontSize = 12,
            Foreground = Brushes.White,
        };
        Border statusBorder = new Border{
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(5),
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        Button menuButton = new Button{
            Content = "⚙",
            Width = 30,
            Height = 30,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            FontSize = 14,
            Padding = new Thickness(5),
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand
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
            DataCode.DeleteProject(projectID);
            ShowProjects(typeid);
        };
        
        stateDefined.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "Defined");
            stateText.Text = "Defined";
            ShowProjects(typeid);
        };
        stateDone.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "Done");
            stateText.Text = "Done";
            ShowProjects(typeid);
        };
        stateInProgress.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "In Progress");
            stateText.Text = "In Progress";
            ShowProjects(typeid);
        };

        stateCanceled.Click += (s, e) => {
            DataCode.UpdateProjectState(projectID, "Canceled");
            stateText.Text = "Canceled";
        };
        switch (state){
            case "Done": statusBorder.Background = Brushes.Green; break;
            case "In Progress": statusBorder.Background = Brushes.Orange; break;
            case "Canceled": statusBorder.Background = Brushes.Red; break;
            default: statusBorder.Background = Brushes.Gray; break;
        }

        statusBorder.Child = stateText;

        // Add state options inside the "Edit" submenu
        editStateItem.Items.Add(stateDefined);
        editStateItem.Items.Add(stateDone);
        editStateItem.Items.Add(stateInProgress);
        editStateItem.Items.Add(stateCanceled);
        contextMenu.Items.Add(editStateItem);
        contextMenu.Items.Add(deleteItem);

        menuButton.ContextMenu = contextMenu;
        // Add elements to StackPanel
        contentStack.Children.Add(nameText);
        contentStack.Children.Add(statusBorder);
        contentStack.Children.Add(menuButton);

        // Place StackPanel in the second row (bottom)
        Grid.SetRow(contentStack, 1);
        grid.Children.Add(contentStack);

        return grid;
    }   
    private void ShowProjectTypes(){
        ProjectTypesPanel.Children.Clear();
        Button allButton = new Button{
            Content= "All", 
            Background= Brushes.White,
            Foreground= Brushes.Gray,
            FontSize= 14, 
            FontWeight= FontWeights.Bold,
            Width= double.NaN, 
            Height= 25 ,
            BorderThickness = new Thickness(0), 
            Margin= new Thickness(5),
        };
        allButton.Click += (s,e) => {
            typeid = 0;
            ShowProjects();
            };
        ProjectTypesPanel.Children.Add(allButton);

        List<ProjectType> list = DataCode.ReturnProjectTypes();
        foreach(var i in list){
            int typeID = i.ID;
            Button typeButton = new Button{
                Content= i.Name, 
                Background= Brushes.White,
                Foreground= Brushes.Gray,
                FontSize= 14, 
                FontWeight= FontWeights.Bold,
                Width= double.NaN, 
                Height= 25 ,
                BorderThickness = new Thickness(0), 
                Margin= new Thickness(5),
            };
                
            typeButton.Click += (s,e) =>{
                typeid = typeID;
                ShowProjects(typeID);
                };
            ProjectTypesPanel.Children.Add(typeButton);
        }
    }
    private void ScrollLeft_Click(object sender, RoutedEventArgs e){
        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - 100);
    }

    private void ScrollRight_Click(object sender, RoutedEventArgs e){
        scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + 100);
    }

    // Handle button click (you can replace this with your action)
    private void ProjectButton_Click(object sender, RoutedEventArgs e){
        Button clickedButton = sender as Button;
        int projectId = (int)clickedButton.Tag;
        ProjectWindow window = new ProjectWindow(projectId);
        window.Show();
        this.Close();
    }
    

    private void ToggleSidebar_Click(object sender, RoutedEventArgs e){
            double newWidth = isSidebarVisible ? 0 : 300; // Toggle between 0 and 150

            // Animate column width
            GridLengthAnimation animation = new GridLengthAnimation{
                From = new GridLength(SidebarColumn.Width.Value, SidebarColumn.Width.GridUnitType),
                To = new GridLength(newWidth),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            SidebarColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
            isSidebarVisible = !isSidebarVisible;
    }

    public class GridLengthAnimation : AnimationTimeline{
        public override Type TargetPropertyType => typeof(GridLength);

        public GridLength From { get; set; }
        public GridLength To { get; set; }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock){
            if (From.Value == To.Value) return From;
            double progress = animationClock.CurrentProgress.Value;
            double newValue = From.Value + ((To.Value - From.Value) * progress);
            return new GridLength(newValue, From.GridUnitType);
        }

        protected override Freezable CreateInstanceCore(){
            return new GridLengthAnimation { From = this.From, To = this.To };
        
        }

    }
}

public class TaskViewModel : INotifyPropertyChanged{
    public ObservableCollection<Task> TodayTasks { get; set; } = new ObservableCollection<Task>();
    public ObservableCollection<KeyValuePair<DateTime, List<Task>>> GroupedTasks { get; set; } = new ObservableCollection<KeyValuePair<DateTime, List<Task>>>();

    private List<Task> _allTasks;
    public List<Task> AllTasks{
        get => _allTasks;
        set{
            _allTasks = value;
            UpdateTaskGrouping();
        }
    }

    public TaskViewModel(List<Task> tasks){
        AllTasks = tasks;
    }

    private void UpdateTaskGrouping(){
        TodayTasks.Clear();
        GroupedTasks.Clear();

        var filteredTasks = AllTasks.Where(t => t.State != "Done" && t.EndDate != DateTime.MinValue).ToList();

        // Separate Today Tasks
        var todayTasks = filteredTasks.Where(t => t.EndDate.Date == DateTime.Today).ToList();
        foreach (var task in todayTasks)
            TodayTasks.Add(task);

        // Group by date, excluding today
        var grouped = filteredTasks
            .Where(t => t.EndDate.Date != DateTime.Today)
            .GroupBy(t => t.EndDate.Date)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var group in grouped)
            GroupedTasks.Add(new KeyValuePair<DateTime, List<Task>>(group.Key, group.Value));

        OnPropertyChanged(nameof(TodayTasks));
        OnPropertyChanged(nameof(GroupedTasks));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}