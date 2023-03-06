using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;


public class MySqlConnector
{ 
   
    // Global variables
    private static MySqlConnection dbConnection;
    private static MySqlConnector instance = null;

    public static MySqlConnector Instance
    {
        get
        {
            if (instance == null)
            {
                lock (typeof(MySqlConnector))
                {
                    if (instance == null)
                    {
                        instance = new MySqlConnector();
                    }
                }
            }
            return instance;
        }
    }


    public MySqlConnector()
    {
        openSqlConnection();
        //doQuery();
    }


    // Connect to database
    private static void openSqlConnection()
    {

        string connectionString = "Server=gliesewar.cdj2pfnkbmf4.ap-northeast-2.rds.amazonaws.com;" +

            "Database=sys;" +

            "User ID=hsson0822;" +

            "Password=gliesewar2023;" +

            "CharSet=utf8;" +

            "Pooling=false";

        dbConnection = new MySqlConnection(connectionString);

        dbConnection.Open();

        Debug.Log("Connected to database.");

    }

    // MySQL Query
    public MySqlDataReader doQuery(string sqlQuery)
    {

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        dbCommand.Dispose();
        dbCommand = null;

        return reader;
    }

    public int doNonQuery(string sqlQuery)
    {
        MySqlCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sqlQuery;
        int dbResult = dbCommand.ExecuteNonQuery();

        dbCommand.Dispose();
        dbCommand = null;

        return dbResult;
    }




    // Disconnect from database
    private static void closeSqlConnection()
    {
        dbConnection.Close();
        dbConnection = null;
        Debug.Log("Disconnected from database.");
    }

}