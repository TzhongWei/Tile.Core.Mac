using System;
using System.Text;
using System.Collections.Generic;
using Tile.LSystem.Util;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using System.Linq.Expressions;


namespace Tile.LSystem
{
    public class RuleExecuter
    {
        public IReadOnlyList<ProductionRule> ProductionRules { get; private set; }
        private int iterationCount = 1;
        public static RuleExecuter Unset => null;
        public List<string> OriginalRule { get; }
        public List<string> TokenResults { get; private set; }
        public string cmd { get; private set; }
        public int IterationCount { get; private set; } = 0;
        /// <summary>
        ///  A list of all terminal tokens
        /// A terminal is the smallest common denominator,
        /// it can not be split into other non-terminals or terminals
        /// </summary>
        private HashSet<string> _terminals;
        /// <summary>
        /// A list of all non-terminal tokens
        /// A non-terminal consists of 0 or more terminals
        /// or other non-terminals
        /// </summary>
        private HashSet<string> _nonterminals;
        /// <summary>
        /// The axiom of this grammar
        /// </summary>
        private string _start { get; set; }
        /// <summary>
        /// All tokens in the production rules
        /// </summary>
        private readonly HashSet<string> _tokens;
        private RuleExecuter()
        {
            this.ProductionRules = new List<ProductionRule>();
            this._tokens = new HashSet<string>();
        }
        public RuleExecuter(IEnumerable<string> Rules) : this()
        {
            OriginalRule = new List<string>(Rules);
        }
        public bool SetAxiom(string Axiom)
        {
            if (this._tokens.Contains(Axiom))
            {
                this._start = Axiom;
                return true;
            }
            else
            {
                cmd += $"{Axiom} isn't exist.";
            }
            return false;
        }
        public bool SetRule()
        {
            var Rules = OriginalRule;
            Rules = Rules.Where(x => !x.Contains("//")).ToList();
            var RulePairs = GenerateRule(Rules).ToList();

            if (RulePairs.Count == 0)
            {
                cmd += "production rule and semantic rule generate failed\n";
                return false;
            }
            Rules = RulePairs.Select(x => $"{x.Item1} = {x.Item2}").ToList();
            this.ProductionRules = new List<ProductionRule>(ProductionRule.CreateProductionRules(Rules));

            for (int i = 0; i < this.ProductionRules.Count; i++)
            {
                var ProRule = ProductionRules[i];
                cmd += ProRule.Head + $", Index : {ProRule.Index}  =>\n";
                cmd += "  " + ProRule.ToString() + "\n";
            }
            ComputeNonterminals();
            ComputeTerminals();
            return true;
        }
        private IEnumerable<(string, string)> GenerateRule(IEnumerable<string> Rules)
        {
            foreach (var Rule in Rules)
            {
                var RulePair = GenerateRule(Rule);
                if (RulePair == ("", ""))
                    cmd += $"{Rule} differentiate failed \n";
                yield return RulePair;
            }
        }
        private (string, string) GenerateRule(string Rule)
        {
            //var Splits = 
            if (!AcceptRule(Rule)) return ("", "");
            var Splits = Rule.Split('=');
            var Head = Tools.CleanSequence(Splits[0]);
            var Body = Tools.CleanSequence(Splits[1]);
            return (Head, Body);

            bool AcceptRule(string AnyRule)
                => AnyRule.Split('=').Length == 2;
        }
        /// <summary>
        /// Get the axiom of grammar
        /// </summary>
        /// <returns></returns>
        public string ComputeStartNonTerminal()
        {
            if (this._start is null)
            {
                this._start = this.ProductionRules[0].Head;
            }
            return this._start;
        }
        /// <summary>
        /// Compute nonterminal string labels from production rule
        /// </summary>
        /// <returns></returns>
        public HashSet<string> ComputeNonterminals()
        {
            if (!(this._nonterminals is null))
                return this._nonterminals;

            HashSet<string> nonterminals = new HashSet<string>();
            foreach (var pro in this.ProductionRules)
                nonterminals.Add(pro.Head);
            return this._nonterminals = nonterminals;
        }
        /// <summary>
        /// Compute terminal string labels from production rule
        /// </summary>
        /// <returns></returns>
        public HashSet<string> ComputeTerminals()
        {
            if (!(this._terminals is null))
                return this._terminals;
            var terminals = new HashSet<string>();
            var nonterminals = this.ComputeNonterminals();
            foreach (var token in this.ComputeToken())
            {
                if (!nonterminals.Contains(token))
                    terminals.Add(token);
            }
            return this._terminals = terminals;
        }
        public HashSet<string> ComputeToken()
        {
            if (!(this._tokens.Count == 0))
                return this._tokens;

            foreach (var Pro in ProductionRules)
            {
                this._tokens.Add(Pro.Head);
                foreach (var body in Pro.Body)
                {
                    this._tokens.Add(body);
                }
            }
            return this._tokens;
        }
        private string _OutputLanguage = "";
        public string OutputLanguage{get => _OutputLanguage;}
        /// <summary>
        /// Run the L-system program
        /// </summary>
        /// <param name="Language"></param>
        /// <param name="IterationCount"></param>
        /// <returns></returns>
        public bool Run(out string Language, int IterationCount = 1)
        {
            var Seed = 0;
            this.IterationCount = IterationCount;
            if (this._start is null)
                this.ComputeStartNonTerminal();
            _OutputLanguage = "";
            Language = "";

            ProductionRule SelectedRule = SelectedARule(this._start);

            var ResultBags = new List<string>(SelectedRule.Body);
            this.iterationCount = IterationCount;

            while (iterationCount > 0)
            {
                var TempBags = new List<string>();

                for (int i = 0; i < ResultBags.Count; i++)
                {
                    var Token = ResultBags[i];
                    if (this._nonterminals.Contains(Token))
                    {
                        var SubSelectedRule = SelectedARule(Token);
                        TempBags.AddRange(SubSelectedRule.Body);
                    }
                    else
                        TempBags.Add(Token);
                }
                ResultBags = TempBags;
                iterationCount--;
            }

            if (ResultBags.Count == 0)
                return false;

            Language = string.Join(" ", ResultBags);
            TokenResults = ResultBags;

            cmd += $"\n Finish running. \n iteration = {IterationCount}\n Rules = {string.Join("\n", ProductionRules.Select(x => x.ToString()))} \n Language = {Language} ";
            _OutputLanguage = Language;
            ProductionRule SelectedARule(string _Head)
            {
                var _rules = ProductionRules.Where(x => x.Head == _Head);
                if (_rules == null)
                    return null;
                if (_rules.ToList().Count == 1) return _rules.FirstOrDefault();
                else
                {
                    var Rand = new Random(Seed);
                    Seed++;
                    return _rules.ToList()[Rand.Next(_rules.ToList().Count)];
                }
            }

            return true;
        }
        /// <summary>
        /// If your Axiom is a rule that isn't defined in the production rule
        /// </summary>
        /// <param name="AxiomRule"></param>
        /// <param name="Language"></param>
        /// <param name="IterationCount"></param>
        /// <returns></returns>
        public bool Run(string AxiomRule, out string Language, int IterationCount = 1)
        {
            if (AxiomRule.Contains("="))
            {
                Language = "NULL";
                return false;
            }
            var Result = true;

            var Seed = 0;
            this.IterationCount = IterationCount;
            if (this._start is null)
                this.ComputeStartNonTerminal();
            _OutputLanguage = "";
            Language = "";

            var ResultBags = new List<string>(AxiomRule.Split(' ').Select(x => Tools.CleanSequence(x)));
            foreach (var AxiomToken in ResultBags)
            {
                if (!this._tokens.Contains(AxiomToken))
                    throw new Exception($"{AxiomToken} is defined in the production rules");
            }

            this.iterationCount = IterationCount;

            while (iterationCount > 0)
            {
                var TempBags = new List<string>();

                for (int i = 0; i < ResultBags.Count; i++)
                {
                    var Token = ResultBags[i];
                    if (this._nonterminals.Contains(Token))
                    {
                        var SubSelectedRule = SelectedARule(Token);
                        TempBags.AddRange(SubSelectedRule.Body);
                    }
                    else
                        TempBags.Add(Token);
                }
                ResultBags = TempBags;
                iterationCount--;
            }

            if (ResultBags.Count == 0)
                return false;

            Language = string.Join(" ", ResultBags);
            TokenResults = ResultBags;

            cmd += $"\n Finish running. \n iteration = {IterationCount}\n Rules = {string.Join("\n", ProductionRules.Select(x => x.ToString()))} \n Language = {Language} ";
            _OutputLanguage = Language;
            ProductionRule SelectedARule(string _Head)
            {
                var _rules = ProductionRules.Where(x => x.Head == _Head);
                if (_rules == null)
                    return null;
                if (_rules.ToList().Count == 1) return _rules.FirstOrDefault();
                else
                {
                    var Rand = new Random(Seed);
                    Seed++;
                    return _rules.ToList()[Rand.Next(_rules.ToList().Count)];
                }
            }

            return Result;
        }

        public override string ToString()
        {
            return cmd;
        }
    }
}