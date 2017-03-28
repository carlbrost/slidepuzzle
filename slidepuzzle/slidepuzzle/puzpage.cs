﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace slidepuzzle
{
    public class puzpage : ContentPage
    {

        private const int SIZE = 4;

        private AbsoluteLayout _absoluteLayout;
        private Dictionary<GridPosition, GridItem> _gridItems;

        public puzpage()
        {
            _gridItems = new Dictionary<GridPosition, GridItem>();
            _absoluteLayout = new AbsoluteLayout
            {
                BackgroundColor = Color.Green,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            Shuffle();

            var counter = 1;
            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = new GridItem(new GridPosition(row, col),
                    counter.ToString());
                    if (counter ==16)
                    {
                        item = new GridItem(new GridPosition(row, col), "empty");
                    
                            }
                    else
                    {
                        item = new GridItem(new GridPosition(row, col), counter.ToString());
                    }
                    //Allows for the numbers to be moved.

                    var tapRecognizer = new TapGestureRecognizer();
                    tapRecognizer.Tapped += OnLabelTapped;
                    item.GestureRecognizers.Add(tapRecognizer);

                    _gridItems.Add(item.Position, item);
                    _absoluteLayout.Children.Add(item);

                    counter++;
                }
            }

            ContentView contentView = new ContentView
            {
                Content = _absoluteLayout
            };

            contentView.SizeChanged += OnContentViewSizeChanged;
            this.Padding = new Thickness(5, Device.OnPlatform(25, 5, 5), 5, 5);
            this.Content = contentView;
        }
        void OnContentViewSizeChanged(object sender, EventArgs args)
        {
            ContentView contentView = (ContentView)sender;
            double squareSize = Math.Min(contentView.Width, contentView.Height) / SIZE;
            for (var row = 0; row < SIZE; row++)
            {
                for (var col = 0; col < SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    Rectangle rect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);
                    AbsoluteLayout.SetLayoutBounds(item, rect);
                }
            }
        }

        void OnLabelTapped(object sender, EventArgs args)
        {
            GridItem item = (GridItem)sender;

            Random rand = new Random();
            int move = rand.Next(0, 4);

            //Adjust random move to account for edges
            if (move == 0 && item.Position.Row == 0)
            {
                move = 2;
            }
            else if (move == 1 && item.Position.Column == SIZE - 1)
            {
                move = 3;
            }
            else if (move == 2 && item.Position.Row == SIZE - 1)
            {
                move = 0;
            }
            else if (move == 3 && item.Position.Column == 0)
            {
                move = 1;
            }

            int row = 0;
            int col = 0;

            if (move == 0) //Move up
            {
                row = item.Position.Row - 1;
                col = item.Position.Column;
            }
            else if (move == 1) // Move Right
            {
                row = item.Position.Row;
                col = item.Position.Column + 1;
            }
            else if (move == 2) // Move Down
            {
                row = item.Position.Row + 1;
                col = item.Position.Column;
            }
            else //Move Left
            {
                row = item.Position.Row;
                col = item.Position.Column - 1;
            }

            GridItem swapWith = _gridItems[new GridPosition(row, col)];
            swap(item, swapWith);
            OnContentViewSizeChanged(this.Content, null);
        }

        void swap(GridItem item1, GridItem item2)
        {
            GridPosition temp = item1.Position;
            item1.Position = item2.Position;
            item2.Position = temp;

            _gridItems[item1.Position] = item1;
            _gridItems[item2.Position] = item2;
        }

        void Swap(GridItem item1, GridItem item2)
        {

            GridPosition temp = item1.Position;
            item1.Position = item2.Position;
            item2.Position = temp;
            _gridItems[item1.Position] = item1;
            _gridItems[item2.Position] = item2;
        }
        void Shuffle()
        {
            Random rand = new Random();
            for (var row=0;row<SIZE; row++)
            {
              for (var col=0; col<SIZE; col++)
                {
                    GridItem item = _gridItems[new GridPosition(row, col)];
                    int swapRow = rand.Next(0, 4);
                    int swapCol = rand.Next(0, 4);
                    GridItem swapItem = _gridItems[new GridPosition(swapRow, swapCol)];
                    Swap(item, swapItem);
                }
            }
        }
        internal class GridItem : Image
        {
            public GridPosition Position

            {
                get;
                set;
            }

            public GridItem(GridPosition position, String src)
            {
                
                Position = position;

                String path = "slidepuzzle." + src + ".png";
                Source = ImageSource.FromResource(path); 
                
                HorizontalOptions = LayoutOptions.FillAndExpand;
                VerticalOptions = LayoutOptions.FillAndExpand;
            }
        }
        internal class GridPosition
        {
            public int Row
            {
                get; set;
            }

            public int Column
            {
                get; set;
            }

            public GridPosition(int row, int col)
            {
                Row = row;
                Column = col;
            }

            public override bool Equals(object obj)
            {
                GridPosition other = obj as GridPosition;
                if (other != null && this.Row == other.Row && this.Column == other.Column)
                {
                    return true;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return 17 * (23 + this.Row.GetHashCode()) * (23 + this.Column.GetHashCode());
            }
        }
    }
}