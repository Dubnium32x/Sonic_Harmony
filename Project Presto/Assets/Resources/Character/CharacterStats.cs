using UnityEngine;
using System.Collections.Generic;
using System;

public class CharacterStats {
    public float physicsScale = 1;
    Dictionary<string, object> stats = new Dictionary<string, object>();

    public bool ContainsKey(string key) => stats.ContainsKey(key);
    public float Get(string key) => GetRaw(key) * physicsScale;

    public float GetRaw(string key) {
        var val = stats[key];
        if (val is float f)
            return f;
        if (val is string s) 
            return GetRaw(s);
        if (val is Func<float> func) 
            return func.Invoke();
        if (val is Func<string> func1) 
            return GetRaw(func1.Invoke());

        return -Mathf.Infinity; // Should have a better fail case
    }

    public void Add(string key, object val) => stats[key] = val;

    public void Add(Dictionary<string, object> data) {
        foreach (var key in data.Keys)
            stats[key] = data[key];
    }
}