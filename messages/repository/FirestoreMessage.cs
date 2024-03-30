using Google.Cloud.Firestore;

namespace Project.messages.repository;

[FirestoreData]
public class FirestoreMessage {
    public FirestoreMessage(string username, Timestamp timestamp, string text, string imageUrl)
    {
        this.username = username;
        this.timestamp = timestamp;
        this.text = text;
        this.imageUrl = imageUrl;
    }

    public FirestoreMessage()
    {
    }

    [FirestoreProperty("username")] public string username { get; set; }

    [FirestoreProperty("timestamp")] public Timestamp timestamp { get; set; }

    [FirestoreProperty("text")] public string text { get; set; }

    [FirestoreProperty("imageUrl")] public string imageUrl { get; set; }

    public override string ToString()
    {
        return
            $"FirestoreMessage{{username='{username}', timestamp={timestamp}, text='{text}', imageUrl='{imageUrl}'}}";
    }

    public override bool Equals(object obj)
    {
        return obj is FirestoreMessage message &&
               username == message.username &&
               EqualityComparer<Timestamp>.Default.Equals(timestamp, message.timestamp) &&
               text == message.text &&
               imageUrl == message.imageUrl;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(username, timestamp, text, imageUrl);
    }
}