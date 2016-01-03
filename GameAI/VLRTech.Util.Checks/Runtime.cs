using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLRTech.Util.Checks
{
    public class Runtime
    {
        public static void AssertThrow<T>(bool bCondition) where T : Exception, new()
        {
            if (bCondition)
            {
                return;
            }
            System.Diagnostics.Debug.Assert(bCondition);
            throw new T();
        }
        public static void AssertThrow(bool bCondition)
        {
            AssertThrow<InvalidOperationException>(bCondition);
        }
    }
}
