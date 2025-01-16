using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace Tile.LSystem.TokenAction
{
    public abstract class ActionBase : ITokenAction
    {
        /// <summary>
        /// This userstring share among the inherited classes
        /// </summary>
        protected Dictionary<string, object> UserString;
        public abstract Transform ActionTransform { get; protected set; }
        private ActionBase()
        {
            UserString = new Dictionary<string, object>();
        }
        public ActionBase(string Name, string Description) : this()
        {
            this.Name = Name;
            this.Description = Description;
        }
        public string Name { get; }
        public abstract bool Execute(TokenPointer _pointer);
        public string Description { get; }
        public override string ToString()
         => $"{this.GetType().Name} => \n {Name} : {Description} ";
    }
}