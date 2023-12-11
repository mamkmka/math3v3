﻿using Match3PlusUltraDeluxEX;
using System;
using System.Collections.Generic;

namespace Match3v3.GameLogic
{
    public class GameGrid
    {
        private readonly Random _random = new Random();

 
        private readonly IFigure[,] _figures;
        private readonly int _gridSize;

        public GameGrid(int gridSize)
        {
            _gridSize = gridSize;
            _figures = new IFigure[_gridSize, _gridSize];
            RandomFill();
        }

        public IFigure GetFigure(Vector2 position) => _figures[position.X, position.Y];

        private void SwapFigures(int x1, int y1, int x2, int y2)
        {
            (_figures[x1, y1].Position, _figures[x2, y2].Position) = (_figures[x2, y2].Position, _figures[x1, y1].Position);
            (_figures[x1, y1], _figures[x2, y2]) = (_figures[x2, y2], _figures[x1, y1]);
        }

        public void SwapFigures(Vector2 firstPosition, Vector2 secondPosition) => SwapFigures(firstPosition.X, firstPosition.Y, secondPosition.X, secondPosition.Y);

        public bool TryMatchAll()
        {
            bool isMatched = false;
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    if (ExecuteMatch(new Vector2(i, j), ref _figures[i, j]))
                    {
                        isMatched = true;
                    }
                }
            }
            return isMatched;
        }

        public bool TryMatch(Vector2 firstPosition, Vector2 secondPosition)
        {
            var firstTry = ExecuteMatch(secondPosition, ref _figures[secondPosition.X, secondPosition.Y]);
            var secondTry = ExecuteMatch(firstPosition, ref _figures[firstPosition.X, firstPosition.Y]);
            return firstTry || secondTry;
        }

        public void PushFiguresDown(out List<Vector2> dropsFrom, out List<Vector2> dropsTo)
        {
            dropsFrom = new List<Vector2>();
            dropsTo = new List<Vector2>();
            for (int i = 0; i < _gridSize; i++)
            {
                int gap = 0;
                for (int j = _gridSize - 1; j >= 0; j--)
                {
                    if (_figures[i, j].IsNullObject)
                    {
                        gap++;
                    }
                    else
                    {
                        SwapFigures(i, j + gap, i, j);
                        dropsFrom.Add(new Vector2(i, j));
                        dropsTo.Add(new Vector2(i, j + gap));
                    }
                }
            }
        }

        public void RandomFill()
        {
            var figureTypes = Enum.GetValues(typeof(FigureType));
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    if (_figures[i, j] == null || _figures[i, j].IsNullObject)
                    {
                        var randomType = (FigureType)figureTypes.GetValue(_random.Next(figureTypes.Length));
                        _figures[i, j] = new BasicFigure(randomType, new Vector2(i, j));
                    }
                }
            }
        }

        private bool ExecuteMatch(Vector2 position, ref IFigure firstFigure)
        {
            var matchList = GetMatchList(position, firstFigure.Type);

            if (matchList.Count == 0)
                return false;

            if (Game.IsInitialized && !TrySetBonus(matchList, ref firstFigure))
            {
                matchList.Add(firstFigure);
            }

            foreach (var figure in matchList)
            {
                figure.Destroy(_figures);
            }
            return true;
        }

        private bool TrySetBonus(List<IFigure> match, ref IFigure figureToSet)
        {
        
            bool isEnoughForLine = match.Count == 3;
         
            return false;
        }

        private List<IFigure> GetMatchList(Vector2 position, FigureType type)
        {
            int horCounter = position.X + 1;
            int vertCounter = position.Y + 1;
            var verticalLine = new List<IFigure>();
            var horizontalLine = new List<IFigure>();
            while (horCounter < _gridSize)
            {
                if (_figures[horCounter, position.Y].Type != type)
                    break;
                horizontalLine.Add(_figures[horCounter, position.Y]);
                horCounter++;
            }
            while (vertCounter < _gridSize)
            {
                if (_figures[position.X, vertCounter].Type != type)
                    break;
                verticalLine.Add(_figures[position.X, vertCounter]);
                vertCounter++;
            }

            horCounter = position.X - 1;
            vertCounter = position.Y - 1;
            while (horCounter >= 0)
            {
                if (_figures[horCounter, position.Y].Type != type)
                    break;
                horizontalLine.Add(_figures[horCounter, position.Y]);
                horCounter--;
            }
            while (vertCounter >= 0)
            {
                if (_figures[position.X, vertCounter].Type != type)
                    break;
                verticalLine.Add(_figures[position.X, vertCounter]);
                vertCounter--;
            }

            if (verticalLine.Count < 2)
            {
                verticalLine.Clear();
            }
            if (horizontalLine.Count < 2)
            {
                horizontalLine.Clear();
            }

            verticalLine.AddRange(horizontalLine);
            return verticalLine;
        }
    }
}