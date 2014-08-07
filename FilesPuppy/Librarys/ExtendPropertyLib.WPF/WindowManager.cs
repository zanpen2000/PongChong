using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ExtendPropertyLib.WPF
{

    public interface IWindowManager
    {
        void Show(ViewModelBase element);

        Nullable<bool> ShowDialog(ViewModelBase element);

        void ShowPopup(ViewModelBase element);

        void ShowMessage(string msg);

        bool ShowConfirm(string confirmText);

        string Prompt(string promptText);

        Size Size { set; get; }

        bool MaxSize { set; get; }

        void Close(ViewModelBase element);
    }


    /// <summary>
    /// WPF窗口管理器实例
    /// </summary>
    public class WindowManager : IWindowManager
    {
        #region Window Style

        private const int GWL_STYLE = -16;

        [Flags]
        public enum WindowStyle : int
        {
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
        }

        // Don't use this version for dealing with pointers
        [DllImport("user32", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Don't use this version for dealing with pointers
        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public static int AlterWindowStyle(Window win,
            WindowStyle orFlags, WindowStyle andNotFlags)
        {
            var interop = new WindowInteropHelper(win);

            int prevStyle = GetWindowLong(interop.Handle, GWL_STYLE);
            if (prevStyle == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "Failed to get window style");
            }

            int newStyle = (prevStyle | (int)orFlags) & ~((int)andNotFlags);
            if (SetWindowLong(interop.Handle, GWL_STYLE, newStyle) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(),
                    "Failed to set window style");
            }
            return prevStyle;
        }

        public static int DisableMaximizeButton(Window win)
        {
            return AlterWindowStyle(win, 0, WindowStyle.WS_MAXIMIZEBOX);
        }
        public static int DisableMinimizeButton(Window win)
        {
            return AlterWindowStyle(win, 0, WindowStyle.WS_MINIMIZEBOX);
        }
        #endregion

        public WindowManager()
        {
            Size = new Size(0, 0);
        }

        public Size Size { set; get; }
        public bool MaxSize { set; get; }

        public void Show(ViewModelBase element)
        {

            ViewLocator vl = new ViewLocator();
            var view = vl.FindView(element);
            ViewModelBinder.Bind(element, view, true);
            Window window = null;
            if (view is Window)
            {
                window = (Window)view;
            }
            else
            {
                window = new Window();
                window.Content = view;
                window.SizeToContent = SizeToContent.WidthAndHeight;
            }

            string title = element.GetViewTitle();
            if (!string.IsNullOrEmpty(title))
            {
                window.Title = title;
            }

            if (this.Size != new Size(0, 0))
            {
                window.Height = Size.Height;
                window.Width = Size.Width;
            }
            window.Closed += (s, e) => element.Closed();
            if (MaxSize)
                window.WindowState = WindowState.Maximized;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();

        }

        public bool? ShowDialog(ViewModelBase element)
        {
            ViewLocator vl = new ViewLocator();
            var view = vl.FindView(element);
            ViewModelBinder.Bind(element, view, true);
            Window window = null;
            if (view is Window)
            {
                window = (Window)view;
            }
            else
            {
                window = new Window();
                window.Content = view;

                window.SizeToContent = SizeToContent.WidthAndHeight;

            }

            string title = element.GetViewTitle();
            if (!string.IsNullOrEmpty(title))
            {
                window.Title = title;
            }

            if (this.Size != new Size(0, 0))
            {
                window.Height = Size.Height;
                window.Width = Size.Width;
            }


            window.Closed += (s, e) => element.Closed();
            window.Loaded += (s, e) =>
            {
                DisableMaximizeButton(window);
                DisableMinimizeButton(window);
            };
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return window.ShowDialog();
        }

        public void ShowPopup(ViewModelBase element)
        {
            var type = element.GetType();
            ViewLocator vl = new ViewLocator();
            var view = vl.FindView(element);
            ViewModelBinder.Bind(element, view, true);
            var popup = new Popup();

            popup.Placement = PlacementMode.MousePoint;
            popup.AllowsTransparency = true;

            if (view is Window)
            {
                var winView = view as Window;
                var ui = winView.Content as UIElement;
                winView.Content = null;
                popup.Child = ui;
            }
            else
            {
                popup.Child = view;
            }

            popup.IsOpen = true;
            popup.CaptureMouse();
        }

        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }



        public bool ShowConfirm(string confirmText)
        {
            throw new NotImplementedException();
        }

        public string Prompt(string promptText)
        {
            throw new NotImplementedException();
        }


        public void Close(ViewModelBase viewModel)
        {
            var elem = viewModel.View as FrameworkElement;

            if (elem != null)
            {

                Window window = null;
                if (elem is Window)
                {
                    window = (Window)elem;
                }
                else
                {
                    window = elem.Parent as Window;
                }

                window.Close();
            }
        }
    }
}
