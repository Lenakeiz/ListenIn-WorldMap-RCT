using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICsvReader<T> {

    // Property signatures:
    int CurrentStep
    {
        get;
    }

    IEnumerable<T> ParseCsv(string path, bool loadFromResources);
}

public interface ICsvWriter<T> {

    void WriteCsv(string path,string filename, IEnumerable<T> listToWrite);

}