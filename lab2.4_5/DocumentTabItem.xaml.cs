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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab2._4_5
{
    /// <summary>
    /// Логика взаимодействия для DocumentTabItem.xaml
    /// </summary>
    public partial class DocumentTabItem : TabItem
    {
        string headerText;
        public string HeaderText { get => headerText; set => headerText = value; }
       
        public DocumentTabItem()
        {
            InitializeComponent();
            
        }      

        public DocumentTabItem(string header)
        {
            InitializeComponent();
            HeaderText = header;        
        }

        public void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            TabControl tabControl = (TabControl)Parent;
            tabControl.Items.RemoveAt(tabControl.SelectedIndex);
        }

        public void SelectFontStyle(FontFamily fontStyle)
        {
            RtbContent.Focus();
            RtbContent.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontStyle);
        }

        public void SelectFontSize(object fontSize)
        {
            RtbContent.Focus();
            RtbContent.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSize);   
        }

        public void TabItemSelected(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;



            if (tab != null)
            {
                TabControl tabControl = (TabControl)Parent;

                RtbContent.Focus();
                FontFamily value;

                if (((MainWindow)System.Windows.Application.Current.MainWindow).tabItemFontFamilies.TryGetValue(HeaderText, out value))
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).newItem = HeaderText;
                    ((MainWindow)System.Windows.Application.Current.MainWindow).fontFamilyComboBox.SelectedItem = value;
                    SelectFontStyle(value);

                }                
                
                
                    
                
                
                ((MainWindow)System.Windows.Application.Current.MainWindow).fontSizeComboBox.SelectedItem = RtbContent.FontSize;
                
            }
            
        }

       
    }
}
