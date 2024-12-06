using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class AccountManager : MonoBehaviour
{
    public InputField nameField;
    public InputField passwordField;
    public InputField repeatField;
    public InputField loginField;
    public InputField loginPasswordField;
    public Text errorRegistration;
    public Text errorAuthenticate;
    public GameObject back;
    public GameObject registrationPanel;
    public GameObject authotizationPanel;
    public bool isRegister = false;

    private int userID;

    private string path = "URI=file:accounts.db";

    public static AccountManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetUserID()
    {
        return userID;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateTable();
        if(!isRegister)
        {
            back.SetActive(true);
        }
    }

    void CreateTable()
    {
        using (var connection = new SqliteConnection(path))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS accounts (" +
                    "id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "name TEXT NOT NULL, " +
                    "password TEXT NOT NULL)";
                command.ExecuteNonQuery();
            }          
        }
    }

    public void ClickRegistrate()
    {
        RegistrateAccount();
    }

    public void ClickAuthenticate()
    {
        AuthenticateAccount();
    }

    public void ClickGoToAuthorization()
    {
        registrationPanel.SetActive(false);
        authotizationPanel.SetActive(true);
    }

    public void ClickGoToRegistration()
    {
        authotizationPanel.SetActive(false);
        registrationPanel.SetActive(true);
    }

    void RegistrateAccount()
    {
        string name = nameField.text;
        string password = passwordField.text;
        string repeat = repeatField.text;

        errorRegistration.text = "";
        if (password != "" && name != "" && repeat != "")
        {
            if (CheckPassword(password, repeat) && CheckName(name))
            {
                using (var connection = new SqliteConnection(path))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO accounts (name, password) VALUES (@name, @password);";
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@password", password);
                        command.ExecuteNonQuery();
                        command.CommandText = "SELECT last_insert_rowid();";
                        userID = Convert.ToInt32(command.ExecuteScalar());
                        errorRegistration.text = "Регистрация успешна!";
                        errorRegistration.color = Color.green;
                        isRegister = true;
                        back.SetActive(false);
                    }

                }
            }
        }
        else
        {
            errorRegistration.text = "Не все поля заполнены!";
        }
    }

    void AuthenticateAccount()
    {
        string loginName = loginField.text;
        string loginPassword = loginPasswordField.text;

        errorAuthenticate.text = "";
        if (loginName != "" && loginPassword != "")
        {
            using (var connection = new SqliteConnection(path))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM accounts WHERE name = @name AND password = @password";
                    command.Parameters.AddWithValue("@name", loginName);
                    command.Parameters.AddWithValue("@password", loginPassword);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    command.CommandText = "SELECT id FROM accounts WHERE name = @name AND password = @password";
                    var result = command.ExecuteScalar();

                    if (count > 0)
                    {
                        userID = Convert.ToInt32(result);
                        errorAuthenticate.text = "Авторизация успешна!";
                        errorAuthenticate.color = Color.green;
                        isRegister = true;
                        back.SetActive(false);
                    }
                    else
                    {
                        errorAuthenticate.text = "Неверное имя пользователя или пароль!";
                    }
                }
            }
        }
        else
        {
            errorAuthenticate.text = "Не все поля заполнены!";
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
                errorRegistration.text = "Пароль слишком легкий!";
                return false;
            }
        }
        else
        {
            errorRegistration.text = "Пароль не совпадает!";
            return false;
        }
    }

    bool CheckName(string name)
    {
        if (!IsEmail(name))
        {
            if (!IsTakenName(name))
            {
                return true;
            }
            else
            {
                errorRegistration.text = "Имя уже занято!";
                return false;
            }
        }
        else
        {
            errorRegistration.text = "Вы не можете использовать почту в качестве имени!";
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

    bool IsTakenName(string name)
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
