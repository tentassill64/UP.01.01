    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Eventing.Reader;
    using System.Globalization;
using System.IO;
using System.Linq;
    using System.Security.Policy;
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
    using up.Model;

namespace up
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private UPEntities1 context;
        public int PageSize { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;

        private string _selectedType = "Все типы";
        public int TotalPages { get; set; }
        public ObservableCollection<DataCompanies> CurrentPageData { get; set; }
        public ObservableCollection<DataCompanies> DataCompany { get; set; }
        public ObservableCollection<string> AgentTypes { get; set; }
        public bool _isSorted = false;
        private List<DataCompanies> _selectedAgents = new List<DataCompanies>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            using (var context = UPEntities1.GetContext())
            {
                AgentTypes = new ObservableCollection<string>(context.Agents.Select(a => a.AgentType).Distinct());

                var agents = context.Agents.ToList();

                var dataCompanies = agents.Select(MapAgentToDataCompany).ToList();

                DataCompany = new ObservableCollection<DataCompanies>(dataCompanies);
            }

            AgentTypes.Insert(0, "Все типы");
            ComboBoxTypes.SelectedIndex = 0;

            TotalPages = (int)Math.Ceiling((double)DataCompany.Count / PageSize);
            CurrentPageData = new ObservableCollection<DataCompanies>(DataCompany.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
            listViewAgents.ItemsSource = CurrentPageData;
            UpdatePageNumbers();
        }


        private void searchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCollection();
            //SplashScreen splashScreen = new SplashScreen("\\agents\\бурунов.jpg");
            //splashScreen.Show(true);
        }
        private DataCompanies MapAgentToDataCompany(Agents agent)
        {
            var context = new UPEntities1();
            {
                var today = DateTime.Today;
                var oneYearAgo = today.AddDays(-365);
                var agentSales = context.ProductSales
                                          .Where(ps => ps.AgentID == agent.AgentID && ps.SaleDate >= oneYearAgo)
                                          .Select(ps => ps.Quantity)
                                          .DefaultIfEmpty(0)
                                          .Sum();
                var agentRevenue = context.ProductSales
                                     .Where(ps => ps.AgentID == agent.AgentID && ps.SaleDate >= oneYearAgo)
                                     .Select(ps => ps.Quantity * ps.Products.MinimumPrice)
                                     .DefaultIfEmpty(0)
                                     .Sum();

                // Calculate the discount rate for the agent
                double discountRate = 0;
                if (agentRevenue >= 500000)
                {
                    discountRate = 0.25;
                }
                else if (agentRevenue >= 150000)
                {
                    discountRate = 0.2;
                }
                else if (agentRevenue >= 50000)
                {
                    discountRate = 0.05;
                }
                Int32.TryParse(agent.Priority.ToString(), out int res);
                {
                    return new DataCompanies
                    {
                        AgentName = agent.AgentName,
                        TypeOfAgent = agent.AgentType,
                        PhoneNumber = agent.Phone,
                        Priority = $"Приоритетность: {agent.Priority}",
                        Email = agent.Email,
                        ImagePath = ImageFinder(agent.Logo),
                        Sales = $"{agentSales} продаж за год",
                        Percent = $"{discountRate * 100:N0}%",
                        PriorityValue = res,
                        AgentId = agent.AgentID,
                        Adress = agent.LegalAddress,
                        Direct = agent.Director,
                        INN = agent.INN,
                        KPP = agent.KPP,
                    };
                }
            }
            
        }


        private string ImageFinder(string imagePath)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(System.IO.Path.Combine(directory, imagePath.TrimStart('\\'))))
                return System.IO.Path.Combine(directory, imagePath.TrimStart('\\'));
            else return $"{System.IO.Path.Combine(directory,"agents\\picture.png")}";
        }
        private void ComboBoxTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTypes.SelectedItem == null)
            {
                return;
            }
            string selectedType = ComboBoxTypes.SelectedItem.ToString();
            _selectedType = selectedType;

            _isSorted = false;

            FilterCollection();

        }

        public void FilterCollection()
        {
            if (DataCompany == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(searchField.Text))
            {
                if (_selectedType == "Все типы")
                {
                    CurrentPageData = new ObservableCollection<DataCompanies>(DataCompany.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
                }
                else
                {
                    var filteredData1 = DataCompany.Where(x => x.TypeOfAgent == _selectedType);
                    CurrentPageData = new ObservableCollection<DataCompanies>(filteredData1.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
                }

                listViewAgents.ItemsSource = CurrentPageData;
                return;
            }

            var comparer = CultureInfo.CurrentCulture.CompareInfo;
            var filteredData = DataCompany.Where(x => x != null && (comparer.IndexOf((x.AgentName ?? string.Empty), searchField.Text, CompareOptions.IgnoreCase) >= 0
                                                                        || comparer.IndexOf((x.PhoneNumber ?? string.Empty), searchField.Text, CompareOptions.IgnoreCase) >= 0
                                                                        || comparer.IndexOf((x.Email ?? string.Empty), searchField.Text, CompareOptions.IgnoreCase) >= 0));

            if (_selectedType != "Все типы")
            {
                filteredData = filteredData.Where(x => x.TypeOfAgent == _selectedType);
            }


            CurrentPageData = new ObservableCollection<DataCompanies>(filteredData.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
            listViewAgents.ItemsSource = CurrentPageData;
        }


        private void UpdatePageNumbers()
        {
            PageNumbersListBox.Text = CurrentPage.ToString();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                FilterCollection();
                UpdatePageNumbers();
                if (_isSorted == true)
                    Sorting();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                FilterCollection();
                UpdatePageNumbers();
                if (_isSorted == true)
                    Sorting();
            }
        }

        private void PageNumbersListBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Int32.TryParse(PageNumbersListBox.Text, out int res))
            {
                if (res <= 0)
                {
                    return;

                }
                else
                {
                    if (res > TotalPages)
                    {
                        PageNumbersListBox.Text = TotalPages.ToString();
                    }
                    else
                    {
                        CurrentPage = res;
                        FilterCollection();
                        UpdatePageNumbers();
                        if (_isSorted == true)
                            Sorting();
                    }
                }

            }

        }

        private void priorityChange_Click(object sender, RoutedEventArgs e)
        {
            int maxPriority = _selectedAgents.Max(a => a.PriorityValue);

            ChangePriority window = new ChangePriority(maxPriority);
            window.ShowDialog();

            if (window.DialogResult == true)
            {
                try
                {
                    using (var context = new UPEntities1()) 
                    {
                        foreach (var agent in _selectedAgents)
                        {
                            var dbAgent = context.Agents.FirstOrDefault(a => a.AgentName == agent.AgentName);
                            if (dbAgent != null)
                            {
                                dbAgent.Priority = window.NewPriority;
                            }
                            else
                            {
                                MessageBox.Show("Agent not found");
                            }
                        }
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

            foreach (var agent in _selectedAgents)
            {
                var dataCompany = DataCompany.FirstOrDefault(d => d.AgentName == agent.AgentName);
                if (dataCompany != null)
                {
                    dataCompany.Priority = $"Приоритетность: {window.NewPriority}";
                    dataCompany.PriorityValue = window.NewPriority;
                }
            }

            FilterCollection();
        }


        private void listViewAgents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedAgents = listViewAgents.SelectedItems.Cast<DataCompanies>().ToList();

            priorityChange.Visibility = _selectedAgents.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorting();
        }

        private void Sorting()
        {
            string selectedSort;
            string selectedMethodSort;
            if (ComboBoxSort.SelectedItem == null)
            {
                return;
            }
            else
            {
                selectedSort = (ComboBoxSort.SelectedItem as ComboBoxItem)?.Content.ToString();
            }
            if (ComboBoxMethod.SelectedItem == null)
            {
                return;
            }
            else
            {
                selectedMethodSort = (ComboBoxMethod.SelectedItem as ComboBoxItem)?.Content.ToString();
            }
            IOrderedEnumerable<DataCompanies> sortedData = null;
            if (selectedMethodSort == "По возрастанию")
            {
                switch (selectedSort)
                {
                    case "Наименованию":
                        sortedData = DataCompany.OrderBy(x => x.AgentName);
                        break;
                    case "Приоритету":
                        sortedData = DataCompany.OrderBy(x => x.PriorityValue);
                        break;
                    case "Размеру скидки":
                        sortedData = DataCompany.OrderBy(x => x.Percent);
                        break;
                }
            }
            else if (selectedMethodSort == "По убыванию")
            {
                switch (selectedSort)
                {
                    case "Наименованию":
                        sortedData = DataCompany.OrderByDescending(x => x.AgentName);
                        break;
                    case "Приоритету":
                        sortedData = DataCompany.OrderByDescending(x => x.PriorityValue);
                        break;
                    case "Размеру скидки":
                        sortedData = DataCompany.OrderByDescending(x => x.Percent);
                        break;
                }
            }


            CurrentPageData = new ObservableCollection<DataCompanies>(sortedData.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
            listViewAgents.ItemsSource = CurrentPageData;
            UpdatePageNumbers();
            if (_isSorted == false)
                _isSorted = true;
        }
        private void AddNewAgent_Click(object sender, RoutedEventArgs e)
        {
            var addAgentWindow = new AddAgent();
            addAgentWindow.MainWindow = this;
            addAgentWindow.ShowDialog();
            if (addAgentWindow.DialogResult == true)
            {
                var agents = new UPEntities1().Agents.ToList();
                var agentsData = agents.Select(MapAgentToDataCompany).ToList();
                DataCompany = new ObservableCollection<DataCompanies>(agentsData);
                FilterCollection();
            }

        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedAgent = listViewAgents.SelectedItem as DataCompanies;

            if (selectedAgent != null)
            {
                var addAgentWindow = new AddAgent(selectedAgent.ToAgents());
                addAgentWindow.ShowDialog();
                if(addAgentWindow.DialogResult == true)
                {
                    var agents = new UPEntities1().Agents.ToList();
                    var agentsData = agents.Select(MapAgentToDataCompany).ToList();
                    DataCompany = new ObservableCollection<DataCompanies>(agentsData);
                    FilterCollection();
                }
                
            }
        }

        private void ComboBoxMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sorting();
        }

        //private void PageNumbersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (PageNumbersListBox.SelectedItem != null)
        //    {
        //        CurrentPage = (int)PageNumbersListBox.SelectedItem;
        //        FilterCollection();
        //    }
        //}
    }
    public class DataCompanies
    {
        public string AgentName { get; set; }

        public string TypeOfAgent { get; set; }

        public string PhoneNumber { get; set; }

        public string Priority { get; set; }

        public string Sales { get; set; }

        public string ImagePath { get; set; }

        public string Percent { get; set; }

        public string Email { get; set; }

        public string Adress { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string Direct {  get; set; }

        public int PriorityValue { get; set; }
        public int AgentId { get; set; }
    }
    public static class Extensions
    {
        public static Agents ToAgents(this DataCompanies dataCompanies)
        {
            return new Agents
            {
                AgentID = dataCompanies.AgentId,
                AgentName = dataCompanies.AgentName,
                AgentType = dataCompanies.TypeOfAgent,
                Priority = dataCompanies.PriorityValue,
                Logo = dataCompanies.ImagePath,
                Email = dataCompanies.Email,
                Phone = dataCompanies.PhoneNumber,
                Director = dataCompanies.Direct,
                INN = dataCompanies.INN,
                KPP = dataCompanies.KPP,
                LegalAddress = dataCompanies.Adress,
            };
        }
    }
}