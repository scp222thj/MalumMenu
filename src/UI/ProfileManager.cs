using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MalumMenu;

public static class ProfileManager
{
    public static readonly string ProfilesDirectory = Path.Combine(BepInEx.Paths.ConfigPath, "MalumProfiles");
    private static string currentProfileName = "Default";
    private static string currentProfilePath => Path.Combine(ProfilesDirectory, $"{currentProfileName}.txt");
    
    public static string CurrentProfileName
    {
        get => currentProfileName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                currentProfileName = SanitizeProfileName(value);
            }
        }
    }
    
    public static string CurrentProfilePath => currentProfilePath;

    private static readonly Dictionary<string, FieldInfo> ToggleFields = new();

    static ProfileManager()
    {
        // Populate toggle fields reflection map
        var fields = typeof(CheatToggles).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                ToggleFields[field.Name] = field;
            }
        }
        
        // Create profiles directory if it doesn't exist
        Directory.CreateDirectory(ProfilesDirectory);
    }

    private static string SanitizeProfileName(string name)
    {
        // Remove invalid filename characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(name);
        
        foreach (var c in invalidChars)
        {
            sanitized = sanitized.Replace(c.ToString(), "");
        }
        
        // Trim whitespace and limit length
        sanitized = sanitized.Trim().Substring(0, Math.Min(sanitized.Length, 50));
        
        // Default name if empty
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = "Default";
        }
        
        return sanitized;
    }

    public static List<string> GetAvailableProfiles()
    {
        var profiles = new List<string>();
        
        if (!Directory.Exists(ProfilesDirectory))
        {
            Directory.CreateDirectory(ProfilesDirectory);
            profiles.Add("Default");
            return profiles;
        }
        
        var files = Directory.GetFiles(ProfilesDirectory, "*.txt");
        
        foreach (var file in files)
        {
            var profileName = Path.GetFileNameWithoutExtension(file);
            if (!string.IsNullOrWhiteSpace(profileName))
            {
                profiles.Add(profileName);
            }
        }
        
        // Add default if no profiles exist
        if (profiles.Count == 0)
        {
            profiles.Add("Default");
        }
        
        profiles.Sort();
        return profiles;
    }

    public static void SaveCurrentProfile()
    {
        var profilePath = CurrentProfilePath;
        
        using var writer = new StreamWriter(profilePath);
        
        writer.WriteLine($"# MalumMenu Profile: {currentProfileName}");
        writer.WriteLine($"# Saved: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        writer.WriteLine("# Format: ToggleName = True/False = KeyCode.KEY");
        writer.WriteLine("# - List of supported keycodes: https://docs.unity3d.com/Packages/com.unity.tiny@0.16/api/Unity.Tiny.Input.KeyCode.html");
        writer.WriteLine("# - Setting a keybind is optional. Use KeyCode.None to not set a keybind");
        writer.WriteLine("# - Multiple toggles may have the same key, but multiple keys per toggle are NOT supported");
        writer.WriteLine("# - Keybinds are only applied after loading this profile by pressing 'Load Profile' in the Config menu");
        writer.WriteLine();
        
        foreach (var field in ToggleFields.Values)
        {
            CheatToggles.Keybinds.TryGetValue(field.Name, out var key);
            writer.WriteLine($"{field.Name} = {field.GetValue(null)} = KeyCode.{key}");
        }
        
        NotificationManager.Show($"Profile '{currentProfileName}' saved!");
    }

    public static void LoadProfile(string profileName)
    {
        var sanitized = SanitizeProfileName(profileName);
        var profilePath = Path.Combine(ProfilesDirectory, $"{sanitized}.txt");
        
        if (!File.Exists(profilePath))
        {
            NotificationManager.Show($"Profile '{sanitized}' not found!");
            return;
        }
        
        currentProfileName = sanitized;
        
        using var reader = new StreamReader(profilePath);
        
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            line = line.Trim();
            if (line.StartsWith("#")) continue;
            
            var parts = line.Split('=', 3);
            if (parts.Length < 2) continue;
            
            var name = parts[0].Trim();
            if (!ToggleFields.TryGetValue(name, out var field)) continue;
            
            if (bool.TryParse(parts[1].Trim(), out var boolVal))
            {
                field.SetValue(null, boolVal);
            }
            
            KeyCode key = KeyCode.None;
            if (parts.Length >= 3)
            {
                var keyPart = parts[2].Trim();
                if (keyPart.StartsWith("KeyCode."))
                {
                    keyPart = keyPart["KeyCode.".Length..];
                }
                
                if (!string.IsNullOrEmpty(keyPart) && System.Enum.TryParse<KeyCode>(keyPart, true, out var parsed))
                {
                    key = parsed;
                }
            }
            
            CheatToggles.Keybinds[name] = key;
        }
        
        NotificationManager.Show($"Profile '{currentProfileName}' loaded!");
    }

    public static void LoadCurrentProfile()
    {
        LoadProfile(currentProfileName);
    }

    public static void DeleteProfile(string profileName)
    {
        var sanitized = SanitizeProfileName(profileName);
        var profilePath = Path.Combine(ProfilesDirectory, $"{sanitized}.txt");
        
        if (!File.Exists(profilePath))
        {
            NotificationManager.Show($"Profile '{sanitized}' not found!");
            return;
        }
        
        File.Delete(profilePath);
        
        // If we deleted the current profile, switch to Default
        if (currentProfileName == sanitized)
        {
            currentProfileName = "Default";
        }
        
        NotificationManager.Show($"Profile '{sanitized}' deleted!");
    }

    public static void RenameProfile(string oldName, string newName)
    {
        var oldSanitized = SanitizeProfileName(oldName);
        var newSanitized = SanitizeProfileName(newName);
        
        var oldPath = Path.Combine(ProfilesDirectory, $"{oldSanitized}.txt");
        var newPath = Path.Combine(ProfilesDirectory, $"{newSanitized}.txt");
        
        if (!File.Exists(oldPath))
        {
            NotificationManager.Show($"Profile '{oldSanitized}' not found!");
            return;
        }
        
        if (File.Exists(newPath))
        {
            NotificationManager.Show($"Profile '{newSanitized}' already exists!");
            return;
        }
        
        File.Move(oldPath, newPath);
        
        // Update current profile name if we renamed the current one
        if (currentProfileName == oldSanitized)
        {
            currentProfileName = newSanitized;
        }
        
        NotificationManager.Show($"Profile renamed to '{newSanitized}'!");
    }

    public static void CreateNewProfile(string profileName)
    {
        var sanitized = SanitizeProfileName(profileName);
        var profilePath = Path.Combine(ProfilesDirectory, $"{sanitized}.txt");
        
        if (File.Exists(profilePath))
        {
            NotificationManager.Show($"Profile '{sanitized}' already exists!");
            return;
        }
        
        currentProfileName = sanitized;
        SaveCurrentProfile();
    }

    public static bool ProfileExists(string profileName)
    {
        var sanitized = SanitizeProfileName(profileName);
        var profilePath = Path.Combine(ProfilesDirectory, $"{sanitized}.txt");
        return File.Exists(profilePath);
    }
}
