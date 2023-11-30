using System;
using System.Collections.Generic;
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
using CLDC.CLWS.CLWCS.WareHouse.DataModel.ViewModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel.View
{
    /// <summary>
    /// BarcodeView.xaml 的交互逻辑
    /// </summary>
    public partial class BarcodeView : UserControl
    {
        public BarcodeView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
            "TitleContent", typeof(object), typeof(BarcodeView), new PropertyMetadata(default(object)));

        /// <summary>
        /// 
        /// </summary>
        public object TitleContent
        {
            get { return (object)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }
        private void BarCodeViewSource_OnFilter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                e.Accepted = true;
                return;
            }
            IFilterable filter = e.Item as IFilterable;
            if (filter==null)
            {
                e.Accepted = false;
                return;
            }
            e.Accepted = filter.Filter(SearchBox.Text);
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            CollectionViewSource viewResource = (CollectionViewSource)this.Resources["BarCodeViewSource"];
            if (viewResource != null)
            {
                viewResource.View.Refresh();
            }
        }

        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            CollectionViewSource viewResource = (CollectionViewSource)this.Resources["BarCodeViewSource"];
            if (viewResource != null)
            {
                viewResource.View.Refresh();
            }
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource viewResource = (CollectionViewSource)this.Resources["BarCodeViewSource"];
            if (viewResource != null)
            {
                viewResource.View.Refresh();
            }
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }
    }
}
