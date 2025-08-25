// JourneyData.cs
// Serializable container for storing the player's selected financial journey.

using System;

[Serializable]
public class JourneyData
{
    public string selectedJourney;  // "Savings", "Budgeting", or "Investing"
    public bool hasJourney;         // true if a journey has been chosen

    // Default constructor
    public JourneyData()
    {
        selectedJourney = "";
        hasJourney = false;
    }

    // Constructor with parameters
    public JourneyData(string journey, bool chosen = true)
    {
        selectedJourney = journey;
        hasJourney = chosen;
    }
}
