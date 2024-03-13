using Google.Cloud.Firestore;

namespace Project.messages.repository;

[FirestoreData]
public class FirestoreMessage {
    public FirestoreMessage(string username, Timestamp timestamp, string text, string imageUrl)
    {
        Username = username;
        Timestamp = timestamp;
        Text = text;
        ImageUrl = imageUrl;
    }

    [FirestoreProperty] public string Username { get; set; }

    [FirestoreProperty] public Timestamp Timestamp { get; set; }

    [FirestoreProperty] public string Text { get; set; }

    [FirestoreProperty] public string ImageUrl { get; set; }

    public override string ToString()
    {
        return
            $"FirestoreMessage{{username='{Username}', timestamp={Timestamp}, text='{Text}', imageUrl='{ImageUrl}'}}";
    }

    public override bool Equals(object obj)
    {
        return obj is FirestoreMessage message &&
               Username == message.Username &&
               EqualityComparer<Timestamp>.Default.Equals(Timestamp, message.Timestamp) &&
               Text == message.Text &&
               ImageUrl == message.ImageUrl;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, Timestamp, Text, ImageUrl);
    }
}