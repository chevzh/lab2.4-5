using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab2._4_5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fontSizeComboBox.ItemsSource = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 36, 48, 72 };
            NewDocument();
            
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DocumentTabItem)tabControl.SelectedItem).SelectFontStyle(fontFamilyComboBox.SelectedItem);
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((DocumentTabItem)tabControl.SelectedItem).SelectFontSize(fontSizeComboBox.SelectedItem);
        }

        private void NewDocument(object sender, RoutedEventArgs e)
        {            
            DocumentTabItem item = new DocumentTabItem("New Document " + (tabControl.Items.Count).ToString());
            item.HeaderText = "New Document " + (tabControl.Items.Count+1).ToString();
            tabControl.Items.Add(item);
            tabControl.SelectedItem = item;

        }

        private void NewDocument()
        {

            DocumentTabItem item = new DocumentTabItem("New Document " + (tabControl.Items.Count).ToString());
            item.HeaderText = "New Document " + (tabControl.Items.Count + 1).ToString();
            tabControl.Items.Add(item);
            tabControl.SelectedItem = item;

        }

       
        private void SaveDocument(object sender, RoutedEventArgs e)
        {
            RichTextBox docBox = (((RichTextBox)((TabItem)(tabControl.Items[tabControl.SelectedIndex])).Content));
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|All files (*.*)|*.*";
            dialog.FileName = "New Document"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
                using (FileStream fs = System.IO.File.Create(dialog.FileName))
                {
                    if (System.IO.Path.GetExtension(dialog.FileName).ToLower() == ".rtf")
                        doc.Save(fs, DataFormats.Rtf);
                    else if (System.IO.Path.GetExtension(dialog.FileName).ToLower() == ".txt")
                        doc.Save(fs, DataFormats.Text);
                    else
                        doc.Save(fs, DataFormats.Xaml);
                }
            }

           
        }

        private void OpenDocument(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "RichText Files (*.rtf)|*.rtf| Text Files (*.txt)|*.txt | XAML Files (*.xaml)|*.xaml|All Files (*.*)|*.*";
            if (openFile.ShowDialog() == true)
            {


                if (System.IO.Path.GetExtension(openFile.FileName).ToLower() == ".txt")
                {
                    NewDocument(sender, e);
                    var sr = new StreamReader(openFile.FileName, Encoding.Default);
                    string text = sr.ReadToEnd();
                    var document = new FlowDocument();
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(text);
                    document.Blocks.Add(paragraph);
                    ((RichTextBox)(((TabItem)(tabControl.Items[tabControl.SelectedIndex])).Content)).Document = document;
                    
                }
                else
                {
                    TextRange documentTextRange = new TextRange(((RichTextBox)((Grid)((TabItem)(tabControl.Items[tabControl.SelectedIndex])).Content).Children[0]).Document.ContentStart, ((RichTextBox)(((TabItem)(tabControl.Items[tabControl.SelectedIndex])).Content)).Document.ContentEnd);
                    using (FileStream fs = System.IO.File.Open(openFile.FileName, FileMode.Open))
                    {

                        if (System.IO.Path.GetExtension(openFile.FileName).ToLower() == ".rtf")
                        {
                            documentTextRange.Load(fs, DataFormats.Rtf);
                        }
                        else if (System.IO.Path.GetExtension(openFile.FileName).ToLower() == ".xaml")
                        {
                            documentTextRange.Load(fs, DataFormats.Xaml);
                        }


                    }
                }
               
                ((TabItem)(tabControl.Items[tabControl.SelectedIndex])).Header = openFile.SafeFileName;

            }
        }

    }
}
