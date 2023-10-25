using Google.Cloud.Firestore;

namespace API
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public int CustomerId { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string Surname { get; set; }

        [FirestoreProperty]
        public double Amount { get; set; }
    }

}