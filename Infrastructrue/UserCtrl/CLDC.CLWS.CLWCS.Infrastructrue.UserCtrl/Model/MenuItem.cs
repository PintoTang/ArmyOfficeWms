using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace CLDC.Infrastructrue.UserCtrl.Model
{
    public class MenuItem : ObservableObject, IEnumerable<MenuItem>
    {
        private object objLock = new object();
        private string _title = string.Empty;
        public string Title { get { return _title; } }

        public object Tag { get; set; }

        public UserControl UserView { get; set; }

        private PackIconKind? _Icon;
        public PackIconKind? Icon
        {
            get
            {
                return _Icon;
            }
        }

        private ObservableCollection<MenuItem> _children = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> Children { get { return _children; } }

        private MenuItem Parent { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (Set(ref _isSelected, value))
                {
                    if (value)
                    {
                        IsExpanded = !IsExpanded;
                    }
                    else
                    {
                        for (MenuItem current = this; current != null;current = current.Parent)
                        {
                            current.IsExpanded = current.IsSelectionWithin;
                        }
                    }
                }
            }
        }

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (Set(ref _IsExpanded, value))
                {
                    if (!value)
                    {
                        IsSelected = false;
                    }
                }
            }
        }

        public MenuItem(string title, PackIconKind icon, UserControl userView)
            : this(title, icon)
        {
            UserView = userView;
        }

        public MenuItem(string title, PackIconKind icon)
            : this(title)
        {
            _Icon = icon;
        }

        public MenuItem(string title, PackIconKind icon,object tag)
            : this(title)
        {
            _Icon = icon;
            Tag = tag;
        }

        public MenuItem(string title, UserControl userView)
            : this(title)
        {
            UserView = userView;
        }

        public MenuItem(string title)
        {
            _title = title;
        }

        private bool IsSelectionWithin
        {
            get
            {
                lock (objLock)
                {
                    return IsSelected || Children.Any(x => x.IsSelectionWithin);
                }
            }
        }

        public void Add(MenuItem child)
        {
            lock (objLock)
            {
                child.Parent = this;
                Children.Add(child);
            }
        
        }

        public void AddRange(ObservableCollection<MenuItem> childs)
        {
            lock (objLock)
            {
                foreach (MenuItem child in childs)
                {
                    child.Parent = this;
                    Children.Add(child);
                }
            }
        }

        public IEnumerator<MenuItem> GetEnumerator()
        {
            lock (objLock)
            {
                return Children.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           return GetEnumerator();
        }
    }
}
