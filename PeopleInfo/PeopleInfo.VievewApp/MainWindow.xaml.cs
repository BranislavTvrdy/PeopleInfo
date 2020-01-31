using PeopleInfo.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PeopleInfo.Presence.ServiceLib;

namespace PeopleInfo.VievewApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PeopleInfoContext DbContext = new PeopleInfoContext();
        public int NumberOfItems { get; set; }
        public int NumberInitiated { get; set; }
        public List<Person> People { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Department> Departments { get; set; }


        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            #region InicializationDB
            DataGridFilteredItems.ItemsSource = null;
            List<Person> toDisplay = DbContext.People.OrderBy(n => n.FirstName).ThenBy(n => n.LastName).ToList();
            List<Room> rooms = DbContext.GetRooms().ToList();
            List<Department> departs = DbContext.Departments.ToList();
            People = toDisplay;
            Rooms = rooms;
            Departments = departs;

            List<string> roomsToSource = new List<string> {"<All rooms>"};
            roomsToSource.AddRange(rooms.OrderBy(r => r.Label).Select(r => r.Label).ToList());
            ComboBoxRoom.ItemsSource = roomsToSource;
            ComboBoxRoom.SelectedIndex = 0;
            //ComboBoxRoom.ItemsSource = rooms.Select(r => r.Label).ToList();

            List<string> departsToSource = new List<string> {"<All departments>"};
            departsToSource.AddRange(departs.OrderBy(d => d.Name).Select(d => d.Abbreviatinon).ToList());
            ComboBoxDepartment.ItemsSource = departsToSource;
            ComboBoxDepartment.SelectedIndex = 0;
            //ComboBoxDepartment.ItemsSource = departs.Select(d => d.Name).ToList();

            DataGridFilteredItems.ItemsSource = toDisplay;
            NumberOfItems = toDisplay.Count;
            NumberInitiated = toDisplay.Count;
            LabCount.Content = "Count: " + NumberOfItems + " / " + NumberInitiated;
            Console.WriteLine("DB Loaded!");
            ButtonGenerator.IsEnabled = false;
            #endregion
        }

        private void ButtonGenerator_Click(object sender, RoutedEventArgs e)
        {
            //DbContext.People.Remove((Person)DataGridFilteredItems.SelectedItem);
            Person selectedPerson = (Person) DataGridFilteredItems.SelectedItem;
            string deparName = DbContext.Departments.FirstOrDefault(d => d.Id == selectedPerson.DepartmentId)?.Name;
            string roomName = DbContext.Rooms.FirstOrDefault(r => r.Id == selectedPerson.RoomId)?.Label;
            ContactDetail contacts = new ContactDetail(selectedPerson, deparName, roomName);
            contacts.ShowDialog();
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            SearchCheck();
        }

        private void ComboBoxRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxRoom.SelectedItem.ToString() == "<All rooms>")
            {
                DataGridFilteredItems.ItemsSource = People;
                LabCount.Content = "Count: " + People.Count + " / " + NumberInitiated;
                return;
            }
            int rId = Rooms.FirstOrDefault(r => r.Label == ComboBoxRoom.SelectedItem.ToString()).Id;
            List<Person> toDisplay = People.Where(p => p.RoomId == rId).ToList();
            DataGridFilteredItems.ItemsSource = toDisplay;
            LabCount.Content = "Count: " + toDisplay.Count + " / " + NumberInitiated;
        }

        private void ComboBoxDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxRoom.SelectedItem.ToString() == "<All departments>")
            {
                DataGridFilteredItems.ItemsSource = People;
                LabCount.Content = "Count: " + People.Count + " / " + NumberInitiated;
                return;
            }
            List<Person> toDisplay = RoomSelectionCheck().Intersect(DepartmentSelectionCheck()).ToList();
            DataGridFilteredItems.ItemsSource = toDisplay;
            LabCount.Content = "Count: " + toDisplay.Count + " / " + NumberInitiated;

        }

        private void SearchCheck()
        {
            var input = TextBoxSearch.Text.ToLower();
            if (!String.IsNullOrWhiteSpace(input))
            {
                List<Person> toDisplay = RoomSelectionCheck().Intersect(DepartmentSelectionCheck()).ToList();
                List<Person> retPersons = new List<Person>();
                foreach (var s in toDisplay)
                {
                    if (s.FirstName.ToLower().Contains(input) ||
                        s.LastName.ToLower().Contains(input) ||
                        (s.Email != null && s.Email.ToLower().Contains(input)) ||
                        (s.Phone != null && s.Phone.ToLower().Contains(input)) ||
                        (s.TitleBefore != null && s.TitleBefore.ToLower().Contains(input)) ||
                        (s.TitleAfter != null && s.TitleAfter.ToLower().Contains(input)) ||
                        (s.JobPosition != null && s.JobPosition.ToLower().Contains(input)))
                    {
                        retPersons.Add(s);
                    }
                }
                DataGridFilteredItems.ItemsSource = retPersons;
                LabCount.Content = "Count: " + retPersons.Count + " / " + NumberInitiated;
            }
            else
            {
                List<Person> toDisplay = RoomSelectionCheck().Intersect(DepartmentSelectionCheck()).ToList();
                DataGridFilteredItems.ItemsSource = toDisplay;
                LabCount.Content = "Count: " + toDisplay.Count + " / " + NumberInitiated;
            }
        }

        private List<Person> RoomSelectionCheck()
        {
            if (ComboBoxRoom.SelectedItem.ToString() == "<All rooms>")
            {
                return People;
            }
            int rId = Rooms.FirstOrDefault(r => r.Label == ComboBoxRoom.SelectedItem.ToString()).Id;
            return People.Where(p => p.RoomId == rId).ToList();
        }

        private List<Person> DepartmentSelectionCheck()
        {
            if (ComboBoxDepartment.SelectedItem.ToString() == "<All departments>")
            {
                return People;
            }
            int rId = Departments.FirstOrDefault(d => d.Abbreviatinon == ComboBoxDepartment.SelectedItem.ToString()).Id;
            return People.Where(d => d.DepartmentId == rId).ToList();
        }


        private void DataGridFilteredItems_OnSelected(object sender, RoutedEventArgs e)
        {
            ButtonGenerator.IsEnabled = true;
            Person selectedPerson = (Person)DataGridFilteredItems.SelectedItem;
            using (var client = new ChannelFactory<IPresenceService>("WcfPeopleInfo"))
            {
                IPresenceService proxyPresenceService = client.CreateChannel();
                var requestedPerson = new PersonRequest
                {
                    FirstName = selectedPerson.FirstName, LastName = selectedPerson.LastName
                };

                ImageState.Stretch = Stretch.Uniform;

                var statusSelectedPerson = proxyPresenceService.GetPersonPresence(requestedPerson);
                switch (statusSelectedPerson.Type)
                {
                    case PresenceType.Present:
                        LState.Content = "Present";
                        ImageState.Source = new BitmapImage(new Uri("present.png", UriKind.Relative));
                        break;
                    case PresenceType.Absent:
                        LState.Content = "Absent";
                        ImageState.Source = new BitmapImage(new Uri("absent.png", UriKind.Relative));
                        break;
                    case PresenceType.Unknown:
                        LState.Content = "Unknown";
                        ImageState.Source = new BitmapImage(new Uri("unknown.png", UriKind.Relative));
                        break;
                    default:
                        LState.Content = "Unknown";
                        ImageState.Source = new BitmapImage(new Uri("unknown.png", UriKind.Relative));
                        break;
                }

            }
        }
    }
}
