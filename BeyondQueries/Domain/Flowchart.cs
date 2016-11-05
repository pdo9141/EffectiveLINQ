//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;

//namespace BeyondQueries.Domain
//{
//    public class Flowchart<TData, TResult>
//    {
//        public List<Shape<TData, TResult>> Shapes { get; set; }

//        public Flowchart()
//        {
//            Shapes = new List<Shape<TData, TResult>>();
//        }

//        public void Validate()
//        {
//            CheckForInvalidDestinations();
//            CheckForDuplicateNames();
//        }

//        public EvaluationResults<TData, TResult> Evaluate(TData data)
//        {
//            var currentShape = Shapes[0];
//            var visitedShapes = new List<Shape<TData, TResult>> { currentShape };
//            var currentArrow = currentShape.Arrows.FirstOrDefault(Arrow => Arrow.Rule(data));

//            while (currentArrow != null)
//            {
//                currentShape = Shapes.Where(shape => shape.Name.Equals(currentArrow.PointsTo)).Single();
//                visitedShapes.Add(currentShape);
//                currentArrow = currentShape.Arrows.FirstOrDefault(arrow => arrow.Rule(data));
//            }

//            return ComputeEvaluationResults(visitedShapes);
//        }

//        private EvaluationResults<TData, TResult> ComputeEvaluationResults(List<Shape<TData, TResult>> visitedShapes)
//        {
//            var results = new EvaluationResults<TData, TResult>();
//            var lastShape = visitedShapes[visitedShapes.Count - 1];
//            results.Result = lastShape.Result;
//            results.RequiredFields = visitedShapes.Where(s => s.RequiredField != null)
//                .Select(s => s.RequiredField)
//                .Distinct(PropertySpecifier<TData>.Comparer)
//                .ToList();
//        }

//        private void CheckForDuplicateNames()
//        {
//            var duplicateShapes = Shapes.GroupBy(s => s.Name).Where(g => g.Count() > 1);            
//            if (duplicateShapes.Count() > 0)
//            {
//                string message = "The following shape names are duplicated: " +
//                    duplicateShapes.Aggregate(new StringBuilder(), (sb, s) => sb.Append(s.Name));

//                throw new InvalidOperationException(message);
//            }
//        }

//        private void CheckForInvalidDestinations()
//        {
//            var names = Shapes.Select(s => s.Name);
//            var problemTransitions = Shapes.SelectMany(s => s.Arrows)
//                                           .Where(t => !names.Contains(t.PointsTo));

//            if (problemTransitions.Count() > 0)
//            {
//                string message = "The following destination names are invalid: " +
//                    problemTransitions.Aggregate(new StringBuilder(), (sb, t) => sb.Append(t.PointsTo));

//                throw new InvalidOperationException(message);
//            }
//        }
//    }
//}