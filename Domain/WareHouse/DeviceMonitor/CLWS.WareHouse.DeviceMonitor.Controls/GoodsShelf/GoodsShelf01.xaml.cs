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
using System.Windows.Threading;

namespace WHSE.Monitor.Framework.UserControls
{
    /// <summary>
    /// GoodsShelf.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsShelf01 : DeviceSimulation
    {
        public GoodsShelf01()
        {
            InitializeComponent();
        }
        private string _userControlName;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string UserControlName
        {
            get { return _userControlName; }
            set { _userControlName = value; }
        }

        private int _gridNumber;
        /// <summary>
        /// 仓位数量（单层）
        /// </summary>
        public int GridNumber
        {
            get { return _gridNumber; }
            set { _gridNumber = value; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GoodsShelfModelViewModel01 goodsmodel = new GoodsShelfModelViewModel01();
            this.DataContext = goodsmodel;
            goodsmodel.ShelfRow = "排：" + ShelfRow;
            goodsmodel.ShelfDeep = "深：" + ShelfDeep;
            this.Dispatcher.InvokeAsync(LoadCellByCellWidth,DispatcherPriority.Background);
        }

        private void LoadCellByCellWidth()
        {
            //隐藏相关的仓位
            bool hideFlag = false;
            List<string> listHide = new List<string>();
            if (!string.IsNullOrEmpty(HideNumber))
            {
                hideFlag = true;
                string[] strHide = HideNumber.Split(',');
                listHide = strHide.ToList();
            }
            //--
            //显示相关的柱子
            bool pillarFlag = false;
            List<string> listPillar = new List<string>();
            if (!string.IsNullOrEmpty(PillarNumber))
            {
                pillarFlag = true;
                string[] strPillar = PillarNumber.Split(',');
                listPillar = strPillar.ToList();
            }
            //--
            double width;
            width = sPanel.ActualWidth;
            
            if (CellNumber <= 0)
            {
                return;
            }

            if (CellDirNum == 0)
            {
                //从左到右
                for (int i = 0; i < CellNumber; i++)
                {
                    ShelfModelViewModel01 viewModel = new ShelfModelViewModel01();
                    ShelfModel01 rectangle = new ShelfModel01
                    {
                        Height = CellWidth,
                        Width = width,
                        DataContext = viewModel,
                        Margin = new Thickness(0, CellMargin, 0, CellMargin),
                        SnapsToDevicePixels = true

                    };
                    viewModel.ColumnNo = i + 1;
                    viewModel.BackColor = "#CAE1FF";
                    if (hideFlag) //隐藏相关的仓位
                    {
                        if (listHide.Contains(viewModel.ColumnNo.ToString()))
                        { 
                            rectangle.Visibility = Visibility.Hidden; 
                        }
                    }
                    if (pillarFlag) //显示相关的柱子
                    {
                        if (listPillar.Contains(viewModel.ColumnNo.ToString()))
                        {
                            rectangle.DataContext = "";
                            viewModel.BackColor = "#FFFF00";
                        }
                    }
                    sPanel.Children.Insert(0, rectangle);
                }
            }
            else if (CellDirNum == 1)
            {
                //从右到左
                for (int i = CellNumber; i > 0; i--)
                {
                    ShelfModelViewModel01 viewModel = new ShelfModelViewModel01();
                    ShelfModel01 rectangle = new ShelfModel01
                    {
                        Height = CellWidth,
                        Width = width,
                        DataContext = viewModel,
                        Margin = new Thickness(0, CellMargin, 0, CellMargin),
                        SnapsToDevicePixels = true
                    };
                    viewModel.ColumnNo = i;
                    viewModel.BackColor = "#CAE1FF";
                    if (hideFlag) //隐藏相关的仓位
                    {
                        if (listHide.Contains(viewModel.ColumnNo.ToString()))
                        {
                            rectangle.Visibility = Visibility.Hidden;
                        }
                    }
                    if (pillarFlag) //显示相关的柱子
                    {
                        if (listPillar.Contains(viewModel.ColumnNo.ToString()))
                        {
                            rectangle.DataContext = "";
                            viewModel.BackColor = "#FFFF00";
                        }
                    }
                    sPanel.Children.Insert(0, rectangle);
                }
            }
          
        }


        private double _cellWidth;
        /// <summary>
        /// 仓位的宽度
        /// </summary>
        public double CellWidth
        {
            get { return _cellWidth; }
            set { _cellWidth = value; }
        }

        private int _cellsNumber;
        /// <summary>
        /// 仓位的个数
        /// </summary>
        public int CellNumber
        {
            get { return _cellsNumber; }
            set { _cellsNumber = value; }
        }

        private double _cellMargin;

        public double CellMargin
        {
            get { return _cellMargin; }
            set { _cellMargin = value; }
        }

        private int _cellDirNum;
        /// <summary>
        /// 数字显示的方向  默认0 从左到右   1从右到左
        /// </summary>
        public int CellDirNum
        {
            get { return _cellDirNum; }
            set { _cellDirNum = value; }
        }
        /// <summary>
        /// 隐藏仓位
        /// </summary>
        private string _hideNumber;
        public string HideNumber
        {
            get { return _hideNumber; }
            set { _hideNumber = value; }
        }
        /// <summary>
        /// 柱子替代仓位
        /// </summary>
        private string _pillarNumber;
        public string PillarNumber
        {
            get { return _pillarNumber; }
            set { _pillarNumber = value; }
        }
        /// <summary>
        /// 排号
        /// </summary>
        private string _shelfRow;
        public string ShelfRow
        {
            get { return _shelfRow; }
            set
            {
                _shelfRow = value;        
            }
        }
        /// <summary>
        /// 排号
        /// </summary>
        private string _shelfDeep;
        public string ShelfDeep
        {
            get { return _shelfDeep; }
            set
            {
                _shelfDeep = value;            
            }
        }

    }
}
