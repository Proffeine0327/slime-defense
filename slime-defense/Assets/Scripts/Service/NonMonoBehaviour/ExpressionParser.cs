using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ExpressionParser
{
    public static string EvaluateExpression(this string str, Stats stats)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            if (FindExpression(str, i, out int end))
            {
                sb.Append(ExpressionToValue(str[i..(end + 1)], stats));
                i = end;
            }
            else
            {
                sb.Append(str[i]);
            }
        }
        return sb.ToString();
    }

    private static bool FindExpression(string str, int start, out int end)
    {
        end = start;
        if (str[start] != '{') return false;
        while (end < str.Length)
        {
            if (str[end] == '}') break;
            end++;
        }
        return true;
    }

    private static string ExpressionToValue(string expression, Stats stats)
    {
        var exp = expression[1..(expression.Length - 1)];
        foreach (var match in new Regex(@"[a-zA-Z]+").Matches(exp))
        {
            var tostr = match.ToString();
            if (Enum.TryParse<Stats.Key>(tostr, out var result))
            {
                var value = stats.GetStat(result);
                exp = exp.Replace(tostr, value.ToString());
            }
        }
        if (ExpressionEvaluator.Evaluate(exp, out float evaluated))
            return evaluated.ToString();

        return expression;
    }
}
