using Gen3.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Metalogic.DataUtil
{
    public static class LinqExtentions
    {
        public static Gen3DataList<TLeft> ToGen3List<TLeft>(this IEnumerable<TLeft> list) where TLeft : class
        {
            var retValue =  new Gen3DataList<TLeft>();
            foreach (var item in list)
            {
                retValue.Add(item);
            }
            return retValue;
        }


        public static IEnumerable<TResult> LeftOuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey,
            Func<TLeft, TRight, TResult> result)
        {
            return left.GroupJoin(right, leftKey, rightKey, (l, r) => new { l, r })
                 .SelectMany(
                     o => o.r.DefaultIfEmpty(),
                     (l, r) => new { lft = l.l, rght = r })
                 .Select(o => result.Invoke(o.lft, o.rght));
        }


        /// <summary>
        /// Return  outer join result.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="leftKey"></param>
        /// <param name="rightKey"></param>
        /// <param name="result"></param>
        /// <returns>
        ///  TResult.l = null if no match found from right
        ///  TResult.r = null if no match found from left
        /// </returns>
        public static IEnumerable<TResult> OuterJoin<TLeft, TRight, TKey, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft, TKey> leftKey, Func<TRight, TKey> rightKey,
            Func<TLeft, TRight, TResult> result)
        {
            //Style 1
            //return left.LeftOuterJoin(right, leftKey, rightKey, result.Invoke)
            //           .Union(right.LeftOuterJoin(left, rightKey, leftKey, (l, r) => new { lft = r, rght = l })
            //                       .Where(x => x.lft == null)
            //                       .Select(x => result.Invoke(x.lft, x.rght)));
            //style 1 does not work propertly with struct or premitive data type such as int
            //var ret2 = new[] { 1, 2 }.LeftRightOuterJoin(new[] { 2, 3 }, x => x, y => y, (x, y) => new { l = x, r = y }).ToList();
            //ret2 only returns new[] { 1, 2 }
            //problem due to "x.lft == null"


            //style 2
            var joinResult = left.Join(right, leftKey, rightKey, (l, r) => new { Left = l, Right = r });

            var leftsFoundInResult = new HashSet<TLeft>();
            foreach (var cur in joinResult.Where(cur => !leftsFoundInResult.Contains((TLeft) cur.Left)))
            {
                leftsFoundInResult.Add(cur.Left);
            }

            var leftsNotFoundInResult = left.Where(x => !leftsFoundInResult.Contains((TLeft) x)).Select(x => result.Invoke(x, default(TRight)));

            var rightsFoundInResult = new HashSet<TRight>();

            foreach (var cur in joinResult.Where(cur => !rightsFoundInResult.Contains((TRight) cur.Right)))
            {
                rightsFoundInResult.Add(cur.Right);
            }

            var rightsNotFoundInResult = right.Where(x => !rightsFoundInResult.Contains((TRight) x)).Select(x => result.Invoke(default(TLeft), x));

            return joinResult.Select(o => result.Invoke(o.Left, o.Right))
                .Union(leftsNotFoundInResult)
                .Union(rightsNotFoundInResult);
        }
    }
    
}
