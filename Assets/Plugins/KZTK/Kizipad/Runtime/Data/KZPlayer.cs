using UnityEngine;
using System.Collections;

//model class for player(kids)
public class KZPlayer {
    public readonly int id;
    public readonly string nickname;
    public readonly int gender; //0: female, 1: male
    public readonly long birth;
    public readonly long lastLogin;
    public readonly string photoPath;

    public KZPlayer(int id, string nickname, int gender, 
            long birth, long lastLogin, string photoPath) {
        this.id = id;
        this.nickname = nickname;
        this.gender = gender;
        this.birth = birth;
        this.lastLogin = lastLogin;
        this.photoPath = photoPath;
    }
}
