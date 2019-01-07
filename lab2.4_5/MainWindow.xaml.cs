using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public List<string> paths = new List<string>();  
        public Dictionary<string, FontFamily> tabItemFontFamilies = new Dictionary<string, FontFamily>();
        public Dictionary<string, double> tabItemFontSizes = new Dictionary<string, double>();
        public string newItem;

        public MainWindow()
        {
            InitializeComponent();
            fontSizeComboBox.ItemsSource = new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 24, 26, 28, 36, 48, 72 };

            App.LanguageChanged += LanguageChanged;
            CultureInfo currLang = App.Language;

            //Заполняем меню смены языка:
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }            

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


        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }

        }

        private void SaveDocument(object sender, RoutedEventArgs e)
        {
            if (paths.Contains(((DocumentTabItem)(tabControl.SelectedItem)).Path))
            {
                RichTextBox docBox = (((RichTextBox)((DocumentTabItem)(tabControl.SelectedItem)).RtbContent));
                TextRange doc = new TextRange(docBox.Document.ContentStart, docBox.Document.ContentEnd);

                string path = ((DocumentTabItem)(tabControl.SelectedItem)).Path;

                using (FileStream fs = System.IO.File.Create(path))
                {
                    if (System.IO.Path.GetExtension(path).ToLower() == ".rtf")
                        doc.Save(fs, DataFormats.Rtf);
                    else if (System.IO.Path.GetExtension(path).ToLower() == ".txt")
                        doc.Save(fs, DataFormats.Text);
                   
                }
            }
            else
            {
                SaveAsDocument(sender, e);
            }

           
        }


        private void SaveAsDocument(object sender, RoutedEventArgs e)
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

                paths.Add(saveDialog.FileName);
                ((DocumentTabItem)tabControl.SelectedItem).Path = saveDialog.FileName;
                ((DocumentTabItem)tabControl.SelectedItem).HeaderText = saveDialog.SafeFileName; //WTF?????

            }

             
        }

        private void OpenDocument(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "Text files(*.txt)|*.txt|Rich Text File(*.rtf)|*.rtf|All files(*.*)|*.*";
            openFile.DefaultExt = ".txt";
            if (openFile.ShowDialog() == true)
            {
                NewDocument(sender, e);

                //Костыль для словарей шрифтов
                tabItemFontFamilies.Remove(((DocumentTabItem)tabControl.SelectedItem).HeaderText);
                tabItemFontSizes.Remove(((DocumentTabItem)tabControl.SelectedItem).HeaderText);

                if (System.IO.Path.GetExtension(openFile.FileName).ToLower() == ".txt")
                {                    
                    var sr = new StreamReader(openFile.FileName, Encoding.Default);
                    string text = sr.ReadToEnd();
                    var document = new FlowDocument();
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(text);
                    document.Blocks.Add(paragraph);
                    ((RichTextBox)(((DocumentTabItem)(tabControl.Items[tabControl.SelectedIndex])).RtbContent)).Document = document;

                    //ломалось, когда открытый файл открывался повторно
                    if (!tabItemFontFamilies.ContainsKey(openFile.SafeFileName))
                    {
                        tabItemFontFamilies.Add(openFile.SafeFileName, new FontFamily("Times New Roman"));
                        tabItemFontSizes.Add(openFile.SafeFileName, 14);
                    }
                    

                    //Костыль на \n
                    ((DocumentTabItem)tabControl.SelectedItem).WordCounterTextBlock.Text = $"Количество слов: {((DocumentTabItem)tabControl.SelectedItem).wordsCount - 1}";
                }
                else
                {
                    TextRange documentTextRange = new TextRange(((RichTextBox)((DocumentTabItem)(tabControl.SelectedItem)).RtbContent).Document.ContentStart, ((RichTextBox)((DocumentTabItem)(tabControl.SelectedItem)).RtbContent).Document.ContentEnd);

                    using (FileStream fs = System.IO.File.Open(openFile.FileName, FileMode.Open))
                    {
                        if (System.IO.Path.GetExtension(openFile.FileName).ToLower() == ".rtf")
                        {                            
                            documentTextRange.Load(fs, DataFormats.Rtf);
                            fontFamilyComboBox.SelectedItem = documentTextRange.GetPropertyValue(FontFamilyProperty);
                            fontSizeComboBox.SelectedItem = documentTextRange.GetPropertyValue(FontSizeProperty);

                          

                        }
                    }



                    if (!tabItemFontFamilies.ContainsKey(openFile.SafeFileName))
                    {
                        //Продолжение костыля
                        tabItemFontFamilies.Add(openFile.SafeFileName, (FontFamily)documentTextRange.GetPropertyValue(FontFamilyProperty));
                        tabItemFontSizes.Add(openFile.SafeFileName, (double)documentTextRange.GetPropertyValue(FontSizeProperty));
                    }

                }

                paths.Add(openFile.FileName);
                ((DocumentTabItem)tabControl.SelectedItem).Path = openFile.FileName;
                ((DocumentTabItem)tabControl.SelectedItem).HeaderText = openFile.SafeFileName;

            }
        }
      
        void SelectDarkTheme(object sender, RoutedEventArgs e)
        {            
            DarkMenuItem.IsChecked = true;
            LightMenuItem.IsChecked = false;
            ThemeChange("dark_theme");

        }

        void SelectLightTheme(object sender, RoutedEventArgs e)
        {
            DarkMenuItem.IsChecked = false;
            LightMenuItem.IsChecked = true;
            ThemeChange("light_theme");
        }


        private void ThemeChange(string s)
        {
            string style = s;
            // определяем путь к файлу ресурсов
            var uri = new Uri("/Resources/" + style + ".xaml", UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
