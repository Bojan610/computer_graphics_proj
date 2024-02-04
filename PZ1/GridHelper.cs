using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PZ1
{
    public class GridHelper
    {

        public static void GenerateRowsAndColoumns(Grid grid)
        {
            grid.ShowGridLines = false;
           
            for (int i = 0; i < 100; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                grid.ColumnDefinitions.Add(cd);
            }

            for (int i = 0; i < 100; i++)
            {
                RowDefinition rd = new RowDefinition();
                grid.RowDefinitions.Add(rd);
            }

            
        }
    }
}
