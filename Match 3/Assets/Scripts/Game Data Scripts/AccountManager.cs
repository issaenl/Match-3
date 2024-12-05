using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using System;


public class AccountManager : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;
    public InputField repeatField;
    public Text error;
    public GameObject back;

    private string path = "URI=file:accounts.db";

    // Start is called before the first frame update
    void Start()
    {
        CreateTable();
    }

    void CreateTable()
    {
        using (var connection = new SqliteConnection(path))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS accounts (id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "name TEXT NOT NULL, password TEXT NOT NULL)";
                command.ExecuteNonQuery();
            }          
        }
    }

    public void ClickRegistrate()
    {
        RegistrateAccount();
    }

    void RegistrateAccount()
    {
        string name = nameField.text;
        string password = passwordField.text;
        string repeat = repeatField.text;

        error.text = "";
        if (password != "" && name != "" && repeat != "")
        {
            if (CheckPassword(password, repeat) && CheckName(name))
            {
                using (var connection = new SqliteConnection(path))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO accounts (name, password) VALUES ('" + name + "', '" + password + "');";
                        command.ExecuteNonQuery();
                        error.text = "Регистрация успешна!";
                        error.color = Color.green;
                        back.SetActive(false);
                    }

                }
            }
        }
        else
        {
            error.text = "Не все поля заполнены!";
        }
    }

    bool CheckPassword(string password, string repeat)
    {
        if (password == repeat)
        {
            if (IsValidPassword(password))
            {
                return true;
            }
            else
            {
                error.text = "Пароль слишком легкий!";
                return false;
            }
        }
        else
        {
            error.text = "Пароль не совпадает!";
            return false;
        }
    }

    bool CheckName(string name)
    {
        if (!IsEmail(name))
        {
            if (!IsUsernameTaken(name))
            {
                return true;
            }
            else
            {
                error.text = "Имя уже занято!";
                return false;
            }
        }
        else
        {
            error.text = "Вы не можете использовать почту в качестве имени!";
            return false;
        }
    }

    bool IsValidPassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?!.*\s).{8,}$");
    }

    bool IsEmail(string input)
    {
        return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    bool IsUsernameTaken(string name)
    {
        using (var connection = new SqliteConnection(path))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM accounts WHERE name = @name";
                command.Parameters.AddWithValue("@name", name);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }
}
