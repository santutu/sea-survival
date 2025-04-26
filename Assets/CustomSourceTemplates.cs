using JetBrains.Annotations;

using UnityEngine;

  

public static class CustomSourceTemplates

{

    [SourceTemplate]

    public static void log<T>(this T x)

    {

        Debug.Log(x); //$ $END$

    }

}