using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;
//using ICSharpCode.AvalonEdit.Rendering.VisualLineElementTextRunProperties;
using System.Windows.Media;
using System.Windows;


namespace WpfApp1
{

    public class ColorizeAvalonEdit : DocumentColorizingTransformer    
    {
        int lineind;
        TextEditor curreditor;

        public ColorizeAvalonEdit(int linendex,ref TextEditor curredit)
        {
            lineind = linendex-1;
            curreditor = curredit;
        }

            
        protected override void ColorizeLine(DocumentLine line)
        {
            string text = curreditor.Document.GetText(line);
            Action<VisualLineElement> act = element => element.TextRunProperties.SetBackgroundBrush(Brushes.Green);
            if (act != null && line != null && text != "" && lineind != 0)
                base.ChangeLinePart(line.Offset, text.Length, act);

            curreditor.TextArea.TextView.Redraw();



            //while ((index = text.IndexOf("", start)) >= 0)
            //{
            //MessageBox.Show($"{text} ");
            //this.ChangeLinePart(line.Offset, line.Offset, element => element.TextRunProperties.SetForegroundBrush(Brushes.Green));

            //VisualLineElement element = 
            //base.ChangeLinePart(
            //    line.Offset, // startOffset
            //    line.Offset, // endOffset
            //    (VisualLineElement element) =>
            //    {
            //            // This lambda gets called once for every VisualLineElement
            //            // between the specified offsets.
            //            Typeface tf = element.TextRunProperties.Typeface;
            //            // Replace the typeface with a modified version of
            //            // the same typeface
            //            element.BackgroundBrush = new SolidColorBrush(Colors.Green);
            //        element.TextRunProperties.SetTypeface(new Typeface(
            //                tf.FontFamily,
            //                FontStyles.Italic,
            //                FontWeights.Bold,
            //                tf.Stretch
            //            ));
            //    });
            //    start = index + 1; // search for next occurrence
            //}
            //MessageBox.Show($"фвдзахъфдзахъ");

        }


        

        public void Colorize(DocumentLine line)
        {
            ColorizeLine(line);
        }

    }
}