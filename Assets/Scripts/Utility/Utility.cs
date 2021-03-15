using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    public static Utility Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetElementVisibility(CanvasGroup element, bool visibility)
    {
        element.blocksRaycasts = visibility;
        element.interactable = visibility;
        element.alpha = visibility ? 1 : 0;
    }

    public void SetElementInteractive(CanvasGroup element, bool interactive)
    {
        element.blocksRaycasts = interactive;
        element.interactable = interactive;
        element.alpha = interactive ? 1 : 0.3f;

        if (!element.GetComponent<Shadow>()) return;

        element.GetComponent<Shadow>().enabled = interactive;
    }

    public byte[] ToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public T FromByteArray<T>(byte[] data)
    {
        if (data == null)
            return default(T);
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(data))
        {
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }

    public double StandardDeviation(IEnumerable<double> values)
    {
        double avg = values.Average();
        return Math.Sqrt(values.Average(v=> (v-avg) * (v-avg)));
    }
}