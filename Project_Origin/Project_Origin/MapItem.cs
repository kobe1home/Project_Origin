using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Origin
{
    class MapItem
    {
        private Node node;
        private int row;
        private int col;

        
        public MapItem(Node node, int row, int col)
        {
            this.node = node;
            this.row = row;
            this.col = col;
        }

        public Node Node
        {
            get { return node; }
            set { node = value; }
        }

        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        public int Col
        {
            get { return col; }
            set { col = value; }
        }
    }
}
