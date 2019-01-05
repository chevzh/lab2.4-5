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
       
        public Dictionary<string, FontFamily> tabItemFontFamilies = new Dictionary<string, FontFamily>();
        public Dictionary<string, double> tabItemFontSizes = new Dictionary<string, double>();
        public string newItem;

        public MainWindow()
        {
            InitializeComponent();
            fontSizeComboBox.ItemsSource = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 36, 48, 72 };
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NewDocument(sender, e);
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabItemFontFamilies[newItem] = (FontFamily)fontFamilyComboBox.SelectedItem;
            ((DocumentTabItem)tabControl.SelectedItem).SelectFontStyle((FontFamily)fontFamilyComboBox.SelectedItem);          
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tabItemFontSizes[newItem] = (double)fontSizeComboBox.SelectedItem;
            ((DocumentTabItem)tabControl.SelectedItem).SelectFontSize(fontSizeComboBox.SelectedItem);
        }

        private void NewDocument(object sender, RoutedEventArgs e)
        {            
            DocumentTabItem item = new DocumentTabItem("New Document " + (tabControl.Items.Count+1).ToString());
            tabItemFontFamilies.Add("New Document " + (tabControl.Items.Count+1).ToString(), new FontFamily("Times New Roman"));
            tabItemFontSizes.Add("New Document " + (tabControl.Items.Count + 1).ToString(), 14);
            tabControl.Items.Add(item);
            tabControl.SelectedItem = item;
           
        }
      

        private void SaveDocument(object sender, RoutedEventArgs e)
        {
            RichTextBox docBox = (((RichTextBox)((DocumentTabItem)(tabControl.SelectedItem)).RtbContent));
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text files(*.txt)|*.txt|Rich Text File(*.rtf)|*.rtf|All files(*.*)|*.*";
            saveDialog.FileName = ((DocumentTabItem)(tabControl.SelectedItem)).HeaderText;// Default file name
            saveDialog.DefaultExt = ".txt"; // Default file extension

            Nullable<bool> result = saveDialog.ShowDialog();
            if (result == true)
            {
                TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);
                using (FileStream fs = System.IO.File.Create(saveDialog.FileName))
                {
                   if (System.IO.Path.GetExtension(saveDialog.FileName).ToLower() == ".txt")
                        doc.Save(fs, DataFormats.Text);
                    else if (System.IO.Path.GetExtension(saveDialog.FileName).ToLower() == ".rtf")
                        doc.Save(fs, DataFormats.Rtf);

                }
            }

           
        }      

        private void OpenDocument(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "Text files(*.txt)|*.txt|Rich Text File(*.rtf)|*.rtf|All files(*.*)|*.*";
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
                    ((RichTextBox)(((DocumentTabItem)(tabControl.Items[tabControl.SelectedIndex])).RtbContent)).Document = document;

                    //Костыль на \n
                    ((DocumentTabItem)tabControl.SelectedItem).WordCounterTextBlock.Text = $"Количество слов: {((DocumentTabItem)tabControl.SelectedItem).wordsCount - 1}";
                }
                else
                {
                    NewDocument(sender, e);
                    TextRange documentTextRange = new TextRange(((RichTextBox)((DocumentTabItem)(tabControl.SelectedItem)).RtbContent).Document.ContentStart, ((RichTextBox)((DocumentTabItem)(tabControl.SelectedItem)).RtbContent).Document.ContentEnd);

                    using (FileStream fs = System.IO.File.Open(openFile.FileName, FileMode.Open))
                    {
                        if (System.IO.Path.GetExtension(openFile.FileName).ToLower() == ".rtf")
                        {                            
                            documentTextRange.Load(fs, DataFormats.Rtf);
                            fontFamilyComboBox.SelectedItem = documentTextRange.GetPropertyValue(FontFamilyProperty);
                            fontSizeComboBox.SelectedItem = documentTextRange.GetPropertyValue(FontSizeProperty);

                            //Костыль для словарей шрифтов
                            tabItemFontFamilies.Remove(((DocumentTabItem)tabControl.SelectedItem).HeaderText);
                            tabItemFontSizes.Remove(((DocumentTabItem)tabControl.SelectedItem).HeaderText);

                        }
                    }

                     ((DocumentTabItem)tabControl.SelectedItem).HeaderText = openFile.SafeFileName;

                    //Продолжение костыля
                    tabItemFontFamilies.Add(openFile.SafeFileName, (FontFamily)documentTextRange.GetPropertyValue(FontFamilyProperty));
                    tabItemFontSizes.Add(openFile.SafeFileName, (double)documentTextRange.GetPropertyValue(FontSizeProperty));

                }          

            }
        }
      

    }
}
