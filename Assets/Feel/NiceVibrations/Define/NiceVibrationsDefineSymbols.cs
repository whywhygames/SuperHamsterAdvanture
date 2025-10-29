#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[InitializeOnLoad]
public class NiceVibrationsDefineSymbols
{
    public static readonly string[] Symbols = new string[]
    {
        "MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED"
    };

    static NiceVibrationsDefineSymbols()
    {
        // Получаем текущие define symbols для выбранной платформы
        var buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
        string scriptingDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget);
        List<string> scriptingDefinesList = scriptingDefinesString.Split(';').ToList();

        // Добавляем недостающие символы
        scriptingDefinesList.AddRange(Symbols.Except(scriptingDefinesList));

        // Применяем обновленный список
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, string.Join(";", scriptingDefinesList));
    }
}
#endif
