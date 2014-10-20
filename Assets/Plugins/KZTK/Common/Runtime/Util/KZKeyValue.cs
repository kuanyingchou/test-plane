using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// read string=string[;string=string]*
//2014.10.13  ken  initial version
public class KZKeyValue {
    private Dictionary<string, string> res = 
            new Dictionary<string, string>();

    public KZKeyValue(string input)
    {
        ReadText(input);
    }

    public string this[string i]
    {
        get { return res[i]; }
        //set { res[i] = value; } //read only for the time being
    }

    public int Count 
    {
        get { return res.Count; }
    }

    public bool GetBool(string key)
    {
        if(res.ContainsKey(key))
        {
            string val = res[key];
            if(val.ToLower().Equals("true"))
            {
                return true;
            } 
            else
            {
                return false;
            }
        } else
        {
            return false; //!
        }
    }

    private void ReadText(string input)
    {
        string[] pairs = input.Split(new char[] {';'});
        foreach(string p in pairs)
        {
            if(string.IsNullOrEmpty(p)) continue;
            if(!p.Contains("=")) continue; // no '='
            string[] kv = p.Trim().Split(new char[] {'='});
            if(kv.Length != 2) continue; 
            string key = kv[0].Trim();
            string val = kv[1].Trim();
            if(string.IsNullOrEmpty(key)) continue; // no key
            if(string.IsNullOrEmpty(val)) continue; // no value
            res[key] = val;
        }
    }
}
