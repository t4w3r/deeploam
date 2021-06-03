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
using ICSharpCode.AvalonEdit.Highlighting;
using System.Reflection;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    public partial class MainWindow : Window
    {
        // Reasonable max and min font size values
        private const double FONT_MAX_SIZE = 60d;
        private const double FONT_MIN_SIZE = 5d;


        dynamic old_cc;

        int tab_index = 1;
        TextEditor lastOpened;
        Window compare_win;


        Dictionary<TextEditor, string> directories = new Dictionary<TextEditor, string>();

        //Dictionary<TextEditor, string> dark_dirs = new Dictionary<TextEditor, string>();

        Key lastpresses = Key.A;
        public MainWindow()
        {
            InitializeComponent();

            add_new_tab("");

            lastOpened = (TextEditor)((TabItem)(tabControl.Items[0])).Content;

            HideScriptErrors(webBrowser, true);




        }

        //private void create_dark_file(TextEditor te, string path)
        //{
        //    if (File.Exists(path))
        //        File.SetAttributes(path, FileAttributes.Normal);
        //    FileStream dark_fs = new FileStream(path, FileMode.Create);
        //    dark_fs.Write(Encoding.UTF8.GetBytes(te.Text), 0, te.Text.Length);
        //    dark_fs.Close();
        //    File.SetAttributes("tab" + tab_index.ToString() + ".html", FileAttributes.Hidden);
        //}



        private void add_new_tab(string filename, string text = "")
        {
            TextEditor textEditor = new TextEditor();

            string name = "NewTab";

            if (filename != "")
                filename = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(filename)));

            string ff = filename.Split('\\')[filename.Split('\\').Length - 1];
            TabItem newTabItem = new TabItem
            {
                Header = filename.Split('\\')[filename.Split('\\').Length - 1],
                Name = name + tab_index.ToString()
            };


            textEditor.KeyDown += textEditor_KeyDown;
            textEditor.KeyUp += textEditor_KeyUp;
            textEditor.PreviewMouseWheel  += WheelMoved;
            textEditor.TextChanged += textEditor_TextChanged;
            textEditor.GotFocus += textEditor_GotFocus;
            textEditor.ShowLineNumbers = true;
            textEditor.MouseRightButtonUp += mouseupte;
            if (filename != "")
            {
                if (filename.Split('.')[1] == "html")
                    textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
                if (filename.Split('.')[1] == "js")
                    textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JS");
                if (filename.Split('.')[1] == "css")
                    textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CSS");
            }
            textEditor.Text = text;
            textEditor.KeyUp += _update_browser;
            newTabItem.Content = textEditor;
            //newTabItem.MouseWheel += WheelMoved;

            //todo вынести задание хидера в отдельную функцию
            var stack = new StackPanel();
            var t = new TextBlock();
            if (filename != "")
                t.Text = filename.Split('\\')[filename.Split('\\').Length - 1];
            else
                t.Text = "NewTab";
            var i = new Button();
            i.Width = 10;
            i.Height = 10;
            i.BorderThickness = new Thickness(0, 0, 0, 0);
            i.Content = 'x';
            i.Background = Brushes.Coral;
            i.Margin = new Thickness(5, 0, 0, 0);
            i.Padding = new Thickness(0, -5, 0, 0);
            i.HorizontalContentAlignment = HorizontalAlignment.Center;
            i.VerticalContentAlignment = VerticalAlignment.Top;
            i.Click += on_btn_closeTab;



            stack.Orientation = Orientation.Horizontal;
            stack.Children.Add(t);
            stack.Children.Add(i);
            newTabItem.Header = stack;


            //MessageBox.Show($"{e.MouseDevice.Target.GetType().ToString()}");
            directories.Add(textEditor, filename);
            tabControl.Items.Add(newTabItem);

            //dark_dirs.Add(textEditor, "tab" + tabControl.Items.Count.ToString() + ".html");

            // create_dark_file(textEditor, dark_dirs[textEditor]);


        }

        private void WheelMoved(object sender, MouseWheelEventArgs e)
        {
            bool ctrl = Keyboard.Modifiers == ModifierKeys.Control;
            if (ctrl)
            {
                this.UpdateFontSize(e.Delta > 0);
                e.Handled = true;
            }
        }

        private void mouseupte(object sender, MouseEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            Button btn = new Button();
            btn.Content = "Paste";
            btn.Click += paste_butt_clicked;
            btn.Margin = new Thickness(0,0,0,0);
            menu.Items.Add(btn);
            
            lastOpened.ContextMenu = menu;
        }



        public void UpdateFontSize(bool increase)
        {
            double currentSize = lastOpened.FontSize;

            if (increase)
            {
                if (currentSize < FONT_MAX_SIZE)
                {
                    double newSize = Math.Min(FONT_MAX_SIZE, currentSize + 1);
                    lastOpened.FontSize = newSize;
                }
            }
            else
            {
                if (currentSize > FONT_MIN_SIZE)
                {
                    double newSize = Math.Max(FONT_MIN_SIZE, currentSize - 1);
                    lastOpened.FontSize = newSize;
                }
            }
        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
            {
                wb.Loaded += (o, s) => HideScriptErrors(wb, hide); //In case we are to early
                return;
            }
            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }

        private void _update_browser(object sender, KeyEventArgs e)
        {
            //webBrowser.
            if (((TextEditor)sender).Text != "")
                webBrowser.NavigateToString(((TextEditor)sender).Text);
        }


        private void textEditor_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }



        private void info_butt_clicked(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }

        private void paste_butt_clicked(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }

        private void copy_butt_clicked(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }
        private void cut_butt_clicked(object sender, EventArgs e)
        {
            lastOpened.Cut();
        }
        private void search_butt_clicked(object sender, EventArgs e)
        {
            ICSharpCode.AvalonEdit.Search.SearchPanel sp = ICSharpCode.AvalonEdit.Search.SearchPanel.Install(lastOpened);
        }

        private void far_butt_clicked(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }

        private void redo_butt_clicked(object sender, EventArgs e)
        {
            lastOpened.Redo();
        }
        private void undo_butt_clicked(object sender, EventArgs e)
        {
            lastOpened.Undo();
        }

        private void light_butt_clicked(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }

        private void dark_butt_clicked(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }

        private void textEditor_KeyUp(object sender, KeyEventArgs e)
        {
            lastpresses = Key.V;
        }

        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            //create_dark_file((TextEditor)sender, dark_dirs[(TextEditor)sender]);
            //webBrowser.

            //webBrowser.NavigateToString(((TextEditor)sender).Text);
            //if (e.Key == Key.F5)
            //{
            //    if (directories[(TextEditor)sender] != null)
            //    {
            //        _update_browser();
            //    }
            //}
            if (e.Key == Key.S && lastpresses == Key.LeftCtrl)
            {
                lastpresses = Key.V;
                webBrowser.Focus();
                String text = ((TextEditor)sender).Text;
                if (directories[(TextEditor)sender] == "")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "HTML page (*.html)|*.html |  JScript (*.js)|*.js  | CSStyle  (*.css)|*.css ";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        directories[(TextEditor)sender] = saveFileDialog.FileName;
                        File.WriteAllText(saveFileDialog.FileName, text);
                        string ext = saveFileDialog.SafeFileName.Split('.')[1];
                        if (ext == "html")
                            ((TextEditor)sender).SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
                        if (ext == "js")
                        {
                            ((TextEditor)sender).SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JS");
                            webBrowser.Visibility = Visibility.Hidden;
                        }
                        if (ext == "css")
                        {
                            ((TextEditor)sender).SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CSS");
                            webBrowser.Visibility = Visibility.Hidden;
                        }
                        var t = new TextBlock();
                        t.Text = saveFileDialog.SafeFileName;
                        var i = new Button();
                        i.Width = 10;
                        i.Height = 10;
                        i.BorderThickness = new Thickness(0, 0, 0, 0);
                        i.Content = 'x';
                        i.Background = Brushes.Coral;
                        i.Margin = new Thickness(5, 0, 0, 0);
                        i.Padding = new Thickness(0, -5, 0, 0);
                        i.HorizontalContentAlignment = HorizontalAlignment.Center;
                        i.VerticalContentAlignment = VerticalAlignment.Top;
                        i.Click += on_btn_closeTab;


                        var stack = new StackPanel();
                        stack.Orientation = Orientation.Horizontal;
                        stack.Children.Add(t);
                        stack.Children.Add(i);
                        ((TabItem)tabControl.SelectedItem).Header = stack;
                    }
                    else
                        return;

                }
                else
                {
                    FileStream fs = new FileStream(directories[(TextEditor)sender], FileMode.Create);
                    //text = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(text)));
                    fs.Write(Encoding.UTF8.GetBytes(text), 0, Encoding.UTF8.GetBytes(text).Length);
                    fs.Close();
                }
                //if (directories[(TextEditor)sender] != null)
                //    webBrowser.Navigate(directories[(TextEditor)sender]);

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



        }








        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.Target.GetType().ToString() == "System.Windows.Controls.Primitives.TabPanel")
            {
                add_new_tab("");
                tab_index++;
            }
        }



        private void compare_butt_clicked(object sender, RoutedEventArgs e)
        {

            TextEditor oldText = new TextEditor();
            compare_win = new Window();
            TextEditor textEditor = new TextEditor();
            textEditor.IsReadOnly = true;
            compare_win.Content = textEditor;
            //textEditor.Text = lastOpened.Text;
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


            if (!(oldText.LineCount == 1) && !(lastOpened.LineCount == 1))
            {
                if (oldText.LineCount < lastOpened.LineCount)
                    for (int i = 1; i < oldText.LineCount - 1; i++)
                    {
                        if (oldText.Text.Contains(lastOpened.Document.GetText(lastOpened.Document.GetLineByNumber(i))))
                        {
                        }
                        else
                        {
                            textEditor.Text += $"[{i.ToString()}] - " + lastOpened.Document.GetText(lastOpened.Document.GetLineByNumber(i)) + '\n';
                        }

                    }

                else
                    for (int i = 1; i < oldText.LineCount - 1; i++)
                    {
                        if (lastOpened.Text.Contains(oldText.Document.GetText(lastOpened.Document.GetLineByNumber(i))))
                        {
                        }
                        else
                        {
                            textEditor.Text += $"[{i.ToString()}] - " + lastOpened.Document.GetText(lastOpened.Document.GetLineByNumber(i)) + '\n';
                        }
                    }

            }

            for (int i = 1; i < oldText.LineCount - 1; i++)
            {

                //MessageBox.Show($"{textEditor.Document.GetText(lastOpened.Document.GetLineByNumber(i))} :::::::: {oldText.Document.GetText(lastOpened.Document.GetLineByNumber(i))}");
                //if (textEditor.Document.GetText(lastOpened.Document.GetLineByNumber(i)) == oldText.Document.GetText(lastOpened.Document.GetLineByNumber(i)))
                //{
                //    //ColorizeAvalonEdit clr = new ColorizeAvalonEdit( i,ref textEditor);

                //    //clr.Colorize(textEditor.Document.GetLineByNumber(i));
                //}

                //if (lastOpened.Text.Contains(oldText.Document.GetText(lastOpened.Document.GetLineByNumber(i))))
                //{
                //    //textEditor.Document.Remove(lastOpened.Document.GetLineByNumber(i).Offset, textEditor.Document.GetText(lastOpened.Document.GetLineByNumber(i)).Length);
                //    //i = 0;
                //}

                //else
                //{
                //    textEditor.Text += lastOpened.Document.GetText(lastOpened.Document.GetLineByNumber(i)) + " - " + i.ToString() + '\n';
                //}

            }
            //    //SystemColors.WindowBrush.Color.B = 0;
            //    //SystemColors.WindowBrush.Color.G = 0;
            //    //SystemColors.WindowBrush.Color.R = 0;
            //    //compare_win. =  SystemColors.WindowBrush;

            Button butt = new Button();
            butt.Width = 100;
            butt.Height= 100;
            //butt.Click += come_back;

            this.KeyDown += come_back;

            old_cc = Content;

            this.Content = compare_win.Content;

            this.Closing += cls;

            //compare_win.Show();
        }

        private void cls(object sender, EventArgs e)
        {
            compare_win.Close();
        }


        private void come_back(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Content = old_cc;
        }



        private void open_butt_clicked(object sender, EventArgs e)
        {

            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == true)
            {

                string text_to_send;

                using (StreamReader sr = new StreamReader(openfile.FileName))
                {
                    text_to_send = sr.ReadToEnd();
                    sr.Close();
                }
                add_new_tab(openfile.FileName, text_to_send);
                tab_index++;
            }

        }

        private void open_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\of.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void save_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\sf.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void cmp_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\cmp.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void undo_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\undo.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void redo_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\redo.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void search_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\search.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void far_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\far.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void cut_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\cut.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void copy_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\copy.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void paste_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\paste.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void info_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\info.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void light_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\light.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void dark_butt_init(object sender, EventArgs e)
        {
            Uri path = new Uri("img\\dark.png", UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(path);
            Image image = new Image() { Source = bitmapImage };
            ((Button)sender).Content = image;
        }

        private void save_butt_clicked(object sender, EventArgs e)
        {
            String text = lastOpened.Text;

            if (directories[lastOpened] == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "HTML page (*.html)|*.html |  JScript (*.js)|*.js  | CSStyle  (*.css)|*.css ";
                if (saveFileDialog.ShowDialog() == true)
                {
                    directories[lastOpened] = saveFileDialog.FileName;
                    File.WriteAllText(saveFileDialog.FileName, text);

                    string ext = saveFileDialog.SafeFileName.Split('.')[1];
                    if (ext == "html")
                        lastOpened.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("HTML");
                    if (ext == "js")
                        lastOpened.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JS");
                    if (ext == "css")
                        lastOpened.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CSS");
                    //((TabItem)tabControl.SelectedItem).Header = saveFileDialog.SafeFileName;
                    var t = new TextBlock();
                    t.Text = saveFileDialog.SafeFileName;
                    var i = new Button();
                    i.Width = 10;
                    i.Height = 10;
                    i.BorderThickness = new Thickness(0, 0, 0, 0);
                    i.Content = 'x';
                    i.Background = Brushes.Coral;
                    i.Margin = new Thickness(5, 0, 0, 0);
                    i.Padding = new Thickness(0, -5, 0, 0);
                    i.HorizontalContentAlignment = HorizontalAlignment.Center;
                    i.VerticalContentAlignment = VerticalAlignment.Top;
                    i.Click += on_btn_closeTab;


                    var stack = new StackPanel();
                    stack.Orientation = Orientation.Horizontal;
                    stack.Children.Add(t);
                    stack.Children.Add(i);
                    ((TabItem)tabControl.SelectedItem).Header = stack;
                }
                else
                    return;

            }
            else
            {
                FileStream fs = new FileStream(directories[lastOpened], FileMode.Create);
                fs.Write(Encoding.UTF8.GetBytes(text), 0, Encoding.UTF8.GetBytes(text).Length);
                fs.Close();
            }
        }


        //private void main_te_init(object sender, EventArgs e)
        //{
        //    directories.Add(maintab, "");

        //}

        private void on_btn_closeTab(object sender, EventArgs e)
        {
            if (tabControl.Items.Count != 1)
                tabControl.Items.Remove(((TabItem)((StackPanel)(((Button)sender).Parent)).Parent));

        }

        private void textEditor_GotFocus(object sender, EventArgs e)
        {
            lastOpened = (TextEditor)sender;
            if (directories[lastOpened] != "")
            {
                if (directories[lastOpened].Split('.')[1] == "js" || directories[lastOpened].Split('.')[1] == "css")
                {
                    webBrowser.Visibility = Visibility.Hidden;
                }
                if (directories[lastOpened].Split('.')[1] == "html")
                {
                    webBrowser.Visibility = Visibility.Visible;
                }
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("13 контейнер и весь сайт резиновым", "Справка");
        }
    }
}






























//ABOBA


