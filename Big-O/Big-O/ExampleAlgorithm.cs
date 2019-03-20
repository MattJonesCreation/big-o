using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Big_O
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public int OperatorId { get; set; }
        public int RunId { get; set; }

        public Assignment(int id, int runId, int operatorId)
        {
            AssignmentId = id;
            RunId = runId;
            OperatorId = operatorId;
        }
    }

    public class Operator
    {
        public int OperatorId { get; set; }
        public string Name { get; set; }

        public Operator(int id)
        {
            OperatorId = id;
            Name = $"Operator {id}";
        }
    }

    public class OperatorAsset
    {
        public Operator Operator { get; set; }
        public List<Assignment> Assignments { get; set; }

        public OperatorAsset(Operator op)
        {
            Operator = op;
            Assignments = new List<Assignment>();
        }
    }

    public class Run
    {
        public int RunId { get; set; }
        public string RunDescription { get; set; }

        public Run(int id)
        {
            RunId = id;
            RunDescription = $"RunDescription: {id}";
        }
    }

    public class ExampleAlgorithm
    {
        private readonly List<Run> _runs;
        private readonly List<Operator> _operators;
        private readonly List<Assignment> _assignments; 

        public ExampleAlgorithm(int countRuns, int countOperators, int countAssignments)
        {
            Random rand = new Random();
            _runs = new List<Run>(); 
            _operators = new List<Operator>();
            _assignments = new List<Assignment>();
            // Build Runs, Operators, Assignments
            for (int i = 0; i < countRuns; i++){ _runs.Add(new Run(i)); }
            for (int i = 0; i < countOperators; i++) { _operators.Add(new Operator(i)); }
            for (int i = 0; i < countAssignments; i++) { _assignments.Add(new Assignment(i, rand.Next(0, countRuns-1), rand.Next(0, countOperators-1))); }
        }

        /// <summary>
        /// Inputs: 
        ///     1. _operators (list of operators), call it n 
        ///     2. _assignments (list of assignments), call it m 
        /// Outputs: 
        ///     Return back a list of OperatorAssets. Each OperatorAsset 
        ///     contains an Operator and his assignments for the day.
        /// </summary>
        public List<OperatorAsset> InefficientAlgorithm()
        {
            var retList = new List<OperatorAsset>();

            // What is the run time of this algorithm??
            foreach (Operator @operator in _operators)
            {
                var operatorAsset = new OperatorAsset(@operator);
                foreach (Assignment assignment in _assignments)
                {
                    if (assignment.OperatorId == @operator.OperatorId)
                    {
                        operatorAsset.Assignments.Add(assignment);
                    }
                }
                retList.Add(operatorAsset);
            }

            return retList;
        }

        /// <summary>
        /// Hiding the horrific run time with LINQ
        /// </summary>
        /// <returns></returns>
        public List<OperatorAsset> IneffiecientLinqAlgorithm()
        {
            var retList = new List<OperatorAsset>();

            foreach (Operator @operator in _operators)
            {
                var operatorAsset = new OperatorAsset(@operator);
                var assignments = _assignments.Where(a => a.OperatorId == @operator.OperatorId);
                operatorAsset.Assignments = assignments.ToList();
                retList.Add(operatorAsset);
            }

            return retList;
        }

        public List<OperatorAsset> MoreEfficientAlgorithm()
        {
            // Build an OperatorID -> List<Assignments> dictionary
            var opToAssignments = new Dictionary<int, List<Assignment>>();
            foreach (Assignment assignment in _assignments)
            {
                if (!opToAssignments.ContainsKey(assignment.OperatorId)) 
                    opToAssignments.Add(assignment.OperatorId, new List<Assignment>());

                opToAssignments[assignment.OperatorId].Add(assignment);
            }

            // Build the return list of operators
            var retList = new List<OperatorAsset>();
            foreach (Operator @operator in _operators)
            {
                var operatorAsset = new OperatorAsset(@operator);

                if (opToAssignments.ContainsKey(@operator.OperatorId))
                    operatorAsset.Assignments = opToAssignments[@operator.OperatorId];
                
                retList.Add(operatorAsset);
            }

            return retList;
        }

    }
}
