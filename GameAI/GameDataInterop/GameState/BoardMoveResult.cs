using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataInterop.GameState
{
    public class BoardGemMatch
    {
        public List<BoardElement> m_oMatchElementList;

        public int m_nCascadeIteration;
        public int m_nSequenceWithinIteration;

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
