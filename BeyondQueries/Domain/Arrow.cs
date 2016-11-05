using System;

namespace BeyondQueries.Domain
{
    public class Arrow<TData>
    {
        public Arrow()
        {
            PointsTo = "";
            Rule = (_) => false;
        }

        public string PointsTo { get; set; }

        public Func<TData, bool> Rule { get; set; }
    }
}