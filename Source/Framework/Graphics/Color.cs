using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace OpenGLF
{
    [Serializable]
    [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
    public struct Color
    {
        byte _r, _g, _b, _a;
        public byte r { get { return _r; } set { _r = value; } }
        public byte g { get { return _g; } set { _g = value; } }
        public byte b { get { return _b; } set { _b = value; } }
        public byte a { get { return _a; } set { _a = value; } }

        public static Color white= new Color(255, 255, 255, 255);
        public static Color black = new Color(0, 0, 0, 255);
        public static Color red = new Color(255, 0, 0, 255);
        public static Color green = new Color(0, 255, 0, 255);
        public static Color blue = new Color(0, 0, 255, 255);

        public static Color cornflowerBlue = new Color(100, 149, 208, 255);
        public static Color gray = new Color(144, 144, 144, 255);
        public static Color lightGray = new Color(210, 210, 210, 255);
        public static Color yellow = new Color(255, 255, 0, 255);
        public static Color airForceBlue = new Color(93, 138, 168, 255);

        public static Color alizarinCrimson = new Color(227, 38, 54, 255);
        public static Color amethyst = new Color(153, 102, 204, 255);
        public static Color canonicalAubergine = new Color(119, 41, 83, 255);
        public static Color oliveDrab = new Color(107, 142, 35, 255);

        public Color(byte R, byte G, byte B, byte A)
        {
            _r = R;
            _g = G;
            _b = B;
            _a = A;
        }

        public Color(byte R, byte G, byte B)
        {
            _r = R;
            _g = G;
            _b = B;
            _a = 255;
        }

        public System.Drawing.Color toSystemColor()
        {
            return System.Drawing.Color.FromArgb(_a, _r, _g, _b);
        }

        public override string ToString()
        {
            return r.ToString() + "," + g.ToString() + "," + b.ToString() + "," + a.ToString();
        }
    }

    public class ColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(Color))
            {
                return value;
            }

            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (svc != null)
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    dlg.ShowDialog();
                    return new Color(dlg.Color.R, dlg.Color.G, dlg.Color.B, dlg.Color.A);
                }
            }

            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            using (System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(((Color)e.Value).a, ((Color)e.Value).r, ((Color)e.Value).g, ((Color)e.Value).b)))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            e.Graphics.DrawRectangle(System.Drawing.Pens.Black, e.Bounds);
        }
    }
}
