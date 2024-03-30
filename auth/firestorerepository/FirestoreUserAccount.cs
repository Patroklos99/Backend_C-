using Google.Cloud.Firestore;

namespace Project.auth.firestorerepository;

[FirestoreData]
public class FirestoreUserAccount {
    public FirestoreUserAccount(string username, string encodedPassword)
    {
        username = username;
        encodedPassword = encodedPassword;
    }

    public FirestoreUserAccount()
    {
    }

    [FirestoreProperty("username")] public string username { get; set; }

    [FirestoreProperty("encodedPassword")] public string _encodedPassword { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;
        var other = (FirestoreUserAccount)obj;
        return username == other.username && _encodedPassword == other._encodedPassword;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(username, _encodedPassword);
    }

    public override string ToString()
    {
        return $"FirestoreUserAccount{{username='{username}', encodedPassword='{_encodedPassword}'}}";
    }

    public string getUsername()
    {
        return username;
    }

    public string getEncodedPassword()
    {
        return _encodedPassword;
    }
}