

using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ProjectOrganizer.Core{
    public partial class ProjectWindow: Window{
    
        int projectID = -1;
        TextBlock title = new TextBlock();
        RichTextBox projectBox = new RichTextBox();
        string projectBoxPath;
        public ProjectWindow(int ProjectID){
            InitializeComponent();
            ShowOverview(this, new RoutedEventArgs());
            projectID = ProjectID;
            title.Text= DataCode.SearchProjectWithID(projectID).Name;
            projectBoxPath = $"Data/Project Files/{title.Text}";
            DataCode.LoadRichText(projectBox,projectBoxPath);

            KeyBindings.SaveFile.InputGestures.Add(new KeyGesture(Key.S,ModifierKeys.Control));
        }
        private void MainWindow_Click(Object sender, RoutedEventArgs e){
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
            ShowTasks(null,null);
        }
        private void ShowOverview(object sender, RoutedEventArgs e){
            Grid overviewGrid = new Grid();
            overviewGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            overviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            title = new TextBlock{
                Text = "Project Title",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            RichTextBox editor = new RichTextBox{
                AcceptsTab = true,
                IsUndoEnabled = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontSize = 16,
                Background = SystemColors.WindowBrush,
                BorderThickness = new Thickness(0)
            };
            projectBox = editor;

            Grid.SetRow(title, 0);
            Grid.SetRow(editor, 1);
            overviewGrid.Children.Add(title);
            overviewGrid.Children.Add(editor);

            MainContent.Content = overviewGrid;
        }   

        private void ShowTasks(object sender, RoutedEventArgs e){
            List<Task> tasks = DataCode.GetProjectTasks(projectID);
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition{Width =  new GridLength(1,GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition{Width =  new GridLength(1,GridUnitType.Star)});

            Grid tasksOverGrid = new Grid();
            tasksOverGrid.RowDefinitions.Add(new RowDefinition{Height =  new GridLength(1,GridUnitType.Star)});
            tasksOverGrid.RowDefinitions.Add(new RowDefinition{Height =  new GridLength(1,GridUnitType.Star)});

            DataGrid taskGrid = new DataGrid{
                AutoGenerateColumns = false,
                Margin = new Thickness(0),
                ItemsSource = tasks,
                Background = SystemColors.WindowBrush,
                IsReadOnly = true
            };
             // Define Columns
           DataGridTemplateColumn checkBoxColumn = new DataGridTemplateColumn{
               Header = "✔",
               Width = 50
           };
           
        
           FrameworkElementFactory checkBoxFactory = new FrameworkElementFactory(typeof(CheckBox));
           checkBoxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("State"){
               Converter = new StateToCheckboxConverter(), // Converter will convert "Done"/"Not Done" to true/false
               Mode = BindingMode.TwoWay,
               UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
           });
        
           checkBoxColumn.CellTemplate = new DataTemplate { VisualTree = checkBoxFactory };
           taskGrid.Columns.Add(checkBoxColumn);
        
            taskGrid.Columns.Add(new DataGridTextColumn{
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            taskGrid.LoadingRow += (s, args) =>{
                DataGridRow row = args.Row;
                Task task = (Task)row.Item;
                ContextMenu contextMenu = new ContextMenu();
                // Delete Option
                MenuItem deleteItem = new MenuItem { Header = "Delete" };
                deleteItem.Click += (s2, e2) =>
                {
                    DataCode.DeleteTask(task.ID);
                    tasks.Remove(task); // Remove from the list
                    taskGrid.Items.Refresh(); // Refresh the DataGrid
                };
                contextMenu.Items.Add(deleteItem);
                row.ContextMenu = contextMenu;
            };          
            /*
            taskGrid.Columns.Add(new DataGridTextColumn{
                Binding = new Binding("EndDate"),
                Width = 100
            });
            taskGrid.Columns.Add(new DataGridTextColumn{
                Binding = new Binding("Money"),
                Width = 100
            });
            taskGrid.Columns.Add(new DataGridTextColumn{
                Binding = new Binding("TimeSpent"),
                Width = 100
            });
            
            Grid paper = new Grid();

            paper.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto});
            paper.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            title = new TextBlock{
                Text = "Project Title",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            RichTextBox textEditor = new RichTextBox{
                AcceptsTab = true,
                IsUndoEnabled = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontSize = 16,
                Background = SystemColors.WindowBrush,
                BorderThickness = new Thickness(0)
            };
            */
            Button AddTask = new Button{
                Content="Add Task",
                FontSize= 16
            };
            AddTask.Click += AddTask_Click;
            /*
            Grid.SetRow(title,0);
            Grid.SetRow(textEditor,1);
            */
            Grid.SetColumn(tasksOverGrid,0);
            Grid.SetRow(taskGrid,0);
            Grid.SetRow(AddTask,1);
            //Grid.SetColumn(paper,1);

            //paper.Children.Add(title);
            //paper.Children.Add(textEditor);
            tasksOverGrid.Children.Add(taskGrid);
            tasksOverGrid.Children.Add(AddTask);
            grid.Children.Add(tasksOverGrid);
            //grid.Children.Add(paper);

            MainContent.Content = grid;
        }
        private void ShowResources(object sender, RoutedEventArgs e){
            MessageBox.Show("This is Still Under Construction \nWait for the other updates");
        }

        private void AddTask_Click(Object sender,RoutedEventArgs e){
            AddTaskWindow window = new AddTaskWindow(projectID);
            window.ShowDialog();
        }
        private void SaveProjectText_Click(Object sender,RoutedEventArgs e){
            DataCode.SaveRichText(projectBox,projectBoxPath);
        }
    }

    public class StateToCheckboxConverter : IValueConverter{
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture){
            return value?.ToString() == "Done"; // Returns true if the state is "Done"
        }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture){
            return (bool)value ? "Done" : "Not Done"; // Converts checkbox state back to text
        }

    }

}