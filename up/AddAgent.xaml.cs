using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using up.Model;

namespace up
{
    /// <summary>
    /// Логика взаимодействия для AddAgent.xaml
    /// </summary>
    public partial class AddAgent : Window
    {
        private Agents _agent;
        private bool _isEditing;
        private bool _isEdit = false;
        private string _pathtoImage;
        public string TitleWin { get; set; }
        public MainWindow MainWindow { get; set; }
        public ObservableCollection<string> AgentTypes { get; set; }
        public ObservableCollection<DataCompanies> AgentsData { get; set; }

        public AddAgent()
        {
            InitializeComponent();
            _isEditing = false;
            addAgent.Content = "Добавить";
            this.DataContext = this;
            TitleWin = "Новый агент";
            using (var context2 = new UPEntities1())
            {
                AgentTypes = new ObservableCollection<string>(context2.Agents.Select(a => a.AgentType).Distinct());
            }
            MainWindow = Application.Current.MainWindow as MainWindow;
        }
            public AddAgent(Agents agent)
            {
                InitializeComponent();
                _agent = agent;
                _isEditing = true;
                this.DataContext = this;
                TitleWin = $"Редактирование {_agent.AgentName}";
                using (var context2 = new UPEntities1())
                {
                AgentTypes = new ObservableCollection<string>(context2.Agents.Select(a => a.AgentType).Distinct());
                }
                nameAgentBox.Text = _agent.AgentName;
                typeAgentCB.SelectedItem = _agent.AgentType;
                priorityBox.Text = _agent.Priority.ToString();
                adressBox.Text = _agent.LegalAddress;
                INNBox.Text = _agent.INN;
                KPPBox.Text = _agent.KPP;
                dirNameBox.Text = _agent.Director;
                telNumBox.Text = _agent.Phone;
                emailBox.Text = _agent.Email;
                addAgent.Content = "Отредактировать";
                deleteAgent.Visibility = Visibility.Visible;
                MainWindow = Application.Current.MainWindow as MainWindow;

        }

        private void AddAgent_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                EditAgentInDatabase();
            }
            else
            {
                AddNewAgentToDatabase();
            }

            DialogResult = true;

            Close();
        }

        private void EditAgentInDatabase()
        {
            using (var context = new UPEntities1())
            {
                var dbAgent = context.Agents.FirstOrDefault(a => a.AgentID == _agent.AgentID);

                if (dbAgent != null)
                {
                    dbAgent.AgentName = nameAgentBox.Text;
                    dbAgent.AgentType = typeAgentCB.SelectedItem.ToString();
                    dbAgent.Priority = int.Parse(priorityBox.Text);
                    if(_isEdit == true) 
                    {
                        dbAgent.Logo = $"\\agents\\{_pathtoImage}";
                    }
                    else
                    {
                        dbAgent.Logo = _agent.Logo;
                    }
                    dbAgent.LegalAddress = adressBox.Text;
                    dbAgent.INN = INNBox.Text;
                    dbAgent.KPP = KPPBox.Text;
                    dbAgent.Director = dirNameBox.Text;
                    dbAgent.Phone = telNumBox.Text;
                    dbAgent.Email = emailBox.Text;

                    context.SaveChanges();
                    var agents = context.Agents.ToList();
                    var agentsData = agents.Select(CreateAgent).ToList();
                    AgentsData = new ObservableCollection<DataCompanies>(agentsData);
                }
            }
        }
        private DataCompanies CreateAgent(Agents agent)
        {
            using (var context = new UPEntities1())
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
            else return $"{System.IO.Path.Combine(directory, "agents\\picture.png")}";
        }
        private void AddNewAgentToDatabase()
        {
            using (var context = new UPEntities1())
            {
                var newAgent = new Agents();

                newAgent.AgentName = nameAgentBox.Text;
                newAgent.AgentType = typeAgentCB.SelectedItem.ToString();
                newAgent.Priority = int.Parse(priorityBox.Text);
                if (_isEdit == true)
                {
                    newAgent.Logo = $"\\agents\\{_pathtoImage}";
                }
                else
                {
                    newAgent.Logo = "\\agents\\picture.png";
                }
                newAgent.LegalAddress = adressBox.Text;
                newAgent.INN = INNBox.Text;
                newAgent.KPP = KPPBox.Text;
                newAgent.Director = dirNameBox.Text;
                newAgent.Phone = telNumBox.Text;
                newAgent.Email = emailBox.Text;

                context.Agents.Add(newAgent);
                context.SaveChanges();
            }
        }

        private void ChooseLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp;*.gif) | *.png;*.jpeg;*.jpg;*.bmp;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                _pathtoImage = openFileDialog.SafeFileName;
                SaveFile(_pathtoImage, openFileDialog.FileName);
                _isEdit = true;
            }
        }
        static void SaveFile(string filename, string path)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string sourcePath = System.IO.Path.Combine(directory, "agents", filename);

            if (File.Exists(sourcePath))
            {
                return;
            }
            else
            {
                File.Copy(path, sourcePath);
            }
        }
        private void DeleteAgentFromDatabase()
        {
            using (var context = new UPEntities1())
            {
                var dbAgent = context.Agents.FirstOrDefault(a => a.AgentID == _agent.AgentID);

                if (dbAgent != null)
                {
                    if (dbAgent.ProductSales.Any())
                    {
                        MessageBox.Show("Невозможно удалить агента, так как он имеет информацию о реализации продукции.");
                        return;
                    }
                    context.Agents.Remove(dbAgent);
                    context.SaveChanges();

                    MessageBox.Show("Агент успешно удален из базы данных.");

                    DialogResult = true;
                    Close();
                }
            }
        }
        

        private void deleteAgent_Click(object sender, RoutedEventArgs e)
        {
            DeleteAgentFromDatabase();
        }
    }

}
