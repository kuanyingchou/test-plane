using UnityEngine;
using System.Collections;

[System.Serializable]
public class KZSize{
    public float width;
    public float height;
    public static KZSize zero = new KZSize( 0, 0 );

    public KZSize(float w, float h){
        width = w;
        height = h;
    }

    public KZSize(KZSize that) {
        width = that.width;
        height = that.height;
    }

    public override bool Equals(object obj)
    {
        KZSize that = obj as KZSize;
        if (that == null)
            return false;
        else
            return this.width == that.width && this.height == that.height;
    }

    public override int GetHashCode()
    { 
        int hash = 42;
        hash = hash * 13 + width.GetHashCode();
        hash = hash * 13 + height.GetHashCode();
        return hash;
    }

    public static bool operator ==(KZSize c1, KZSize c2){
        return c1.Equals(c2); 
    }

    public static bool operator !=(KZSize c1, KZSize c2){
        return !( c1 == c2 ) ; 
    }
} 
