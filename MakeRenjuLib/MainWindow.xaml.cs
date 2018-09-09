using RenjuCoachWebServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace MakeRenjuLib
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String RenjuPoints = this.Points.Text;
            //忽略空行
            string[] ContentLines = RenjuPoints.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


            BoardMatrix boardMatrix = new BoardMatrix(15);
            foreach (String item in ContentLines)
            {
                String points = item.Replace(" ", "");
                string[] point = points.Split(',');

                int x = int.Parse(point[0]);
                int y = int.Parse(point[1]);
                int p = int.Parse(point[2]);
                boardMatrix.SetMatrixPices(x, y, p);

            }

            String ChessString = "";
            for (int i = 0; i < ContentLines.Length; i++)
            {
                String points = ContentLines[i].Replace(" ", "");
                string[] point = points.Split(',');

                int x = int.Parse(point[0]);
                int y = int.Parse(point[1]);
                int p = int.Parse(point[2]);

                ChessString += String.Format("     {0,2} {1}{2,-2}",
                    i+1,
                     ((Char)((Char)'A'+15-x)).ToString(),
                    y
                    );
                if (i % 2 == 1) ChessString += Environment.NewLine;

            }


            this.RenJunLibString.Text = boardMatrix.ToRenJunLib()+ ChessString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Points.Text =
@"1,4,1
1,7,2
2,9,1
2,5,2
3,3,1
2,8,2
3,5,1
2,13,2
3,7,1
3,6,2
3,12,1
3,9,2
4,4,1
3,10,2
4,8,1
3,11,2
4,11,1
4,5,2
5,6,1
4,6,2
5,8,1
4,7,2
5,10,1
4,9,2
5,11,1
4,13,2
5,12,1
5,7,2
5,13,1
5,9,2
6,1,1
5,14,2
6,6,1
6,2,2
6,7,1
6,3,2
6,8,1
6,4,2
6,9,1
6,5,2
6,11,1
6,10,2
7,2,1
6,12,2
7,3,1
7,4,2
7,5,1
7,7,2
7,6,1
7,8,2
7,10,1
7,9,2
7,12,1
8,3,2
8,2,1
8,7,2
8,4,1
8,11,2
8,5,1
9,2,2
8,8,1
9,3,2
9,1,1
9,4,2
9,5,1
9,6,2
9,7,1
9,10,2
9,9,1
10,5,2
10,1,1
10,8,2
10,6,1
10,9,2
10,10,1
10,11,2
11,4,1
11,7,2
11,6,1
11,11,2
11,8,1
12,4,2
12,5,1
12,6,2
12,11,1
13,5,2
14,4,1
13,7,2";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            String FileName = System.Guid.NewGuid()+".txt";
            File.WriteAllText(FileName, this.RenJunLibString.Text);
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = @" /select, "+ FileName;
            p.Start();
        }
    }
}
