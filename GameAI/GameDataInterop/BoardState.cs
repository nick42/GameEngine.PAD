using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataInterop
{
    public enum BoardElementBaseType
    {
        Unknown,
        Gem_Red,
        Gem_Blue,
        Gem_Green,
        Gem_Light,
        Gem_Dark,
        Gem_Health,
        Token_Blocker,
        Token_Poison,
        Transitory_Empty,
        OutOfBounds,
    }

    public class CellIndex
    {
        public int m_nRow;
        public int m_nColumn;
    }

    public class BoardElement
    {
        public CellIndex m_CellIndex;
        public BoardElementBaseType m_eBoardElementBaseType;

        public bool IsOutOfBounds()
        {
            return (m_eBoardElementBaseType == BoardElementBaseType.OutOfBounds);
        }
        public bool IsEmpty()
        {
            return (m_eBoardElementBaseType == BoardElementBaseType.Transitory_Empty);
        }
    }

    public class BoardState
    {
        protected static BoardElement m_oBoardElement_OutOfBounds = new BoardElement
        {
            m_CellIndex = new CellIndex { m_nColumn = -1, m_nRow = -1 },
            m_eBoardElementBaseType = BoardElementBaseType.OutOfBounds,
        };

        public BoardElement[,] m_oBoardElementArray;

        public void InitializeEmptyBoard_DefaultSize()
        {
            m_oBoardElementArray = new BoardElement[6, 5];
            for (var nColumn = m_oBoardElementArray.GetLowerBound(0); nColumn <= m_oBoardElementArray.GetUpperBound(0); ++nColumn)
            {
                for (var nRow = m_oBoardElementArray.GetLowerBound(1); nRow <= m_oBoardElementArray.GetUpperBound(1); ++nRow)
                {
                    m_oBoardElementArray[nColumn, nRow] = new BoardElement
                    {
                        m_CellIndex = new CellIndex { m_nColumn = nColumn, m_nRow = nRow },
                        m_eBoardElementBaseType = BoardElementBaseType.Transitory_Empty,
                    };
                }
            }
        }

        public int GetNumColumns()
        {
            return m_oBoardElementArray.GetUpperBound(0) + 1;
        }
        public int GetNumRows()
        {
            return m_oBoardElementArray.GetUpperBound(1) + 1;
        }

        public BoardElement GetBoardElementByCellIndex(CellIndex oCellIndex)
        {
            if (oCellIndex.m_nColumn < m_oBoardElementArray.GetLowerBound(0))
            {
                return m_oBoardElement_OutOfBounds;
            }
            if (oCellIndex.m_nColumn > m_oBoardElementArray.GetUpperBound(0))
            {
                return m_oBoardElement_OutOfBounds;
            }
            if (oCellIndex.m_nRow < m_oBoardElementArray.GetLowerBound(1))
            {
                return m_oBoardElement_OutOfBounds;
            }
            if (oCellIndex.m_nRow > m_oBoardElementArray.GetUpperBound(1))
            {
                return m_oBoardElement_OutOfBounds;
            }

            return m_oBoardElementArray[oCellIndex.m_nColumn, oCellIndex.m_nRow];
        }
    }
}
