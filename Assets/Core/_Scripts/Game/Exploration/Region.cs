using System;
using UnityEngine;

/// <summary>
/// Data class defining a grid of sectors (tiles) containing resources.
/// </summary>
public class Region
{
    public const int DEFAULT_REGION_SIZE = 5;

    public static Region Generate()
    {
        var region = new Region(DEFAULT_REGION_SIZE);

        for (int i = 0; i < region.m_sectors.Length; i++)
        {
            region.m_sectors[i] = Sector.Generate(region.GetIdentifier(i));
        }

        region.m_sectors.Print();

        return region;
    }

    private const int MAX_SIZE = 26; // Limited by alphabet A-Z

    private readonly int m_size;
    private readonly Sector[] m_sectors;

    private Region(int size)
    {
        if (size <= 0 || size > MAX_SIZE)
            throw new ArgumentOutOfRangeException(nameof(size), $"Size must be between 1 and {MAX_SIZE}");

        m_size = size;
        m_sectors = new Sector[size * size];
    }

    public int GetSize()
    {
        return m_size * m_size;
    }

    public Sector[] GetSectors()
    {
        return m_sectors;
    }

    public Sector GetSector(string identifier)
    {
        if (string.IsNullOrEmpty(identifier) || identifier.Length < 2)
        {
            Debug.LogError("Identifier must be in format 'A1', 'B2', etc.");
            return null;
        }

        // Extract column letter and row number
        char colChar = char.ToUpper(identifier[0]);
        if (!int.TryParse(identifier.Substring(1), out int row) || row < 1 || row > m_size)
        {
            Debug.LogError($"Invalid row number. Must be between 1 and {m_size}");
            return null;
        }

        // Convert column letter to 0-based index
        int col = colChar - 'A';
        if (col < 0 || col >= m_size)
        {
            Debug.LogError($"Invalid column letter. Must be between A and {(char)('A' + m_size - 1)}");
            return null;
        }

        // Calculate index in the one-dimensional array
        int index = (row - 1) * m_size + col;
        return m_sectors[index];
    }

    public string GetIdentifier(int index)
    {
        if (index < 0 || index >= m_sectors.Length)
            throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {m_sectors.Length - 1}");

        int row = index / m_size + 1;        // 1-based row number
        int col = index % m_size;            // 0-based column index
        char colChar = (char)('A' + col);    // Convert to letter

        return $"{colChar}{row}";
    }
}
