using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;

namespace Harbour.RedisTempData.Test
{
    public static class MockExtensions
    {
        public static IReturnsResult<T> ReturnsInOrder<T, TResult>(this ISetup<T, TResult> setup, params TResult[] results) where T : class
        {
            return setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}
