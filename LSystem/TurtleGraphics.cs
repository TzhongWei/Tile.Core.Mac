using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Geometry;
using Tile.LSystem.TokenAction;

namespace Tile.LSystem
{
    public class TurtleGraphic
    {
        public string cmd { get; private set; } = "Turtle Graphic Program \n";
        public List<string> ExecuteTokens { get; private set; }
        public Dictionary<string, ActionBase> TokenAction { get; private set; }
        private TokenPointer _pointer;
        public TurtleGraphic(RuleExecuter ruleExecuter)
        {
            this.ExecuteTokens = ruleExecuter.TokenResults;
            TokenAction = new Dictionary<string, ActionBase>();
            _pointer = new TokenPointer();
        }
        public TurtleGraphic(RuleExecuter ruleExecuter, Plane Location) : this(ruleExecuter)
        {
            _pointer = new TokenPointer(Location);
        }
        public TokenPointer GetPointer => _pointer;
        public List<GeometryBase> GetGeometries => _pointer.DisplayGeo.Select(x => x.Duplicate()).ToList();
        public void AddAction(ActionBase action)
            => this.TokenAction.Add(action.Name, action);
        public void AddAction(IEnumerable<ActionBase> actions)
        {
            foreach (var item in actions)
                this.AddAction(item);
        }
        public override string ToString()
         => cmd;
        public bool RunStepByStep(ref int Iteration)
        {
            if (this.TokenAction.Count == 0)
            {
                this.cmd += "TokenAction doesn't have anything";
                return false;
            }
            if (ExecuteTokens == null)
            {
                this.cmd += "ExecuteTokens list is empty";
                return false;
            }
            if (Iteration >= this.ExecuteTokens.Count)
                return false;

            bool Result = true;
            var Token = ExecuteTokens[Iteration];
            if (TokenAction.ContainsKey(Token))
            {

                Result |= TokenAction[Token].Execute(this._pointer);
                var PostActionList = TokenAction
                                        .Where(x => x.Value is IPostTokenAction)
                                        .Select(x => x.Value)
                                        .OfType<IPostTokenAction>() // Cast to the interface type
                                        .ToList();

                foreach (var post in PostActionList)
                {
                    if (post.IsActive(Token))
                    {
                        Result |= post.PostExecute(_pointer); // Pass an instance of XTokenPointer
                    }
                }
            }
            else
            {
                cmd += $"{Token} isn't defined an action \n";
                Result = false;
            }
            return Result;
        }
        public bool Run()
        {
            if (this.TokenAction.Count == 0)
            {
                this.cmd += "TokenAction list is empty";
                return false;
            }
            if (ExecuteTokens == null)
            {
                this.cmd += "ExecuteTokens list is empty";
                return false;
            }
            bool Result = true;
            foreach (string Token in ExecuteTokens)
            {
                if (TokenAction.ContainsKey(Token))
                {
                    Result |= TokenAction[Token].Execute(_pointer);
                    var PostActionList = TokenAction
                                        .Where(x => x.Value is IPostTokenAction)
                                        .Select(x => x.Value)
                                        .OfType<IPostTokenAction>() // Cast to the interface type
                                        .ToList();

                    foreach (var post in PostActionList)
                    {
                        if (post.IsActive(Token))
                        {
                            Result |= post.PostExecute(_pointer); // Pass an instance of XTokenPointer
                        }
                    }
                    cmd += $"{Token} is execuated \n Execute : {TokenAction[Token].Description} \n";
                }
                else
                {
                    cmd += $"{Token} isn't defined an action \n";
                    Result = false;
                    break;
                }
            }
            return Result;
        }
    }
}