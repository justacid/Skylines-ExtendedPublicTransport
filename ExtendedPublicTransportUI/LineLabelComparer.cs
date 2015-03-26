using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EPTUI
{
    public class LineLabelComparer : Comparer<GameObject>
    {
        public override int Compare(GameObject x, GameObject y)
        {
            var lhs = x.GetComponent<UITransportLineRow>().LineName;
            var rhs = y.GetComponent<UITransportLineRow>().LineName;

            if (lhs == rhs)
                return 0;

            var tokensLhs = Regex.Split(lhs.Replace(" ", ""), "([0-9]+)");
            var tokensRhs = Regex.Split(rhs.Replace(" ", ""), "([0-9]+)");

            for (var i = 0; i < tokensLhs.Length && i < tokensRhs.Length; ++i)
            {
                if (tokensLhs[i] == tokensRhs[i]) 
                    continue;

                int valueLhs;
                if (!int.TryParse(tokensLhs[i], out valueLhs))
                    return String.Compare(tokensLhs[i], tokensRhs[i], StringComparison.OrdinalIgnoreCase);

                int valueRhs;
                if (!int.TryParse(tokensRhs[i], out valueRhs))
                    return String.Compare(tokensLhs[i], tokensRhs[i], StringComparison.OrdinalIgnoreCase);

                return valueLhs.CompareTo(valueRhs);
            }

            if (tokensLhs.Length < tokensRhs.Length)
                return -1;
            if (tokensLhs.Length > tokensRhs.Length)
                return 1;

            return 0;
        }
    }
}