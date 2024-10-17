using System;
using System.Collections.Generic;
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

    public List<Sector> GetAdjacentSectors(string identifier)
    {
        List<Sector> adjacentSectors = new List<Sector>();

        // Get the current sector's row and column indices
        Sector currentSector = GetSector(identifier);
        if (currentSector == null)
        {
            Debug.LogError("Invalid sector identifier.");
            return adjacentSectors;
        }

        char colChar = identifier[0];
        int row = int.Parse(identifier.Substring(1));

        // Convert column letter to 0-based index
        int col = colChar - 'A';

        // Get adjacent sectors if they exist
        // Top (row - 1)
        if (row > 1)
        {
            string topIdentifier = $"{colChar}{row - 1}";
            adjacentSectors.Add(GetSector(topIdentifier));
        }

        // Bottom (row + 1)
        if (row < m_size)
        {
            string bottomIdentifier = $"{colChar}{row + 1}";
            adjacentSectors.Add(GetSector(bottomIdentifier));
        }

        // Left (col - 1)
        if (col > 0)
        {
            char leftColChar = (char)(colChar - 1);
            string leftIdentifier = $"{leftColChar}{row}";
            adjacentSectors.Add(GetSector(leftIdentifier));
        }

        // Right (col + 1)
        if (col < m_size - 1)
        {
            char rightColChar = (char)(colChar + 1);
            string rightIdentifier = $"{rightColChar}{row}";
            adjacentSectors.Add(GetSector(rightIdentifier));
        }

        // Return the list of adjacent sectors
        return adjacentSectors;
    }

    /// <summary>
    /// Converts a sector identifier (e.g., "A1") to a tuple representing the (x, y) coordinates.
    /// </summary>
    /// <param name="identifier">The sector identifier in the format "A1", "B2", etc.</param>
    /// <returns>A tuple (x, y) representing the coordinates, or (-1, -1) if the identifier is invalid.</returns>
    private (int x, int y) ConvertIdentifierToCoordinates(string identifier)
    {
        if (string.IsNullOrEmpty(identifier) || identifier.Length < 2)
        {
            Debug.LogError("Identifier must be in the format 'A1', 'B2', etc.");
            return (-1, -1);
        }

        // Extract column letter and row number
        char colChar = char.ToUpper(identifier[0]);
        if (!int.TryParse(identifier.Substring(1), out int row) || row < 1 || row > m_size)
        {
            Debug.LogError($"Invalid row number. Must be between 1 and {m_size}");
            return (-1, -1);
        }

        // Convert column letter to 0-based index
        int col = colChar - 'A';
        if (col < 0 || col >= m_size)
        {
            Debug.LogError($"Invalid column letter. Must be between A and {(char)('A' + m_size - 1)}");
            return (-1, -1);
        }

        // Convert to 0-based (x, y) coordinates
        int x = col;
        int y = row - 1;

        return (x, y);
    }

    /// <summary>
    /// Gets the coordinates (x, y) of a given sector using its identifier.
    /// </summary>
    /// <param name="sector">The sector to get the coordinates for.</param>
    /// <returns>A tuple (x, y) representing the coordinates, or (-1, -1) if the sector is invalid.</returns>
    public (int x, int y) GetSectorCoordinates(Sector sector)
    {
        if (sector == null)
        {
            Debug.LogError("Sector cannot be null.");
            return (-1, -1);
        }

        string identifier = sector.GetIdentifier();
        return ConvertIdentifierToCoordinates(identifier);
    }
}
