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

        // Generate enemies (between 1 and 3)
        var rng = GameManager.RNG;
        int enemyCount = rng.Next(1, 4); // Generate 1 to 3 enemies
        for (int i = 0; i < enemyCount; i++)
        {
            // Create an enemy with a random strength (1 to 10)
            int strength = JRandom.RollDice(4, 1, 6, rng);

            // Assign the enemy to a random sector
            int randomIndex = rng.Next(region.m_sectors.Length);
            string sectorIdentifier = region.GetIdentifier(randomIndex);
            var enemy = new Enemy(strength, sectorIdentifier);

            // Add the enemy to the region's list and the sector
            region.m_enemies.Add(enemy);
            region.m_sectors[randomIndex].SetEnemy(enemy);
        }

        region.m_sectors.Print();

        return region;
    }

    private const int MAX_SIZE = 26; // Limited by alphabet A-Z

    private readonly int m_size;
    private readonly Sector[] m_sectors;
    private readonly List<Enemy> m_enemies = new List<Enemy>();

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

    public List<Enemy> GetEnemies()
    {
        return m_enemies;
    }

    /// <summary>
    /// Moves an enemy from its current sector to another sector.
    /// </summary>
    /// <param name="enemy">The enemy to move.</param>
    /// <param name="newSectorIdentifier">The identifier of the target sector.</param>
    /// <returns>True if the move was successful, false otherwise.</returns>
    public bool MoveEnemy(Enemy enemy, string newSectorIdentifier)
    {
        if (enemy == null || string.IsNullOrEmpty(newSectorIdentifier))
        {
            Debug.LogError("Enemy or target sector identifier is invalid.");
            return false;
        }

        // Find the current sector of the enemy
        Sector currentSector = GetSector(enemy.GetLocation());
        if (currentSector == null)
        {
            Debug.LogError("Current sector not found.");
            return false;
        }

        // Find the target sector
        Sector targetSector = GetSector(newSectorIdentifier);
        if (targetSector == null)
        {
            Debug.LogError("Target sector not found.");
            return false;
        }

        // Move the enemy to the new sector
        currentSector.RemoveEnemy(); // Clear enemy from the current sector
        enemy.SetLocation(newSectorIdentifier); // Update the enemy's location
        targetSector.SetEnemy(enemy); // Assign enemy to the target sector

        return true;
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
}
