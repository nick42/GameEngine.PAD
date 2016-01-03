using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataInterop.GameState
{
    public class BoardElementInMatch
    {
        public BoardElement m_oBoardElement;
    }

    public class BoardGemMatch
    {
        public List<BoardElementInMatch> m_oMatchElementList = new List<BoardElementInMatch>();
        public Dictionary<int, BoardElementInMatch> m_oCompositeIdentifierToBoardElementInMatchDict = new Dictionary<int, BoardElementInMatch>();

        public BoardElementBaseType m_eGemMatchType;

        public int m_nCascadeIteration;
        public int m_nSequenceWithinIteration;

        public void AddBoardElementToMatch(BoardElement oBoardElement)
        {
            if (m_oCompositeIdentifierToBoardElementInMatchDict.ContainsKey(oBoardElement.m_CellIndex.GetCompositeIdentifier()))
            {
                return;
            }

            BoardElementInMatch oBoardElementInMatch = new BoardElementInMatch { m_oBoardElement = oBoardElement };
            m_oMatchElementList.Add(oBoardElementInMatch);
            m_oCompositeIdentifierToBoardElementInMatchDict.Add(oBoardElement.m_CellIndex.GetCompositeIdentifier(), oBoardElementInMatch);
        }
        public bool IsBoardElementInMatch(BoardElement oBoardElement)
        {
            return m_oCompositeIdentifierToBoardElementInMatchDict.ContainsKey(oBoardElement.m_CellIndex.GetCompositeIdentifier());
        }

        public int GetBasePointValue()
        {
            return 0;
        }
    }

    public class BoardMoveResult
    {
        public List<BoardGemMatch> m_oBoardMatchList_PreCascade;
        public List<BoardGemMatch> m_oBoardMatchList_PostCascade;

        public int m_nPointTotal;
    }
}
