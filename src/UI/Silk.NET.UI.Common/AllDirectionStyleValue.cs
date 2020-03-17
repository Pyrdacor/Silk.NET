using System;
using System.Text.RegularExpressions;

namespace Silk.NET.UI.Common
{
    public struct AllDirectionStyleValue<T> where T : struct
    {
        private static readonly Regex Pattern = new Regex(@"^([^ ]+) ?([^ ]+)? ?([^ ]+)? ?([^ ]+)?\s*$", RegexOptions.Compiled);

        public T Left;
        public T Right;
        public T Top;
        public T Bottom;

        public AllDirectionStyleValue(T all)
        {
            Left = Right = Top = Bottom = all;
        }
        
        public AllDirectionStyleValue(T topBottom, T leftRight)
        {
            Left = Right = leftRight;
            Top = Bottom = topBottom;
        }

        public AllDirectionStyleValue(T top, T leftRight, T bottom)
        {
            Left = Right = leftRight;
            Top = top;
            Bottom = bottom;
        }

        public AllDirectionStyleValue(T top, T right, T bottom, T left)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public static implicit operator AllDirectionStyleValue<T>(T value)
        {
            return new AllDirectionStyleValue<T>(value);
        }

        public static implicit operator AllDirectionStyleValue<T>(Tuple<T, T> value)
        {
            return new AllDirectionStyleValue<T>(value.Item1, value.Item2);
        }

        public static implicit operator AllDirectionStyleValue<T>(Tuple<T, T, T> value)
        {
            return new AllDirectionStyleValue<T>(value.Item1, value.Item2, value.Item3);
        }

        public static implicit operator AllDirectionStyleValue<T>(Tuple<T, T, T, T> value)
        {
            return new AllDirectionStyleValue<T>(value.Item1, value.Item2, value.Item3, value.Item4);
        }

        public static implicit operator AllDirectionStyleValue<T>(string value)
        {
            var match = Pattern.Match(value);

            if (match.Success)
            {
                if (match.Groups.Count >= 5 && !string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    return new AllDirectionStyleValue<T>
                    (
                        (T)Convert.ChangeType(match.Groups[1].Value, typeof(T)),
                        (T)Convert.ChangeType(match.Groups[2].Value, typeof(T)),
                        (T)Convert.ChangeType(match.Groups[3].Value, typeof(T)),
                        (T)Convert.ChangeType(match.Groups[4].Value, typeof(T))
                    );
                }
                else if (match.Groups.Count >= 4 && !string.IsNullOrEmpty(match.Groups[3].Value))
                {
                    return new AllDirectionStyleValue<T>
                    (
                        (T)Convert.ChangeType(match.Groups[1].Value, typeof(T)),
                        (T)Convert.ChangeType(match.Groups[2].Value, typeof(T)),
                        (T)Convert.ChangeType(match.Groups[3].Value, typeof(T))
                    );
                }
                else if (match.Groups.Count >= 3 && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    return new AllDirectionStyleValue<T>
                    (
                        (T)Convert.ChangeType(match.Groups[1].Value, typeof(T)),
                        (T)Convert.ChangeType(match.Groups[2].Value, typeof(T))
                    );
                }
                else if (match.Groups.Count >= 2 && !string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    return new AllDirectionStyleValue<T>
                    (
                        (T)Convert.ChangeType(match.Groups[1].Value, typeof(T))
                    );
                }
            }
            
            throw new FormatException("Invalid input value format.");
        }
    }
}