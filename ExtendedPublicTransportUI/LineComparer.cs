using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EPTUI
{
	public class LineComparer : Comparer<GameObject>
	{
		private string _sortFieldName;
		
		public LineComparer(string sortFieldName="LineName") : base()
		{
			_sortFieldName = sortFieldName;
		}

		public override int Compare(GameObject x, GameObject y)
		{

			PropertyInfo xproperty = x.GetComponent<UITransportLineRow>().GetType().GetProperty(_sortFieldName);
			PropertyInfo yproperty = y.GetComponent<UITransportLineRow>().GetType().GetProperty(_sortFieldName);
			var lhs = xproperty.GetValue(x.GetComponent<UITransportLineRow>(), null);
			var rhs = yproperty.GetValue(y.GetComponent<UITransportLineRow>(), null);

			if (lhs == rhs)
				return 0;

			var tokensLhs = Regex.Split(lhs.ToString().Replace(" ", ""), "([0-9]+)");
			var tokensRhs = Regex.Split(rhs.ToString().Replace(" ", ""), "([0-9]+)");

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