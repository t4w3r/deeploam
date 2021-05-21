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
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    public partial class MainWindow : Window
    {


        int tab_index = 1;
        WebBrowser webBrowser;
        TabControl tabControl;
        TextEditor lastOpened;
        TextEditor maintab;
        Window compare_win;


        Dictionary<TextEditor, string> directories = new Dictionary<TextEditor, string>();

        Key lastpresses = Key.A;
        public MainWindow()
        {
            InitializeComponent();
            

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Pressed(object sender, RoutedEventArgs e)
        {

        }

        private void textEditor_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && lastpresses == Key.LeftCtrl)
            {
                lastpresses = Key.V;
                webBrowser.Focus();
                String text = ((TextEditor)sender).Text;
                if (directories[(TextEditor)sender] == "")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "HTML page (*.html)|*.html";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        directories[(TextEditor)sender] = saveFileDialog.FileName;
                        File.WriteAllText(saveFileDialog.FileName, text);
                    }
                    else
                        return;

                }
                FileStream fs = new FileStream(directories[(TextEditor)sender], FileMode.Create);
                fs.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
                fs.Close();
                // Uri uri = new Uri(directories[(TextEditor)sender]);

                //webBrowser.Source = uri;
                //webBrowser.NavigateToString(directories[(TextEditor)sender]);
                webBrowser.Navigate(directories[(TextEditor)sender]);
                //webBrowser.Refresh(true);

            }
            if (e.Key == Key.F && lastpresses == Key.LeftCtrl)
            {
                lastpresses = Key.V;
                ICSharpCode.AvalonEdit.Search.SearchPanel sp = ICSharpCode.AvalonEdit.Search.SearchPanel.Install((TextEditor)sender);

            }
            //if (e.Key == Key.Z && lastpresses == Key.LeftCtrl)
            //{
            //    ((TextEditor)sender).Undo();
            //}
            //if (e.Key == Key.Y && lastpresses == Key.LeftCtrl)
            //{
            //    ((TextEditor)sender).Redo();
            //}
            lastpresses = e.Key;
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            //FileStream fs = new FileStream("c:\\test\\test.html",FileMode.Create);
            //String text = ((TextEditor)sender).Text;
            //fs.Write(Encoding.UTF8.GetBytes(text),0,text.Length);
            //fs.Close();
        }

        private void WebBrowser_Initialized(object sender, EventArgs e)
        {
            webBrowser = (WebBrowser)sender;

            this.Background = Brushes.DarkGray;
             

        }

        private void WebBrowser_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }


        private void TabControl_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TabControl_Initialized(object sender, EventArgs e)
        {
            tabControl = (TabControl)sender;
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.Target.GetType().ToString() == "System.Windows.Controls.Primitives.TabPanel")
            {
                TextEditor textEditor = new TextEditor();
                string name = "NewTab";
                TabItem newTabItem = new TabItem
                {
                    Header = name + tab_index.ToString(),
                    Name = name + tab_index.ToString()
                };
                textEditor.KeyDown += textEditor_KeyDown;
                textEditor.TextChanged += textEditor_TextChanged;
                textEditor.GotFocus += textEditor_GotFocus;
                textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("HTML");



                newTabItem.Content = textEditor;

                //MessageBox.Show($"{e.MouseDevice.Target.GetType().ToString()}");
                directories.Add(textEditor, "");
                tabControl.Items.Add(newTabItem);
                tab_index++;
            }
        }

        private void ToolBarPanel_Initialized(object sender, EventArgs e)
        {
            ToolBar tb = (ToolBar)sender;
            Button butt = new Button();
            butt.Width = 25;
            butt.Height = 25;
            butt.Click += open_butt_clicked;
            Uri path = new Uri("C:\\lbb\\of.png");
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            butt.Content = image;
            tb.Items.Add(butt);
            //-------------------------------------------------
            butt = new Button();
            butt.Width = 25;
            butt.Height = 25;
            butt.Click += save_butt_clicked;
            path = new Uri("C:\\lbb\\sf.png");
            bitmapImage = new BitmapImage(path);
            image = new Image() { Source = bitmapImage };
            butt.Content = image;
            tb.Items.Add(butt);

            //------------------------------------------------
            butt = new Button();
            butt.Width = 25;
            butt.Height = 25;
            butt.Click += compare_butt_clicked;
            path = new Uri("C:\\lbb\\cmp.png");
            bitmapImage = new BitmapImage(path);
            image = new Image() { Source = bitmapImage };
            butt.Content = image;
            tb.Items.Add(butt);


        }

        private void compare_butt_clicked(object sender, RoutedEventArgs e)
        {
            TextEditor oldText = new TextEditor();
            compare_win = new Window();
            TextEditor textEditor = new TextEditor();
            textEditor.IsReadOnly = true;
            compare_win.Content = textEditor;
            textEditor.Text = lastOpened.Text;
            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == true)
            {
                compare_win.Title = openfile.FileName.Split('\\')[openfile.FileName.Split('\\').Length - 1];

                using (StreamReader sr = new StreamReader(openfile.FileName))
                {
                    oldText.Text = sr.ReadToEnd();
                    sr.Close();
                }
            }
            else
                return;
            for (int i = 1; i < lastOpened.LineCount+1; i++)
            {

                //MessageBox.Show($"{textEditor.Document.GetText(lastOpened.Document.GetLineByNumber(i))} :::::::: {oldText.Document.GetText(lastOpened.Document.GetLineByNumber(i))}");
                if (textEditor.Document.GetText(lastOpened.Document.GetLineByNumber(i)) == oldText.Document.GetText(lastOpened.Document.GetLineByNumber(i)))
                {
                    ColorizeAvalonEdit clr = new ColorizeAvalonEdit( i, textEditor);
                    clr.Colorize(textEditor.Document.GetLineByNumber(i));
                }
            }
            //SystemColors.WindowBrush.Color.B = 0;
            //SystemColors.WindowBrush.Color.G = 0;
            //SystemColors.WindowBrush.Color.R = 0;
            //compare_win. =  SystemColors.WindowBrush;

           
            


            compare_win.ShowDialog();
        }


   
        private void open_butt_clicked(object sender, EventArgs e)
        {

            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == true)
            {
                TextEditor textEditor = new TextEditor();
                string name = "NewTab";
                TabItem newTabItem = new TabItem
                {
                    Header = openfile.FileName.Split('\\')[openfile.FileName.Split('\\').Length - 1],
                    Name = name + tab_index.ToString()
                };

                textEditor.KeyDown += textEditor_KeyDown;
                textEditor.TextChanged += textEditor_TextChanged;
                textEditor.GotFocus += textEditor_GotFocus;
                textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("HTML");


                newTabItem.Content = textEditor;




                //MessageBox.Show($"{e.MouseDevice.Target.GetType().ToString()}");
                directories.Add(textEditor, openfile.FileName);
                tabControl.Items.Add(newTabItem);

                using (StreamReader sr = new StreamReader(openfile.FileName))
                {
                    textEditor.Text = sr.ReadToEnd();
                    sr.Close();
                }
            }

        }


        private void save_butt_clicked(object sender, EventArgs e)
        {
            String text = lastOpened.Text;

            if (directories[lastOpened] == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "HTML page (*.html)|*.html";
                if (saveFileDialog.ShowDialog() == true)
                {
                    directories[lastOpened] = saveFileDialog.FileName;
                    File.WriteAllText(saveFileDialog.FileName, text);
                }
                else
                    return;

            }
            FileStream fs = new FileStream(directories[lastOpened], FileMode.Create);
            fs.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
            fs.Close();
        }

        private void textEditor_Initialized(object sender, EventArgs e)
        {
            directories.Add((TextEditor)sender, "");
        }

        private void maintab_init(object sender, EventArgs e)
        {
            maintab = (TextEditor)sender;
            directories.Add((TextEditor)sender, "");
        }

        private void textEditor_GotFocus(object sender, EventArgs e)
        {
            lastOpened = (TextEditor)sender;
        }
    }
}






























//ABOBA


