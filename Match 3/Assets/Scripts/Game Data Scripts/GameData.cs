using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mono.Data.Sqlite;
using UnityEngine.Analytics;
using System.Runtime.CompilerServices;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;

    public SaveData()
    {
        isActive = new bool[] { true, false, false, false };
        highScores = new int[4];
        stars = new int[4];
    }
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;
    public int userID;

    public string path = "URI=file:progress.db";

    private void CreateTable()
    {
        using (var connection = new SqliteConnection(path))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS progress (
                    UserID INTEGER PRIMARY KEY,
                    IsActive TEXT,
                    HighScores TEXT,
                    Stars TEXT
                )";
                command.ExecuteNonQuery();
            }
        }
    }

    void Awake()
    {
        userID = AccountManager.Instance.GetUserID();
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        CreateTable();
        Load();
    }

    public void Save()
    {
        using (var connection = new SqliteConnection(path))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT OR REPLACE INTO progress (UserID, IsActive, HighScores, Stars) VALUES (@UserID, @IsActive, @HighScores, @Stars)";
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@IsActive", string.Join(",", saveData.isActive));
                command.Parameters.AddWithValue("@HighScores", string.Join(",", saveData.highScores));
                command.Parameters.AddWithValue("@Stars", string.Join(",", saveData.stars));
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public void Load()
    {
        using (var connection = new SqliteConnection(path))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT IsActive, HighScores, Stars FROM progress WHERE UserID = @UserID";
                command.Parameters.AddWithValue("@UserID", userID); 
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        saveData.isActive = Array.ConvertAll(reader["IsActive"].ToString().Split(','), bool.Parse);
                        saveData.highScores = Array.ConvertAll(reader["HighScores"].ToString().Split(','), int.Parse);
                        saveData.stars = Array.ConvertAll(reader["Stars"].ToString().Split(','), int.Parse);
                    }
                    else
                    {
                        saveData = new SaveData();
                        Save();
                    }
                }
            }
            connection.Close();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }
}
