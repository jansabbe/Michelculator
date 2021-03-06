﻿using System;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Linq;

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
            for (int i = 0; i < 1000000000; i++)
            {
                var possibleSolution = CreatePossibleSolution(start, goal, maxMoves, possibleMoves);
                if (possibleSolution.Replay(start) == goal) 
                {
                    return possibleSolution;
                }
            }
            throw new Exception("dees kannanie?");
        }

        private static PlayedMoves CreatePossibleSolution(int start, int goal, int maxMoves, List<IMove> possibleMoves) 
        {
            var rand = new Random();
            var result = new PlayedMoves();
            for (int i = 0; i < maxMoves; i++) 
            {
                var move = possibleMoves[rand.Next(possibleMoves.Count)];
                result.Add(move);
            }
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
        private readonly ITestOutputHelper output;

        public SolverTest(ITestOutputHelper output)
        {
            this.output = output;
        }
        
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

            output.WriteLine($"Van {start} naar {goal} in {maxMoves}: {actual}");
        }
    }
}
