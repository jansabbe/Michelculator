using System;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace calculate
{

    public interface IMove
    {
        int Execute(int number);
    }

    class Divide : IMove 
    {
        private int _divisor;
        public Divide(int divisor)
        {
            _divisor = divisor;
        }

        public int Execute(int number) {
            return number / _divisor;
        }
        
        public override string ToString() {
            return $"Divide({_divisor})";
        }
    }

    public class Add : IMove 
    {
        private int _addition;
        public Add(int addition)
        {
            _addition = addition;
        }

        public int Execute(int number) {
            return number + _addition;
        }
        
        public override string ToString() {
            return $"Add({_addition})";
        }
    }

    public class Replace : IMove 
    {
        private int _searchString;
        private int _replacement;
        
        public Replace(int searchString, int replacement)
        {
            _searchString = searchString;
            _replacement = replacement;
        }

        public int Execute(int number) {
            var numAsString = number.ToString().Replace(_searchString.ToString(), _replacement.ToString());
            return int.Parse(numAsString);
        }

        public override string ToString() 
        {
            return $"Replace({_searchString}, {_replacement})";
        }
    }

    public class PlayedMoves
    {
        private List<IMove> _moves = new List<IMove>();

        public int NumberOfMoves => _moves.Count;

        public void InsertAtStart(IMove move) 
        {
            _moves.Insert(0, move);
        }

        public void Add(IMove move) 
        {
            _moves.Add(move);
        }

        public int Replay(int start) 
        {

            return _moves.Aggregate(start, (result, move) => move.Execute(result));
        }

        public override string ToString() {
            return string.Join(", ", _moves);
        }
    }

    public static class Solver 
    {
        public static PlayedMoves Solve(int start, int goal, int maxMoves, List<IMove> possibleMoves) 
        {
            if (maxMoves == 1) 
            {
                return CreateSolutionInOneStep(start, goal, possibleMoves);
            }
            foreach(var possibleFirstMove in possibleMoves)
            {
                var afterFirstMove = possibleFirstMove.Execute(start);
                var solutionAfterFirstMove = Solve(afterFirstMove, goal, maxMoves - 1, possibleMoves);
                if (solutionAfterFirstMove != null) {
                    solutionAfterFirstMove.InsertAtStart(possibleFirstMove);
                    return solutionAfterFirstMove;
                } 
            }
            return null;
        }

        private static PlayedMoves CreateSolutionInOneStep(int start, int goal, List<IMove> possibleMoves)
        {
            var solvingMove = possibleMoves.FirstOrDefault(t => t.Execute(start) == goal);
            if (solvingMove == null) return null;
            var result = new PlayedMoves();
            result.Add(solvingMove);
            return result;

        }

    }

    public class PlayedMovesTest
    {
        [Fact]
        public void Replay() 
        {
            var playedMoves = new PlayedMoves();
            playedMoves.Add(new Divide(3));

            Assert.Equal(3, playedMoves.Replay(9));
        }

        [Fact]
        public void ReplayWithMultiple() 
        {
            var playedMoves = new PlayedMoves();
            playedMoves.Add(new Divide(3));
            playedMoves.Add(new Add(4));
            
            Assert.Equal(7, playedMoves.Replay(9));
        }
    }

    public class DivideTest 
    {
        [Theory]
        [InlineData(4, 2, 2)]
        [InlineData(6, 2, 3)]
        [InlineData(11, 2, 5)]
        public void CanDivide(int deeltal, int deler, int resultaat)
        {
            var divider = new Divide(deler);
            var result = divider.Execute(deeltal);
            Assert.Equal(resultaat, result);
        }
    }

    public class AddTest 
    {
        [Theory]
        [InlineData(4, 2, 6)]
        [InlineData(6, 2, 8)]
        [InlineData(11, 2, 13)]
        public void CanDivide(int add, int add2, int resultaat)
        {
            var addition = new Add(add2);
            var result = addition.Execute(add);
            Assert.Equal(resultaat, result);
        }
    }


    public class ReplaceTest 
    {
        [Fact]
        public void CanReplace()
        {
            var replace = new Replace(4, 2);
            var result = replace.Execute(343414);
            Assert.Equal(323212, result);
        }
    }


    public class SolverTest
    {

        public static IEnumerable<object[]> CanSolveWithMultipleMovesTestData()
        {
            yield return new object[] { 6, 3, 1, new List<IMove> {new Divide(2)} };
            yield return new object[] { 1, 2, 1, new List<IMove> {new Divide(2), new Add(3), new Divide(3), new Add(1)} };
            yield return new object[] { 1, 3, 2, new List<IMove> {new Divide(2), new Add(3), new Divide(3), new Add(1)} };
            yield return new object[] { 11, 29, 5, new List<IMove> {new Divide(2), new Add(3), new Replace(1, 2), new Replace(2, 9)} };
            
        }

        [Theory]
        [MemberData(nameof(CanSolveWithMultipleMovesTestData))]
        public void CanSolveWithMultipleMoves(int start, int goal, int maxMoves, List<IMove> possibleMoves)
        {
            var actual = Solver.Solve(start, goal, maxMoves, possibleMoves);
            Assert.Equal(goal, actual.Replay(start));
            Assert.Equal(maxMoves, actual.NumberOfMoves);
        }
    }
}
