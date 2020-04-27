using System;
using UnityEngine;

public struct PrefsPair
{
    #region static

    public const string IntString = "System.Int32";
    public const string FloatString = "System.Single";
    public const string StringString = "System.String";
    public const string BoolString = "System.Boolean";
    
    public const string SimpleIntString = "int";
    public const string SimpleFloatString = "float";
    public const string SimpleStringString = "string";
    public const string SimpleBoolString = "bool";

    #endregion

    public string Key { get; set; }

    public object Value { get; set; }

    public Type Type
    {
        get => Value.GetType();
    }

    public string SimpleTypeString
    {
        get
        {
            switch (Type.ToString())
            {
                case IntString:
                    return SimpleIntString;

                case StringString:
                    return SimpleStringString;

                case FloatString:
                    return SimpleFloatString;

                case BoolString:
                    return SimpleBoolString;
            }

            return string.Empty;
        }
    }

    public Color TypeColor
    {
        get
        {
            Color color = Color.cyan;

            switch (SimpleTypeString)
            {
                case SimpleIntString:
                    color = Color.cyan;
                    break;
                case SimpleStringString:
                    color = Color.green;
                    break;
                case SimpleFloatString:
                    color = Color.magenta;
                    break;
                case SimpleBoolString:
                    color = Color.red;
                    break;
            }

            return color;
        }
    }
}

