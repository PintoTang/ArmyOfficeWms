using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CLDC.Infrastructrue.UserCtrl
{
    public static class TreeViewHelper
    {
        public static readonly DependencyProperty SingleExpandPathProperty = DependencyProperty.RegisterAttached(
            "SingleExpandPath", typeof(bool), typeof(TreeViewHelper), new PropertyMetadata(default(bool), OnSingleExpandPathChanged));

        private static void OnSingleExpandPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue as bool? == true)
            {
                ((TreeView)d).SelectedItemChanged += TreeViewOnSelectedItemChanged;
            }
            else
            {
                ((TreeView)d).SelectedItemChanged -= TreeViewOnSelectedItemChanged;
            }
        }

        private static TreeView curTreeView;

        private static void TreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var seen = new HashSet<TreeViewItem>();

            curTreeView = (TreeView)sender;

            if (GetTreeViewItem(e.NewValue) != null)
            {
                TreeViewItem newTvi = GetTreeViewItem(e.NewValue) as TreeViewItem;
                newTvi.IsExpanded = !newTvi.IsExpanded;
                foreach (var parents in GetParents(newTvi))
                {
                    seen.Add(parents);
                }

                foreach (TreeViewItem tvi in GetExpanded(curTreeView))
                {
                    if (!seen.Contains(tvi))
                        tvi.IsExpanded = false;
                }
            }
            else if (GetTreeViewItem(e.OldValue, curTreeView) != null)
            {
                TreeViewItem oldTvi = GetTreeViewItem(e.OldValue, curTreeView) as TreeViewItem;
                oldTvi.IsExpanded = !oldTvi.IsExpanded;
            }
        }
        static IEnumerable<TreeViewItem> GetExpanded(ItemsControl parent)
        {
            foreach (TreeViewItem tvi in GetTreeViewItems(parent).Where(x => x != null))
            {
                if (tvi.IsExpanded)
                {
                    yield return tvi;

                    foreach (TreeViewItem child in GetExpanded(tvi))
                    {
                        yield return child;
                    }
                }
            }
        }

        static IEnumerable<TreeViewItem> GetTreeViewItems(ItemsControl parent)
        {
            return parent.Items.Cast<object>().Select(x => GetTreeViewItem(x, parent));
        }

        static TreeViewItem GetTreeViewItem(object item, ItemsControl parent = null)
        {
            if (item is TreeViewItem)
            {
                TreeViewItem tvi = item as TreeViewItem;
                return tvi;
            }
            return (parent ?? curTreeView).ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
        }

        static IEnumerable<TreeViewItem> GetParents(TreeViewItem tvi)
        {
            for (; tvi != null; tvi = tvi.Parent as TreeViewItem)
                yield return tvi;
        }

        public static void SetSingleExpandPath(DependencyObject element, bool value)
        {
            element.SetValue(SingleExpandPathProperty, value);
        }

        public static bool GetSingleExpandPath(DependencyObject element)
        {
            return (bool)element.GetValue(SingleExpandPathProperty);
        }

        public static readonly DependencyProperty SelectedCommandProperty = DependencyProperty.RegisterAttached(
            "SelectedCommand", typeof(ICommand), typeof(TreeViewHelper), new PropertyMetadata(default(ICommand), OnSelectedCommandChanged));

        private static void OnSelectedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is TreeViewItem)
            {
                TreeViewItem treeViewItem = dependencyObject as TreeViewItem;
                if (e.NewValue != null)
                {
                    treeViewItem.Selected += TreeViewItemOnSelected;
                }
                else
                {
                    treeViewItem.Selected -= TreeViewItemOnSelected;
                }
            }
        }

        private static void TreeViewItemOnSelected(object sender, RoutedEventArgs routedEventArgs)
        {
            var tvi = (TreeViewItem)sender;
            if (tvi.IsSelected)
            {
                ICommand command = GetSelectedCommand(tvi);
                object parameter = GetSelectedCommandParameter(tvi);
                if (command.CanExecute(parameter) == true)
                {
                    command.Execute(parameter);
                }
            }
        }

        public static void SetSelectedCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(SelectedCommandProperty, value);
        }

        public static ICommand GetSelectedCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(SelectedCommandProperty);
        }

        public static readonly DependencyProperty SelectedCommandParameterProperty = DependencyProperty.RegisterAttached(
            "SelectedCommandParameter", typeof(object), typeof(TreeViewHelper), new PropertyMetadata(default(object)));

        public static void SetSelectedCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(SelectedCommandParameterProperty, value);
        }

        public static object GetSelectedCommandParameter(DependencyObject element)
        {
            return element.GetValue(SelectedCommandParameterProperty);
        }
    }
}
