﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class JUtils
{
    public static void Print<T>(this T[] array)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < array.Length; i++)
        {
            sb.Append(array[i].ToString());
            sb.Append(i < array.Length - 1 ? ", " : "");
        }
        sb.Append("]");
        Debug.Log(sb);
    }

    public static void Print<T>(this List<T> list)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < list.Count; i++)
        {
            sb.Append(list[i].ToString());
            sb.Append(i < list.Count - 1 ? ", " : "");
        }
        sb.Append("]");
        Debug.Log(sb);
    }

    public static string GenerateTextSlider(int value, int min, int max, int count = 5)
    {
        count = Mathf.Max(count, 0);

        StringBuilder sb = new StringBuilder();

        // Calculate the fraction of the range covered by the current value
        float fraction = Mathf.InverseLerp(min, max, value);
        int filledCount = Mathf.RoundToInt(fraction * count);

        sb.Append('[');
        for (int i = 0; i < count; i++)
        {
            if (i < filledCount)
            {
                sb.Append('■');
            }
            else
            {
                sb.Append('□');
            }
        }
        sb.Append(']');

        return sb.ToString();
    }
}
