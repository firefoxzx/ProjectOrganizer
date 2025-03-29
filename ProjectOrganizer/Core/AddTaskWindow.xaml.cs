

using System.Globalization;
using System.Windows;

namespace ProjectOrganizer.Core{
    public partial class AddTaskWindow:Window{
        int projectID = -1;
        public AddTaskWindow(int ProjectID){
            InitializeComponent();
            projectID = ProjectID;
        }

        private void Submit_Click(object sender, RoutedEventArgs e){
            // Get the Name (string)
            string taskName = NameBox.Text;
            if(string.IsNullOrEmpty(taskName)){
                MessageBox.Show("Invalid Name format! Please enter a valid date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Parse EndDate (DateTime)
            DateTime endDate;
            if (!DateTime.TryParse(DateBox.Text, out endDate)){
                endDate = new DateTime();
            }

            // Parse Money (float)
            float money;
            if (!float.TryParse(MoneyBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out money)){
                money = 0;
            }

            // Parse Time (int)
            int time;
            if (!int.TryParse(TimeBox.Text, out time) || time < 0){
                time = 0;
            }

            // You can now use these values to add the task to your list
            DataCode.AddTask(taskName,projectID,endDate,money,time);
            this.Close();
        }
    }
}