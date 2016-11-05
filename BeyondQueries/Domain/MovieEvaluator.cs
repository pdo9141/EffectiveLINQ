using System;
using System.Linq;
using BeyondQueries.Models;

namespace BeyondQueries.Domain
{
    public class MovieEvaluator
    {
        public bool IsValid(Movie movie)
        {
            Func<Movie, bool>[] rules =
            {
                m => String.IsNullOrEmpty(m.Title),
                m => m.Length < 60 || m.Length > 400,
                m => m.ReleaseDate.Value.Year < 1903
            };

            return rules.All(rule => rule(movie) == false);
        }
    }
}