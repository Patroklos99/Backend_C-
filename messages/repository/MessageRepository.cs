using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Project.messages.model;

// Adjust namespace based on your actual model namespace

namespace Project.messages.repository;

public class MessageRepository {
    private static readonly string BucketName = "chat-project-d5a0e.appspot.com";
    private static readonly string CollectionName = "messages";
    private readonly FirestoreDb firestore = FirestoreDb.Create("chat-project-d5a0e"); // Specify Firebase project ID

    public async Task<List<Message>> GetMessages(string fromId = null)
    {
        var result = new List<Message>();
        var collectionRef = firestore.Collection(CollectionName);
        var query = collectionRef.OrderBy("timestamp").Limit(20);

        if (!string.IsNullOrEmpty(fromId))
        {
            var snapshot = await collectionRef.Document(fromId).GetSnapshotAsync();
            if (!snapshot.Exists)
                throw new Exception("Not Found"); // Consider a more specific exception or error handling strategy
            query = collectionRef.OrderBy("timestamp").StartAfter(snapshot);
        }

        var querySnapshot = await query.GetSnapshotAsync();
        foreach (var documentSnapshot in querySnapshot.Documents)
        {
            var firestoreMessage = documentSnapshot.ConvertTo<FirestoreMessage>();
            if (firestoreMessage != null)
            {
                var message = new Message(documentSnapshot.Id, firestoreMessage.Username,
                    firestoreMessage.Timestamp.ToDateTime().Ticks, firestoreMessage.Text, firestoreMessage.ImageUrl);
                result.Add(message);
            }
        }

        return result;
    }

    public async Task<Message> CreateMessage(MessageRequest messageRequest)
    {
        string imageUrl = null;
        var collectionRef = firestore.Collection(CollectionName);
        var docRef = collectionRef.Document();

        if (string.IsNullOrWhiteSpace(messageRequest.imageData.data))
        {
            var storageClient = StorageClient.Create();
            var bucket = storageClient.GetBucket(BucketName);
            var path = $"images/{docRef.Id}.{messageRequest.imageData.type}";
            var imageData =
                Convert.FromBase64String(messageRequest.imageData
                    .data); // Adjust based on how you're handling image data
            var imageObject = storageClient.UploadObject(BucketName, path, null, new MemoryStream(imageData),
                new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });
            imageUrl = $"https://storage.googleapis.com/{BucketName}/{path}";
        }

        var firestoreMessage = new FirestoreMessage(
            messageRequest.username,
            Timestamp.GetCurrentTimestamp(),
            messageRequest.text,
            imageUrl
        );

        await docRef.SetAsync(firestoreMessage);
        return new Message(docRef.Id, messageRequest.text, firestoreMessage.Timestamp.ToDateTime().Ticks,
            messageRequest.text, imageUrl);
    }
}