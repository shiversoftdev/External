using Refract.UI.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Refract.UI.Core.Singletons
{
    internal struct UIThemeInfo
    {
        public Color BackColor;
        public Color AccentColor;
        public Color TextColor;
        public Color TitleBarColor;
        public FlatStyle ButtonFlatStyle;
        public Color ButtonHoverColor;
        public Color LightBackColor;
        public Color ButtonActive;
        public Color TextInactive;

        public static UIThemeInfo Default()
        {
            UIThemeInfo theme = new UIThemeInfo();
            theme.BackColor = Color.FromArgb(28, 28, 28);
            theme.TextColor = Color.WhiteSmoke;
            theme.AccentColor = Color.DodgerBlue;
            theme.TitleBarColor = Color.FromArgb(36, 36, 36);
            theme.ButtonFlatStyle = FlatStyle.Flat;
            theme.ButtonHoverColor = Color.FromArgb(50, 50, 50);
            theme.LightBackColor = Color.FromArgb(36, 36, 36);
            theme.ButtonActive = Color.DodgerBlue;
            theme.TextInactive = Color.Gray;
            return theme;
        }
    }

    internal delegate void ThemeChangedCallback(UIThemeInfo themeData);

    internal static class UIThemeManager
    {
        public static UIThemeInfo CurrentTheme { get; private set; }
        private static HashSet<Control> ThemedControls = new HashSet<Control>();
        private static Dictionary<Type, ThemeChangedCallback> CustomTypeHandlers = new Dictionary<Type, ThemeChangedCallback>();
        private static Dictionary<Control, ThemeChangedCallback> CustomControlHandlers = new Dictionary<Control, ThemeChangedCallback>();
        static UIThemeManager()
        {
            CurrentTheme = UIThemeInfo.Default();
        }

        /// <summary>
        /// Makes this control, and all the children of this control, theme aware. Any classes which have not had a theme handler registered will throw an exception.
        /// </summary>
        /// <param name="control"></param>
        internal static void SetThemeAware(this IThemeableControl control)
        {
            if (!(control is Control ctrl)) throw new InvalidOperationException($"Cannot theme control of type '{control.GetType()}' because it is not derived from Control");
            foreach (Control c in control.GetThemedControls())
            {
                if(c is IThemeableControl themed_c) SetThemeAware(themed_c);
                else RegisterAndThemeControl(c);
            }
            RegisterAndThemeControl(ctrl);
        }

        /// <summary>
        /// When a themed control is disposed, we are going remove it from the controls registry so it no longer receives theming data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ThemedControlDisposed(object sender, EventArgs e)
        {
            ThemedControls.Remove(sender as Control);
        }

        internal static void ApplyTheme(UIThemeInfo theme)
        {
            CurrentTheme = theme;
            foreach (var control in ThemedControls)
            {
                ThemeSpecificControl(control);
            }
        }

        /// <summary>
        /// Register a handler for a non-default type when theming is requested.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public static void RegisterCustomThemeHandler(Type type, ThemeChangedCallback callback)
        {
            if (callback == null)
            {
                CustomTypeHandlers.Remove(type);
                return;
            }
            CustomTypeHandlers[type] = callback;
        }

        public static void OnThemeChanged(Control control, ThemeChangedCallback callback)
        {
            if (control == null) return;
            if(callback == null)
            {
                CustomControlHandlers.Remove(control);
                return;
            }
            CustomControlHandlers[control] = callback;
            control.Disposed += CustomThemeCallback_ControlDisposed;
        }

        private static void CustomThemeCallback_ControlDisposed(object sender, EventArgs e)
        {
            CustomControlHandlers.Remove(sender as Control);
        }

        private static void ThemeSpecificControl(Control control)
        {
            if (CustomTypeHandlers.ContainsKey(control.GetType()))
            {
                CustomTypeHandlers[control.GetType()]?.Invoke(CurrentTheme);
            }
            else
            {
                switch (control)
                {
                    case Form form:
                        form.BackColor = CurrentTheme.BackColor;
                        form.ForeColor = CurrentTheme.TextColor;
                        break;
                    case Button button:
                        button.BackColor = CurrentTheme.BackColor;
                        button.FlatAppearance.BorderColor = CurrentTheme.AccentColor;
                        button.FlatStyle = CurrentTheme.ButtonFlatStyle;
                        button.ForeColor = CurrentTheme.TextColor;
                        button.FlatAppearance.MouseOverBackColor = CurrentTheme.ButtonHoverColor;
                        break;
                    case Label label:
                        label.ForeColor = CurrentTheme.TextColor;
                        break;
                    case Panel panel:
                        panel.BackColor = CurrentTheme.BackColor;
                        break;
                    default: throw new NotImplementedException($"Theming procedure for control type: '{control.GetType()}' has not been implemented.");
                }
            }

            // invoke registered callbacks for theme changed
            if (CustomControlHandlers.ContainsKey(control))
                CustomControlHandlers[control].Invoke(CurrentTheme);
        }

        private static void RegisterAndThemeControl(Control control)
        {
            control.Disposed += ThemedControlDisposed;
            ThemedControls.Add(control);
            ThemeSpecificControl(control);
        }
    }
}
