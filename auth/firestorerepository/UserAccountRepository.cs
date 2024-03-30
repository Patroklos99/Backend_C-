using Google.Cloud.Firestore;
using Project.auth.firestorerepository;

public class UserAccountRepository {
    private const string CollectionName = "userAccounts";
    private readonly FirestoreDb _firestore;

    public UserAccountRepository()
    {
        _firestore = FirestoreDb.Create("chat-project-d5a0e"); // Replace with your actual project ID
    }

    public async Task<FirestoreUserAccount> GetUserAccountAsync(string username)
    {
        var docRef = _firestore.Collection(CollectionName).Document(username);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
            return snapshot.ConvertTo<FirestoreUserAccount>();
        return null;
    }

    public async Task SetUserAccountAsync(FirestoreUserAccount userAccount)
    {
        var docRef = _firestore.Collection(CollectionName).Document(userAccount.getUsername());
        await docRef.SetAsync(userAccount);
    }
}