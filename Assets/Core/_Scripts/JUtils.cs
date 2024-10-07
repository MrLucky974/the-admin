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
}
