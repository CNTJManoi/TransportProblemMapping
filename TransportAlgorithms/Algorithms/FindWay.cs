using System;
using System.Windows;

namespace TransportAlgorithms.Algorithms
{
    internal class FindWay
    {
        private readonly Point Begining;
        private FindWay[] Childrens;

        private readonly FindWay Father;

        //true - вниз/вверх
        //false - влево/вправо
        private readonly bool flag;
        private readonly Point[] mAllowed;
        private Point Root;

        public FindWay(int x, int y, bool _flag, Point[] _mAllowed, Point _Beg, FindWay _Father)
        {
            Begining = _Beg;
            flag = _flag;
            Root = new Point(x, y);
            mAllowed = _mAllowed;
            Father = _Father;
        }

        public bool BuildTree()
        {
            var ps = new Point[mAllowed.Length];
            var Count = 0;
            for (var i = 0; i < mAllowed.Length; i++)
                if (flag)
                {
                    if (Root.Y == mAllowed[i].Y)
                    {
                        Count++;
                        ps[Count - 1] = mAllowed[i];
                    }
                }
                else if (Root.X == mAllowed[i].X)
                {
                    Count++;
                    ps[Count - 1] = mAllowed[i];
                }

            var fwu = this;
            Childrens = new FindWay[Count];
            //Point[] ss = new Point[mAllowed.Length];
            var k = 0;
            for (var i = 0; i < Count; i++)
            {
                if (ps[i] == Root) continue;
                if (ps[i] == Begining)
                {
                    while (fwu != null)
                    {
                        mAllowed[k] = fwu.Root;
                        fwu = fwu.Father;
                        k++;
                    }

                    ;
                    for (; k < mAllowed.Length; k++) mAllowed[k] = new Point(-1, -1);
                    return true;
                }

                if (!Array.TrueForAll(ps, p => p.X == 0 && p.Y == 0))
                {
                    Childrens[i] = new FindWay((int)ps[i].X, (int)ps[i].Y, !flag, mAllowed, Begining, this);
                    var result = Childrens[i].BuildTree();
                    if (result) return true;
                }
            }

            return false;
        }
    }
}