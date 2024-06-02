using System;
using System.Collections.Generic;
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

namespace up
{
    /// <summary>
    /// Логика взаимодействия для ChangePriority.xaml
    /// </summary>
    public partial class ChangePriority : Window
    {
        public int NewPriority { get; private set; }

        public ChangePriority(int maxPriority)
        {
            InitializeComponent();

            changePrior.Text = maxPriority.ToString();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(changePrior.Text, out int newPriority))
            {
                NewPriority = newPriority;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Необходимо ввести числовое значение");
            }
        }
    }


}
