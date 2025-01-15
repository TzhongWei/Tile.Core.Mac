using Rhino;
using Rhino.DocObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tile.Core.Util
{
    public class BlockInstanceManager : IList<BlockInstance>
    {
        private HashSet<BlockInstance> _BlockInstance = new HashSet<BlockInstance>();
        public BlockInstance this[int index] { get => this._BlockInstance.ToList()[index]; set
            {
                if (value == null || _BlockInstance.Contains(value))
                    throw new ArgumentException("Value is repeated.");
                else
                    this._BlockInstance.Add(value);
            } 
        }

        public int Count => this._BlockInstance.Count;

        public bool IsReadOnly => this._BlockInstance.Count > 0;

        public void Add(BlockInstance item)
        {
            if (item == null || _BlockInstance.Contains(item))
                throw new ArgumentException("Value is repeated.");
            else
                this._BlockInstance.Add(item);
        }
        public void Add(InstanceDefinition instance)
        {
            var blockName = instance.GetUserString("BlockName");
            if (string.IsNullOrWhiteSpace(blockName) || this.Contains(blockName))
                throw new ArgumentException($"Block with name '{blockName}' already exists.");

            Label label = ParseLabel(instance.GetUserString("Label"));
            var HatInstance = new BlockInstance(label, instance.GetUserString("BlockName"));
            this._BlockInstance.Add(HatInstance);
        }
        private Label ParseLabel(string labelString)
        {
            switch (labelString)
            {
                case "H1":
                    return Label.H1;
                case "H":
                    return Label.H;
                case "T":
                    return Label.T;
                case "P":
                    return Label.P;
                case "F":
                    return Label.F;
                default:
                    throw new ArgumentException($"Invalid label: {labelString}");
            }
        }
        public void Clear()
        {
            this._BlockInstance.Clear();
            Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions.Clear();
        }
        public void Reinitial()
        {
            this._BlockInstance.Clear();
            var RHDoc = Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions;
            foreach (var instance in RHDoc)
            {
                if (RhinoDoc.ActiveDoc.InstanceDefinitions.Find(instance.Name) == null) continue;
                if (instance.GetUserString("Hat") != "HatDoc") continue;

                try
                {
                    this.Add(instance);
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine($"Failed to add instance: {instance.Name}. Error: {ex.Message}");
                }
            }
        }
        public bool Contains(BlockInstance item)
        => this._BlockInstance.Contains(item);
        public bool Contains(string Name)
            => this._BlockInstance.Where(x => x.BlockName == Name).ToList().Count == 0 ? false : true;
        

        public void CopyTo(BlockInstance[] array, int arrayIndex)
        {
            this._BlockInstance.CopyTo(array, arrayIndex);
        }

        public IEnumerator<BlockInstance> GetEnumerator()
            => this._BlockInstance.GetEnumerator();
        

        public int IndexOf(BlockInstance item)
        => this._BlockInstance.ToList().IndexOf(item);

        public void Insert(int index, BlockInstance item)
        {
            var instanceList = _BlockInstance.ToList();
            if (instanceList.Contains(item))
                throw new ArgumentException("Value is repeated");

            instanceList.Insert(index, item);
            _BlockInstance = new HashSet<BlockInstance>(instanceList);
        }
        /// <summary>
        /// Remove the block instance from both this program and rhino instances
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(BlockInstance item)
            => item == null ? false : this._BlockInstance.Remove(item) & 
                Rhino.RhinoDoc.ActiveDoc.InstanceDefinitions.Delete(item.BlockIndex, true, true);
        public bool Remove(string Name)
            => this.Remove(this.Find(Name));

        public BlockInstance Find(string Name)
            => _BlockInstance.FirstOrDefault(x => x.BlockName == Name);
        public BlockInstance FindID(int index)
        {
            var Instance = this._BlockInstance.Where(x => x.BlockIndex == index).ToList();
            return Instance.Count == 0? null : Instance[0];
        }
        public void RemoveAt(int index)
        {
            this._BlockInstance.ToList().RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this._BlockInstance?.GetEnumerator();
    }
}
