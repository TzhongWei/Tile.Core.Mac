using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Core.Util
{
    public struct HatGroup<T> where T : class
    {
        public T Hat_H_ID { get; set; }
        public T Hat_H1_ID { get; set; }
        public T Hat_T_ID { get; set; }
        public T Hat_P_ID { get; set; }
        public T Hat_F_ID { get; set; }
        public T this[int ID]
        {
            get
            {
                switch (ID)
                {
                    case 0: return Hat_H_ID;
                    case 1: return Hat_H1_ID;
                    case 2: return Hat_T_ID;
                    case 3: return Hat_P_ID;
                    case 4: return Hat_F_ID;
                    default: throw new ArgumentOutOfRangeException(nameof(ID), "Index out of range");
                }
            }
            set
            {
                switch (ID)
                {
                    case 0:
                        Hat_H_ID = value;
                        break;
                    case 1:
                        Hat_H1_ID = value;
                        break;
                    case 2:
                        Hat_T_ID = value;
                        break;
                    case 3:
                        Hat_P_ID = value;
                        break;
                    case 4:
                        Hat_F_ID = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(ID), "Index out of range");
                }
            }
        }
        public HatGroup((T, T, T, T, T) ID)
        {
            Hat_H_ID = ID.Item1;
            Hat_H1_ID = ID.Item2;
            Hat_T_ID = ID.Item3;
            Hat_P_ID = ID.Item4;
            Hat_F_ID = ID.Item5;
        }
        public HatGroup(T H_1, T H1_2, T T_2, T P_3, T F_4)
        {
            Hat_H_ID = H_1;
            Hat_H1_ID = H1_2;
            Hat_T_ID = T_2;
            Hat_P_ID = P_3;
            Hat_F_ID = F_4;
        }
        public int Length => 5;
        public delegate object SetAll(T Item, object sender = null);
        public bool SystemSetting(SetAll Set, out List<object> Result)
        {
            Result = new List<object>();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Result.Add(Set(this[i], null));
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        public bool SystemSetting(SetAll Set, object Item, out List<object> Result)
        {
            Result = new List<object>();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Result.Add(Set(this[i], Item));
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        public List<T> ToList() => new List<T> { this[0], this[1], this[2], this[3], this[4] };
    }
}
