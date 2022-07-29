using System;
using System.Collections.Generic;
using System.Linq;
using CommonResources.Game;

namespace HanamikojiMonoGameClient;

// TODO: can be unit tested
public static class GameDataDiffersProvider
{
    
    private static void GetDifferenceBetweenLists<T>(
        List<T> previousList, 
        List<T> currentList, 
        out List<T> missingData,
        out List<T> addedData,
        Func<T, T, bool> compareFunc)
    {
        missingData = new List<T>();
        addedData = new List<T>();

        foreach (var obj in currentList)
        {
            if(!previousList.Exists(x => compareFunc(obj, x))) addedData.Add(obj);
        }

        foreach (var obj in previousList)
        {
            if(!currentList.Exists(x => compareFunc(obj, x))) missingData.Add(obj);
        }
    }
}

