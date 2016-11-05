using System.Collections.Generic;

namespace BeyondQueries.Domain
{
    public class Shape<TData, TResult>
    {
        public Shape()
        {
            Arrows = new List<Arrow<TData>>();
            Result = default(TResult);
        }

        public TResult Result { get; set; }

        public string Name { get; set; }

        public PropertySpecifier<TData> RequiredField { get; set; }

        public List<Arrow<TData>> Arrows { get; set; }
    }
}